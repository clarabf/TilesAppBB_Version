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
                    //App.ViewModel.Readers.SerialReaders.Add(device);
                    await DisplayAlert("Device was connected",
                     "Name : " + device.DeviceName
                     , "Close alert");
                }
            });
            MessagingCenter.Subscribe<Application, UsbDevice>(Application.Current, "DeviceDetached", async (s, device) => {
                if (device != null)
                {
                    //App.ViewModel.Readers.SerialReaders.Clear();
                    await DisplayAlert("Device was disconnected",
                     "Name : " + device.DeviceName
                     , "Close alert");
                }
            });
        }
    }
}
