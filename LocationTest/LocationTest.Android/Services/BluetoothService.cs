using LocationTest.Droid.Services;
using LocationTest.Services;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(BluetoothService))]
namespace LocationTest.Droid.Services
{
    public class BluetoothService : IBluetoothService
    {
        private const string ServiceGuid = "6e400001-b5a3-f393-e0a9-e50e24dcca9e";
        private const string ReadGuid = "6e400003-b5a3-f393-e0a9-e50e24dcca9e";
        private const string WriteGuid = "6e400002-b5a3-f393-e0a9-e50e24dcca9e";

        private static readonly string[] AllowedDevices = { "CA:81:BA:4B:DC:02", "E2:4D:DB:60:C0:6B" };

        public async Task Scan()
        {
            if (!CrossBluetoothLE.Current.IsOn)
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "Please enable Bluetooth.", Android.Widget.ToastLength.Short).Show();
                return;
            }

            if (CrossBluetoothLE.Current.Adapter.IsScanning)
            {
                await CrossBluetoothLE.Current.Adapter.StopScanningForDevicesAsync();
            }

            await CrossBluetoothLE.Current.Adapter.StartScanningForDevicesAsync();
        }

        public List<string> GetConnectedDevices()
        {
            return CrossBluetoothLE.Current.Adapter.ConnectedDevices.Select(device => device.Name).ToList();
        }

        public void Listen(BluetoothHandler handler = null)
        {

            IBluetoothLE bluetooth = CrossBluetoothLE.Current;
            bluetooth.Adapter.ScanMode = ScanMode.Balanced;
            bluetooth.Adapter.DeviceDiscovered += (device, args) =>
            {
                ReadData(bluetooth.Adapter, args, handler?.OnConnect);
            };
            bluetooth.Adapter.DeviceDisconnected += (_, args) => handler?.OnDisconnect?.Invoke(args.Device.Name);
            bluetooth.StateChanged += (_, args) =>
            {
                Console.WriteLine($"The bluetooth state changed to {args.NewState}");
            };
        }

        private static async void ReadData(IAdapter adapter, DeviceEventArgs args, Action<string> onConnect = null)
        {
            try
            {
                if (!AllowedDevices.Any(device => args.Device.NativeDevice.ToString().StartsWith(device)))
                {
                    // Device is not in allowed devices
                    return;
                }

                await adapter.ConnectToDeviceAsync(args.Device);

                onConnect(args.Device.Name);

                IList<IService> services = await args.Device.GetServicesAsync();
                IService relevantService = services.FirstOrDefault(x => x.Id.Equals(Guid.Parse(ServiceGuid)));
                IList<ICharacteristic> characteristics = await relevantService.GetCharacteristicsAsync();

                ICharacteristic read = characteristics.FirstOrDefault(c => c.Uuid == ReadGuid);
                ICharacteristic write = characteristics.FirstOrDefault(c => c.Uuid == WriteGuid);

                if (read == null)
                {
                    return;
                }

                string filePath = Path.Combine("storage", "emulated", "0", "Android", "data", "com.copd.COPDMonitor.Android", "files", "data" + args.Device.NativeDevice.ToString().Replace(":", "") + ".csv");

                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Dispose();
                }

                read.ValueUpdated += (o, _) =>
                {
                    byte[] data = read.Value;
                    using (FileStream stream = new FileStream(filePath, FileMode.Append))
                    {
                        stream.Write(data, 0, data.Length);
                    }
                };

                await read.StartUpdatesAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(args.Device.Name + ": " + exception.Message);
            }

        }
    }
}