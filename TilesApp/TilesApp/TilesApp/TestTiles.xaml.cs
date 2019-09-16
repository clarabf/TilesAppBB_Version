using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TilesApp
{
    public partial class TestTiles : ContentPage
    {
        int current_tile;

        //Argument will be List<Tile>
        public TestTiles()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            // Create table with first row (etiquettes)
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
            
            // Scan the tiles table
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
                // Store tile_id (not showing in the table)
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
            ViewCell vc = (ViewCell)sender;
            Console.WriteLine(vc.ClassId);

            //Guardar tile_id
            // current_tile = vc.ClassId;

            //if !frame_code => SCANVIEW

            // else 
            // int max_steps = GetStepsCount(tile_id)
            // int task_id = GetNextTask(tile_id)
            // Step next_step = GetStep(step_id)
            // int next_step_order = next_step.order;
            // string next_step_url = next_step.url;
            // if (next_step.order == 0) TestFirstStep();
            // else if (next_step.order == max_steps) TestLastStep();
            // else TestGeneralStep()

            //////////////////////////////////// TESTS /////////////////////////////////
            current_tile = 1;
            int max_steps;
            int task_id;
            string worker;
            string next_step_url; 
            int next_step_order;

            switch (int.Parse(vc.ClassId))
            {
                // First Step
                case 0:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        task_id = 1;
                        max_steps = 12;
                        worker = "cbonillo";
                        next_step_url = "http://step.1.test.com/";
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new TestFirstStep(current_tile, task_id, max_steps, worker, next_step_url));
                    });
                    break;
                // Steps 2 - [n-1]
                case 1:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        task_id = 1;
                        max_steps = 12;
                        worker = "cbonillo";
                        next_step_order = 5;
                        next_step_url = "http://step.5.test.com/";
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new TestGeneralStep(current_tile, task_id, max_steps, next_step_order, worker, next_step_url));
                    });
                    break;
                // Last step
                case 2:
                    //List<TileTask> listSkipped = GetSkippedTasks(current_tile)
                    List<string> listSkipped = new List<string>();
                    task_id = 1;
                    max_steps = 12;
                    worker = "cbonillo";
                    next_step_url = "http://step.12.test.com/";

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new TestLastStep(listSkipped, current_tile, task_id, max_steps, worker, next_step_url));
                    });
                    break;
            }
        }
    }
}
