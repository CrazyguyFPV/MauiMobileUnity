using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;
using Ryu_Maui;

public class ResultsScreen : MonoBehaviour
{
    [SerializeField] UIManager uiMgr;
    [SerializeField] TextMeshProUGUI bannerText;
    [SerializeField] TextMeshProUGUI cashText;
    [SerializeField] TextMeshProUGUI myScoreText;
    [SerializeField] TextMeshProUGUI opponentScoreText;

    [Header("My Loot")]
    [SerializeField] GameObject myLootObj;
    [SerializeField] GameObject myCashIcon;
    [SerializeField] GameObject myGemIcon;
    [SerializeField] TextMeshProUGUI myWinAmount;

    [Header("Opponent Loot")]
    [SerializeField] GameObject opponentLootObj;
    [SerializeField] GameObject opponentCashIcon;
    [SerializeField] GameObject opponentGemIcon;
    [SerializeField] TextMeshProUGUI opponentWinAmount;
    [SerializeField] TextMeshProUGUI opponentUsername;

    // Start is called before the first frame update
    public void Init(bool won, string matchType, double matchAmount)
    {
        Debug.Log("<color=red>[SBG] >> " + "yup" + "</color>");
        SetBannerText(won);
        SetCashBalance();
        SetPlayerScores(won);

        uiMgr.LoadBalances();

        // setup loot UI
        myLootObj.SetActive(won);
        opponentLootObj.SetActive(!won);
        if(won)
        {
            myCashIcon.SetActive(matchType != "O12348");
            myGemIcon.SetActive(matchType == "O12348");
            if (matchType == "O12348")
                myWinAmount.text = matchAmount.ToString();
            else
                myWinAmount.text = matchAmount.ToString("C", new CultureInfo("en-US"));
        }
        else
        {
            opponentCashIcon.SetActive(matchType != "O12348");
            opponentGemIcon.SetActive(matchType == "O12348");
            if (matchType == "O12348")
                opponentWinAmount.text = matchAmount.ToString();
            else
                opponentWinAmount.text = matchAmount.ToString("C", new CultureInfo("en-US"));

            string opponentName = "";
            if (TicTacToeManager.Instance.playerPiece == "X")
                opponentName = TicTacToeManager.Instance.playerO;
            else
                opponentName = TicTacToeManager.Instance.playerX;

            opponentUsername.text = opponentName;

        }

        gameObject.SetActive(true);
    }

    public void SetBannerText(bool won)
    {
        bannerText.text = "You Won!";
        if (!won)
            bannerText.text = "Game Over";
    }

    public void SetCashBalance()
    {
        cashText.text = uiMgr.myCash.ToString("C", new CultureInfo("en-US"));
    }

    public void SetPlayerScores(bool won)
    {
        myScoreText.text = won ? "Won" : "Lost";
        opponentScoreText.text = !won ? "Won" : "Lost";
    }

    public void ExitPressed()
    {
        gameObject.SetActive(false);
    }

}
