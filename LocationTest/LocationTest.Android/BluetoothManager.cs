using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LocationTest.Droid
{
    public class BluetoothManager
    {
        private const string ServiceGuid = "6e400001-b5a3-f393-e0a9-e50e24dcca9e";
        private const string ReadGuid = "6e400003-b5a3-f393-e0a9-e50e24dcca9e";
        private const string WriteGuid = "6e400002-b5a3-f393-e0a9-e50e24dcca9e";

        private readonly IAdapter _adapter;
        private readonly string[] AllowedDevices = { "CA:81:BA:4B:DC:02", "E2:4D:DB:60:C0:6B" };

        public BluetoothManager(IAdapter adapter)
        {
            this._adapter = adapter;
        }

        public async void DeviceDiscovered(DeviceEventArgs e, Action<byte[]> onReceiveData)
        {
            try
            {
                if (!this.AllowedDevices.Any(device => e.Device.NativeDevice.ToString().StartsWith(device)))
                {
                    // Device is not in allowed devices
                    return;
                }

                await this._adapter.ConnectToDeviceAsync(e.Device);

                foreach (IDevice dev in this._adapter.ConnectedDevices)
                {
                    IList<IService> services = await dev.GetServicesAsync();
                    IService relevantService = services.FirstOrDefault(x => x.Id.Equals(Guid.Parse(ServiceGuid)));
                    IList<ICharacteristic> characteristics = await relevantService.GetCharacteristicsAsync();

                    ICharacteristic read = characteristics.FirstOrDefault(c => c.Uuid == ReadGuid);
                    ICharacteristic write = characteristics.FirstOrDefault(c => c.Uuid == WriteGuid);

                    read.ValueUpdated += (o, args) => onReceiveData(read.Value);

                    await read.StartUpdatesAsync();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(e.Device.Name + ": " + exception.Message);
            }

        }
    }
}