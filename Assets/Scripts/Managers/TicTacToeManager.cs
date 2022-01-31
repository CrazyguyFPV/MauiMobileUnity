using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Ryu_Maui;

public class TicTacToeManager : Singleton<TicTacToeManager>
{
    [SerializeField] List<Cell> gameboardCells;
    [SerializeField] public GameObject yourTurn;
    [SerializeField] public GameObject youWon;
    [SerializeField] public GameObject gameOverObj;
    [SerializeField] public MauiInterface Maui;

    public string playerPiece;
    public string matchId;
    public bool gameOver;
    public string winner;
    public string turn;
    public JArray[] board;

    public string playerX = "";
    public string playerO = "";

    public string optionId;

    public IEnumerator Find()
    {
        string playerId = StorageManager.Instance.GetUsername();
        string endPoint = GameConstants.GameHostURL + "find";
        endPoint += "?" + "playerId=" + playerId;

        if (optionId != "") {
            endPoint += "&optionId=" + optionId;
        }

        string response = "";
        StringResult r = new();
        yield return StartCoroutine(MauiUtils.MauiGetRequest(endPoint, value => response = value));

        // error?
        if (response == "")
            Debug.Log("<color=red>[SBG] >> " + "Error" + "</color>");
        else
        {
            r = JsonConvert.DeserializeObject<StringResult>(response);
            matchId = r.result;
            Debug.Log("<color=green>[SBG] >> " + "Found match " + matchId + "</color>");
        }
    }

    public IEnumerator Join()
    {
        string endPoint = GameConstants.GameHostURL + "join";
        string playerId = StorageManager.Instance.GetUsername();
        WWWForm parameters = new();
        parameters.AddField("playerId", playerId);
        parameters.AddField("matchId", matchId);

        if (optionId != "") {
            double entry = 0;
            double prize = 18;
            if (optionId == "O12348") 
            {
                entry = 0;
                prize = 18;
            } else if (optionId == "O12346") 
            {
               entry = 3.00;
                prize = 5;
            }
            else if (optionId == "O12347") 
            {
                entry = 300.00;
                prize = 500;
            }
            else 
                Debug.Log("Unrecognized optionId " + optionId);

            GameManager.Instance.matchAmount = prize;

            string gameId = "GTICTACTOEOAHU";
            string joinMessage = "gameId=" + gameId + "&entry=" + entry.ToString() + "&matchId=" + matchId;
            parameters.AddField("joinMessage", joinMessage);

            string[] info = Maui.getJoinSignature(joinMessage);
            string address = info[0];
            string publicKey = info[1];
            string joinSignature = info[2];

            parameters.AddField("address", address);
            parameters.AddField("publicKey", publicKey);
            parameters.AddField("joinSignature", joinSignature);
        }

        string response = "";
        JoinResult r = new();

        yield return StartCoroutine(MauiUtils.MauiPostRequest(endPoint, parameters, value => response = value));
        // error?
        if (response == "")
            Debug.Log("<color=red>[SBG] >> " + "sendNewCode Error" + "</color>");
        else
        {
            //Debug.Log("<color=red>[SBG] >> " + "-- " + response + "</color>");
            r = JsonConvert.DeserializeObject<JoinResult>(response);
            JoinResponse resp = r.result;
            playerPiece = resp.player;
            gameOver = resp.gameOver;
            turn = resp.turn;
            board = resp.board;
            playerX = resp.players.X;
            playerO = resp.players.O;

            Debug.Log("<color=red>[SBG] >> -------------------------- " + playerX + "    " + playerO + "</color>");
        }
    }

    public IEnumerator SubmitMove(int row, int col)
    {
        string endPoint = GameConstants.GameHostURL + "submitMove";
        string playerId = StorageManager.Instance.GetUsername();
        WWWForm parameters = new();
        parameters.AddField("playerId", playerId);
        parameters.AddField("matchId", matchId);
        parameters.AddField("row", row);
        parameters.AddField("column", col);

        string response = "";
        JoinResult r = new();

        yield return StartCoroutine(MauiUtils.MauiPostRequest(endPoint, parameters, value => response = value));
        // error?
        if (response == "")
            Debug.Log("<color=red>[SBG] >> " + "Error" + "</color>");
        else
        {
            //Debug.Log("<color=red>[SBG] >> " + "response from submitmove " + response + "</color>");
            r = JsonConvert.DeserializeObject<JoinResult>(response);
            JoinResponse resp = r.result;
            gameOver = resp.gameOver;
            turn = resp.turn;
            board = resp.board;
            UpdateGameboard();
        }
    }

    public IEnumerator GetGameState()
    {
        string endPoint = GameConstants.GameHostURL + "get";
        string playerId = StorageManager.Instance.GetUsername();
        endPoint += "?" + "matchId=" + matchId + "&playerId=" + playerId;

        string response = "";
        JoinResult r = new();
        yield return StartCoroutine(MauiUtils.MauiGetRequest(endPoint, value => response = value));

        // error?
        if (response == "")
            Debug.Log("<color=red>[SBG] >> " + "Error" + "</color>");
        else
        {
            r = JsonConvert.DeserializeObject<JoinResult>(response);
            JoinResponse resp = r.result;
            gameOver = resp.gameOver;
            turn = resp.turn;
            board = resp.board;
            winner = resp.winner;
            playerX = resp.players.X;
            playerO = resp.players.O;

            UpdateGameboard();
        }
    }

    public IEnumerator EndGame()
    {
        string endPoint = GameConstants.GameHostURL + "endGame";
        endPoint += "?" + "matchId=" + matchId;

        string response = "";
        JoinResult r = new();
        yield return StartCoroutine(MauiUtils.MauiGetRequest(endPoint, value => response = value));

        // clean up
        yourTurn.SetActive(false);
        youWon.SetActive(false);
        gameOverObj.SetActive(false);
}


    public void UpdateGameboard()
    {
        if (!GameManager.Instance.gameStarted)
            return;

        for(int row = 0; row < 3; row++)
        {
            for(int col = 0; col < 3; col++)
            {
                CellStates newState = CellStates.empty;
                string symbol = (string)board[row][col];
                if (symbol == "X") newState = CellStates.X;
                if (symbol == "O") newState = CellStates.O;
                gameboardCells[row * 3 + col].GetComponent<Cell>().SetState(newState);
            }
        }
        yourTurn.SetActive(turn == playerPiece);
        if(gameOver && winner != "")
        {
            GameManager.Instance.acceptInput = false;
            youWon.SetActive(winner == playerPiece);
            gameOverObj.SetActive(winner != playerPiece);
        }
    }

    public void topPlayClicked()
    {
        optionId = "O12348";
        GameManager.Instance.matchType = optionId;
        GameManager.Instance.Play();
        Debug.Log("<color=red>[SBG] >> " + "Playing option " + optionId + "</color>");
    }

    public void middlePlayClicked()
    {
        optionId = "O12346";
        GameManager.Instance.matchType = optionId;
        GameManager.Instance.Play();
        Debug.Log("<color=red>[SBG] >> " + "Playing option " + optionId + "</color>");
    }

    public void bottomPlayClicked()
    {
        optionId = "O12347";
        GameManager.Instance.matchType = optionId;
        GameManager.Instance.Play();
        Debug.Log("<color=red>[SBG] >> " + "Playing option " + optionId + "</color>");
    }
}
