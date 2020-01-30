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
using TilesApp.Models;
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
        List<string> ouiTransportIds = new List<string>();
        public DeviceMonitor()
        {
            vendorIds = new List<string>(ConfigurationManager.AppSettings["VENDOR_IDS"].Split(new char[] { ';' }));
            productIds = new List<string>(ConfigurationManager.AppSettings["PRODUCT_IDS"].Split(new char[] { ';' }));
            ouiVendorIds = new List<string>(ConfigurationManager.AppSettings["OUI_VENDOR_IDS"].Split(new char[] { ';' }));
            ouiTransportIds = new List<string>(ConfigurationManager.AppSettings["OUI_TRANSPORTS_IDS"].Split(new char[] { ';' }));
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
                        foreach (var serialDevice in App.ViewModel.Readers.SerialReaders.ToList())
                        {
                            if (serialDevice.SerialNumber == device.SerialNumber)
                            {
                                return;
                            }
                        }
                        App.ViewModel.Readers.SerialReaders.Add(device);
                    }
                    
                    break;
                case UsbManager.ActionUsbDeviceDetached:
                    foreach (var serialDevice in App.ViewModel.Readers.SerialReaders.ToList())
                    {
                        if (serialDevice.SerialNumber == device.SerialNumber)
                        {
                            App.ViewModel.Readers.SerialReaders.Remove(serialDevice);
                        }
                    }
                    device = MainActivity.device = null;
                    break;
                case BluetoothDevice.ActionFound:
                    // Get found bluetooth device
                    bluetoothDevice = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    mac = bluetoothDevice.Address.Split(':');
                    oui = mac[0] + mac[1] + mac[2];
                    if (ouiTransportIds.Contains(oui))
                    {
                        return;
                    }
                    else if (ouiVendorIds.Contains(oui))
                    {
                        ComplexBluetoothDevice pairedDevice = new ComplexBluetoothDevice(bluetoothDevice, ComplexBluetoothDevice.States.Paired);
                        foreach (var compDevice in App.ViewModel.Readers.BluetoothCameraReaders.ToList())
                        {
                            if (compDevice.Device.Address == pairedDevice.Device.Address)
                            {
                                int i = App.ViewModel.Readers.BluetoothCameraReaders.IndexOf(compDevice);
                                if (i != -1)
                                    App.ViewModel.Readers.BluetoothCameraReaders[i].State = pairedDevice.State;
                                return;
                            }
                        }
                        App.ViewModel.Readers.BluetoothCameraReaders.Add(pairedDevice);
                    }
                    break;
                case BluetoothDevice.ActionAclConnected:
                    // Get connected bluetooth device
                    bluetoothDevice = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    mac = bluetoothDevice.Address.Split(':');
                    oui = mac[0] + mac[1] + mac[2];
                    if (ouiTransportIds.Contains(oui))
                    {
                        return;
                    }
                    else if (ouiVendorIds.Contains(oui))
                    {
                        ComplexBluetoothDevice activeDevice = new ComplexBluetoothDevice(bluetoothDevice, ComplexBluetoothDevice.States.Active);
                        foreach (var compDevice in App.ViewModel.Readers.BluetoothCameraReaders.ToList())
                        {
                            if (compDevice.Device.Address == activeDevice.Device.Address)
                            {
                                int i = App.ViewModel.Readers.BluetoothCameraReaders.IndexOf(compDevice);
                                if (i != -1)
                                    App.ViewModel.Readers.BluetoothCameraReaders[i].State = activeDevice.State;
                                return;
                            }
                        }
                        App.ViewModel.Readers.BluetoothCameraReaders.Add(activeDevice);
                    }
                    break;
                case BluetoothDevice.ActionAclDisconnected:
                    // Get disconnected bluetooth device
                    bluetoothDevice = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    if (bluetoothDevice.Type == BluetoothDeviceType.Le)
                    {
                        ComplexBluetoothDevice inActiveDevice = new ComplexBluetoothDevice(bluetoothDevice, ComplexBluetoothDevice.States.Disconnected);
                        foreach (var compDevice in App.ViewModel.Readers.BluetoothCameraReaders.ToList())
                        {
                            if (compDevice.Device.Address == inActiveDevice.Device.Address)
                            {
                                int i = App.ViewModel.Readers.BluetoothCameraReaders.IndexOf(compDevice);
                                if (i != -1)
                                    App.ViewModel.Readers.BluetoothCameraReaders[i].State = inActiveDevice.State;
                            }
                        }
                    }
                    break;
                case Intent.ActionPowerConnected:
                    // Charger connected
                    break;
                case Intent.ActionPowerDisconnected:
                    // Charger disconnected
                    break;
                default:
                    break;
            }


        }

    }



}