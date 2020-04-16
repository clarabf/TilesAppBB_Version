using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesApp.Models.DataModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TilesApp.Views.Other_Functionalities
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PendingOperations : ContentPage
    {
        List<PendingOperation> opts = new List<PendingOperation>();
        public ObservableCollection<PendingOperation> PendingOpts { get; set; } = new ObservableCollection<PendingOperation>();
        public PendingOperations()
        {
            InitializeComponent();
            BindingContext = this;
            opts = App.Database.GetPendingOperations();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (opts.Count > 0)
            {
                lblTitleLine.IsVisible = true;
            }
            foreach (PendingOperation opt in opts)
            {
                PendingOpts.Add(opt);
            }
        }
    }
}