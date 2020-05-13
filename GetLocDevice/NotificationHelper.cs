using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Firebase.Iid;
using Firebase.Messaging;
using GetLocDevice;
using GetLocDevice.Droid;
using Newtonsoft.Json;
using Android.Locations;

namespace GetLocDevice.Droid
{
    public class NotificationHelper : INotification
    {
        ISharedPreferences sharedPreferences;
        private NotificationCompat.Builder mBuilder;
        public static String NOTIFICATION_CHANNEL_ID = "10023";

        public void CreateNotification(String title, String message)
        {
            try
            {
                var intent = new Intent(global::Android.App.Application.Context, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                intent.PutExtra("title", message);
                var pendingIntent = PendingIntent.GetActivity(global::Android.App.Application.Context, 0, intent, PendingIntentFlags.OneShot);

                var sound = global::Android.Net.Uri.Parse(ContentResolver.SchemeAndroidResource + "://" + global::Android.App.Application.Context.PackageName + "/" + Resource.Raw.notification);
                // Creating an Audio Attribute
                var alarmAttributes = new AudioAttributes.Builder()
                    .SetContentType(AudioContentType.Sonification)
                    .SetUsage(AudioUsageKind.Notification).Build();

                mBuilder = new NotificationCompat.Builder(global::Android.App.Application.Context);
                mBuilder.SetSmallIcon(Resource.Drawable.abc);
                mBuilder.SetContentTitle(title)
                        .SetSound(sound)
                        .SetAutoCancel(true)
                        .SetContentTitle(title)
                        .SetContentText(message)
                        .SetChannelId(NOTIFICATION_CHANNEL_ID)
                        .SetPriority((int)NotificationPriority.High)
                        .SetVibrate(new long[0])
                        .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate)
                        .SetVisibility((int)NotificationVisibility.Public)
                        .SetSmallIcon(Resource.Drawable.abc)
                        .SetContentIntent(pendingIntent);



                NotificationManager notificationManager = global::Android.App.Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

                if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O)
                {
                    NotificationImportance importance = global::Android.App.NotificationImportance.High;

                    NotificationChannel notificationChannel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, title, importance);
                    notificationChannel.EnableLights(true);
                    notificationChannel.EnableVibration(true);
                    notificationChannel.SetSound(sound, alarmAttributes);
                    notificationChannel.SetShowBadge(true);
                    notificationChannel.Importance = NotificationImportance.High;
                    notificationChannel.SetVibrationPattern(new long[] { 100, 200, 300, 400, 500, 400, 300, 200, 400 });

                    if (notificationManager != null)
                    {
                        mBuilder.SetChannelId(NOTIFICATION_CHANNEL_ID);
                        notificationManager.CreateNotificationChannel(notificationChannel);
                    }
                }

                notificationManager.Notify(0, mBuilder.Build());

            }
            catch (Exception ex)
            {
                //
            }
        }
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


            /*sharedPreferences = Application.Context.GetSharedPreferences("sharedprefrences", 0);

            DataTable dtlatlon = new DataTable();
            string deviceid = Android.Provider.Settings.Secure.GetString(Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

            WebReference.BasicHttpBinding_IService1 notif = new WebReference.BasicHttpBinding_IService1();

            WebReference.TB latlon = new WebReference.TB();
            latlon = notif.GetLatLon(deviceid);

            dtlatlon = latlon.table;

            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            editor.PutString("lat", dtlatlon.Rows[0][0].ToString());
            editor.PutString("lon", dtlatlon.Rows[0][1].ToString());
            editor.Commit();

            Intent ij = new Intent(global::Android.App.Application.Context, typeof(MyLocationActivity));
            global::Android.App.Application.Context.StartActivity(ij);*/
        }
        public async void GetLocations(string userid)
        {
            try
            {
                var request = new Xamarin.Essentials.GeolocationRequest(Xamarin.Essentials.GeolocationAccuracy.Best);
                var location = await Xamarin.Essentials.Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    GetAddress(userid, location.Latitude, location.Longitude);
                }
            }
            catch (Xamarin.Essentials.FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (Xamarin.Essentials.FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (Xamarin.Essentials.PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }
    }
}