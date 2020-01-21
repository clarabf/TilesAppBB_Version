using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using TilesApp.Services;

namespace TilesApp.Models
{
    class AppBasicOperation
    {
        //Variables for global usage
        public enum OperationType
        {
            Login,
            Logout,
            Error,
        }
        private OperationType _operation;
        [BsonIgnoreIfNull]
        public string Operation { get { return _operation.ToString(); } }
        // automatically retrieved
        [BsonIgnoreIfNull]
        public DateTime TimeStamp { get { return DateTime.Now; } }

        [BsonIgnoreIfNull]
        public int? UserId
        {
            get
            {
                return OdooXMLRPC.userID;
            }
        }
        [BsonIgnoreIfNull]
        public string UserName
        {
            get
            {
                return OdooXMLRPC.userName;
            }
        }
        [BsonIgnoreIfNull]
        public string DeviceSerialNumber
        {
            get
            {
                return App.DeviceSerialNumber != null ? App.DeviceSerialNumber : null;
            }
        }

        public AppBasicOperation(OperationType op) {
            _operation = op;
        }
    }
}
