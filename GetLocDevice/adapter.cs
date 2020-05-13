using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GetLocDevice.GetterSetter;

namespace GetLocDevice.Adapter
{
    class adapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick, ItemLongClick;
        List<GetSet> recyclelist;

        public adapter(List<GetSet> Recyclelist)
        {
            this.recyclelist = Recyclelist;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                     Inflate(Resource.Layout.listdevices, parent, false);

            RecycleViewHolderInbox vh = new RecycleViewHolderInbox(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {

            RecycleViewHolderInbox vh = holder as RecycleViewHolderInbox;

            vh.parentdeviceid.Text = recyclelist[position].getparentdeviceid();
            vh.userid.Text = recyclelist[position].getuserid();
        }


        public override int ItemCount
        {
            get { return recyclelist == null ? 0 : recyclelist.Count; }
        }
        void OnClick(int position)
        {
            ItemClick(this, position);
        }
        void OnLongClick(int position)
        {
            ItemLongClick(this, position);
        }
    }
    public class RecycleViewHolderInbox : RecyclerView.ViewHolder
    {
        public int count = 0;
        //public ImageView circleView { get; private set; }
        public TextView parentdeviceid { get; private set; }
        public TextView userid { get; private set; }

        public RecycleViewHolderInbox(View itemView, Action<int> listener) : base(itemView)
        {
            //circleView = itemView.FindViewById<ImageView>(Resource.Id.circleView);
            parentdeviceid = itemView.FindViewById<TextView>(Resource.Id.parentdeviceid);
            userid = itemView.FindViewById<TextView>(Resource.Id.userid);

            itemView.Click += (sender, e) => listener(base.LayoutPosition);

        }
    }
}