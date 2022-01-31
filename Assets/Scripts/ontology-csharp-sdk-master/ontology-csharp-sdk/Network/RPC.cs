using OntologyCSharpSDK.Common;
using OntologyCSharpSDK.Interface;
using OntologyCSharpSDK.Network;
using System;
using System.Collections.Generic;

namespace OntologyCSharpSDK.Network
{

    public class RPC : IConnectionMethod
    {
        private readonly IList<object> param = new List<object>();
        public string NodeHost { get; set; }
        public RPC(string nodehost)
        {
            NodeHost = nodehost;
        }

        public int getBlockHeight()
        {
            try
            {
                param.Clear();
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getblockcount", param);
                return (int)response.JobjectResponse["result"];
            }
            catch { throw; }
        }

        public long getBlockHeightByTxHash(string txHash)
        {
            try
            {
                param.Clear();
                param.Add(txHash);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getblockheightbytxhash", param);
                return (int)response.JobjectResponse["result"];
            }
            catch { throw; }
        }

        public string getBlockHex(int blockHeight)
        {
            try
            {
                param.Clear();
                param.Add(blockHeight);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getblock", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string getBlockHex(string blockHash)
        {
            try
            {
                param.Clear();
                param.Add(blockHash);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getblock", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string getBlockJson(int blockHeight)
        {
            try
            {
                param.Clear();
                param.Add(blockHeight);
                param.Add(1);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getblock", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string getBlockJson(string blockHash)
        {
            try
            {
                param.Clear();
                param.Add(blockHash);
                param.Add(1);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getblock", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public int getNodeCount()
        {
            try
            {
                param.Clear();
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getconnectioncount", param);
                return (int)response.JobjectResponse["result"];
            }
            catch { throw; }
        }

        public string getAddressBalance(string address)
        {
            try
            {
                param.Clear();
                param.Add(address);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getbalance", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string getRawTransactionHex(string txHash)
        {
            try
            {
                param.Clear();
                param.Add(txHash);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getrawtransaction", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string getRawTransactionJson(string txHash)
        {
            try
            {
                param.Clear();
                param.Add(txHash);
                param.Add(1);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getrawtransaction", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }


        public string getSmartCodeEvent(int blockHeight)
        {
            try
            {
                param.Clear();
                param.Add(blockHeight);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getsmartcodeevent", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string getSmartCodeEvent(string txHash)
        {
            try
            {
                param.Clear();
                param.Add(txHash);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getsmartcodeevent", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string getBestBlockHash()
        {
            try
            {
                param.Clear();
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getbestblockhash", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string getBlockHashByHeight(int blockHeight)
        {
            try
            {
                param.Clear();
                param.Add(blockHeight);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getblockhash", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string getStorage(string contractHash, string key)
        {
            try
            {
                param.Clear();
                param.Add(contractHash);
                param.Add(key);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getstorage", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public int getVersion()
        {
            try
            {
                param.Clear();
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getversion", param);
                return (int)response.JobjectResponse["result"];
            }
            catch { throw; }
        }

        public int getBlockSysFee(int blockHeight)
        {
            try
            {
                param.Clear();
                param.Add(blockHeight);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getblocksysfee", param);
                return (int)response.JobjectResponse["result"];
            }
            catch { throw; }
        }

        public string getContractJson(string contractHash)
        {
            try
            {
                param.Clear();
                param.Add(contractHash);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getcontractstate", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string getMempoolTxState(string txHash)
        {
            try
            {
                param.Clear();
                param.Add(txHash);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getmempooltxstate", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string setSendRawTransaction(string tx, bool preExec)
        {
            try
            {
                param.Clear();
                param.Add(tx);
                param.Add(Convert.ToInt32(preExec));
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "sendrawtransaction", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string getMerkleProof(string hash)
        {
            try
            {
                param.Clear();
                param.Add(hash);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getmerkleproof", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string getGasPrice()
        {
            try
            {
                param.Clear();
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getgasprice", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string getAllowance(string asset, string fromAddress, string toAddress)
        {
            try
            {
                param.Clear();
                param.Add(asset);
                param.Add(fromAddress);
                param.Add(toAddress);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getallowance", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string getBlockTxsByHeight(int blockHeight)
        {
            try
            {
                param.Clear();
                param.Add(blockHeight);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getblocktxsbyheight", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }

        public string getUnclaimedONG(string address)
        {
            try
            {
                param.Clear();
                param.Add(address);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.RPC, "POST", "getunclaimong", param);
                return response.JobjectResponse["result"].ToString();
            }
            catch { throw; }
        }
    }
}

