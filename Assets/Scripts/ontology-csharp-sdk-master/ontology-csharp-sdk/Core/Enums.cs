namespace OntologyCSharpSDK.Core
{

        
    public enum TxType
    {
        BookKeeping = 0x00,
        IssueAsset = 0x01,
        BookKeeper = 0x02,
        Claim = 0x03,
        PrivacyPayload = 0x20,
        RegisterAsset = 0x40,
        TransferAsset = 0x80,
        Record = 0x81,
        Deploy = 0xd0,
        Invoke = 0xd1,
        DataFile = 0x12,
        Enrollment = 0x04,
        Vote = 0x05
    }


    public enum SignatureSchema
    {
        SHA224withECDSA = 0,
        SHA256withECDSA = 1,
        SHA384withECDSA,
        SHA512withECDSA,
        SHA3_224withECDSA,
        SHA3_256withECDSA,
        SHA3_384withECDSA,
        SHA3_512withECDSA,
        RIPEMD160withECDSA,
        SM3withSM2,
        SHA512withEDDSA
    }

 
    public enum KeyType
    {
        PK_ECDSA = 0x12,
        PK_SM2 = 0x13,
        PK_EDDSA = 0x14,
    }


    public enum VmType
    {
        NativeVM = 0xFF,
        NEOVM = 0x80,
    }
}

