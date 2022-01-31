using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToeResponses : MonoBehaviour
{
 
}


public class JoinResult
{
    public JoinResponse result;
}
public class JoinResponse
{
    public string player;
    public Players players;
    public string matchId;
    public bool gameOver;
    public string winner;
    public string turn;
    public JArray[] board;
}

public class Players
{
    public string X = "";
    public string O = "";
}


