using System.Collections;

namespace OntologyCSharpSDK.Wallet
{
    public class Account
    {
        public string Address { get; set; } = string.Empty;
        public string EncAlg { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
        public string Algorithm { get; set; } = string.Empty;
        public IDictionary Parameters { get; set; } = new Hashtable();
        public string Label { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string SignatureScheme { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public bool Lock { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public object Extra { get; set; }

        public Account()
        {
            Algorithm = "ECDSA";
            Parameters["curve"] = "secp256r1";
            EncAlg = "aes-256-ctr";
            Extra = null;
        }

    }
}

