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

namespace GetLocDevice
{
    [Activity(Label = "Register Devices")]
    public class RegisterListDevices : Activity
    {
        ISharedPreferences sharedPreferences;
        Button btnsave, btncancel;
        EditText edtdeviceid, edtuserid;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.registerlistdevices);

            edtdeviceid = FindViewById<EditText>(Resource.Id.deviceid);
            edtuserid = FindViewById<EditText>(Resource.Id.userid);
            btnsave = FindViewById<Button>(Resource.Id.btnsave);
            btncancel = FindViewById<Button>(Resource.Id.btncancel);

            sharedPreferences = this.GetSharedPreferences("sharedprefrences", 0);

            edtdeviceid.Text = sharedPreferences.GetString("registerdeviceid", null);

            btnsave.Click += delegate
            {
                string deviceid = Android.Provider.Settings.Secure.GetString(Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

                WebReference.BasicHttpBinding_IService1 MyClient = new WebReference.BasicHttpBinding_IService1();

                MyClient.SetListDevice(deviceid, sharedPreferences.GetString("registerdeviceid", null), edtuserid.Text);

                new MainActivity.LoadDataForActivity().ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor);

                Finish();
            };

            btncancel.Click += delegate
            {
                Finish();
            };
            
        }
    }
}