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

namespace LocationTest.Droid
{
    [Activity(Label = "LocationTest", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        FusedLocationProviderClient fusedLocationProviderClient;
        bool _errythingOk;       
	
MyLocationCallback locationCallback;
        FusedLocationProviderClient client;

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
                        ActivityCompat.RequestPermissions(this,
         new string[] { Manifest.Permission.AccessFineLocation },
         1);
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
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            _errythingOk = IsGooglePlayServicesInstalled()&&AccesToFineLocation();
            fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
           Task locationUpdate = StartLocationUpdatesAsync();
           // GetLastLocationFromDevice().Wait();
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        //async Task GetLastLocationFromDevice()
        //{

        //    Android.Locations.Location location = await fusedLocationProviderClient.GetLastLocationAsync();

        //    if (location == null)
        //    {
        //        // Seldom happens, but should code that handles this scenario
        //    }
        //    else
        //    {
        //        // Do something with the location 
        //        Log.Debug("Sample", "The latitude is " + location.Latitude);
        //    }
        //}
        bool AccesToFineLocation()
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
        void OnLocationResult(object sender, Location location)
        {
            //location.Latitude;
            //location.Longitude;
        }

    }
}