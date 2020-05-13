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

namespace GetLocDevice.GetterSetter
{
    public class GetSet
    {
        //public byte[] gambar;
        public string parent_device_id;
        public string user_id;
        public GetSet()
        {

        }

        public GetSet(string pdeviceid, string puserid)
        {
            this.parent_device_id = pdeviceid;
            this.user_id = puserid;
        }
        public string getparentdeviceid() { return parent_device_id; }
        public void Setparentdeviceid(string parent_device_id) { this.parent_device_id = parent_device_id; }
        public string getuserid() { return user_id; }
        public void Setuserid(string user_id) { this.user_id = user_id; }

    }
}