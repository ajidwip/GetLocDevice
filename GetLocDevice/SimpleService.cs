using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Android.Locations;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Data;

namespace GetLocDevice.Droid.Services
{
    [Service]
    public class SimpleService : Service, ILocationListener
    {
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        const string NOTIFICATION_CHANNEL_ID = "com.service.app.channel";
        static Context mContext;
        protected LocationManager LocMgr = Application.Context.GetSystemService("location") as LocationManager;
        ISharedPreferences sharedPreferences;
        public string action;
        public string userid;
        public string keyfcm;
        public string deviceid;

        public class result
        {
            public string formatted_address { get; set; }
        }
        public class DataList
        {
            public List<result> results { get; set; }
        }
        public async void GetAddress(string userid, double lat, double lon)
        {
            var URL = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + lat + "," + lon + "&key=AIzaSyCyS0sAM18a1JhzYSwZEBkfyE5--qFoN1U";
            var myHttpClient = new HttpClient();
            var response = await myHttpClient.GetAsync(URL);
            var json = response.Content.ReadAsStringAsync().Result;

            DataList ObjDataList = new DataList();
            ObjDataList = JsonConvert.DeserializeObject<DataList>(json);
            var address = ObjDataList.results[1].formatted_address;


            WebReference.BasicHttpBinding_IService1 MyClient = new WebReference.BasicHttpBinding_IService1();

            MyClient.SetLatLon(Android.Provider.Settings.Secure.GetString(Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId), userid, lat.ToString(), lon.ToString(), address);

            StopSelf();
        }
        public void OnLocationChanged(Android.Locations.Location location)
        {
            LocationChanged(this, new LocationChangedEventArgs(location));
            GetAddress(userid, location.Latitude, location.Longitude);
        }

        public void OnProviderDisabled(string provider)
        {
            ProviderDisabled(this, new ProviderDisabledEventArgs(provider));
        }

        public void OnProviderEnabled(string provider)
        {
            ProviderEnabled(this, new ProviderEnabledEventArgs(provider));
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            StatusChanged(this, new StatusChangedEventArgs(provider, status, extras));
        }
        public async void StartLocationUpdates()
        {
            /*var locationCriteria = new Criteria();

            locationCriteria.Accuracy = Accuracy.NoRequirement;
            locationCriteria.PowerRequirement = Power.NoRequirement;

            var locationProvider = LocMgr.GetBestProvider(locationCriteria, true);

            LocMgr.RequestLocationUpdates(locationProvider, 0, 20, this);*/

            var request = new Xamarin.Essentials.GeolocationRequest(Xamarin.Essentials.GeolocationAccuracy.Medium, TimeSpan.FromSeconds(5));
            var location = await Xamarin.Essentials.Geolocation.GetLocationAsync(request);

            if (location != null)
            {

                try
                {
                    WebReference.BasicHttpBinding_IService1 MyClient = new WebReference.BasicHttpBinding_IService1();

                    MyClient.SetLatLon(Android.Provider.Settings.Secure.GetString(Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId), userid, location.Latitude.ToString(), location.Longitude.ToString(), "");

                    StopSelf();
                }
                catch (Exception ex)
                {
                    StartLocationUpdates();
                }
                //GetAddress(userid, location.Latitude, location.Longitude);
            }

        }

        public event EventHandler<LocationChangedEventArgs> LocationChanged = delegate { };
        public event EventHandler<ProviderDisabledEventArgs> ProviderDisabled = delegate { };
        public event EventHandler<ProviderEnabledEventArgs> ProviderEnabled = delegate { };
        public event EventHandler<StatusChangedEventArgs> StatusChanged = delegate { };

        public override void OnCreate()
        {
            base.OnCreate();
            sharedPreferences = this.GetSharedPreferences("sharedprefrences", 0);
            action = sharedPreferences.GetString("action", null);
            userid = sharedPreferences.GetString("userid", null);
            keyfcm = sharedPreferences.GetString("keyfcm", null);
            deviceid = sharedPreferences.GetString("deviceid", null);
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            // Check if device is running Android 8.0 or higher and call StartForeground() if so
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {

                var notification = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID)
                    .SetContentTitle(Resources.GetString(Resource.String.notification_title))
                    .SetContentText(Resources.GetString(Resource.String.notification_text))
                    .SetSmallIcon(Resource.Drawable.abc)
                    .SetOngoing(true)
                    .Build();

                var notificationManager =
                    GetSystemService(NotificationService) as NotificationManager;

                var chan = new NotificationChannel(NOTIFICATION_CHANNEL_ID, "On-going Notification", NotificationImportance.Min);

                notificationManager.CreateNotificationChannel(chan);

                StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
            }
            StartLocationUpdates();

            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            // This is a started service, not a bound service, so we just return null.
            return null;
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }        
    }
}