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
        public ObservableCollection<Dictionary<string, object>> InputData { get; set; } = new ObservableCollection<Dictionary<string, object>>();
        private ReadersViewModel readersViewModel;

        public enum ReadersTypes
        {            
            Serial1D,
            Serial2D,
            BluetoothRFID,
            Bluetooth1D,
            Bluetooth2D
        }
        public enum InputDataProps
        {
            Value,
            Timestamp,
            ReaderType,
        }
        public BasePage(){
            App.ViewModel.Inventory.Transponders.CollectionChanged += Transponders_CollectionChanged;
            InputData.CollectionChanged += InputData_CollectionChanged;
            this.readersViewModel = App.ViewModel.Readers;
        }




        // OVERRIDES
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.ViewModel.Inventory.ClearCommand.Execute(null);

            // SUBSCRIBE TO ALL THE EVENTS THAT MIGHT AFFECT THE PAGE
            MessagingCenter.Subscribe<Application, Dictionary<string, object>>(Application.Current, "BarcodeScanned", (s, InputWithDevice) => {
                InputDevice device = (InputDevice)InputWithDevice["Device"];
                foreach (var compDevice in readersViewModel.BluetoothCameraReaders)
                {
                    if (compDevice.Device.Name == device.Name)
                    {
                        ProcessInput((string)InputWithDevice["Value"], ReadersTypes.Bluetooth2D);
                        return;
                    }
                }
                foreach (var serDevice in readersViewModel.SerialReaders)
                {
                    if (serDevice.ProductId == device.ProductId)
                    {
                        ProcessInput((string)InputWithDevice["Value"], ReadersTypes.Serial1D);
                        return;
                    }
                }
                DisplayAlert("Error", "Reader not recognized!", "Ok");
            });
            MessagingCenter.Subscribe<Application, UsbDevice>(Application.Current, "DeviceAttached", async (s, device) => {
                if (device != null)
                {
                    readersViewModel.SerialReaders.Add(device);
                }
            });
            MessagingCenter.Subscribe<Application, UsbDevice>(Application.Current, "DeviceDetached", async (s, device) => {
                if (device != null)
                {
                    readersViewModel.SerialReaders.Clear();
                }
            });
            MessagingCenter.Subscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceFound", async (s, device) => {
                if (device != null)
                {
                    ComplexBluetoothDevice pairedDevice = new ComplexBluetoothDevice(device, ComplexBluetoothDevice.States.Paired);
                    readersViewModel.BluetoothCameraReaders.Add(pairedDevice);
                }
            });
            MessagingCenter.Subscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceConnected", async (s, device) => {
                if (device != null)
                {
                    ComplexBluetoothDevice activeDevice = new ComplexBluetoothDevice(device, ComplexBluetoothDevice.States.Active);
                    foreach (var compDevice in readersViewModel.BluetoothCameraReaders)
                    {
                        if (compDevice.Device.Address == activeDevice.Device.Address)
                        {
                            int i = readersViewModel.BluetoothCameraReaders.IndexOf(compDevice);
                            if (i != -1)
                                readersViewModel.BluetoothCameraReaders[i].State = activeDevice.State;
                        }
                    }
                }
            });
            MessagingCenter.Subscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceDisconnected", async (s, device) => {
                if (device != null)
                {
                    ComplexBluetoothDevice activeDevice = new ComplexBluetoothDevice(device, ComplexBluetoothDevice.States.Disconnected);
                    foreach (var compDevice in readersViewModel.BluetoothCameraReaders)
                    {
                        if (compDevice.Device.Address == activeDevice.Device.Address)
                        {
                            int i = readersViewModel.BluetoothCameraReaders.IndexOf(compDevice);
                            if (i != -1)
                                readersViewModel.BluetoothCameraReaders[i].State = activeDevice.State;
                        }
                    }
                }
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // UNSUBSRCIBE WHEN PAGE IS CLOSED
            MessagingCenter.Unsubscribe<Application, string>(Application.Current, "BarcodeScanned");
            MessagingCenter.Unsubscribe<Application, UsbDevice>(Application.Current, "DeviceAttached");
            MessagingCenter.Unsubscribe<Application, UsbDevice>(Application.Current, "DeviceDetached");
            MessagingCenter.Unsubscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceFound");
            MessagingCenter.Unsubscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceConnected");
            MessagingCenter.Unsubscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceDisconnected");
        }



        //EVENTS
        private void Transponders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (var item in args.NewItems.Cast<IdentifiedItem>())
                {
                    ProcessInput(item.Identifier, ReadersTypes.BluetoothRFID);
                }
            }

        }
        private void InputData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (var item in args.NewItems.Cast<Dictionary<string, object>>())
                {
                    InputDataDetected(item);
                }
            }

        }
        private void ProcessInput(string code, Enum reader) {
            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add(nameof(InputDataProps.Value), code);
            input.Add(nameof(InputDataProps.ReaderType), reader);
            input.Add(nameof(InputDataProps.Timestamp), DateTime.Now);
            InputData.Add(input);
        }      



        // VIRTUAL FUNCTIONS
        public virtual void InputDataDetected(Dictionary<string, object> input)
        {
            // This function should be implemented in child classes
        }






    }
}
