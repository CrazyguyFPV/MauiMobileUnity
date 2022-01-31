namespace OntologyCSharpSDK.Core
{
    public class InvokeCode
    {
        public byte[] code { get; set; }
        public InvokeCode()
        {
            //gasLimit = new Fixed64();
        }
        public byte[] serialize()
        {
            return code;
        }
    }
}