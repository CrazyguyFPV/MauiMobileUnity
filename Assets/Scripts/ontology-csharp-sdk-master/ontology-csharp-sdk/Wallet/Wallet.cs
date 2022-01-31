using System.Collections.Generic;

namespace OntologyCSharpSDK.Wallet
{
    public class Wallet
    {
        public string Name => "OntologyWallet";
        public string Version => "1.0";
        public Scrypt Scrypt = new Scrypt();
        public object Extra { get; } = null;
        public IList<Identity> Identities { get; } = new List<Identity>();
        public IList<Account> Accounts { get; } = new List<Account>();

        public Wallet()
        {
            Identities.Clear();
            Identities.Add(new Identity { Label = "NewIdentity", IsDefault = true, Lock = false, Extra = null, Controls = new List<Control> { new Control("6Pxxxxxxxxx", "1") }, Ontid = "did:ont:AMxxxxxxxxxxxxxxxxxxxxxxx" });
            Accounts.Add(new Account { Address = "TA6xxxxxxxxxxxx", Key = "6Pxxxxxxxxxxx", Hash = "sha256", Label = "NewAccount", PublicKey = "1202xxxxxxxxxxxxxx", SignatureScheme = "SHA256withECDSA", PasswordHash = "passwordhash" });
            Scrypt = new Scrypt(16384, 8, 8);
        }
    }
}
