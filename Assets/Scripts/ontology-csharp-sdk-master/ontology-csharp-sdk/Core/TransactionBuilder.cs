using System;
using System.Numerics;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OntologyCSharpSDK.Common;

namespace OntologyCSharpSDK.Core
{
    public static class TransactionBuilder
    {
        private static readonly string _ontContractHash = "0000000000000000000000000000000000000001";
        private static string _ongContractHash = "0000000000000000000000000000000000000002";

        
        public static void createCodeParamsScript(ScriptBuilder sb, List<object> list)
        {
            for (int i = list.Count-1; i >=0; i--)
            {
                 if (list[i] is string)
                {
                    string tmp = (string)list[i];
                    if (tmp.StartsWith("String:")) 
                    {
                        string value = tmp.Substring(7);
                        sb.EmitPushBytes(System.Text.Encoding.Default.GetBytes(value));
                    }
                    else if (tmp.StartsWith("ByteArray:"))
                    {
                        string value = tmp.Substring(10);
                        sb.EmitPushBytes(Helper.HexString2Bytes(value));
                    }
                    else if (tmp.StartsWith("Address:"))
                    {
                        string value = tmp.Substring(8);
                        sb.EmitPushBytes(Helper.GetPublicKeyHashFromAddress(value).data);
                    }
                    else if (tmp.StartsWith("Long:"))
                    {
                        string value = tmp.Substring(5);
                        sb.EmitPushBytes(BigInteger.Parse(value).ToByteArray());
                    }
                    else
                    {
                        string value = tmp.Substring(0);
                        sb.EmitPushBytes(System.Text.Encoding.Default.GetBytes(value));            
                    }

                }
                else if (list[i] is Boolean)
                {
                    sb.EmitPushBool((bool)list[i]);
                }
                else if (list[i] is Int64)
                {
                    sb.EmitPushNumber(new BigInteger((long)list[i]));
                }
                else if (list[i] is Byte[]) {
                    sb.EmitPushBytes((Byte[])list[i]);
                }
                else if (list[i] is List<object>)
                {
                    List<object> tmp = (List<object>)list[i];
                    int count = tmp.Count;
                    createCodeParamsScript(sb, tmp);
                    sb.EmitPushNumber(new BigInteger(count));
                    sb.Emit(OpCode.PACK); 
                }
                
            }
        }
        public static void convertToList(List<object> list, JValue value) {
            JValue tmp = (JValue)value;
            if (tmp.Value.GetType().Name == "Int64")
            {
                list.Add(value.ToObject<Int64>());
            }
            else if (tmp.Value.GetType().Name == "String")
            {
                list.Add(value.ToObject<string>());
            }
            else if (tmp.Value.GetType().Name == "Boolean")
            {
                list.Add(value.ToObject<bool>());
            }
        }
        public static void convertToList(List<object> list, JArray array)
        {
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i] is JArray)
                {
                    List<object> listTmp = new List<object>();
                    convertToList(listTmp, (JArray)array[i]);
                    list.Add(listTmp);
                }
                else if (array[i] is JValue)
                {
                    convertToList(list, (JValue)array[i]);
                }
            }
        }
        public static Transaction MakeInvokeTransaction(JObject action)
        {
            JObject invokeConfig = (JObject)action["params"];
            string contractHash = (string)invokeConfig["invokeConfig"]["contractHash"];
            string payer = (string)invokeConfig["invokeConfig"]["payer"];
            ulong gasLimit = (ulong)invokeConfig["invokeConfig"]["gasLimit"];
            ulong gasPrice = (ulong)invokeConfig["invokeConfig"]["gasPrice"];
            JObject function = (JObject)invokeConfig["invokeConfig"]["functions"][0];
            string method = (string)function["operation"];
            JArray args = (JArray)function["args"];

            List<object> paramlist = new List<object>();
            paramlist.Add(System.Text.Encoding.Default.GetBytes(method));

            List<object> list = new List<object>();
            for (int i = 0 ; i < args.Count ; i++)
            {
                JObject obj = (JObject)args[i];
                if (obj["value"] is JArray)
                {
                    List<object> listTmp = new List<object>();
                    convertToList(listTmp, (JArray)obj["value"]);
                    list.Add(listTmp);
                }
                else if (obj["value"] is JValue)
                {
                    convertToList(list, (JValue)obj["value"]);
                }
            }
            paramlist.Add(list);
            var sb = new ScriptBuilder();
            createCodeParamsScript(sb, paramlist);
            byte[] param = sb.ToArray();

            List<byte> byteSource = new List<byte>();
            byteSource.AddRange(param);
            byteSource.AddRange(new List<byte>() { 0x67 });
            byteSource.AddRange(new Hash160(Helper.HexString2Bytes(contractHash)).Reverse());
            var tx = new Transaction
            {
                version = 0x00,
                txType = 0xd1,
                gasLimit = gasLimit,
                gasPrice = gasPrice,
                payer = Helper.GetPublicKeyHashFromAddress(payer),
                payload = new InvokeCode { code = byteSource.ToArray() },
                nonce = (ulong)new Random().Next()
            };
            return tx;
        }
        public static Transaction MakeTransferTransaction(string token, List<State> states, string payer, uint gasLimit, uint gasPrice)
        {
            ScriptBuilder sb = new ScriptBuilder();
            sb.EmitPushNumber(0);
            sb.Emit(OpCode.NEWSTRUCT);
            sb.Emit(OpCode.TOALTSTACK);
            for (int i = 0; i < states.Count; i++)
            {
                sb.EmitPushBytes(Helper.GetPublicKeyHashFromAddress(states[i].from).data);
                sb.Emit(OpCode.DUPFROMALTSTACK);
                sb.Emit(OpCode.SWAP);
                sb.Emit(OpCode.APPEND);
                sb.EmitPushBytes(Helper.GetPublicKeyHashFromAddress(states[i].to).data);
                sb.Emit(OpCode.DUPFROMALTSTACK);
                sb.Emit(OpCode.SWAP);
                sb.Emit(OpCode.APPEND);
                sb.EmitPushNumber(5);
                sb.Emit(OpCode.DUPFROMALTSTACK);
                sb.Emit(OpCode.SWAP);
                sb.Emit(OpCode.APPEND);
            }
            sb.Emit(OpCode.FROMALTSTACK);
            sb.EmitPushNumber(states.Count);
            sb.Emit(OpCode.PACK);
            byte[] args = sb.ToArray();
            var sbCode = new ScriptBuilder();
            if (args.Length > 0)
            {
                sbCode.Write(args);
            }
            sbCode.EmitPushString("transfer");
            string contracthash = "";
            if (token.ToLower() == "ont")
            {
                contracthash = _ontContractHash;
            }
            else if (token.ToLower() == "ong")
            {
                contracthash = _ongContractHash;

            }
            else
            {
                throw new ArgumentException("token name error, only support ont or ong.");
            }
            sbCode.EmitPushBytes(Helper.HexString2Bytes(contracthash));
            sbCode.EmitPushNumber(0);
            sbCode.Emit(OpCode.SYSCALL);
            sbCode.EmitPushString("Ontology.Native.Invoke");
            Transaction tx = new Transaction
            {
                version = 0x00,
                txType = 0xd1,
                gasLimit = gasLimit,
                gasPrice = gasPrice,
                payer = Helper.GetPublicKeyHashFromAddress(payer),
                payload = new InvokeCode { code = sbCode.ToArray() },
                nonce = (ulong)new Random().Next() 
            };
            return tx;
        }
        public static Transaction MakeTransferTransaction(string token, string from, string to, long value, string payer,uint gasLimit,uint gasPrice)
        {
            State state = new State { from = from, to = to, value = value };
            List<State> states = new List<State> { state };
            return MakeTransferTransaction(token, states, payer, gasLimit, gasPrice);
        }

        
    }
}
