using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common;
using Android.Gms.Location;
using Android.Util;
using System.Threading.Tasks;
using Android.Locations;
using Android.Support.V4.Content;
using Android;
using Android.Support.V4.App;
using Android.Bluetooth;
using Plugin.BLE;
using System.Collections.Generic;
using System.Linq;
using Plugin.BLE.Abstractions.Contracts;
using System.Text;
using System.IO;

namespace LocationTest.Droid
{
    [Activity(Label = "LocationTest", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        FusedLocationProviderClient fusedLocationProviderClient;
        bool _errythingOk;
        private Plugin.BLE.Abstractions.Contracts.IBluetoothLE _ble; 
        private Plugin.BLE.Abstractions.Contracts.IAdapter _adapter;
        MyLocationCallback locationCallback;
        FusedLocationProviderClient client;
        private List<Plugin.BLE.Abstractions.Contracts.IDevice> _deviceList;
        private IList<Plugin.BLE.Abstractions.Contracts.IService> _services;
        private IList<Plugin.BLE.Abstractions.Contracts.IService> services;
        private IList<Plugin.BLE.Abstractions.Contracts.ICharacteristic> _characteristics;
        private Dictionary<IDevice, ICharacteristic> rx = new Dictionary<IDevice, ICharacteristic>();
        private Dictionary<IDevice, ICharacteristic> tx = new Dictionary<IDevice, ICharacteristic>();
        Dictionary<IDevice, IList<IService>> devices = new Dictionary<IDevice, IList<IService>>();
        Dictionary<Tuple<IDevice,IService>, IList<ICharacteristic>> servicesWithCharacteristics = new Dictionary<Tuple<IDevice, IService>, IList<ICharacteristic>>();
        List<string> data = new List<string>();
        int _mtu;
        List<byte> langDing = new List<byte>();
        async Task StartLocationUpdatesAsync()
        {
            // Create a callback that will get the location updates
            if (locationCallback == null)
            {
                locationCallback = new MyLocationCallback();
                locationCallback.LocationUpdated += OnLocationResult;
            }

            // Get the current client
            if (client == null)
                client = LocationServices.GetFusedLocationProviderClient(this);

            try
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
                {
                    //Create request and set intervals:
                    //Interval: Desired interval for active location updates, it is inexact and you may not receive upates at all if no location servers are available
                    //Fastest: Interval is exact and app will never receive updates faster than this value
                    var locationRequest = new LocationRequest()
                                          .SetInterval(10000)
                                          .SetFastestInterval(5000)
                                          .SetPriority(LocationRequest.PriorityHighAccuracy);

                    await client.RequestLocationUpdatesAsync(locationRequest, locationCallback);
                }
                else
                {
                    while (!AccesToFineLocation())
                    {
                        ActivityCompat.RequestPermissions(this,new string[] { Manifest.Permission.AccessFineLocation },1);
                        ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.Bluetooth }, 1);
                        ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.BluetoothAdmin }, 1);
                        ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.BluetoothPrivileged }, 1);
                        ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.WriteExternalStorage }, 1);

                        ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessWifiState }, 1);
                    }
                    var locationRequest = new LocationRequest()
                                         .SetInterval(10000)
                                         .SetFastestInterval(5000)
                                         .SetPriority(LocationRequest.PriorityHighAccuracy);

                    await client.RequestLocationUpdatesAsync(locationRequest, locationCallback);
                }
            }
            catch (Exception ex)
            {
                //Handle exception here if failed to register
            }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            _ble = CrossBluetoothLE.Current;
            _adapter = CrossBluetoothLE.Current.Adapter;
            _ble.StateChanged += _ble_StateChanged;
            _deviceList = new List<Plugin.BLE.Abstractions.Contracts.IDevice>();

            _adapter.DeviceDiscovered += _adapter_DeviceDiscovered;
            _adapter.ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode.Balanced;
            Task scan = _adapter.StartScanningForDevicesAsync();
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
         
            base.OnCreate(savedInstanceState);
            _errythingOk = IsGooglePlayServicesInstalled() && AccesToFineLocation();
            fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            Task locationUpdate = StartLocationUpdatesAsync();
            //cleaner dan huidige oplossing, kijken of het werkt
            this.RequestPermissions(new[]
           {
                Manifest.Permission.AccessFineLocation,
                Manifest.Permission.BluetoothPrivileged
            }, 0);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        private async void _adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            try
            {


                if (e.Device.NativeDevice.ToString() == "CA:81:BA:4B:DC:02" || e.Device.NativeDevice.ToString() == "E2:4D:DB:60:C0:6B")
                {
                    _deviceList.Add(e.Device);
                    await _adapter.ConnectToDeviceAsync(e.Device);
                    var i = _adapter.ConnectedDevices;
                    foreach (var dev in i)
                    {
                        var temp = await dev.GetServicesAsync();
                        devices.Add(dev, temp);
                        var t = temp.ToList().Find(x=>x.Id== Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e"));
                            var chara = await t.GetCharacteristicsAsync();
                            servicesWithCharacteristics.Add(Tuple.Create(dev, t), chara);
                            var read= chara.ToList().Find(c => c.Uuid == "6e400003-b5a3-f393-e0a9-e50e24dcca9e");
                            var write= chara.ToList().Find(c => c.Uuid == "6e400002-b5a3-f393-e0a9-e50e24dcca9e");
                           
                            read.ValueUpdated += (o, args) =>
                            {
                                byte[] bytes = read.Value;
                                if (read.StringValue.Contains("end"))
                                {                                    
                                    langDing.AddRange(bytes);
                                    File.WriteAllBytes("/storage/emulated/0/android/data/com.companyname.LocationTest.Android/files/foo"+
                                    System.DateTime.Now.ToString("MM-dd-hh-mm-ss")+".CSV", langDing.ToArray());
                                    langDing.Clear();
                                }
                                    
                                    langDing.AddRange(bytes);

                            };
                            await read.StartUpdatesAsync();

                        await write.WriteAsync(Encoding.ASCII.GetBytes("r"));
                        //foreach (var c in chara)
                        //{
                        //    //if (c.CanRead)
                        //    //{
                        //    //  var bytes = await c.ReadAsync();
                        //    //    string tostring = System.Text.Encoding.UTF8.GetString(bytes);
                        //    //    data.Add(tostring);
                        //    //}
                        //    if (c.Uuid== "6e400003-b5a3-f393-e0a9-e50e24dcca9e")
                        //    {
                        //        c.ValueUpdated += (o, args) =>
                        //        {

                        //            string tostring = c.StringValue;
                        //        };
                        //        await c.StartUpdatesAsync();
                        //    }
                        //    if(c.Uuid == "6e400002-b5a3-f393-e0a9-e50e24dcca9e")
                        //    {
                        //        var cc = c;
                        //        await c.WriteAsync(Encoding.ASCII.GetBytes("r"));
                        //    }
                        //}


                    }

                    //Task mtu = GetMtu(e.Device);
                    // _characteristics = GetCharacteristics();


                }
            }
            catch (Exception exept)
            {
                Console.Write(e.Device.Name);
            }
            
        }
        private async Task GetServices(IDevice device)
        {
            _services = await device.GetServicesAsync();
        }
        private async Task<IList<Plugin.BLE.Abstractions.Contracts.IService>> GetService(IDevice device)
        {
            return await device.GetServicesAsync();
        }
        //private async Task<IList<Plugin.BLE.Abstractions.Contracts.ICharacteristic>> GetCharacteristics(IDevice device)
        //{

        //}
        //private async Task GetMtu(IDevice device)
        //{
        //   var mtu = await device.RequestMtuAsync(100);
        //    _mtu = mtu;

        //}
        private async Task GetCharacteristics()
        {
            var t= await _services[3].GetCharacteristicsAsync();
            // UUID.fromString("6e400001-b5a3-f393-e0a9-e50e24dcca9e") service uuid
            //private static final UUID kUartTxCharacteristicUUID = UUID.fromString("6e400002-b5a3-f393-e0a9-e50e24dcca9e");
            //private static final UUID kUartRxCharacteristicUUID = UUID.fromString("6e400003-b5a3-f393-e0a9-e50e24dcca9e");
         
            foreach (var chara in t) {
                chara.ValueUpdated += (o, args) =>
                {
                    var bytes = args.Characteristic.Value;
                };

                await chara.StartUpdatesAsync();
            }
        }
        private void _ble_StateChanged(object sender, Plugin.BLE.Abstractions.EventArgs.BluetoothStateChangedArgs e)
        {
            Console.WriteLine($"The bluetooth state changed to {e.NewState}");

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            foreach (var permis in permissions)
                Console.Write(permis);
            foreach (var res in grantResults)
                Console.Write(res);
        }
        private bool AccesToFineLocation()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                return true;
            }
            else
            {
                // The app does not have permission ACCESS_FINE_LOCATION 


                return false;
            }
        }
        bool IsGooglePlayServicesInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info("MainActivity", "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                // Check if there is a way the user can resolve the issue
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error("MainActivity", "There is a problem with Google Play Services on this device: {0} - {1}",
                          queryResult, errorString);

                // Alternately, display the error to the user.
            }

            return false;
        }
        /// <summary>
        /// Event happens on location update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="location"></param>
        void OnLocationResult(object sender, Location location)
        {
            //location.Latitude;
            //location.Longitude;
        }

    }
}