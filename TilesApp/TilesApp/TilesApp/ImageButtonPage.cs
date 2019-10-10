using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TilesApp
{
    public class ImageButtonPage : ContentPage
    {
        Label header = new Label
        {
            Text = "ImageButton",
            FontSize = 50,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center
        };

        ImageButton imageButton = new ImageButton
        {
            Source = "CameraLens.png",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.CenterAndExpand
        };

     

    }
}
