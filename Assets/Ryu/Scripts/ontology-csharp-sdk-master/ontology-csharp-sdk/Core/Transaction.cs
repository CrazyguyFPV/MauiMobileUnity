using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.IO;
using OntologyCSharpSDK.Common;

namespace OntologyCSharpSDK.Core
{

    public enum TransactionType : byte
    {
        DeployTransaction = 0xd0,
        InvocationTransaction = 0xd1
    }
    
    public class Transaction
    {
        public byte version { get; set; }
        public byte txType { get; set; }
        public InvokeCode payload { get; set; }
        public UInt64 nonce { get; set; }
        public UInt64 gasPrice { get; set; }
        public UInt64 gasLimit { get; set; }
        public Hash160 payer { get; set; }

        public List<Sig> sigs { get; set; }

        public Transaction()
        {
            sigs = new List<Sig>();
            txType = 0xd1;
            version = 0x00;
        }

        public byte[] getHash()
        {
            byte[] data = null;
            using (var ms = new System.IO.MemoryStream())
            {
                SerializeUnsigned(ms);
                data = ms.ToArray();
            }
            return Helper.Sha256(Helper.Sha256(data));

        }
        public string toHexString()
        {
            byte[] msg = null;
            using (var ms = new System.IO.MemoryStream())
            {
                serialize(ms);
                msg = ms.ToArray();
            }
            return Helper.Bytes2HexString(msg);
        }
        public void serialize(System.IO.Stream writer)
        {

            SerializeUnsigned(writer);
            SerializeSigned(writer);
        }
        public void SerializeUnsigned(System.IO.Stream writer)
        {
            writer.WriteByte(version);
            writer.WriteByte(0xd1);
            writer.Write(BitConverter.GetBytes(nonce), 0, 4);
            writer.Write(BitConverter.GetBytes(gasPrice), 0, 8);
            writer.Write(BitConverter.GetBytes(gasLimit), 0, 8);
            writer.Write(payer.data, 0, 20);
            var len = payload.serialize().Length;
            writeVarInt(writer, (ulong)len);
            writer.Write(payload.serialize(), 0, len);
            writeVarInt(writer, (ulong)0);
        }

        public void SerializeSigned(System.IO.Stream writer)
        {
            writeVarInt(writer, (ulong)sigs.Count);
            foreach (var t in sigs)
            {
               t.serialize(writer);
            }
        }
        public static void writeVarInt(System.IO.Stream stream, UInt64 value)
        {
            if (value > 0xffffffff)
            {
                stream.WriteByte((byte)(0xff));
                var bs = BitConverter.GetBytes(value);
                stream.Write(bs, 0, 8);
            }
            else if (value > 0xffff)
            {
                stream.WriteByte((byte)(0xfe));
                var bs = BitConverter.GetBytes((UInt32)value);
                stream.Write(bs, 0, 4);
            }
            else if (value > 0xfc)
            {
                stream.WriteByte((byte)(0xfd));
                var bs = BitConverter.GetBytes((UInt16)value);
                stream.Write(bs, 0, 2);
            }
            else
            {
                stream.WriteByte((byte)value);
            }
        }
        public static UInt64 readVarInt(System.IO.Stream stream, UInt64 max = 9007199254740991)
        {
            var fb = (byte)stream.ReadByte();
            UInt64 value = 0;
            byte[] buf = new byte[8];
            if (fb == 0xfd)
            {
                stream.Read(buf, 0, 2);
                value = (UInt64)(buf[1] * 256 + buf[0]);
            }
            else if (fb == 0xfe)
            {
                stream.Read(buf, 0, 4);
                value = (UInt64)(buf[1] * 256 * 256 * 256 + buf[1] * 256 * 256 + buf[1] * 256 + buf[0]);
            }
            else if (fb == 0xff)
            {
                stream.Read(buf, 0, 8);
                //我懒得展开了，规则同上
                value = BitConverter.ToUInt64(buf, 0);// (UInt64)(buf[1] * 256 * 256 * 256 + buf[1] * 256 * 256 + buf[1] * 256 + buf[0]);
            }
            else
                value = fb;
            if (value > max) throw new Exception("to large.");
            return value;
        }
    }
}