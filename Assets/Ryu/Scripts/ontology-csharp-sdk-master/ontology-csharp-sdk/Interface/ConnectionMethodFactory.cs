using System;
using OntologyCSharpSDK.Network;

namespace OntologyCSharpSDK.Interface
{
    public class ConnectionMethodFactory
    {
        public enum ConnectionMethod
        {
            RPC,
            REST,
            Websocket
        }

        public virtual IConnectionMethod SetConnectionMethod(string nodehost,ConnectionMethod method)
        {
            try
            {
                IConnectionMethod connectionMethod;

                switch (method)
                {
                    case ConnectionMethod.RPC:
                        connectionMethod = new RPC(nodehost);
                        break;

                    case ConnectionMethod.REST:
                        connectionMethod = new REST(nodehost);
                        break;

                    case ConnectionMethod.Websocket:
                        connectionMethod = new Websocket(nodehost);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(method), method, null);
                }

                return connectionMethod;
            }
            catch { throw; }
        }
    }
}
