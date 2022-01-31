<h1 align="center">Ontology C# SDK </h1>

## Overview

This C# SDK aims to help .NET developers when writing applications for the Ontology Blockchain.  


## Setup / Usage

Requires .NET v4.7

1. Download repository
```
https://github.com/OntologyCommunityDevelopers/ontology-csharp-sdk.git
```
2. Build the repository, ddl files should be available in `ontology-csharp-sdk\ontology-csharp-sdk\bin\Debug folder`.

3. Add the ddl files in your client project, invoke the library

```
using OntologyCSharpSDK;
```

4. Create instance by specifying node parameter and ConnectionMethod (RPC, REST or Websocket), e.g.
```
OntologySDK ontSDK = new OntologySDK(node, ConnectionMethod.REST);
```
node parameter is constructed by domain/IP + port

| Network | Port |
| :---| :---|
| REST | 20334|
| Websocket | 20335|
| RPC | 20336|

For domain/IP, use current testnet address `http://polaris1.ont.io` or your private net address

For example, calling REST method for testnet(currently 0.75) will be below:
```
OntologySDK ontSDK = new OntologySDK("http://polaris1.ont.io:20334", ConnectionMethod.REST);
```

5. Call methods, e.g.
```
int BlockHeight = ontSDK.connectionManager.getBlockHeight();
```

See more code examples in `ontology-csharp-demo` project


## Methods

All RPC, REST and Websocket methods implemented by the Ontology network are available via this SDK, for a list of each based on connection method, see below:

REST:      https://github.com/ontio/documentation/blob/master/docs/pages/doc_en/restfulapi_en.md<br>
RPC:       https://github.com/ontio/documentation/blob/master/docs/pages/doc_en/ontrpcapi_en.md<br>
Websocket: https://github.com/ontio/documentation/blob/master/docs/pages/doc_en/websocket_en.md<br>


## Current Contributors

- xizho10

Github: https://github.com/xizho10

- Panther

Github: https://github.com/panther142
Discord: Panther#3489





