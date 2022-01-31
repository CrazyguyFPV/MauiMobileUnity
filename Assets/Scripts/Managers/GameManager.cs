using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public enum CellStates { empty, X, O };

public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameObject uiObject;
    [SerializeField] ResultsScreen resultsScreen;

    TicTacToeManager tManager => TicTacToeManager.Instance;
    public bool acceptInput = false;
    public bool gameStarted = false;

    public string matchType = "";
    public double matchAmount = 0;

    // Start is called before the first frame update
    void Start()
    {
        // clean up ui
        tManager.yourTurn.SetActive(false);
        tManager.youWon.SetActive(false);
        tManager.gameOverObj.SetActive(false);
        gameStarted = false;


        //matchType = "O12348";
        //matchAmount = 300;
        //TicTacToeManager.Instance.playerX = "PlayerX";
        //TicTacToeManager.Instance.playerO = "PlayerO";
        //resultsScreen.Init(false, matchType, matchAmount);

    }

    public void Play() {
        StartCoroutine(PlayGame());
    }

    // Update is called once per frame
    IEnumerator PlayGame()
    {
        yield return StartCoroutine(tManager.Find());
        yield return StartCoroutine(tManager.Join());

        // wait for a second player to join
        bool gotMatch = false;
        while(!gotMatch)
        {
            yield return StartCoroutine(tManager.GetGameState());
            yield return new WaitForSeconds(0.3f);
            gotMatch = tManager.playerX != "" && tManager.playerO != "";
        }

        Debug.Log("<color=red>[SBG] >> " + "Got Match, starting game" + "</color>");
        uiObject.SetActive(false);
        gameStarted = true;

        while (true)
        {
            yield return StartCoroutine(tManager.GetGameState());
            if (tManager.turn == tManager.playerPiece)
                acceptInput = true;
            else
                acceptInput = false;

            if(tManager.gameOver)
            {
                Debug.Log("<color=red>[SBG] >> " + "Game Over - winner: " + tManager.winner + "</color>");
                AudioManager.Instance.PlayGameOverClip();
                yield return new WaitForSeconds(2);
                uiObject.SetActive(true);           // turn ui back on

                resultsScreen.Init(tManager.winner == tManager.playerPiece, matchType, matchAmount);
                yield return StartCoroutine(tManager.EndGame());
                yield break;
            }

            yield return new WaitForSeconds(0.2f);          // slow updates down so we don't flood the network with traffic
        }

    }

}
