using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design;
using Android.Support.Design.Internal;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace GetLocDevice
{
    class BottomNavigationViewHelper
    {
        public static void disableShiftMode(BottomNavigationView view)
        {
            BottomNavigationMenuView menuView = (BottomNavigationMenuView)view.GetChildAt(0);
            try
            {

                for (int i = 0; i < menuView.ChildCount; i++)
                {
                    BottomNavigationItemView item = (BottomNavigationItemView)menuView.GetChildAt(i);
                    //noinspection RestrictedApi
                    item.SetShiftingMode(false);
                    // set once again checked value, so view will be updated
                    //noinspection RestrictedApi
                    item.SetChecked(item.ItemData.IsChecked);
                }
            }
            catch (NoSuchFieldException e)
            {
                //Log.("BNVHelper", "Unable to get shift mode field", e);
            }
            catch (IllegalAccessException e)
            {
               // Log.e("BNVHelper", "Unable to change value of shift mode", e);
            }
        }
    }
}