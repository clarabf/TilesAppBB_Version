using Android.Bluetooth;
using Android.Hardware.Usb;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TilesApp.Models;
using TilesApp.Rfid.Models;
using TilesApp.Rfid.ViewModels;
using Xamarin.Forms;

namespace TilesApp
{
    public class BasePage : ContentPage, INotifyPropertyChanged
    {
        public ReadersViewModel ReadersViewModel { get;set; }
        public ObservableCollection<string> ViewableReads { get; set; } = new ObservableCollection<string>();

        public enum ReadersTypes
        {            
            Serial1D ,
            Serial2D,
            BluetoothRFID,
            Bluetooth1D,
            Bluetooth2D
        }
        
        public BasePage(){
            this.ReadersViewModel = App.ViewModel.Readers;
            App.ViewModel.Inventory.ClearCommand.Execute(null);
            App.Inventory.Clear();
        }

        // OVERRIDES
        protected override void OnAppearing()
        {
            App.Inventory.CollectionChanged += Inventory_CollectionChanged;
            base.OnAppearing();
        }

        private void Inventory_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (var InputWithDevice in args.NewItems.Cast<Dictionary<string, object>>())
                {
                    try
                    {
                        InputDevice device = (InputDevice)InputWithDevice["Device"];
                        foreach (var compDevice in ReadersViewModel.BluetoothCameraReaders.ToList())
                        {
                            if (compDevice.Device.Name == device.Name)
                            {
                                ProcessInput((string)InputWithDevice["Value"], ReadersTypes.Bluetooth2D.ToString(), device.Descriptor);
                                return;
                            }
                        }
                        foreach (var serDevice in ReadersViewModel.SerialReaders.ToList())
                        {
                            if (serDevice.ProductId == device.ProductId)
                            {
                                ProcessInput((string)InputWithDevice["Value"], ReadersTypes.Serial1D.ToString(), device.Descriptor);
                                return;
                            }
                        }
                    }
                    catch
                    {
                        ProcessInput((string)InputWithDevice["Value"], ReadersTypes.BluetoothRFID.ToString(), App.ViewModel.Readers.Readers.FirstOrDefault().SerialNumber);
                    }
                }
            }
        }

        protected override void OnDisappearing()
        {
            // UNSUBSRCIBE WHEN PAGE IS CLOSED
            //Unsubscribe();
            App.Inventory.CollectionChanged -= Inventory_CollectionChanged;
            base.OnDisappearing();
        }
        private void ProcessInput(string code, string reader, string readerSerialNumber) {
            Dictionary<string, object> input = new Dictionary<string, object>
            {
                { nameof(BaseMetaData.InputDataProps.Value), code },
                { nameof(BaseMetaData.InputDataProps.ReaderType), reader },
                { nameof(BaseMetaData.InputDataProps.ReaderSerialNumber), readerSerialNumber },
                { nameof(BaseMetaData.InputDataProps.Timestamp), DateTime.Now }
            };
            ScannerReadDetected(input);
        }     
        
        public void CleanReaders()
        {
            ReadersViewModel = null;
            ViewableReads.Clear();
        }

        // VIRTUAL FUNCTIONS
        public virtual void ScannerReadDetected(Dictionary<string, object> input)
        {
            // This function should be implemented in child classes
        }


    }

}
