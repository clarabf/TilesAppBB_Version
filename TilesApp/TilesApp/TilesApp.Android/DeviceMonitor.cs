using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using PCLAppConfig;
using Xamarin.Forms;

namespace TilesApp.Droid
{
    [BroadcastReceiver(Enabled = true)]
    [Android.App.IntentFilter(actions: new[] { UsbManager.ActionUsbDeviceAttached, UsbManager.ActionUsbDeviceDetached, BluetoothDevice.ActionFound, BluetoothDevice.ActionAclConnected, BluetoothDevice.ActionAclDisconnected, Intent.ActionPowerConnected, Intent.ActionPowerDisconnected })]
    [MetaData(UsbManager.ActionUsbDeviceAttached, Resource = "@xml/device_filter")]
    [MetaData(UsbManager.ActionUsbDeviceDetached, Resource = "@xml/device_filter")]
    class DeviceMonitor : BroadcastReceiver
    {
        UsbDevice device = MainActivity.device;
        List<string> vendorIds = new List<string>();
        List<string> productIds = new List<string>();
        List<string> ouiVendorIds = new List<string>();
        public DeviceMonitor()
        {
            vendorIds = new List<string>(ConfigurationManager.AppSettings["VENDOR_IDS"].Split(new char[] { ';' }));
            productIds = new List<string>(ConfigurationManager.AppSettings["PRODUCT_IDS"].Split(new char[] { ';' }));
            ouiVendorIds = new List<string>(ConfigurationManager.AppSettings["OUI_VENDOR_IDS"].Split(new char[] { ';' }));
        }

        public override void OnReceive(Context context, Intent intent)
        {
            UsbManager manager = (UsbManager)context.GetSystemService(Context.UsbService);
            var DeviceList = manager.DeviceList;
            BluetoothDevice bluetoothDevice;
            string[] mac;
            string oui;
            // depending on the action, inform the Application
            switch (intent.Action)
            {
                case UsbManager.ActionUsbDeviceAttached:
                    // if the device is connected, then get it from the UsbManager
                    if (device == null)
                    {
                        try
                        {
                            device = MainActivity.device = (DeviceList.Values.ToArray())[0];
                        }
                        catch (Exception) { }
                    }
                    if (vendorIds.Contains(device.VendorId.ToString()) && productIds.Contains(device.ProductId.ToString()))
                    {
                        MessagingCenter.Send(Xamarin.Forms.Application.Current, "DeviceAttached", device);
                    }
                    
                    break;
                case UsbManager.ActionUsbDeviceDetached:
                    MessagingCenter.Send(Xamarin.Forms.Application.Current, "DeviceDetached", device);
                    device = MainActivity.device = null;
                    break;
                case BluetoothDevice.ActionFound:
                    // Get found bluetooth device
                    bluetoothDevice = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    mac = bluetoothDevice.Address.Split(':');
                    oui = mac[0] + mac[1] + mac[2];
                    if (ouiVendorIds.Contains(oui))
                        MessagingCenter.Send(Xamarin.Forms.Application.Current, "BluetoothDeviceFound", bluetoothDevice);
                    break;
                case BluetoothDevice.ActionAclConnected:
                    // Get connected bluetooth device
                    bluetoothDevice = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    mac = bluetoothDevice.Address.Split(':');
                    oui = mac[0] + mac[1] + mac[2];
                    if (ouiVendorIds.Contains(oui))
                        MessagingCenter.Send(Xamarin.Forms.Application.Current, "BluetoothDeviceConnected", bluetoothDevice);
                    break;
                case BluetoothDevice.ActionAclDisconnected:
                    // Get disconnected bluetooth device
                    bluetoothDevice = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    if (bluetoothDevice.Type == BluetoothDeviceType.Le)
                        MessagingCenter.Send(Xamarin.Forms.Application.Current, "BluetoothDeviceDisconnected", bluetoothDevice);
                    break;
                case Intent.ActionPowerConnected:
                    MessagingCenter.Send(Xamarin.Forms.Application.Current, "ChargerConnected", true);
                    break;
                case Intent.ActionPowerDisconnected:
                    MessagingCenter.Send(Xamarin.Forms.Application.Current, "ChargerDisconnected", true);
                    break;
                default:
                    break;
            }


        }

    }



}