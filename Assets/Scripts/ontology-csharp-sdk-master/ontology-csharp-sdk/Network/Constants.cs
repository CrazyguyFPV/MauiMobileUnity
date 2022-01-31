namespace OntologyCSharpSDK.Network
{
    public class Constants
    {

        #region REST URL's
        public const string REST_getBlockHeight = "/api/v1/block/height";
        public const string REST_getBlockHeightByTxHash = "/api/v1/block/height/txhash/{0}"; //+hash
        public const string REST_getNodeCount = "/api/v1/node/connectioncount";
        public const string REST_getBlockByHeight = "/api/v1/block/details/height/{0}"; //+height 
        public const string REST_getBlockHexByHeight = "/api/v1/block/details/height/{0}?raw=1";
        public const string REST_getBlockHexByHash = "/api/v1/block/details/hash/{0}?raw=1"; //+hash
        public const string REST_getBlockByHash = "/api/v1/block/details/hash/{0}"; //+hash
        public const string REST_getTransactionByHash = "/api/v1/transaction/{0}"; //+hash 
        public const string REST_getTransactionHexByHash = "/api/v1/transaction/{0}?raw=1"; //+hash 
        public const string REST_getAddressBalance = "/api/v1/balance/{0}"; //+addr
        public const string REST_getContract = "/api/v1/contract/{0}"; //+hash
        public const string REST_getSmartCodeEventByHeight = "/api/v1/smartcode/event/transactions/{0}"; //+height
        public const string REST_getSmartCodeEventByTxHash = "/api/v1/smartcode/event/txhash/{0}"; //+hash
        public const string REST_getTransactionsInBlock = "/api/v1/block/transactions/height/{0}"; //+height
        public const string REST_getMerkleProof = "/api/v1/merkleproof/{0}";//+hash 
        public const string REST_getStorage = "/api/v1/storage/{0}/{1}"; //+hash + /key
        public const string REST_sendRawTransaction = "/api/v1/transaction?preExec=0";
        public const string REST_sendRawTransactionPreExec = "/api/v1/transaction?preExec=1";
        public const string REST_getGasPrice = "/api/v1/gasprice";
        public const string REST_getAllowance = "/api/v1/allowance/{0}/{1}/{2}"; //+asset + /from + /to
        public const string REST_getBlockTxsByHeight = "/api/v1/block/transactions/height/{0}"; //+height
        public const string REST_getUnclaimedONG = " /api/v1/unclaimong/{0}"; //+addr"
        public const string REST_getMempoolTxState = "/api/v1/mempool/txstate/{0}"; //+hash"
        #endregion


    }
}
