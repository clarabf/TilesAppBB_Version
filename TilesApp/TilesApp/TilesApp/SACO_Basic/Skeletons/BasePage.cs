using Android.Bluetooth;
using Android.Hardware.Usb;
using TilesApp.Models;
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
                        if (compDevice.Device.Address == activeDevice.Device.Address) {
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
    }
}
