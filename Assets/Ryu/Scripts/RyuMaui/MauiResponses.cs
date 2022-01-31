//
//  MauiResponses.cs
//
//  Created by Rick Ellis on 1/5/2022.
//  Copyright © 2022 Ryu Games. All rights reserved.
//

namespace Ryu_Maui
{

    // SendNewCode
    public class NewCodeResult
    {
        public NewCodeResponse result;
    }
    public class NewCodeResponse
    {
        public string messageId;
        public string address;
        public bool exists;
    }

    // GetAddress
    public class GetAddressResult
    {
        public GetAddressResponse result;
    }
    public class GetAddressResponse
    {
        public string address;
        public string username;
    }

    // Generics
    public class BoolResult
    {
        public bool result;
    }

    public class StringResult
    {
        public string result;
    }
    public class IntResult
    {
        public int result;
    }

    public class DoubleResult
    {
        public int result;
    }

    public class GetBalancesResponse
    {
        public GetBalancesResult result;
    }
    public class GetBalancesResult
    {
        public int pearls;
        public double cash;
    }

    public class LoginWithCodeResponse
    {
        public LoginWithCodeResult result;
    }
    public class LoginWithCodeResult
    {
        public string encryptedKey;
        public string username;
        public string address;
    }
}

