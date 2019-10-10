using Android.Content;
using Android.Graphics;
using TilesApp.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TilesApp.FontAwesomeIcon), typeof(FontAwesomeIconRenderer))]

namespace TilesApp.Droid
{
    class FontAwesomeIconRenderer : LabelRenderer
    {
        private readonly Context _context;

        public FontAwesomeIconRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                //The ttf in /Assets is CaseSensitive, so name it FontAwesome.ttf
                Control.Typeface = Typeface.CreateFromAsset(_context.Assets, FontAwesomeIcon.Typeface + ".ttf");
            }
        }
    }
}