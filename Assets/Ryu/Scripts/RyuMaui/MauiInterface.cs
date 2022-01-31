//
//  MauiInterface.cs
//  Maui -> Brazil Interface for Unity
//
//  Created by Rick Ellis on 1/6/22.
//  Copyright © 2021 Ryu Games. All rights reserved.
//

using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;
using OntologyCSharpSDK.Common;

namespace Ryu_Maui
{
    public class MauiInterface : MonoBehaviour
    {
        private NewCodeResponse errorHandlingNewCodeResult;
        private LoginWithCodeResult loginErrorResult;


        public IEnumerator SendNewCode(string phoneNumber, System.Action<NewCodeResponse> callback)
        {
            string endPoint = MauiConstants.BrazilURL + MauiConstants.BrazilSendNewCode;

            WWWForm parameters = new();
            parameters.AddField("apiKey", MauiConstants.BrazilAPIKey);
            parameters.AddField("gameId", MauiConstants.BrazilGameID);
            parameters.AddField("phoneNumber", phoneNumber);
            parameters.AddField("retryMethod", "1");

            string response = "";
            NewCodeResult r = new();

            yield return StartCoroutine(MauiUtils.MauiPostRequest(endPoint, parameters, value => response = value));
            // error?
            if (response == "")
            {
                callback(errorHandlingNewCodeResult);
            }
            else
            {
                r = JsonConvert.DeserializeObject<NewCodeResult>(response);
                NewCodeResponse resp = r.result;
                callback(resp);
            }
        }

        public IEnumerator isUsernameTaken(string username, System.Action<bool> callback)
        {
            string endPoint = MauiConstants.BrazilURL + MauiConstants.BrazilIsUsernameTaken;
            endPoint += "?" + "username=" + username + "&apiKey=" + MauiConstants.BrazilAPIKey + "&gameId=" + MauiConstants.BrazilGameID;

            string response = "";
            BoolResult r = new();
            yield return StartCoroutine(MauiUtils.MauiGetRequest(endPoint, value => response = value));

            // error?
            if (response == "")
            {
                callback(true);
            }
            else
            {
                r = JsonConvert.DeserializeObject<BoolResult>(response);
                callback(r.result);
            }
        }

        public string[] getJoinSignature(string message)
        {
            string privateKeyString = StorageManager.Instance.GetPrivateKey();
            byte[] publicKey = Helper.GetPublicKeyFromPrivateKey(Helper.HexString2Bytes(privateKeyString));
            string address = Helper.GetAddressFromPublicKey(publicKey);
            string publicKeyString = Helper.Bytes2HexString(publicKey);
            string signature = Helper.Bytes2HexString(Helper.Sign(Encoding.UTF8.GetBytes(message), Helper.HexString2Bytes(privateKeyString)));
            return new string[] { address, publicKeyString, signature };
        }

        public string[] getSignature()
        {
            string privateKeyString = StorageManager.Instance.GetPrivateKey();
            byte[] publicKey = Helper.GetPublicKeyFromPrivateKey(Helper.HexString2Bytes(privateKeyString));
            string address = Helper.GetAddressFromPublicKey(publicKey);
            string publicKeyString = Helper.Bytes2HexString(publicKey);
            string message = "thisisatest";
            string signature = Helper.Bytes2HexString(Helper.Sign(Encoding.UTF8.GetBytes(message), Helper.HexString2Bytes(privateKeyString)));
            return new string[] { address, publicKeyString, message, signature };
        }

        //Withdraw
        public IEnumerator Withdraw(double amount, System.Action<bool> callback)
        {
            string endPoint = MauiConstants.BrazilURL + MauiConstants.BrazilWithdraw;

            string[] info = getSignature();
            string address = info[0];
            string publicKeyString = info[1];
            string message = info[2];
            string signature = info[3];

            WWWForm parameters = new();
            parameters.AddField("amount", amount.ToString());
            parameters.AddField("address", address);
            parameters.AddField("publicKey", publicKeyString);
            parameters.AddField("message", message);
            parameters.AddField("signature", signature);
            parameters.AddField("apiKey", MauiConstants.BrazilAPIKey);
            parameters.AddField("gameId", MauiConstants.BrazilGameID);

            string response = "";
            BoolResult r = new();

            yield return StartCoroutine(MauiUtils.MauiPostRequest(endPoint, parameters, value => response = value));

            // error?
            if (response == "")
            {
                callback(false);
            }
            else
            {
                r = JsonConvert.DeserializeObject<BoolResult>(response);
                callback(r.result);
            }
        }

