using System.Collections;

namespace OntologyCSharpSDK.Wallet
{
    public class Control
    {
        public string Algorithm = "ECDSA";
        public string Id = "";
        public string Key = "";
        public IDictionary Parameters = new Hashtable();

        public Control()
        {
        }

        public Control(string key, string id)
        {
            this.Key = key;
            Algorithm = "ECDSA";
            this.Id = id;
            Parameters["curve"] = "secp256r1";
        }
    }
}

