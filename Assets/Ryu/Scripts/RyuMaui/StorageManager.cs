using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class StorageManager : Singleton<StorageManager>
{
    // vars that will be stored in playerprefs for user preferences
    public string usernameKey = "username-key";
    public string privateKeyKey = "privatekey-key";
    public string phoneNumberKey = "phoneNumberKey-key";

    public void SetUsername(string username)
    {
        PlayerPrefs.SetString(usernameKey, username);

        PlayerPrefs.Save();
    }

    public bool isLoggedIn()
    {
        return PlayerPrefs.HasKey(usernameKey);
    }

    public string GetUsername()
    {
        string username = PlayerPrefs.GetString(usernameKey);
        return username;
    }

    public void StorePrivateKey(string privateKey)
    {
        PlayerPrefs.SetString(privateKeyKey, privateKey);

        PlayerPrefs.Save();
    }

    public string GetPrivateKey()
    {
        return PlayerPrefs.GetString(privateKeyKey);
    }

    public bool HasBackedupPhone()
    {
        return PlayerPrefs.HasKey(phoneNumberKey);
    }

    public string GetPhoneNumber()
    {
        Debug.Log(PlayerPrefs.GetString(phoneNumberKey));
        return PlayerPrefs.GetString(phoneNumberKey);
    }

    public void StorePhoneNumber(string phoneNumber)
    {
        PlayerPrefs.SetString(phoneNumberKey, phoneNumber);

        PlayerPrefs.Save();
    }

    public void Logout()
    {
        PlayerPrefs.DeleteKey(usernameKey);
        PlayerPrefs.DeleteKey(privateKeyKey);
        PlayerPrefs.DeleteKey(phoneNumberKey);
    }
}
