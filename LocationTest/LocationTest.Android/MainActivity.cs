using Android;
using Android.App;
using Android.Content.PM;
using Android.Gms.Common;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationTest.Droid
{
    [Activity(
        Label = "COPD Monitor",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation
    )]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private IAdapter _adapter;
        private readonly Dictionary<IDevice, IList<IService>> devices = new Dictionary<IDevice, IList<IService>>();
        private readonly Dictionary<Tuple<IDevice, IService>, IList<ICharacteristic>> servicesWithCharacteristics = new Dictionary<Tuple<IDevice, IService>, IList<ICharacteristic>>();
        private readonly List<byte> bluetoothBuffer = new List<byte>();

        private void RequestPermissions()
        {
            string[] requiredPermissions = new[]
            {
                Manifest.Permission.Bluetooth,
                Manifest.Permission.BluetoothAdmin,
                Manifest.Permission.AccessWifiState,
                Manifest.Permission.AccessFineLocation
            };

            while (requiredPermissions.Any(permission => ContextCompat.CheckSelfPermission(this, permission) != Permission.Granted))
            {
                IEnumerable<string> unmatchedPermissions = requiredPermissions.Where(permission => ContextCompat.CheckSelfPermission(this, permission) != Permission.Granted);
                foreach (string permission in unmatchedPermissions)
                {
                    ActivityCompat.RequestPermissions(this, new string[] { permission }, 1);
                }
            }

        }
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            this.IsGooglePlayServicesInstalled();
            this.RequestPermissions();
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            this.LoadApplication(new App());
            IBluetoothLE bluetooth = CrossBluetoothLE.Current;
            this._adapter = bluetooth.Adapter;
            bluetooth.StateChanged += this.BluetoothStateChanged;
            _adapter.DeviceDiscovered += this._adapter_DeviceDiscovered;
            _adapter.ScanMode = ScanMode.Balanced;
            Task scan = _adapter.StartScanningForDevicesAsync();

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;





        }

        private async void _adapter_DeviceDiscovered(object sender, DeviceEventArgs e)
        {
            const string filePath = "/storage/emulated/0/android/data/com.companyname.LocationTest.Android/files/foo";
            string[] allowedDevices = { "CA:81:BA:4B:DC:02", "E2:4D:DB:60:C0:6B" };

            try
            {
                if (!allowedDevices.Contains(e.Device.NativeDevice))
                {
                    // No devices connected
                    return;
                }

                await this._adapter.ConnectToDeviceAsync(e.Device);

                foreach (IDevice dev in this._adapter.ConnectedDevices)
                {
                    IList<IService> services = await dev.GetServicesAsync();
                    this.devices.Add(dev, services);

                    IService relevantService = services.FirstOrDefault(x => x.Id.Equals(Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e")));
                    IList<ICharacteristic> characteristics = await relevantService.GetCharacteristicsAsync();
                    this.servicesWithCharacteristics.Add(Tuple.Create(dev, relevantService), characteristics);

                    ICharacteristic read = characteristics.FirstOrDefault(c => c.Uuid == "6e400003-b5a3-f393-e0a9-e50e24dcca9e");
                    ICharacteristic write = characteristics.FirstOrDefault(c => c.Uuid == "6e400002-b5a3-f393-e0a9-e50e24dcca9e");

                    read.ValueUpdated += (o, args) =>
                    {
                        byte[] bytes = read.Value;
                        if (read.StringValue.Contains("end"))
                        {
                            bluetoothBuffer.AddRange(bytes);

                            File.Delete(filePath + ".CSV");
                            File.WriteAllBytes(filePath + System.DateTime.Now.ToString("MM-dd-hh-mm-ss") + ".CSV", this.bluetoothBuffer.ToArray());
                            bluetoothBuffer.Clear();
                        }
                        this.bluetoothBuffer.AddRange(bytes);

                    };

                    await read.StartUpdatesAsync();

                    await write.WriteAsync(Encoding.ASCII.GetBytes("r"));
                }
            }
            catch (Exception exception)
            {
                Console.Write(e.Device.Name + ": " + exception.Message);
            }

        }

        private void BluetoothStateChanged(object sender, BluetoothStateChangedArgs e)
        {
            Console.WriteLine($"The bluetooth state changed to {e.NewState}");
        }

        private bool IsGooglePlayServicesInstalled()
        {
            int queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info("MainActivity", "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                // Check if there is a way the user can resolve the issue
                string errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error("MainActivity", "There is a problem with Google Play Services on this device: {0} - {1}",
                          queryResult, errorString);

                // Alternately, display the error to the user.
            }

            return false;
        }

    }
}