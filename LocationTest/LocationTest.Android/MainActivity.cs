using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Auth0.OidcClient;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;
using IAdapter = Plugin.BLE.Abstractions.Contracts.IAdapter;

namespace LocationTest.Droid
{
    [Activity(
        Label = "COPD Monitor",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTask,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation
    )]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "com.copd.copdmonitor.android",
        DataHost = "copd.eu.auth0.com",
        DataPathPrefix = "/android/com.copd.copdmonitor.android/callback"
    )]
    public class MainActivity : FormsAppCompatActivity
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
            this.LoadApplication(new App(new SignInViewModel()));

            IBluetoothLE bluetooth = CrossBluetoothLE.Current;
            this._adapter = bluetooth.Adapter;
            bluetooth.StateChanged += this.BluetoothStateChanged;
            this._adapter.DeviceDiscovered += this._adapter_DeviceDiscovered;
            this._adapter.ScanMode = ScanMode.Balanced;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Task scan = this._adapter.StartScanningForDevicesAsync();
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            ActivityMediator.Instance.Send(intent.DataString);
        }

        private async void _adapter_DeviceDiscovered(object sender, DeviceEventArgs e)
        {
            const string storagePath = "/storage/emulated/0/android/data/com.copd.COPDMonitor.Android/files/";
            string[] allowedDevices = { "CA:81:BA:4B:DC:02", "E2:4D:DB:60:C0:6B" };

            try
            {
                Console.WriteLine(e.Device.NativeDevice);
                if (!allowedDevices.Any(device => e.Device.NativeDevice.ToString().StartsWith(device)))
                {
                    // Device is not in allowed devices
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


                    string filePath = Path.Combine(storagePath, "data.csv");
                    if (!File.Exists(filePath))
                    {
                        File.Create(filePath).Dispose();
                    }

                    read.ValueUpdated += (o, args) =>
                    {
                        byte[] bytes = read.Value;
                        using (FileStream stream = new FileStream(filePath, FileMode.Append))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                        }
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