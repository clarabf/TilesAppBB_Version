using Android.Bluetooth;
using Android.Hardware.Usb;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace TilesApp
{
    public class BasePage : ContentPage
    {
        public BasePage(){
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
                    App.ViewModel.Readers.BluetoothCameraReaders.Add(device);
                }
            });
            MessagingCenter.Subscribe<Application, UsbDevice>(Application.Current, "BluetoothDeviceLost", async (s, device) => {
                if (device != null)
                {
                    App.ViewModel.Readers.BluetoothCameraReaders.Clear();
                }
            });
        }
    }
}
