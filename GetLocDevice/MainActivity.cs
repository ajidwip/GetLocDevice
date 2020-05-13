using System;
using Android.App;
using Android.Views;
using Android.OS;
using Android.Locations;
using Android.Support.V4.Content;
using Android;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Widget;
using Android.Content;
using Android.Support.V7.Widget;
using GetLocDevice.Adapter;
using GetLocDevice.GetterSetter;
using System.Collections.Generic;
using System.Data;
using ZXing.Mobile;
using GetLocDevice.FragmentAct;
using Android.Runtime;

namespace GetLocDevice
{
    [Activity(Label = "Location Device", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        static readonly int RC_REQUEST_LOCATION_PERMISSION = 1000;
        static readonly string[] REQUIRED_PERMISSIONS = { Manifest.Permission.AccessFineLocation };
        public static List<GetSet> recyclelist = new List<GetSet>();
        static RecyclerView mRecyclerView;
        public static ProgressDialog progressDialog;
        static ImageView image;
        static TextView textview;
        static adapter mAdapter;
        static RecyclerView recyclerView;
        public static Fragment currentFragment;
        public static BottomNavigationView bottomNavigationView;
        public static Fragment listdata = new listdata();
        public static int isloaded = 0;
        public static int countback = 0;
        ISharedPreferences sharedPreferences;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource  
            SetContentView(Resource.Layout.activity_main);

            Xamarin.Essentials.Platform.Init(Application);

            image = FindViewById<ImageView>(Resource.Id.barcodeImage);
            textview = FindViewById<TextView>(Resource.Id.textview);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            bottomNavigationView.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
            BottomNavigationViewHelper.disableShiftMode(bottomNavigationView);
            sharedPreferences = this.GetSharedPreferences("sharedprefrences", 0);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted)
            {

            }
            else
            {
                RequestLocationPermission();
            }

            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            mRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
            mAdapter = new adapter(recyclelist);
            mAdapter.ItemClick += OnItemClick;

            new LoadDataForActivity().ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor);
        }
        void OnItemClick(object sender, int position)
        {
            DataTable dt = new DataTable();

            string parentdeviceid = recyclelist[position].getparentdeviceid();
            string userid = recyclelist[position].getuserid();
            var appid = "AAAAIS9WtFE:APA91bF_EYn_n1l50-PUDfn9RIjZhj7TT4e_02aB15eKc-mlSCZCLU-Q7NQDFH21Wflg-rIQ1WNlZoIhT94hwxrAvLKAkFg6_KjF1Gv2UcIeJ94Q93HHJC9JMANbl5BUjpaFtXca0_yb";
            var senderid = "142528132177";

            WebReference.BasicHttpBinding_IService1 notif = new WebReference.BasicHttpBinding_IService1();

            WebReference.TB key = new WebReference.TB();
            key = notif.GetKeyFCM(parentdeviceid);

            dt = key.table;

            ISharedPreferencesEditor keyfcm = sharedPreferences.Edit();
            keyfcm.PutString("keyfcm", dt.Rows[0][0].ToString());
            keyfcm.PutString("deviceid", parentdeviceid);
            keyfcm.PutString("userid", userid);
            keyfcm.Commit();

            notif.PushNotification(appid, senderid, dt.Rows[0][0].ToString(), "start", userid);

            DataTable dtlatlon = new DataTable();

            WebReference.TB latlon = new WebReference.TB();
            latlon = notif.GetLatLon(parentdeviceid);

            dtlatlon = latlon.table;

            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            editor.PutString("lat", dtlatlon.Rows[0][0].ToString());
            editor.PutString("lon", dtlatlon.Rows[0][1].ToString());
            editor.PutString("update", dtlatlon.Rows[0][2].ToString());
            editor.Commit();


            Intent ij = new Intent(this, typeof(MyLocationActivity));
            StartActivity(ij);

        }
        public class LoadDataForActivity : AsyncTask
        {

            DataTable dt = new DataTable();
            public static int flag = 0;
            protected override void OnPreExecute()
            {


            }

            protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
            {
                WebReference.BasicHttpBinding_IService1 MyClient = new WebReference.BasicHttpBinding_IService1();
                try
                {

                    WebReference.TB emp = new WebReference.TB();
                    string deviceid = Android.Provider.Settings.Secure.GetString(Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                    emp = MyClient.GetListDevice(deviceid);

                    dt = emp.table;


                    recyclelist.Clear();
                    if (dt.Rows.Count > 0)
                    {

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            recyclelist.Add(new GetSet(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString()));
                        }

                    }

                }
                catch (Exception ex)
                {

                    Snackbar snackbar = Snackbar.Make(findViewById(Android.Resource.Id.Content), "Error Connection", Snackbar.LengthLong);
                    snackbar.Show();
                }
                return null;
            }

            private View findViewById(object content)
            {
                throw new NotImplementedException();
            }

            protected override void OnPostExecute(Java.Lang.Object result)
            {

                mRecyclerView.SetAdapter(mAdapter);

            }
        }
        void RequestLocationPermission()
        {
            if (Android.Support.V4.App.ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
            {
                var layout = FindViewById(Android.Resource.Id.Content);
                Snackbar.Make(layout, Resource.String.permission_location_rationale, Snackbar.LengthIndefinite)
                        .SetAction(Resource.String.ok,
                        new Action<View>(delegate
                        {
                            Android.Support.V4.App.ActivityCompat.RequestPermissions(this, REQUIRED_PERMISSIONS,
                                                                RC_REQUEST_LOCATION_PERMISSION);
                        })
                        ).Show();
            }
            else
            {
                Android.Support.V4.App.ActivityCompat.RequestPermissions(this, REQUIRED_PERMISSIONS, RC_REQUEST_LOCATION_PERMISSION);
            }
        }
        public void GetData()
        {
            DataTable dt = new DataTable();
            WebReference.BasicHttpBinding_IService1 MyClient = new WebReference.BasicHttpBinding_IService1();
            try
            {

                WebReference.TB emp = new WebReference.TB();
                string deviceid = Android.Provider.Settings.Secure.GetString(Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                emp = MyClient.GetListDevice(deviceid);

                dt = emp.table;


                recyclelist.Clear();
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        recyclelist.Add(new GetSet(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString()));
                    }

                }

            }
            catch (Exception ex)
            {

            }
        }
        private void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {

            LoadFragment(e.Item.ItemId);

        }
        public async void LoadFragment(int id)
        {

            currentFragment = FragmentManager.FindFragmentById(Resource.Id.containerbody);
            switch (id)
            {
                case Resource.Id.action_home:

                    image.Visibility = ViewStates.Gone;
                    textview.Visibility = ViewStates.Gone;
                    recyclerView.Visibility = ViewStates.Visible;

                    break;

                case Resource.Id.action_barcode:

                    image.Visibility = ViewStates.Visible;
                    textview.Visibility = ViewStates.Visible;
                    recyclerView.Visibility = ViewStates.Gone;

                    var metrics = Resources.DisplayMetrics;
                    var writer = new BarcodeWriter
                    {
                        Format = ZXing.BarcodeFormat.QR_CODE,
                        Options = new ZXing.Common.EncodingOptions
                        {
                            Height = metrics.HeightPixels / 2 + 100,
                            Width = metrics.WidthPixels / 2 + 100
                        }
                    };
                    var bitmap = writer.Write(Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId));

                    image.SetImageBitmap(bitmap);
                    textview.Text = Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId);

                    break;
                case Resource.Id.action_scanner:

                    image.Visibility = ViewStates.Gone;
                    textview.Visibility = ViewStates.Gone;
                    recyclerView.Visibility = ViewStates.Gone;

                    scanner();

                    break;
            }

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public async void scanner()
        {

            MobileBarcodeScanner.Initialize(Application);

            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            var result = await scanner.Scan();

            if (result != null)
            {
                ISharedPreferencesEditor keyfcm = sharedPreferences.Edit();
                keyfcm.PutString("registerdeviceid", result.Text);
                keyfcm.Commit();

                Intent i = new Intent(this, typeof(RegisterListDevices));
                StartActivity(i);
            }
            View view = bottomNavigationView.FindViewById(Resource.Id.action_home);
            view.PerformClick();
            
        }
        public override void OnBackPressed()
        {
            if (countback == 0)
            {
                Toast.MakeText(this, "Tekan lagi untuk keluar", ToastLength.Short).Show();
                countback++;
            }
            else
            {

                Finish();
            }
        }
    }
}