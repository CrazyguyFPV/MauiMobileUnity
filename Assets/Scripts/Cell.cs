using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] GameObject xImage;
    [SerializeField] GameObject oImage;
    [SerializeField] int row;
    [SerializeField] int col;

    CellStates state = CellStates.empty;
    TicTacToeManager tManager => TicTacToeManager.Instance;

    void Start()
    {
        SetState(CellStates.empty);
    }

    public void SetState(CellStates newState)
    {
        state = newState;
        xImage.SetActive(state == CellStates.X);
        oImage.SetActive(state == CellStates.O);
    }

    public void Touched()
    {
        if (tManager.turn == tManager.playerPiece)       // can I tap??
        {
            StartCoroutine(tManager.SubmitMove(row, col));
            AudioManager.Instance.PlayTapClip();
        }
    }
}