        public IEnumerator NewAccountWithoutPhone(string username, OntologyCSharpSDK.Wallet.Account account, string apiKey, string gameId, System.Action<bool> callback)
        {
            string endPoint = MauiConstants.BrazilURL + MauiConstants.BrazilLoginUser;

            byte[] privateKey = Helper.CreatePrivateKey();
            string privateKeyString = Helper.Bytes2HexString(privateKey);
            byte[] publicKey = Helper.GetPublicKeyFromPrivateKey(Helper.HexString2Bytes(privateKeyString));
            string publicKeyString = Helper.Bytes2HexString(publicKey);
            string address = Helper.GetAddressFromPublicKey(publicKey);

            string passphrase = MauiUtils.ComputeSha256Hash("STRING");
            string eKey = MauiUtils.NEP2Encrypt(Helper.GetWifFromPrivateKey(privateKey), passphrase);

            StorageManager.Instance.StorePrivateKey(privateKeyString); // Private Key storage in player prefs for the moment

            WWWForm parameters = new();
            parameters.AddField("address", address);
            parameters.AddField("publicKey", publicKeyString);
            parameters.AddField("username", username);
            parameters.AddField("encryptedKey", eKey);
            parameters.AddField("deviceType", "PC");
            string message = MauiUtils.RandomString();
            parameters.AddField("message", message);
            string signature = Helper.Bytes2HexString(Helper.Sign(Encoding.UTF8.GetBytes(message), privateKey));
            parameters.AddField("signature", signature);
            parameters.AddField("apiKey", apiKey);
            parameters.AddField("gameId", gameId);

            string response = "";
            BoolResult r = new();

            yield return StartCoroutine(MauiUtils.MauiPostRequest(endPoint, parameters, value => response = value));

            // error?
            callback(response == "");
        }

        public IEnumerator Backup(string code, string phoneNumber, System.Action<bool> callback)
        {
            string endPoint = MauiConstants.BrazilURL + MauiConstants.BrazilBackup;

            string[] info = getSignature();
            string address = info[0];
            string publicKeyString = info[1];
            string message = info[2];
            string signature = info[3];
            string username = StorageManager.Instance.GetUsername();

            string privateKeyString = StorageManager.Instance.GetPrivateKey();
            string passphrase = MauiUtils.ComputeSha256Hash(phoneNumber + "-STRING");
            string encryptedKey = MauiUtils.NEP2Encrypt(privateKeyString, passphrase);

            WWWForm parameters = new();
            parameters.AddField("address", address);
            parameters.AddField("username", username);
            parameters.AddField("code", code);
            parameters.AddField("message", message);
            parameters.AddField("publicKey", publicKeyString);
            parameters.AddField("signature", signature);
            parameters.AddField("phoneNumber", phoneNumber);
            parameters.AddField("encryptedKey", encryptedKey);
            parameters.AddField("apiKey", MauiConstants.BrazilAPIKey);
            parameters.AddField("gameId", MauiConstants.BrazilGameID);

            string response = "";
            BoolResult r = new();

            yield return StartCoroutine(MauiUtils.MauiPostRequest(endPoint, parameters, value => response = value));

            // error?
            if (response == "")
            {
                callback(false);
            }
            else
            {
                r = JsonConvert.DeserializeObject<BoolResult>(response);
                if (r.result)
                {
                    StorageManager.Instance.StorePhoneNumber(phoneNumber);
                }
                callback(r.result);
            }
        }

