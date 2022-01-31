using System.Collections.Generic;

namespace OntologyCSharpSDK.Core
{
    public class Sig
    {
        public List<byte[]> pubKeys { get; set; }
        public int M { get; set; }
        public List<byte[]> sigData { get; set; }

        public void serialize(System.IO.Stream writer)
        {
            byte[] param = programFromParams();
            Transaction.writeVarInt(writer, (ulong)param.Length);
            writer.Write(param, 0, param.Length);
            byte[] paramPKs = programFromPubKeys();
            Transaction.writeVarInt(writer, (ulong)paramPKs.Length);
            writer.Write(paramPKs, 0, paramPKs.Length);
        }
        public byte[] programFromParams()
        {            
            var sb = new ScriptBuilder();
            sb.EmitPushBytes(sigData[0]);
            return sb.ToArray();
        }
        public byte[] programFromPubKeys()
        {
            var sb = new ScriptBuilder();
            sb.EmitPushBytes(pubKeys[0]);
            sb.Emit(OpCode.CHECKSIG);
            return sb.ToArray();
        }
    }
}