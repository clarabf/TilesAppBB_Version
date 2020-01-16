using MongoDB.Bson.Serialization.Attributes;
using System;
using Xamarin.Essentials;
using TilesApp.Services;

namespace TilesApp.Models
{
    public class BaseData
    {
        [BsonIgnoreIfNull]
        public Location Location {get; private set;}
        public string DeviceSerialNumber { get; set; }
        public string AppName { get; set; }
        public string AppType { get; set; }
        public string Station { get; set; }
        public int? UserId { get; private set; }
        public string UserName { get; private set; }
        public enum InputDataProps
        {
            Value,
            Timestamp,
            ReaderType,
            ReaderSerialNumber,
        }
        public BaseData()
        {
            getLocation(); // sets the Location property to the current location

            DeviceSerialNumber = App.DeviceSerialNumber!=null ? App.DeviceSerialNumber : null ;
            UserId = OdooXMLRPC.userID;
            UserName = OdooXMLRPC.userName;
        }
        private async void getLocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    Location = location;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                Console.WriteLine("Error: " + fnsEx);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                Console.WriteLine("Error: " + fneEx);
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                Console.WriteLine("Error: " + pEx);
            }
            catch (Exception ex)
            {
                // Unable to get location
                Console.WriteLine("Error: " + ex);
            }
        }
    }
}
