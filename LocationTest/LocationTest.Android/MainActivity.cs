using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.OS;
using Android.Util;
using Auth0.OidcClient;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;

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
        private const string StoragePath = "/storage/emulated/0/android/data/com.copd.COPDMonitor.Android/files/";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.IsGooglePlayServicesInstalled();

            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            this.LoadApplication(new App(new SignInViewModel()));

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            PermissionManager.RequestPermissions(this);


            IBluetoothLE bluetooth = CrossBluetoothLE.Current;
            BluetoothManager bluetoothManager = new BluetoothManager(bluetooth.Adapter);
            bluetooth.Adapter.ScanMode = ScanMode.Balanced;
            bluetooth.Adapter.DeviceDiscovered += (_, args) =>
            {
                string filePath = Path.Combine(StoragePath, "data.csv");
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Dispose();
                }

                bluetoothManager.DeviceDiscovered(args, data =>
                {
                    using (FileStream stream = new FileStream(filePath, FileMode.Append))
                    {
                        stream.Write(data, 0, data.Length);
                    }
                });
            };
            bluetooth.StateChanged += (_, args) =>
            {
                Console.WriteLine($"The bluetooth state changed to {args.NewState}");
            };

            Task scan = bluetooth.Adapter.StartScanningForDevicesAsync();
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            ActivityMediator.Instance.Send(intent.DataString);
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