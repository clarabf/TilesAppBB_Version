﻿using Android.Content;
using TilesApp;
using TilesApp.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(NoUnderlineSearchBar), typeof(NoUnderlineSearchBarRenderer))]
namespace TilesApp.Droid
{
    public class NoUnderlineSearchBarRenderer : SearchBarRenderer
    {
        public NoUnderlineSearchBarRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var plateId = Resources.GetIdentifier("android:id/search_plate", null, null);
                var plate = Control.FindViewById(plateId);
                plate.SetBackgroundColor(Android.Graphics.Color.Transparent);
                //this.Control.SetBackgroundColor(Android.Graphics.Color.Argb(0, 0, 0, 0));
            }
        }
    }
}