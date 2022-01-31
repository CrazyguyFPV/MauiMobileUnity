using Newtonsoft.Json;
using OntologyCSharpSDK.Interface;
using OntologyCSharpSDK.Network;
using Newtonsoft.Json.Linq;
using OntologyCSharpSDK.Common;
using OntologyCSharpSDK.Core;
using System.Collections.Generic;
using System;

namespace OntologyCSharpSDK
{
    public class OntologySdk
    {
        public string NodeHost { get; set; }
        public IConnectionMethod Connection { get; }
        public WebsocketSubscribe WebsocketSubscribe = new WebsocketSubscribe();


        public OntologySdk(string node, ConnectionMethodFactory.ConnectionMethod connectionMethod)
        {
            NodeHost = node;
            var factory = new ConnectionMethodFactory();
            Connection = factory.SetConnectionMethod(node,connectionMethod);
            
        }

        public string CreateWallet()
        {
            var wallet = new Wallet.Wallet();
            var json = JsonConvert.SerializeObject(wallet, Formatting.Indented).Replace("enc_alg", "enc-alg");
            return json;
        }
        public void sign(Transaction tx, List<byte[]> privatekeys)
        {
            byte[] msg = null;
            tx.sigs = new List<Sig> { };
            using (var ms = new System.IO.MemoryStream())
            {
                tx.SerializeUnsigned(ms);
                msg = ms.ToArray();

                for(int i=0;i< privatekeys.Count; i++) { 
                    byte[] signature = Helper.Sign(Helper.Sha256(Helper.Sha256(msg)), privatekeys[i]);

                    List<byte> bytes = new List<byte>();
                    bytes.AddRange(new byte[] { 0x01 });
                    bytes.AddRange(signature);
 
                    tx.sigs.Add(new Sig());
                    tx.sigs[i].M = 1;
                    tx.sigs[i].pubKeys = new List<byte[]> { Helper.GetPublicKeyFromPrivateKey(privatekeys[i]) };
                    tx.sigs[i].sigData = new List<byte[]> { bytes.ToArray() };
                }
            }
        }

        //preExec = true， for check this transaction
        public NetworkResponse transfer(string name, string from, string to, long value, string payer, uint gasLimit, uint gasPrice, List<byte[]> privatekeys, bool preExec)
        {
            Transaction tx = TransactionBuilder.MakeTransferTransaction(name, from, to, value, payer, gasLimit, gasPrice);
            sign(tx, privatekeys);

            NetworkResponse result = send(tx.toHexString(), preExec);
            return result;
        }

        public NetworkResponse invokeTransaction(JObject action, List<byte[]> privatekeys,bool preExec)
        {
            Transaction tx = TransactionBuilder.MakeInvokeTransaction(action);
            sign(tx, privatekeys);
            NetworkResponse result = send(tx.toHexString(), preExec);
            return result;
        }
        public NetworkResponse send(string data,bool preExec)
        {
            NetworkResponse result;
            IList<object> param = new List<object> { data };
            if (preExec)
            {
                result = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "POST", Constants.REST_sendRawTransactionPreExec, param);
            }
            else
            {
                result = NetworkHelper.SendNetworkRequest(NodeHost, Protocol.REST, "POST", Constants.REST_sendRawTransaction, param);
            }  
            return result;
        }

    }
}

