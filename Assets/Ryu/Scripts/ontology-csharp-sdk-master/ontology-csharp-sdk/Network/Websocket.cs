using OntologyCSharpSDK.Common;
using OntologyCSharpSDK.Interface;
using OntologyCSharpSDK.Network;
using System;
using System.Collections.Generic;

namespace OntologyCSharpSDK.Network
{

    public class Websocket : IConnectionMethod
    {
        private readonly IList<object> param = new List<object>();
        public string NodeHost { get; set; }
        public Websocket(string nodehost)
        {
            NodeHost = nodehost;
        }
        public string getAddressBalance(string address)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Raw", 0));
                param.Add(new KeyValuePair<string, object>("Addr", address));
                var response = NetworkHelper.SendNetworkRequest(NodeHost,Protocol.Websocket, "", "getbalance", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getAllowance(string asset, string fromAddress, string toAddress)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Asset", "ont"));
                param.Add(new KeyValuePair<string, object>("From", fromAddress));
                param.Add(new KeyValuePair<string, object>("To", toAddress));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getallowance", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getBestBlockHash()
        {
            throw new NotImplementedException();
        }


        public string getBlockHashByHeight(int blockHeight)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Height", blockHeight));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getblockhash", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public int getBlockHeight()
        {
            try
            {
                param.Clear();
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getblockheight", param);
                return (int)response.JobjectResponse["Result"];
            }
            catch { throw; }
        }

        public long getBlockHeightByTxHash(string txHash)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Hash", txHash));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getblockheightbytxhash", param);
                return (int)response.JobjectResponse["Result"];
            }
            catch { throw; }
        }

        public string getBlockHex(int blockHeight)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Raw", 1));
                param.Add(new KeyValuePair<string, object>("Height", blockHeight));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getblockbyheight", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getBlockHex(string blockHash)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Raw", 1));
                param.Add(new KeyValuePair<string, object>("Hash", blockHash));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getblockbyhash", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getBlockJson(int blockHeight)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Raw", 0));
                param.Add(new KeyValuePair<string, object>("Height", blockHeight));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getblockbyheight", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getBlockJson(string blockHash)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Raw", 0));
                param.Add(new KeyValuePair<string, object>("Hash", blockHash));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getblockbyhash", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public int getBlockSysFee(int index)
        {
            throw new NotImplementedException();
        }

        public string getBlockTxsByHeight(int blockHeight)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Height", blockHeight));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getblocktxsbyheight", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getContractJson(string contractHash)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Hash", contractHash));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getcontract", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getGasPrice()
        {
            try
            {
                param.Clear();
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getgasprice", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getMempoolTxState(string txHash)
        {
            try
            {
                param.Clear();
                param.Add(txHash);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getmempooltxstate", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string getMerkleProof(string hash)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Hash", hash));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getmerkleproof", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public int getNodeCount()
        {
            try
            {
                param.Clear();
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getconnectioncount", param);
                return (int)response.JobjectResponse["Result"];
            }
            catch { throw; }
        }

        public string getRawTransactionHex(string txHash)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Raw", 1));
                param.Add(new KeyValuePair<string, object>("Hash", txHash));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "gettransaction", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getRawTransactionJson(string txHash)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Raw", 0));
                param.Add(new KeyValuePair<string, object>("Hash", txHash));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "gettransaction", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getSmartCodeEvent(int blockHeight)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Height", blockHeight));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getsmartcodeeventbyheight", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getSmartCodeEvent(string txHash)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Height", txHash));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getsmartcodeeventbyhash", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getStorage(string contractHash, string key)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Hash", contractHash));
                param.Add(new KeyValuePair<string, object>("Key", key));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getstorage", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getUnclaimedONG(string address)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("Addr", address));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "getunclaimong", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public int getVersion()
        {
            throw new NotImplementedException();
        }

        public string setSendRawTransaction(string tx, bool preExec)
        {
            try
            {
                param.Clear();
                param.Add(new KeyValuePair<string, object>("PreExec", Convert.ToInt32(preExec)));
                param.Add(new KeyValuePair<string, object>("Data", tx));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.Websocket, "", "sendrawtransaction", param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }
    }
}


