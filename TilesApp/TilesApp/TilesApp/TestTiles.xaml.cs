using System;
using Xamarin.Forms;

namespace TilesApp
{
    public partial class TestTiles : ContentPage
    {
        int current_tile;

        //Como argumento la tabla de tiles
        public TestTiles()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            var table = new TableView();
            table.BackgroundColor = Color.MediumAquamarine;
            table.Intent = TableIntent.Settings;
            var layout = new StackLayout() { Orientation = StackOrientation.Horizontal };
            layout.Children.Add(new Label()
            {
                Text = "Tile type",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.StartAndExpand
            });
            layout.Children.Add(new Label()
            {
                Text = "Frame code",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            });
            layout.Children.Add(new Label()
            {
                Text = "Current step",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.EndAndExpand
            });
            TableRoot troot = new TableRoot();
            TableSection section = new TableSection();
            ViewCell viewCell;
            // Adding first row of the table
            section.Add(new ViewCell() { View = layout });
            for (int i = 0; i < 3; i++)
            {
                // Format tiles information
                layout = new StackLayout() { Orientation = StackOrientation.Horizontal };
                layout.Children.Add(new Label()
                {
                    Text = i.ToString(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.StartAndExpand
                });
                layout.Children.Add(new Label()
                {
                    Text = "Code" + i,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.CenterAndExpand
                });
                layout.Children.Add(new Label()
                {
                    Text = "Step" + i,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.EndAndExpand
                });
                viewCell = new ViewCell();
                viewCell.ClassId = i.ToString();
                viewCell.Tapped += TileSelected;
                viewCell.View = layout;
                section.Add(viewCell);
            }
            troot.Add(section);
            table.Root = troot;
            Content = table;
        }

        private async void TileSelected(object sender, EventArgs args)
        {
            ViewCell b = (ViewCell)sender;
            Console.WriteLine(b.ClassId);

            //Guardar tile_id
            current_tile = 1;

            //if !frame_code => SCANVIEW

            // else 
            // int max_steps = GetStepsCount(tile_id)
            int max_steps = 7;
            // GetNextTask(tile_id)
            // next_step = GetStep(step_id)
            // if (next_step == 0) TestFirstStep();
            // else if (next_step == max_steps) TestLastStep();
            // else TestGeneralStep()
            switch (int.Parse(b.ClassId))
            {
                case 0:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new TestFirstStep(current_tile, max_steps));
                    });
                    break;
                case 1:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        //Navigation.PushModalAsync(new TestGeneralStep());
                    });
                    break;
                case 2:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        //Navigation.PushModalAsync(new TestLastStep());
                    });
                    break;
            }
        }
    }
}
