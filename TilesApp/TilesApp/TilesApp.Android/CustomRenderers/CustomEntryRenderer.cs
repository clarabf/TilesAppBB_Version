using Android.Content;
using Android.Views;
using System.ComponentModel;
using TilesApp;
using TilesApp.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace TilesApp.Droid
{

    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Entry> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null) return;
            Control.SetPadding(Control.PaddingLeft, Control.PaddingTop, Control.PaddingRight, 50);
        }
    }
}