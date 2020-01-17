using Android.Bluetooth;
using Android.Hardware.Usb;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TilesApp.Models;
using TilesApp.Rfid.Models;
using TilesApp.Rfid.ViewModels;
using TilesApp.Services;
using Xamarin.Essentials;
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
            Subscribe();      
        }

        // OVERRIDES
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.ViewModel.Inventory.ClearCommand.Execute(null);
            Subscribe();
        }

        protected override void OnDisappearing()
        {
            // UNSUBSRCIBE WHEN PAGE IS CLOSED
            MessagingCenter.Unsubscribe<Application, string>(Application.Current, "BarcodeScanned");
            MessagingCenter.Unsubscribe<Application, string>(Application.Current, "EpcScanned");
            MessagingCenter.Unsubscribe<Application, UsbDevice>(Application.Current, "DeviceAttached");
            MessagingCenter.Unsubscribe<Application, UsbDevice>(Application.Current, "DeviceDetached");
            MessagingCenter.Unsubscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceFound");
            MessagingCenter.Unsubscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceConnected");
            MessagingCenter.Unsubscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceDisconnected");
            MessagingCenter.Unsubscribe<Application, BluetoothDevice>(Application.Current, "ChargerConnected");
            MessagingCenter.Unsubscribe<Application, BluetoothDevice>(Application.Current, "ChargerDisconnected");
            base.OnDisappearing();
        }

        //EVENTS

        private void ScannerReads_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (var item in args.NewItems.Cast<Dictionary<string, object>>())
                {
                    ScannerReadDetected(item);
                }
            }

        }

        //CHECK HOW TO DO IT. PROCESS INPUT NOW AVAILABLE IN METADATA. MAKES USE OF VALID CODE STRUCTURE
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

        // VIRTUAL FUNCTIONS
        public virtual void ScannerReadDetected(Dictionary<string, object> input)
        {
            // This function should be implemented in child classes
        }

        private void Subscribe() {

            // SUBSCRIBE TO ALL THE EVENTS THAT MIGHT AFFECT THE PAGE
            MessagingCenter.Subscribe<Application, Dictionary<string, object>>(Application.Current, "BarcodeScanned", (s, InputWithDevice) => {
                InputDevice device = (InputDevice)InputWithDevice["Device"];
                foreach (var compDevice in ReadersViewModel.BluetoothCameraReaders.ToList())
                {
                    if (compDevice.Device.Name == device.Name)
                    {
                        ProcessInput((string)InputWithDevice["Value"], ReadersTypes.Bluetooth2D.ToString(),device.Descriptor);
                        return;
                    }
                }
                foreach (var serDevice in ReadersViewModel.SerialReaders.ToList())
                {
                    if (serDevice.ProductId == device.ProductId)
                    {
                        ProcessInput((string)InputWithDevice["Value"], ReadersTypes.Serial1D.ToString(),device.Descriptor);
                        return;
                    }
                }
                DisplayAlert("Error", "Reader not recognized!", "Ok");
            });
            // Catch input from the RFID Reader
            MessagingCenter.Subscribe<Application, string>(Application.Current, "EpcScanned", (s, Input) => {
                        ProcessInput(Input, ReadersTypes.BluetoothRFID.ToString(),App.ViewModel.Readers.Readers.FirstOrDefault().SerialNumber);
                        return;                  
            });            
            MessagingCenter.Subscribe<Application, UsbDevice>(Application.Current, "DeviceAttached", (s, device) => {
                if (device != null)
                {
                    foreach (var serialDevice in ReadersViewModel.SerialReaders.ToList())
                    {
                        if (serialDevice.SerialNumber == device.SerialNumber)
                        {
                            return;
                        }
                    }
                    ReadersViewModel.SerialReaders.Add(device);
                }
            });
            MessagingCenter.Subscribe<Application, UsbDevice>(Application.Current, "DeviceDetached", (s, device) => {
                if (device != null)
                {
                    foreach (var serialDevice in ReadersViewModel.SerialReaders.ToList())
                    {
                        if (serialDevice.SerialNumber == device.SerialNumber)
                        {
                            ReadersViewModel.SerialReaders.Remove(serialDevice);
                        }
                    }
                }
            });
            MessagingCenter.Subscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceFound", (s, device) => {
                if (device != null)
                {
                    ComplexBluetoothDevice pairedDevice = new ComplexBluetoothDevice(device, ComplexBluetoothDevice.States.Paired);
                    foreach (var compDevice in ReadersViewModel.BluetoothCameraReaders.ToList())
                    {
                        if (compDevice.Device.Address == pairedDevice.Device.Address)
                        {
                            int i = ReadersViewModel.BluetoothCameraReaders.IndexOf(compDevice);
                            if (i != -1)
                                ReadersViewModel.BluetoothCameraReaders[i].State = pairedDevice.State;
                            return;
                        }
                    }
                    ReadersViewModel.BluetoothCameraReaders.Add(pairedDevice);
                }
            });
            MessagingCenter.Subscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceConnected", (s, device) => {
                if (device != null)
                {
                    ComplexBluetoothDevice activeDevice = new ComplexBluetoothDevice(device, ComplexBluetoothDevice.States.Active);
                    foreach (var compDevice in ReadersViewModel.BluetoothCameraReaders.ToList())
                    {
                        if (compDevice.Device.Address == activeDevice.Device.Address)
                        {
                            int i = ReadersViewModel.BluetoothCameraReaders.IndexOf(compDevice);
                            if (i != -1)
                                ReadersViewModel.BluetoothCameraReaders[i].State = activeDevice.State;
                            return;
                        }
                    }
                    ReadersViewModel.BluetoothCameraReaders.Add(activeDevice);
                }
            });
            MessagingCenter.Subscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceDisconnected", (s, device) => {
                if (device != null)
                {
                    ComplexBluetoothDevice inActiveDevice = new ComplexBluetoothDevice(device, ComplexBluetoothDevice.States.Disconnected);
                    foreach (var compDevice in ReadersViewModel.BluetoothCameraReaders.ToList())
                    {
                        if (compDevice.Device.Address == inActiveDevice.Device.Address)
                        {
                            int i = ReadersViewModel.BluetoothCameraReaders.IndexOf(compDevice);
                            if (i != -1)
                                ReadersViewModel.BluetoothCameraReaders[i].State = inActiveDevice.State;
                        }
                    }
                }
            });
            MessagingCenter.Subscribe<Application, bool>(Application.Current, "ChargerConnected", (s, chargerConnected) => {
                if (chargerConnected)
                {
                    Console.WriteLine("============Charger was connected!===========");
                }
            });
            MessagingCenter.Subscribe<Application, bool>(Application.Current, "ChargerDisconnected", (s, chargerDisconnected) => {
                if (chargerDisconnected)
                {
                    Console.WriteLine("============Charger was disconnected!===========");
                }
            });
            MessagingCenter.Subscribe<Application, string>(Application.Current, "Error", async (s, errorMessage) => {
                await DisplayAlert("Error", errorMessage, "Ok");
            });
        }
    }
}
