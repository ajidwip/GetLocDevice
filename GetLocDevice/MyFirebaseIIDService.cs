using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Iid;

namespace GetLocDevice.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "MyFirebaseIIDService";
        static ISharedPreferences sharedPreferences;

        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            SendRegistrationToServer(refreshedToken);
        }
        void SendRegistrationToServer(string token)
        {
            sharedPreferences = GetSharedPreferences("sharedprefrences", 0);
            WebReference.BasicHttpBinding_IService1 MyClient = new WebReference.BasicHttpBinding_IService1();
            WebReference.KeyFCM empfcm = new WebReference.KeyFCM();
            empfcm = MyClient.SetKeyFCM(Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId), token);
            // send this token to server
        }
    }
}