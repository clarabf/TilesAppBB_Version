﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TilesApp.Models;
using Xamarin.Forms;
using System.Linq;

namespace TilesApp
{
    public partial class StepsPage : ContentPage
    {

        Tile tile;
        int task_id;
        int max_steps;
        int current_step;
        string worker;
        string pdf;
        Styles styles = new Styles();
        private double width = 0;
        private double height = 0;

        public StepsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            width = this.Width;
            height = this.Height;
        }

        public StepsPage(Tile t, int t_id, int m_steps, string wor, string url, int s_order)
        {
            InitializeComponent();
            tile = t;
            task_id = t_id;
            max_steps = m_steps;
            current_step = s_order;
            worker = wor;
            pdf = url;

            for (int i = 0; i < m_steps; i++)
            {
                Style s;
                if (i == (current_step - 1)) s = styles.selectedStyle;
                else if (i < (current_step - 1)) s = styles.alreadyDoneStyle;
                else s = styles.unselectedStyle;

                var button = new Button()
                {
                    Text = $"{i + 1}",
                    ClassId = $"{i + 1}",
                    Style = s
                };

                button.Clicked += Handle_Clicked;
                stepBar.Children.Add(button);

                if (i < m_steps - 1)
                {
                    var separatorLine = new BoxView()
                    {
                        BackgroundColor = Color.Silver,
                        HeightRequest = 1,
                        WidthRequest = 5,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    };
                    stepBar.Children.Add(separatorLine);
                }
            }

            skiplabel.Text = "Step " + current_step + "/" + max_steps;
            Device.BeginInvokeOnMainThread(() =>
            {
                pdfViewer.Source = new UrlWebViewSource() { Url = "http://drive.google.com/viewerng/viewer?embedded=true&url=" + url };
            });

            //// TESTS for now
            if (worker=="wrong")
            {
                WRONGView.IsVisible = true;
            }
            else
            {
                if (current_step == 4) SCANNEDView.IsVisible = true;
                else if (current_step > 4 && worker=="show") CONTINUATIONView.IsVisible = true;
            }

            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void Handle_Clicked(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            if (b.Text=="3")
            {
                Tile t = new Tile();
                t.id = 4;
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopModalAsync(true);
                    Navigation.PushModalAsync(new ScanQR(t,"STEP 3/21\nSCAN STICKER QR", 2));
                });
            }
            else if (b.Text == max_steps.ToString())
            {
                COMPLETEDView.IsVisible = true;
            }
            else
            {
                b.Style = styles.selectedStyle;
                string next_step_url = "http://oboria.net/docs/pdf/ftp/6/" + b.Text + ".PDF";
                skiplabel.Text = "Step " + b.Text + "/" + max_steps;
                pdfViewer.Source = new UrlWebViewSource() { Url = "http://drive.google.com/viewerng/viewer?embedded=true&url=" + next_step_url };

                var buttons = stepBar.Children.Where(x => x is Button).ToList();
                foreach (Button bu in buttons)
                {
                    if (int.Parse(bu.ClassId) < int.Parse(b.ClassId)) bu.Style = styles.alreadyDoneStyle;
                    else if (int.Parse(bu.ClassId) > int.Parse(b.ClassId)) bu.Style = styles.unselectedStyle;
                    bu.CornerRadius = 40;
                }
            }
        }

        private void PausePressed( object sender, EventArgs args)
        {
            PAUSEView.IsVisible = true;
        }

        private void Pause_Interrupt(object sender, EventArgs args)
        {
            PAUSEView.IsVisible = false;
            WARNINGView.IsVisible = true;
        }

        private void Pause_Resume(object sender, EventArgs args)
        {
            PAUSEView.IsVisible = false;
        }

        private async void Warning_Confirm(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

        private void Warning_Cancel(object sender, EventArgs args)
        {
            WARNINGView.IsVisible = false;
        }

        private async void Complete_Confirm(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

        private void Complete_Cancel(object sender, EventArgs args)
        {
            COMPLETEDView.IsVisible = false;
        }

        private void Retry(object sender, EventArgs args)
        {
            Tile t = new Tile();
            t.id = 2;
            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PopModalAsync(true);
                Navigation.PushModalAsync(new ScanQR(t, "STEP 3/21\nSCAN STICKER QR", 2));
            });
        }

        private void Scanned_Confirm(object sender, EventArgs args)
        {
            SCANNEDView.IsVisible = false;
        }

        private void Continuation_Confirm(object sender, EventArgs args)
        {
            Tile t = new Tile();
            t.id = current_step;
            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PopModalAsync(true);
                Navigation.PushModalAsync(new ScanQR(t, "RESTART OF WORK\nSCAN STICKER QR", 2));
            });
        }
    }
}
