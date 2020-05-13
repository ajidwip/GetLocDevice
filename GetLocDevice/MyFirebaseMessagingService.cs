using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Iid;
using Firebase.Messaging;
using GetLocDevice.Droid.Services;

namespace GetLocDevice.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {

        protected LocationManager LocMgr = Application.Context.GetSystemService("location") as LocationManager;
        ISharedPreferences sharedPreferences;
        public MyFirebaseMessagingService()
        {

        }
        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);

            sharedPreferences = Application.Context.GetSharedPreferences("sharedprefrences", 0);

            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            editor.PutString("action", message.Data["title"]);
            editor.PutString("userid", message.Data["body"]);
            editor.Commit();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                //startLocation();
                Application.Context.StartForegroundService(new Intent(Application.Context, typeof(SimpleService)));
            }
            else // For older versions, use the traditional StartService() method
            {
                //startLocation();
                Application.Context.StartService(new Intent(Application.Context, typeof(SimpleService)));
            }
        }

    }
}