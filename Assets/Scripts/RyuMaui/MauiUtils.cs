//
//  MauiUtils.cs
//
//  Created by Rick Ellis on 1/5/2022.
//  Copyright © 2022 Ryu Games. All rights reserved.
//

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using OntologyCSharpSDK.Common;
using System.Text;
using System;
using Scrypt;
using System.Security.Cryptography;

namespace Ryu_Maui
{
    public static class MauiUtils
    {
        static System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create();

        // Brazil network comms
        public static IEnumerator MauiGetRequest(string uri, System.Action<string> result)
        {
            UnityWebRequest uwr = UnityWebRequest.Get(uri);
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
                result("");
            else
                result(uwr.downloadHandler.text);
        }

        public static IEnumerator MauiPostRequest(string url, WWWForm form, System.Action<string> result)
        {
            UnityWebRequest uwr = UnityWebRequest.Post(url, form);
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
                result("");
            else
                result(uwr.downloadHandler.text);

        }

        public static string RandomString()
        {
            StringBuilder str = new StringBuilder();
            char c;
            System.Random random = new System.Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < 32; i++)
            {
                c = Convert.ToChar(Convert.ToInt32(Math.Floor(58 * random.NextDouble() + 64)));
                str.Append(c);
            }
            return str.ToString();
        }

        //===============================================================================
        //Encryption steps
        //  Compute the NEO address(ASCII), and take the first four bytes of SHA256(SHA256()) of it.Let's call this "addresshash".
        //  Derive a key from the passphrase using scrypt
        //  Parameters: passphrase is the passphrase itself encoded in UTF-8 and normalized using Unicode Normalization Form C(NFC).
        //  Salt is the addresshash from the earlier step, n = 16384, r = 8, p = 8, length = 64
        //  Let 's split the resulting 64 bytes in half, and call them derivedhalf1 and derivedhalf2.
        //  Do AES256Encrypt(block = privkey[0...15] xor derivedhalf1[0...15], key = derivedhalf2), call the 16-byte result encryptedhalf1
        //  Do AES256Encrypt(block = privkey[16...31] xor derivedhalf1[16...31], key = derivedhalf2), call the 16-byte result encryptedhalf2
        //  The encrypted private key is the Base58Check - encoded concatenation of the following, which totals 39 bytes without Base58 checksum:
        //  0x01 0x42 + flagbyte + addresshash + encryptedhalf1 + encryptedhalf2
        //===============================================================================
        public static string NEP2Encrypt(string wif, string passphrase) // (s string, address string, err error)
        {
            byte[] privateKey = Helper.GetPrivateKeyFromWIF(wif);
            Debug.Log("<color=red>[SBG] >> " + "private key: " + Helper.Bytes2HexString(privateKey) + "</color>");
            string address = Helper.GetAddressFromPublicKey(Helper.GetPublicKeyFromPrivateKey(privateKey));
            byte[] byteAddress = Encoding.UTF8.GetBytes(address);
            byte[] addressHash = Helper.Sha256(Helper.Sha256(byteAddress))[0..4];

            string nPassphrase = passphrase.Normalize(NormalizationForm.FormC);     // normalize the pass string
            var passwordBytes = Encoding.UTF8.GetBytes(nPassphrase);

            var derivedKey = ScryptEncoder.CryptoScrypt(passwordBytes, addressHash, 16384, 8, 8, 64);

            byte[] derivedKey1 = derivedKey[..32];
            byte[] derivedKey2 = derivedKey[32..];

            var xr = XOR(privateKey, derivedKey1);
            var encrypted = Helper.AES256Encrypt(xr, derivedKey2);

            byte[] buf = new byte[39];
            buf[0] = 0x01;                  // nep header
            buf[1] = 0x42;
            buf[2] = 0xE0;                  // nep flag

            for (int x = 0; x < 4; x++)     // address hash (4 bytes)
                buf[3 + x] = addressHash[x];

            for (int x = 0; x < 32; x++)        // copy aes
            {
                buf[7 + x] = encrypted[x];
            }

            string b58Str = Helper.Base58CheckEncode(buf);
            //Debug.Log("<color=red>[SBG] >> " + "final string " + b58Str + "</color>");

            return b58Str;
        }

        //===================================================================================================
        //Decryption steps
        //  Collect encrypted private key and passphrase from user.
        //  Derive derivedhalf1 and derivedhalf2 by passing the passphrase and addresshash into scrypt function.
        //  Decrypt encryptedhalf1 and encryptedhalf2 using AES256Decrypt, merge the two parts and XOR the result
        //  with derivedhalf1 to form the plaintext private key.
        //  Convert that plaintext private key into a NEO address.
        //  Hash the NEO address, and verify that addresshash from the encrypted private key record matches the hash.
        //  If not, report that the passphrase entry was incorrect.
        //===================================================================================================
        public static string NEP2Decrypt(string key, string passphrase) // (s string, address string, err error)
        {
            // get decoded key
            var encrypted = Helper.Base58CheckDecode(key);

            // extract hash
            var addressHash = encrypted[3..7];

            // normalize passphrase
            string nPassphrase = passphrase.Normalize(NormalizationForm.FormC);     // normalize the pass string
            var phraseNorm = Encoding.UTF8.GetBytes(nPassphrase);

            // Cryptoscrypt it
            var derivedKey = ScryptEncoder.CryptoScrypt(phraseNorm, addressHash, 16384, 8, 8, 64);
            string derivedKeyS = Helper.Bytes2HexString(derivedKey);

            // separate data from key
            byte[] derivedKeyPart1 = derivedKey[..32];
            byte[] derivedKeyPart2 = derivedKey[32..];

            byte[] encryptedBytes = encrypted[7..];

            var decrypted = Helper.AES256Decrypt(encryptedBytes, derivedKeyPart2);
            var privBytes = XOR(decrypted, derivedKeyPart1);
            string privKey = Helper.Bytes2HexString(privBytes);

            return privKey;
        }

        public static byte[] XOR(byte[] key, byte[] PAN)
        {
            if (key.Length == PAN.Length)
            {
                byte[] result = new byte[key.Length];
                for (int i = 0; i < key.Length; i++)
                {
                    result[i] = (byte)(key[i] ^ PAN[i]);
                }
                return result;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}