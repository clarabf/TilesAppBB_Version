using Android.Bluetooth;
using Java.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace TilesApp.Models
{
    public class ComplexBluetoothDevice : INotifyPropertyChanged
    {
        private BluetoothDevice _device;
        private bool _isActive;
        public enum States
        {
            Disconnected,
            Paired,
            Active
        }
        private States _state;

        public event PropertyChangedEventHandler PropertyChanged;

        public BluetoothDevice Device { get { return _device; } }
        public States State { get { return _state; } set {
                this._state = value;
                this._isActive = value == States.Active ? true : false;
                RaisePropertyChanged();
            } }
        public bool IsActive { get { return _isActive; } }
        public ComplexBluetoothDevice(BluetoothDevice device, States state)
        {
            this._device = device;
            this._state = state;
            this._isActive = state == States.Active ? true : false;
        }

        public void RaisePropertyChanged(string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
