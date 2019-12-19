using Android.Bluetooth;
using Android.Hardware.Usb;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TilesApp.Models;
using TilesApp.Rfid.Models;
using Xamarin.Forms;

namespace TilesApp
{
    public class BasePage : ContentPage, INotifyPropertyChanged
    {

        public BasePage(){
            App.ViewModel.Inventory.Transponders.CollectionChanged += Transponders_CollectionChanged;

        }




        // OVERRIDES
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.ViewModel.Inventory.ClearCommand.Execute(null);

            // SUBSCRIBE TO ALL THE EVENTS THAT MIGHT AFFECT THE PAGE
            MessagingCenter.Subscribe<Application, String>(Application.Current, "BarcodeScanned", (s, a) => {
                BarcodeDetected(a);
            });
            MessagingCenter.Subscribe<Application, UsbDevice>(Application.Current, "DeviceAttached", async (s, device) => {
                if (device != null)
                {
                    App.ViewModel.Readers.SerialReaders.Add(device);
                }
            });
            MessagingCenter.Subscribe<Application, UsbDevice>(Application.Current, "DeviceDetached", async (s, device) => {
                if (device != null)
                {
                    App.ViewModel.Readers.SerialReaders.Clear();
                }
            });
            MessagingCenter.Subscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceFound", async (s, device) => {
                if (device != null)
                {
                    ComplexBluetoothDevice pairedDevice = new ComplexBluetoothDevice(device, ComplexBluetoothDevice.States.Paired);
                    App.ViewModel.Readers.BluetoothCameraReaders.Add(pairedDevice);
                }
            });
            MessagingCenter.Subscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceConnected", async (s, device) => {
                if (device != null)
                {
                    ComplexBluetoothDevice activeDevice = new ComplexBluetoothDevice(device, ComplexBluetoothDevice.States.Active);
                    foreach (var compDevice in App.ViewModel.Readers.BluetoothCameraReaders)
                    {
                        if (compDevice.Device.Address == activeDevice.Device.Address)
                        {
                            int i = App.ViewModel.Readers.BluetoothCameraReaders.IndexOf(compDevice);
                            if (i != -1)
                                App.ViewModel.Readers.BluetoothCameraReaders[i].State = activeDevice.State;
                        }
                    }
                }
            });
            MessagingCenter.Subscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceDisconnected", async (s, device) => {
                if (device != null)
                {
                    ComplexBluetoothDevice activeDevice = new ComplexBluetoothDevice(device, ComplexBluetoothDevice.States.Disconnected);
                    foreach (var compDevice in App.ViewModel.Readers.BluetoothCameraReaders)
                    {
                        if (compDevice.Device.Address == activeDevice.Device.Address)
                        {
                            int i = App.ViewModel.Readers.BluetoothCameraReaders.IndexOf(compDevice);
                            if (i != -1)
                                App.ViewModel.Readers.BluetoothCameraReaders[i].State = activeDevice.State;
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
                    TransponderDetected(item.Identifier);
                }
            }

        }

        public virtual void BarcodeDetected(string code) {
            // This function should be implemented in child classes
        }
        public virtual void TransponderDetected(string transponder)
        {
            // This function should be implemented in child classes
        }









    }
}
