using OntologyCSharpSDK.Common;
using OntologyCSharpSDK.Network;
using System;


namespace OntologyCSharpSDK.ExceptionHandling
{

    public class NetworkException : Exception
    {
        public readonly string _networkRequestSent;
        public readonly int _networkErrorCode;
        public readonly string _networkErrorDescription;
        public readonly string _networkRawError;
        public readonly Protocol _connectionMethod;


        public NetworkException()
        {
        }

        public NetworkException(string message)
            : base(message)
        {
        }

        public NetworkException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public NetworkException(string message, NetworkResponse response, Protocol connectionmethod, string request)
            : base(message)
        {
            try
            {
                var errorfield = "Error";
                var descriptionfield = "Desc";

                if (connectionmethod == Protocol.RPC)
                {
                    errorfield = "error";
                    descriptionfield = "desc";
                }

                _networkErrorCode = Convert.ToInt32(response.JobjectResponse.GetValue(errorfield).ToString());
                _networkErrorDescription = response.JobjectResponse.GetValue(descriptionfield).ToString();
                _networkRawError = response.RawResponse;
                _connectionMethod = connectionmethod;
                _networkRequestSent = request;
            }
            catch { throw; }
        }
    }
}
