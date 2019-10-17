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
using System.Xml;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace XmlRpc {

    /// <summary>
    /// XML-RPC client
    /// </summary>
    public class XmlRpcClient : WebServiceConnection {

        protected XmlDocument xmlRequest;
        protected XmlDocument xmlResponse;
        protected XmlRpcRequest request;
        protected XmlRpcResponse response;

        /// <summary>
        /// Gets the xml request
        /// </summary>
        /// <returns>The xml request</returns>
        public XmlDocument GetXmlRequest() {
            return xmlRequest;
        }

        /// <summary>
        /// Gets the xml response
        /// </summary>
        /// <returns>The xml response</returns>
        public XmlDocument GetXmlResponse() {
            return xmlResponse;
        }

        /// <summary>
        /// Writes the request
        /// </summary>
        /// <param name="fileName">File name</param>
        public void WriteRequest(String fileName) {
            TextWriter streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
            WriteRequest(streamWriter);
        }

        /// <summary>
        /// Writes the request
        /// </summary>
        /// <param name="outStream">Out stream</param>
        public void WriteRequest(TextWriter outStream) {
            xmlRequest.Save(outStream);
        }

        /// <summary>
        /// Writes the response
        /// </summary>
        /// <param name="fileName">File name</param>
        public void WriteResponse(String fileName) {
            TextWriter streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
            WriteResponse(streamWriter);
        }

        /// <summary>
        /// Writes the response
        /// </summary>
        /// <param name="outStream">Out stream</param>
        public void WriteResponse(TextWriter outStream) {
            xmlResponse.Save(outStream);           
        }

        /// <summary>
        /// Execute the specified methodName and parameters
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="parameters">Parameters</param>
        public XmlRpcResponse Execute(string methodName, List<object> parameters){
            return Execute(new XmlRpcRequest(methodName, parameters));
        }

        /// <summary>
        /// Execute the specified methodName and parameters
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="parameters">Parameters</param>
        public XmlRpcResponse Execute(string methodName, object[] parameters){
            return Execute(new XmlRpcRequest(methodName, parameters));
        }

        /// <summary>
        /// Execute the specified methodName
        /// </summary>
        /// <param name="methodName">Method name</param>
        public XmlRpcResponse Execute(string methodName){
            return Execute(new XmlRpcRequest(methodName));
        }

        /// <summary>
        /// Execute the specified request
        /// </summary>
        /// <param name="request">Request</param>
        public virtual XmlRpcResponse Execute(XmlRpcRequest request){
            this.request = request;

            XmlDocument xmlRequest = RequestFactory.BuildRequest(request);
            this.xmlRequest = xmlRequest;

            XmlDocument xmlResponse = SendRequest(xmlRequest);
            this.xmlResponse = xmlResponse;

            this.response = ResponseFactory.BuildResponse(xmlResponse);

            return response;
        }
    }
}

