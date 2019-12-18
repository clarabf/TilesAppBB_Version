using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace TilesApp.Droid
{
    [BroadcastReceiver(Enabled = true)]
    [Android.App.IntentFilter(actions: new[] { UsbManager.ActionUsbDeviceAttached, UsbManager.ActionUsbDeviceDetached})]
    [MetaData(UsbManager.ActionUsbDeviceAttached, Resource = "@xml/device_filter")]
    [MetaData(UsbManager.ActionUsbDeviceDetached, Resource = "@xml/device_filter")]
    class DeviceMonitor : BroadcastReceiver
    {
        UsbDevice device = MainActivity.device;
        public DeviceMonitor()
        {
        }

        public override void OnReceive(Context context, Intent intent)
        {
            UsbManager manager = (UsbManager)context.GetSystemService(Context.UsbService);
            var DeviceList = manager.DeviceList;


            // depending on the action, inform the SACOLogin.xml.cs
            if (intent.Action == UsbManager.ActionUsbDeviceAttached)
            {
                // if the device is connected, then get it from the UsbManager
                if (device == null)
                {
                    try
                    {
                        device = MainActivity.device =(DeviceList.Values.ToArray())[0];
                    }
                    catch (Exception) { }
                }
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "DeviceAttached", device);
            }
            else if (intent.Action == UsbManager.ActionUsbDeviceDetached)
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "DeviceDetached", device);
                device = MainActivity.device = null;
            }           
        }
    }



}