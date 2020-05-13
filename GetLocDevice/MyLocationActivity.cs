using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;
using GetLocDevice.Droid.Services;
using System.Data;
using System.Timers;

namespace GetLocDevice
{
    [Activity(Label = "Maps", Theme = "@style/MyTheme")]
    public class MyLocationActivity : AppCompatActivity, IOnMapReadyCallback
    {
        static readonly string TAG = "MyLocationActivity";

        static readonly int REQUEST_PERMISSIONS_LOCATION = 1000;

        GoogleMap theMap;

        ISharedPreferences sharedPreferences;

        static Timer timer;

        Button refresh;

        public void OnMapReady(GoogleMap googleMap)

        {
            var lat = sharedPreferences.GetString("lat", null);
            var lon = sharedPreferences.GetString("lon", null);
            var update = sharedPreferences.GetString("update", null);
            theMap = googleMap;
            LatLng latlng = new LatLng(System.Convert.ToDouble(lat), System.Convert.ToDouble(lon));
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, 16);
            theMap.MoveCamera(camera);
            MarkerOptions options = new MarkerOptions().SetPosition(latlng).SetTitle("Waiting Location Update");
            Marker marker = theMap.AddMarker(options);
            marker.ShowInfoWindow();
            if (this.PerformRuntimePermissionCheckForLocation(REQUEST_PERMISSIONS_LOCATION))
            {
                InitializeUiSettingsOnMap();
            }
            timer = new System.Timers.Timer();
            timer.Interval = 3000;
            timer.Enabled = true;
            timer.Elapsed += (sender, args) =>
            {
                RunOnUiThread(() =>
                {
                    UpdateMarker();
                });

            };
        }

        public void UpdateMarker()
        {
            DataTable dtlatlon = new DataTable();
            
            WebReference.BasicHttpBinding_IService1 MyClient = new WebReference.BasicHttpBinding_IService1();

            WebReference.TB latlon = new WebReference.TB();
            latlon = MyClient.GetLatLon(sharedPreferences.GetString("deviceid", null));

            dtlatlon = latlon.table;

            theMap.Clear();

            LatLng latlng = new LatLng(System.Convert.ToDouble(dtlatlon.Rows[0][0].ToString()), System.Convert.ToDouble(dtlatlon.Rows[0][1].ToString()));
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, theMap.CameraPosition.Zoom);
            theMap.MoveCamera(camera);
            MarkerOptions options = new MarkerOptions().SetPosition(latlng).SetTitle("Last Update on " + dtlatlon.Rows[0][2].ToString());
            Marker marker = theMap.AddMarker(options);
            marker.ShowInfoWindow();
            if (this.PerformRuntimePermissionCheckForLocation(REQUEST_PERMISSIONS_LOCATION))
            {
                InitializeUiSettingsOnMap();
            }
        }

        void InitializeUiSettingsOnMap()
        {
            theMap.UiSettings.CompassEnabled = true;
            theMap.UiSettings.ZoomControlsEnabled = true;
        
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MyLocationLayout);

            sharedPreferences = this.GetSharedPreferences("sharedprefrences", 0);

            this.AddMapFragmentToLayout(Resource.Id.map_container);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            refresh = toolbar.FindViewById<Button>(Resource.Id.refresh);
            refresh.Click += delegate
            {
                theMap.Clear();

                LatLng latlng = new LatLng(System.Convert.ToDouble(sharedPreferences.GetString("lat", null)), System.Convert.ToDouble(sharedPreferences.GetString("lon", null)));
                CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, theMap.CameraPosition.Zoom);
                theMap.MoveCamera(camera);
                MarkerOptions options = new MarkerOptions().SetPosition(latlng).SetTitle("Waiting Location Update");
                Marker marker = theMap.AddMarker(options);
                marker.ShowInfoWindow();
                if (this.PerformRuntimePermissionCheckForLocation(REQUEST_PERMISSIONS_LOCATION))
                {
                    InitializeUiSettingsOnMap();
                }

                DataTable dt = new DataTable();

                var appid = "AAAAIS9WtFE:APA91bF_EYn_n1l50-PUDfn9RIjZhj7TT4e_02aB15eKc-mlSCZCLU-Q7NQDFH21Wflg-rIQ1WNlZoIhT94hwxrAvLKAkFg6_KjF1Gv2UcIeJ94Q93HHJC9JMANbl5BUjpaFtXca0_yb";
                var senderid = "142528132177";

                WebReference.BasicHttpBinding_IService1 notif = new WebReference.BasicHttpBinding_IService1();

                var a = sharedPreferences.GetString("keyfcm", null);
                var b = sharedPreferences.GetString("userid", null);

                notif.PushNotification(appid, senderid, sharedPreferences.GetString("keyfcm", null), "start", sharedPreferences.GetString("userid", null));
                };
        }

            public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
            {
                if (requestCode == REQUEST_PERMISSIONS_LOCATION)
                {
                    if (grantResults.AllPermissionsGranted())
                    {
                        // Permissions granted, nothing to do.
                        // Carry on and let the MapFragment do it's own thing.
                        InitializeUiSettingsOnMap();
                    }
                    else
                    {
                        // Permissions not granted!
                        Log.Info(TAG, "The app does not have location permissions");

                        var layout = FindViewById(Android.Resource.Id.Content);
                        Snackbar.Make(layout, Resource.String.location_permission_missing, Snackbar.LengthLong).Show();
                        Finish();
                    }
                }
                else
                {
                    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                }
            }
            public override void OnBackPressed()
            {
                /*DataTable dt = new DataTable();

                var appid = "AAAAIS9WtFE:APA91bF_EYn_n1l50-PUDfn9RIjZhj7TT4e_02aB15eKc-mlSCZCLU-Q7NQDFH21Wflg-rIQ1WNlZoIhT94hwxrAvLKAkFg6_KjF1Gv2UcIeJ94Q93HHJC9JMANbl5BUjpaFtXca0_yb";
                var senderid = "142528132177";

                WebReference.BasicHttpBinding_IService1 notif = new WebReference.BasicHttpBinding_IService1();

                notif.PushNotification(appid, senderid, sharedPreferences.GetString("keyfcm", null), "stop", sharedPreferences.GetString("userid", null));
                */
                Finish();
        }
        protected override void OnDestroy()
        {
            /*DataTable dt = new DataTable();

            var appid = "AAAAIS9WtFE:APA91bF_EYn_n1l50-PUDfn9RIjZhj7TT4e_02aB15eKc-mlSCZCLU-Q7NQDFH21Wflg-rIQ1WNlZoIhT94hwxrAvLKAkFg6_KjF1Gv2UcIeJ94Q93HHJC9JMANbl5BUjpaFtXca0_yb";
            var senderid = "142528132177";

            WebReference.BasicHttpBinding_IService1 notif = new WebReference.BasicHttpBinding_IService1();

            notif.PushNotification(appid, senderid, sharedPreferences.GetString("keyfcm", null), "stop", sharedPreferences.GetString("userid", null));
            */
            base.OnDestroy();
        }

    }
}
