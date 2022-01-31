using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryu_Maui
{
    public static class MauiConstants
    {
        // Access strings
        public static string BrazilGameID = "GTICTACTOEOAHU";
        public static string BrazilAPIKey = "A1552332591";

        // Brazil interface strings
        public static string BrazilURL = "https://dev-api.ryu.games";
        public static string BrazilGetUsername = "/v2/accounts/getUsername";
        public static string BrazilSendNewCode = "/v2/accounts/sendNewCode";
        public static string BrazilIsUsernameTaken = "/v2/accounts/isUsernameTaken";
        public static string BrazilGetAddress = "/v2/accounts/getAddress";
        public static string BrazilBackup = "/v2/accounts/backup";
        public static string BrazilLoginUser = "/v2/accounts/loginUser";
        public static string BrazilGetBalances = "/v2/accounts/getBalances";
        public static string BrazilGetWithdrawableBalance = "/v2/accounts/getWithdrawableBalance";
        public static string BrazilWithdraw = "/v2/accounts/withdrawS2S";

        // Test
        public static string BrazilTestPhoneNumber = "+17772222223";
    }
}