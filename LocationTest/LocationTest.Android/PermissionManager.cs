using Android;
using Android.App;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using System.Linq;

namespace LocationTest.Droid
{
    internal static class PermissionManager
    {
        private static readonly string[] RequiredPermissions = new[]
        {
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.AccessWifiState,
            Manifest.Permission.AccessFineLocation
        };

        public static void RequestPermissions(Activity origin)
        {
            bool IsDenied(string permission) => ContextCompat.CheckSelfPermission(origin, permission) != Permission.Granted;

            while (RequiredPermissions.Any(IsDenied))
            {
                RequiredPermissions
                    .Where(IsDenied)
                    .ToList()
                    .ForEach(permission =>
                    {
                        ActivityCompat.RequestPermissions(origin, new string[] { permission }, 1);
                    });
            }
        }
    }
}