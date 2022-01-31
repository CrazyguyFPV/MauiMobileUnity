using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryu_Maui;

public class RyuInterface : MonoBehaviour
{
    private static MauiInterface Maui;

    private void Update()
    {
        if (Maui == null)
            Maui = FindObjectOfType<MauiInterface>();
    }


    public static void SendNewCode(string phoneNumber, System.Action<NewCodeResponse> callback)
    {
        StaticCoroutine.Start(Maui.SendNewCode(phoneNumber, balances =>
        {
            callback(balances);
        }));
    }

    public static void IsUsernameTaken(string username, System.Action<bool> callback)
    {
        Maui = FindObjectOfType<MauiInterface>();
        Debug.Log("<color=red>[SBG] >> " + "maui [" + Maui + "]" + "</color>");
        StaticCoroutine.Start(Maui.isUsernameTaken(username, isTaken =>
        {
            callback(isTaken);
        }));
    }

    public static void Withdraw(double amount, System.Action<bool> callback)
    {
        StaticCoroutine.Start(Maui.Withdraw(amount, didSucceed =>
        {
            callback(didSucceed);
        }));
    }

    public static void NewAccountWithoutPhone(string attemptedUserName, OntologyCSharpSDK.Wallet.Account account, string apiKey, string gameId, System.Action<bool> callback)
    {
        StaticCoroutine.Start(Maui.NewAccountWithoutPhone(attemptedUserName, account, apiKey, gameId, didSucceed =>
        {
            callback(didSucceed);
        }));
    }
    
    public static void Backup(string code, string phoneNumber, System.Action<bool> callback)
    {
        StaticCoroutine.Start(Maui.Backup(code, phoneNumber, didSucceed =>
        {
            callback(didSucceed);
        }));
    }

    public static void GetBalances(System.Action<GetBalancesResult> callback)
    {
        StaticCoroutine.Start(Maui.GetBalances(balances =>
        {
            callback(balances);
        }));
    }

    public static void GetWithdrawableBalance(System.Action<double> callback)
    {
        StaticCoroutine.Start(Maui.GetWithdrawableBalance(balances =>
        {
            callback(balances);
        }));
    }

    public static void LoginUser(string code, string phoneNumber, System.Action<LoginWithCodeResult> callback)
    {
        StaticCoroutine.Start(Maui.LoginUser(code, phoneNumber, logResult =>
        {
            callback(logResult);
        }));
    }
}
