using OntologyCSharpSDK.Common;
using OntologyCSharpSDK.Interface;
using OntologyCSharpSDK.Network;
using System;
using System.Collections.Generic;

namespace OntologyCSharpSDK.Network
{

    public class REST : IConnectionMethod
    {
        private readonly IList<object> param = new List<object>();
        public string NodeHost { get; set; }
        public REST(string nodehost)
        {
            NodeHost = nodehost;
        }
        public string getAddressBalance(string address)
        {
            try
            {
                param.Clear();
                param.Add(address);
                var response = NetworkHelper.SendNetworkRequest(NodeHost,Protocol.REST, "GET", Constants.REST_getAddressBalance, param);
                return response.JobjectResponse["Result"].ToString();
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
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getBlockHeightByTxHash, param);
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
            throw new NotImplementedException();
        }

        public int getBlockHeight()
        {
            try
            {
                param.Clear();
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getBlockHeight, param);
                return (int)response.JobjectResponse["Result"];
            }
            catch { throw; }
        }

        public long getBlockHeightByTxHash(string txHash)
        {
            
            param.Clear();
            param.Add(txHash);
            var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getBlockHeightByTxHash, param);               
            return (long)response.JobjectResponse["Result"];
            
        }

        public string getBlockHex(int blockHeight)
        {
            try
            {
                param.Clear();
                param.Add(blockHeight);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getBlockHexByHeight, param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getBlockHex(string blockHash)
        {
            try
            {
                param.Clear();
                param.Add(blockHash);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getBlockHexByHash, param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getBlockJson(int blockHeight)
        {
            try
            {
                param.Clear();
                param.Add(blockHeight);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getBlockByHeight, param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getBlockJson(string blockHash)
        {
            try
            {
                param.Clear();
                param.Add(blockHash);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getBlockByHash, param);
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
                param.Add(blockHeight);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getBlockTxsByHeight, param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getContractJson(string contractHash)
        {
            try
            {
                param.Clear();
                param.Add(contractHash);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getContract, param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getGasPrice()
        {
            try
            {
                param.Clear();
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getGasPrice, param);
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
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getMempoolTxState, param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getMerkleProof(string hash)
        {
            try
            {
                param.Clear();
                param.Add(hash);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getMerkleProof, param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public int getNodeCount()
        {
            try
            {
                param.Clear();
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getNodeCount, param);
                return (int)response.JobjectResponse["Result"];
            }
            catch { throw; }
        }

        public string getRawTransactionHex(string txHash)
        {
            try
            {
                param.Clear();
                param.Add(txHash);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getTransactionHexByHash, param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getRawTransactionJson(string txHash)
        {
            try
            {
                param.Clear();
                param.Add(txHash);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getTransactionByHash, param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getSmartCodeEvent(int blockHeight)
        {
            try
            {
                param.Clear();
                param.Add(blockHeight);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getSmartCodeEventByHeight, param);
                return response.JobjectResponse["Result"].ToString();
            }
            catch { throw; }
        }

        public string getSmartCodeEvent(string txHash)
        {
            try
            {
                param.Clear();
                param.Add(txHash);
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getSmartCodeEventByTxHash, param);
                return response.JobjectResponse["Result"].ToString();
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
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getStorage, param);
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
                var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "GET", Constants.REST_getUnclaimedONG, param);
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
                param.Add(tx);
                if (preExec)
                {
                    var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "POST", Constants.REST_sendRawTransactionPreExec, param);
                    return response.JobjectResponse["result"].ToString();
                }
                else
                {
                    var response = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "POST", Constants.REST_sendRawTransaction, param);
                    return response.JobjectResponse["result"].ToString();
                }
            }
            catch { throw; }
        }
    }
}

