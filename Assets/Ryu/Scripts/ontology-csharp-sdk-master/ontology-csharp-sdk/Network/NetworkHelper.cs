using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OntologyCSharpSDK.Common;
using OntologyCSharpSDK.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using WebSocketSharp;


namespace OntologyCSharpSDK.Network
{
    internal class NetworkHelper
    {
        //TODO: Nodelist implementation

        public static NetworkResponse SendNetworkRequest(string host,Protocol protocol, string requestType, string method, IList<object> param)
        {

            try
            {
                requestType = requestType.ToUpper();

                string request;

                switch (protocol)
                {
                    case Protocol.RPC:
                        {
                            request = RpcRequestBuilder(method, param);
                            return SendRpcRequest(request, requestType, host);
                        }

                    case Protocol.REST:
                        {
                            request = requestType == "GET" ? RestRequestBuilder(method, param) : RestBuildSendRawTransaction(method, param);
                            return SendRestRequest(request, requestType, method, host);
                        }

                    case Protocol.Websocket:
                        {
                            request = WebSocketRequestBuilder(method, param);
                            return SendWebSocketRequest(request, host);
                        }

                    default:
                        return null;
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private static NetworkResponse SendRpcRequest(string request, string requestType, string host)
        {
            NetworkResponse response = null;

            HttpWebRequest webRequest = null;
            byte[] byteArray;

            // Create WebRequest object
            try
            {
                webRequest = (HttpWebRequest)WebRequest.Create(host);
                webRequest.ContentType = "application/json-rpc";
                webRequest.Method = requestType;
                byteArray = Encoding.UTF8.GetBytes(request);
                webRequest.ContentLength = byteArray.Length;
            }
            catch (Exception ex)
            {
                throw new NetworkException("Error while creating HttpWebRequest object", ex.InnerException);
            }

            // Send network request
            try
            {

                using (var requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(byteArray, 0, byteArray.Length);
                }
            }
            catch (WebException)
            {
                throw;
            }


            response = new NetworkResponse();
            //Receive response from network node
            try
            {
                WebResponse webResponse;
                using (webResponse = webRequest.GetResponse())
                {
                    using (var str = webResponse.GetResponseStream())
                    {
                        using (var sr = new StreamReader(str))
                        {
                            response.RawResponse = sr.ReadToEnd();
                            response.JobjectResponse = JsonConvert.DeserializeObject<JObject>(response.RawResponse);

                            if (Convert.ToInt32(response.JobjectResponse.GetValue("error")) == 0)
                            {
                                return response;
                            }

                            throw new NetworkException("An error response was received from the server.", response, Protocol.RPC, request);
                        }
                    }
                }
            }
            catch (WebException)
            {
                throw;
            }
        }

        private static NetworkResponse SendRestRequest(string request, string requestType,string method, string host)
        {

            NetworkResponse response = null;
            HttpWebRequest webRequest = null;

            // Create WebRequest object
            try
            {
                switch (requestType)
                {
                    case "GET":
                        webRequest = (HttpWebRequest)WebRequest.Create(host + request);
                        break;
                    case "POST":
                        webRequest = (HttpWebRequest)WebRequest.Create(host + method);
                        break;
                    default:
                        break;
                }

                webRequest.ContentType = "application/json";
                webRequest.Method = requestType;


            }
            catch (Exception ex)
            {
                throw new Exception("Error while creating HttpWebRequest object", ex.InnerException);
            }

            response = new NetworkResponse();

            // Send request and receive network response
            try
            {

                if (requestType == "POST")
                {
                    var byteArray = Encoding.UTF8.GetBytes(request);
                    webRequest.ContentLength = byteArray.Length;

                    using (var stream = webRequest.GetRequestStream())
                    {
                        stream.Write(byteArray, 0, byteArray.Length);
                    }
                }

                WebResponse webResponse;
                using (webResponse = webRequest.GetResponse())
                {
                    using (var str = webResponse.GetResponseStream())
                    {
                        using (var sr = new StreamReader(str))
                        {
                            response.RawResponse = sr.ReadToEnd();
                            response.JobjectResponse = JsonConvert.DeserializeObject<JObject>(response.RawResponse);
                            if (Convert.ToInt32(response.JobjectResponse.GetValue("Error")) == 0)
                            {
                                return response;
                            }
                            else {
                                throw new ArgumentException(response.JobjectResponse+"");
                            }

                            throw new NetworkException("An error response was received from the server.", response, Protocol.REST, request);
                        }
                    }
                }
            }
            catch (WebException)
            {
                throw;
            }
        }

        private static NetworkResponse SendWebSocketRequest(string request, string host)
        {
            try
            {

                var response = new NetworkResponse();

                using (var ws = new WebSocket(host))
                {

                    ws.OnMessage += (sender, e) =>
                    {
                        response.RawResponse = e.Data;
                        response.JobjectResponse = JsonConvert.DeserializeObject<JObject>(response.RawResponse);
                        ws.Close();
                    };

                    ws.OnError += (sender, e) =>
                    {
                        response.RawResponse = e.Exception.InnerException.ToString();
                        ws.Close();
                    };

                    ws.Connect();
                    ws.Send(request);

                    while (ws.IsAlive)
                    {
                    }

                    if (Convert.ToInt32(response.JobjectResponse.GetValue("Error")) == 0)
                    {
                        return response;
                    }

                    throw new NetworkException("An error response was received from the server.", response, Protocol.REST, request);
                }
            }
            catch { throw; }
        }

        private static string RpcRequestBuilder(string method, IList<object> param)
        {
            try
            {
                var jsonObject = new JObject { ["jsonrpc"] = "2.0", ["method"] = method, ["id"] = 1 };


                var jsonArray = new JArray(param);

                jsonObject.Add("params", jsonArray);

                var json = JsonConvert.SerializeObject(jsonObject);
                return json;
            }
            catch { throw; }
        }


        private static string RestRequestBuilder(string method, IList<object> param)
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append(method);

                if (param != null)
                {
                    for (int i=0; i< param.Count;i++)
                    {
                        //sb.Append("/" + o);
                        sb.Replace("{" + i + "}", param[i].ToString());
                    }

                    return sb.ToString();
                }

                return sb.ToString();
            }
            catch { throw; }
        }

        private static string WebSocketRequestBuilder(string method, IList<object> param)
        {
            try
            {
                var jsonObject = new JObject { ["Action"] = method, ["Version"] = "1.0.0" };


                foreach (KeyValuePair<string, object> kvp in param)
                {
                    var jt = JToken.FromObject(kvp.Value);
                    jsonObject.Add(kvp.Key, jt);
                }

                var json = JsonConvert.SerializeObject(jsonObject);
                return json;
            }
            catch { throw; }
        }

        public static string RestBuildSendRawTransaction(string method, IList<object> param)
        {
            try
            {
                var jsonObject = new JObject
                {
                    ["Action"] = "sendrawtransaction",
                    ["Version"] = "1.0.0",
                    ["Data"] = param[0].ToString()
                };
                var json = JsonConvert.SerializeObject(jsonObject);
                return json;
            }
            catch { throw; }
        }


    }

    //TODO: Implement status codes
    public class NetworkResponse
    {
        public string RawResponse { get; set; }
        public JObject JobjectResponse { get; set; }
    }
}
