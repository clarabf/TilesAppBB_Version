////
/// Copyright (c) 2016 Saúl Piña <sauljabin@gmail.com>.
/// 
/// This file is part of xmlrpcwsc.
/// 
/// xmlrpcwsc is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Lesser General Public License as published by
/// the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
/// 
/// xmlrpcwsc is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Lesser General Public License for more details.
/// 
/// You should have received a copy of the GNU Lesser General Public License
/// along with xmlrpcwsc.  If not, see <http://www.gnu.org/licenses/>.
////

using System;
using System.Collections.Generic;

namespace XmlRpc {

    /// <summary>
    /// XML-RPC Parameter helper
    /// </summary>
    public class XmlRpcParameter {

         /// <summary>
        /// Empty the struct.
        /// </summary>
        /// <returns>The struct</returns>
        public static Dictionary<string,object> EmptyStruct() {
            return new Dictionary<string,object>();
        }

        /// <summary>
        /// Empty array
        /// </summary>
        /// <returns>The array</returns>
        public static List<object> EmptyArray() {
            return new List<object>();
        }

        /// <summary>
        /// As array
        /// </summary>
        /// <returns>The array.</returns>
        /// <param name="list">List.</param>
        public static List<object> AsArray(params object[] list) {
            List<object> listReturn = new List<object>();
            listReturn.AddRange(list);
            return listReturn;
        }

        /// <summary>
        /// As struct
        /// </summary>
        /// <returns>The struct</returns>
        /// <param name="list">List</param>
        public static Dictionary<string, object> AsStruct(params KeyValuePair<string,object>[] list) {
            Dictionary<string,object> dictReturn = new Dictionary<string,object>();

            foreach (KeyValuePair<string,object> pair in list) {
                dictReturn.Add(pair.Key, pair.Value);
            }

            return dictReturn;
        }

        /// <summary>
        /// As member
        /// </summary>
        /// <returns>The member</returns>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static KeyValuePair<string,object> AsMember(string key, object value) {
            KeyValuePair<string,object> pairReturn = new KeyValuePair<string, object>(key, value);
            return pairReturn;
        }
    }
}
