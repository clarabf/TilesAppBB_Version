using Xamarin.Forms;

namespace TilesApp.Models
{
    public class Styles
    {
        public Style unselectedStyle = new Style(typeof(Button))
        {
            Setters = {
                    new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.LightGray },
                    new Setter { Property = Button.TextColorProperty,   Value = Color.White },
                    new Setter { Property = Button.BorderColorProperty,   Value = Color.LightGray },
                    new Setter { Property = Button.BorderWidthProperty,   Value = 0.5 },
                    new Setter { Property = Button.CornerRadiusProperty,   Value = 20 },
                    new Setter { Property = VisualElement.HeightRequestProperty,   Value = 40 },
                    new Setter { Property = VisualElement.WidthRequestProperty,   Value = 40 }
                }
        };

        public Style selectedStyle = new Style(typeof(Button))
        {
            Setters = {
                    new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.Red },
                    new Setter { Property = Button.TextColorProperty, Value = Color.White },
                    new Setter { Property = Button.BorderColorProperty, Value = Color.Red },
                    new Setter { Property = Button.BorderWidthProperty,   Value = 0.5 },
                    new Setter { Property = Button.CornerRadiusProperty,   Value = 20 },
                    new Setter { Property = VisualElement.HeightRequestProperty,   Value = 40 },
                    new Setter { Property = VisualElement.WidthRequestProperty,   Value = 40 },
                    new Setter { Property = Button.FontAttributesProperty,   Value = FontAttributes.Bold }
                }
        };

    }
}