        private GetBalancesResult errorHandlingResult;
        public IEnumerator GetBalances(System.Action<GetBalancesResult> callback)
        {
            string[] info = getSignature();
            string endPoint = MauiConstants.BrazilURL + MauiConstants.BrazilGetBalances;
            string address = info[0];
            string publicKeyString = info[1];
            string message = info[2];
            string signature = info[3];
            endPoint += "?" + "address=" + address + "&publicKey=" + publicKeyString + "&message=" + message + "&signature=" + signature + "&apiKey=" + MauiConstants.BrazilAPIKey + "&gameId=" + MauiConstants.BrazilGameID;

            string response = "";
            GetBalancesResponse r = new();
            yield return StartCoroutine(MauiUtils.MauiGetRequest(endPoint, value => response = value));

            // error?
            if (response == "")
            {
                callback(errorHandlingResult);
            }
            else
            {
                r = JsonConvert.DeserializeObject<GetBalancesResponse>(response);
                callback(r.result);
            }
        }

        public IEnumerator GetWithdrawableBalance(System.Action<double> callback)
        {
            string privateKeyString = StorageManager.Instance.GetPrivateKey();
            byte[] publicKey = Helper.GetPublicKeyFromPrivateKey(Helper.HexString2Bytes(privateKeyString));
            string address = Helper.GetAddressFromPublicKey(publicKey);

            string endPoint = MauiConstants.BrazilURL + MauiConstants.BrazilGetWithdrawableBalance;
            endPoint += "?" + "address=" + address + "&apiKey=" + MauiConstants.BrazilAPIKey + "&gameId=" + MauiConstants.BrazilGameID;

            string response = "";
            DoubleResult r = new();
            yield return StartCoroutine(MauiUtils.MauiGetRequest(endPoint, value => response = value));

            // error?
            if (response == "")
            {
                callback(0);
            }
            else
            {
                r = JsonConvert.DeserializeObject<DoubleResult>(response);
                callback(r.result);
            }
        }

        public IEnumerator LoginUser(string code, string phoneNumber, System.Action<LoginWithCodeResult> callback)
        {
            string endPoint = MauiConstants.BrazilURL + MauiConstants.BrazilLoginUser;

            WWWForm parameters = new();
            parameters.AddField("code", code);
            parameters.AddField("phoneNumber", phoneNumber);
            parameters.AddField("apiKey", MauiConstants.BrazilAPIKey);
            parameters.AddField("gameId", MauiConstants.BrazilGameID);

            string response = "";
            LoginWithCodeResponse r = new();

            yield return StartCoroutine(MauiUtils.MauiPostRequest(endPoint, parameters, value => response = value));

            // error?
            if (response == "")
            {
                callback(loginErrorResult);
            }
            else
            {
                r = JsonConvert.DeserializeObject<LoginWithCodeResponse>(response);
                callback(r.result);
            }
        }



        //======================================
        // Unused for now
        //======================================
        public IEnumerator GetAddress(string username)
        {
            string endPoint = MauiConstants.BrazilURL + MauiConstants.BrazilGetAddress;
            endPoint += "?" + "username=" + username + "&apiKey=" + MauiConstants.BrazilAPIKey + "&gameId=" + MauiConstants.BrazilGameID;

            string response = "";
            GetAddressResult r = new();
            yield return StartCoroutine(MauiUtils.MauiGetRequest(endPoint, value => response = value));

            // error?
            if (response == "")
                Debug.Log("<color=red>>> " + "GetAddress Error" + "</color>");
            else
            {
                r = JsonConvert.DeserializeObject<GetAddressResult>(response);
                GetAddressResponse resp = r.result;
            }
        }

        public IEnumerator GetUsername(string address)
        {
            string endPoint = MauiConstants.BrazilURL + MauiConstants.BrazilGetUsername;
            endPoint += "?" + "address=" + address + "&apiKey=" + MauiConstants.BrazilAPIKey + "&gameId=" + MauiConstants.BrazilGameID;

            string response = "";
            StringResult r = new();
            yield return StartCoroutine(MauiUtils.MauiGetRequest(endPoint, value => response = value));

            // error?
            if (response == "")
                Debug.Log("<color=red>[SBG] >> " + "Error" + "</color>");
            else
            {
                r = JsonConvert.DeserializeObject<StringResult>(response);
                Debug.Log("<color=red>[SBG] >> username: " + r.result + "</color>");
            }
        }

    }
}