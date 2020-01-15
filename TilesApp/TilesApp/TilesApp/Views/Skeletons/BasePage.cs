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
        public ObservableCollection<Dictionary<string, object>> ScannerReads { get; set; } = new ObservableCollection<Dictionary<string, object>>();
        //public BaseData BaseData  = new BaseData();
        private ReadersViewModel readersViewModel;

        public enum ReadersTypes
        {            
            Serial1D ,
            Serial2D,
            BluetoothRFID,
            Bluetooth1D,
            Bluetooth2D
        }
        
        public BasePage(){
            App.ViewModel.Inventory.Transponders.CollectionChanged += Transponders_CollectionChanged;
            ScannerReads.CollectionChanged += ScannerReads_CollectionChanged;
            this.readersViewModel = App.ViewModel.Readers;
            subscribe();      
        }

        // OVERRIDES
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.ViewModel.Inventory.ClearCommand.Execute(null);
            subscribe();
        }

        protected override void OnDisappearing()
        {
            // UNSUBSRCIBE WHEN PAGE IS CLOSED
            MessagingCenter.Unsubscribe<Application, string>(Application.Current, "BarcodeScanned");
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
        private void Transponders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (var item in args.NewItems.Cast<IdentifiedItem>())
                {
                    ProcessInput(item.Identifier, ReadersTypes.BluetoothRFID.ToString());
                }
            }

        }

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
        private void ProcessInput(string code, string reader) {
            foreach (var item in ScannerReads.ToList())
            {
                if (item[nameof(BaseData.InputDataProps.Value)].ToString() == code)
                {
                    return;
                }
            }
            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add(nameof(BaseData.InputDataProps.Value), code);
            input.Add(nameof(BaseData.InputDataProps.ReaderType), reader);
            input.Add(nameof(BaseData.InputDataProps.Timestamp), DateTime.Now);
            ScannerReads.Add(input);
        }      

        // VIRTUAL FUNCTIONS
        public virtual void ScannerReadDetected(Dictionary<string, object> input)
        {
            // This function should be implemented in child classes
        }

        private void subscribe() {

            // SUBSCRIBE TO ALL THE EVENTS THAT MIGHT AFFECT THE PAGE
            MessagingCenter.Subscribe<Application, Dictionary<string, object>>(Application.Current, "BarcodeScanned", (s, InputWithDevice) => {
                InputDevice device = (InputDevice)InputWithDevice["Device"];
                foreach (var compDevice in readersViewModel.BluetoothCameraReaders.ToList())
                {
                    if (compDevice.Device.Name == device.Name)
                    {
                        ProcessInput((string)InputWithDevice["Value"], ReadersTypes.Bluetooth2D.ToString());
                        return;
                    }
                }
                foreach (var serDevice in readersViewModel.SerialReaders.ToList())
                {
                    if (serDevice.ProductId == device.ProductId)
                    {
                        ProcessInput((string)InputWithDevice["Value"], ReadersTypes.Serial1D.ToString());
                        return;
                    }
                }
                DisplayAlert("Error", "Reader not recognized!", "Ok");
            });
            MessagingCenter.Subscribe<Application, UsbDevice>(Application.Current, "DeviceAttached", async (s, device) => {
                if (device != null)
                {
                    foreach (var serialDevice in readersViewModel.SerialReaders.ToList())
                    {
                        if (serialDevice.SerialNumber == device.SerialNumber)
                        {
                            return;
                        }
                    }
                    readersViewModel.SerialReaders.Add(device);
                }
            });
            MessagingCenter.Subscribe<Application, UsbDevice>(Application.Current, "DeviceDetached", async (s, device) => {
                if (device != null)
                {
                    foreach (var serialDevice in readersViewModel.SerialReaders.ToList())
                    {
                        if (serialDevice.SerialNumber == device.SerialNumber)
                        {
                            readersViewModel.SerialReaders.Remove(serialDevice);
                        }
                    }
                }
            });
            MessagingCenter.Subscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceFound", async (s, device) => {
                if (device != null)
                {
                    ComplexBluetoothDevice pairedDevice = new ComplexBluetoothDevice(device, ComplexBluetoothDevice.States.Paired);
                    foreach (var compDevice in readersViewModel.BluetoothCameraReaders.ToList())
                    {
                        if (compDevice.Device.Address == pairedDevice.Device.Address)
                        {
                            int i = readersViewModel.BluetoothCameraReaders.IndexOf(compDevice);
                            if (i != -1)
                                readersViewModel.BluetoothCameraReaders[i].State = pairedDevice.State;
                            return;
                        }
                    }
                    readersViewModel.BluetoothCameraReaders.Add(pairedDevice);
                }
            });
            MessagingCenter.Subscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceConnected", async (s, device) => {
                if (device != null)
                {
                    ComplexBluetoothDevice activeDevice = new ComplexBluetoothDevice(device, ComplexBluetoothDevice.States.Active);
                    foreach (var compDevice in readersViewModel.BluetoothCameraReaders.ToList())
                    {
                        if (compDevice.Device.Address == activeDevice.Device.Address)
                        {
                            int i = readersViewModel.BluetoothCameraReaders.IndexOf(compDevice);
                            if (i != -1)
                                readersViewModel.BluetoothCameraReaders[i].State = activeDevice.State;
                            return;
                        }
                    }
                    readersViewModel.BluetoothCameraReaders.Add(activeDevice);
                }
            });
            MessagingCenter.Subscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceDisconnected", async (s, device) => {
                if (device != null)
                {
                    ComplexBluetoothDevice inActiveDevice = new ComplexBluetoothDevice(device, ComplexBluetoothDevice.States.Disconnected);
                    foreach (var compDevice in readersViewModel.BluetoothCameraReaders.ToList())
                    {
                        if (compDevice.Device.Address == inActiveDevice.Device.Address)
                        {
                            int i = readersViewModel.BluetoothCameraReaders.IndexOf(compDevice);
                            if (i != -1)
                                readersViewModel.BluetoothCameraReaders[i].State = inActiveDevice.State;
                        }
                    }
                }
            });
            MessagingCenter.Subscribe<Application, bool>(Application.Current, "ChargerConnected", async (s, chargerConnected) => {
                if (chargerConnected)
                {
                    Console.WriteLine("============Charger was connected!===========");
                }
            });
            MessagingCenter.Subscribe<Application, bool>(Application.Current, "ChargerDisconnected", async (s, chargerDisconnected) => {
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
