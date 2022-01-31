namespace OntologyCSharpSDK.Interface
{
    public interface IConnectionMethod
    {

        /// <summary>
        /// Returns the current network block height (most recent block)
        /// </summary>
        /// <returns></returns>
        int getBlockHeight();

        /// <summary>
        /// Returns a JSON string representing the ONT/ONG balance of an address
        /// </summary>
        /// <param name="ONTAddress"></param>
        /// <returns></returns>
        string getAddressBalance(string ONTAddress);

        /// <summary>
        /// Returns the number of nodes connected
        /// </summary>
        /// <returns></returns>
        int getNodeCount();

        /// <summary>
        /// Returns the block height for a given transaction hash
        /// </summary>
        /// <param name="txHash"></param>
        /// <returns></returns>

        long getBlockHeightByTxHash(string txHash);

        /// <summary>
        /// Returns the hex representation of a block based on block height
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        string getBlockHex(int blockHeight);

        /// <summary>
        /// Returns the hex representation of a block based on block hash
        /// </summary>
        /// <param name="blockHash"></param>
        /// <returns></returns>
        string getBlockHex(string blockHash);

        /// <summary>
        /// Returns a JSON string representing a block based on block height
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        string getBlockJson(int blockHeight);

        /// <summary>
        /// Returns a JSON string representing a block based on block hash
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        string getBlockJson(string blockHash);

        /// <summary>
        /// Returns the hex representation of a transaction based on transaction hash
        /// </summary>
        /// <param name="txHash"></param>
        /// <returns></returns>
        string getRawTransactionHex(string txHash);


        /// <summary>
        /// Returns a JSON string representing  a transaction based on transaction hash
        /// </summary>
        /// <param name="txHash"></param>
        /// <returns></returns>
        string getRawTransactionJson(string txHash);

        /// <summary>
        ///  Returns hash of the highest block
        /// </summary>
        /// <returns></returns>        
        string getBestBlockHash();

        /// <summary>
        /// Returns the hash of the specified block height
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        string getBlockHashByHeight(int blockHeight);

        /// <summary>
        /// Returns the stored value (hex string) according to the contract script hashes and stored key
        /// </summary>
        /// <param name="txHash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        string getStorage(string contractHash, string key);

        /// <summary>
        /// Returns the version of the queried node
        /// </summary>
        /// <returns></returns>
        int getVersion();

        /// <summary>
        /// Returns the system fee (in ONG) before the specified block
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        int getBlockSysFee(int blockHeight);

        /// <summary>
        /// Returns the contract information of the supplied script hash
        /// </summary>
        /// <param name="scriptHash"></param>
        /// <returns></returns>
        string getContractJson(string contractHash);

        /// <summary>
        /// Returns the stranasction status as per the memory pool
        /// </summary>
        /// <param name="txHash"></param>
        /// <returns></returns>
        string getMempoolTxState(string txHash);

        /// <summary>
        /// Returns merkle proof
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        string getMerkleProof(string hash);

        /// <summary>
        /// Broadcasts a transaction to the network (must be a valid sign transaction in hex format)
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="preExec"></param>
        /// <returns></returns>
        string setSendRawTransaction(string tx, bool preExec);

        /// <summary>
        /// Gets the current GAS price (transaction fee)
        /// </summary>
        /// <returns></returns>
        string getGasPrice();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="fromAddress"></param>
        /// <param name="toAddress"></param>
        /// <returns></returns>
        string getAllowance(string asset, string fromAddress, string toAddress);

        /// <summary>
        /// Returns the transactions in a given block
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        string getBlockTxsByHeight(int blockHeight);

        /// <summary>
        /// Returns the amount of unclaimed ONG for given address
        /// </summary>
        /// <param name="ONTAddress"></param>
        /// <returns></returns>
        string getUnclaimedONG(string ONTAddress);
        string getSmartCodeEvent(string txHash);
        string getSmartCodeEvent(int blockHeight);
    }
}
