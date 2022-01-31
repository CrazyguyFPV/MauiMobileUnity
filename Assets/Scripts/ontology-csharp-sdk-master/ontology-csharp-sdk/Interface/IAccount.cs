using OntologyCSharpSDK;
using OntologyCSharpSDK.Network;

namespace OntologyCSharpSDK.Interface
{
    interface IAccount
    {
        string createPrivateKey();
        string getPublicKey(string privatekey);
        string createAddressFromPublickKey(string publicKey);
                

        NetworkResponse transfer(string name, string fromaddress, string toaddress, long value, string payer, uint gasLimit, uint gasPrice, byte[] privatekey);

    }
}
