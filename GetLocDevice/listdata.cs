using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace GetLocDevice.FragmentAct
{
    public class listdata : Fragment, IOnMapReadyCallback
    {
        static View itemView;
        private GoogleMap GMap;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            itemView = LayoutInflater.From(container.Context).
                Inflate(Resource.Layout.listdata, container, false);

            InitMapFragment();

            /*MapFragment mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.maps);
            mapFragment.GetMapAsync(this);*/

            return itemView;
        }
        private void InitMapFragment()
        {
            MapFragment mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
            if (mapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeSatellite)
                    .InvokeZoomControlsEnabled(false)
                    .InvokeCompassEnabled(true);

                FragmentTransaction fragTx = FragmentManager.BeginTransaction();
                mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.map, mapFragment, "map");
                fragTx.Commit();
            }
            mapFragment.GetMapAsync(this);
        }
        /*public void OnMapReady(GoogleMap googleMap)
        {
            this.GMap = googleMap;
            GMap.UiSettings.ZoomControlsEnabled = true;

            LatLng latlng = new LatLng(Convert.ToDouble(13.0291), Convert.ToDouble(80.2083));
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, 15);
            GMap.MoveCamera(camera);
            MarkerOptions options = new MarkerOptions().SetPosition(latlng).SetTitle("Chennai");
            GMap.AddMarker(options);
        }*/
        public void OnMapReady(GoogleMap googleMap)
        {
            this.GMap = googleMap;
        }
    }
}