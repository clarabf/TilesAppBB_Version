using Xamarin.Forms;

namespace TilesApp
{
    public class ExtendedButton : Button
    {
        [System.Obsolete]
        public static BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create<ExtendedButton, Xamarin.Forms.TextAlignment>(x => x.HorizontalTextAlignment, Xamarin.Forms.TextAlignment.Center);

        [System.Obsolete]
        public Xamarin.Forms.TextAlignment HorizontalTextAlignment
        {
            get
            {
                return (TextAlignment)GetValue(HorizontalTextAlignmentProperty);
            }
            set
            {
                SetValue(HorizontalTextAlignmentProperty, value);
            }
        }
    }
}
