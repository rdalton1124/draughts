using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
    public enum states { Empty, Red, RedKing, Black, BlackKing };
    bool isSelected = false;
    int currentState = 0;
    GameObject circle;
    public Sprite[] sprites;
    Board board;
    int row, col;
    void Start()
    {
        board = transform.parent.gameObject.GetComponent<Board>();
        circle = transform.GetChild(0).gameObject;
        circle.GetComponent<Renderer>().enabled = false;
    }

    public void KingMe()
    {
        if (currentState == (int)states.Red || currentState == (int)states.Black)
            ChangeState(currentState + 1);
    }
    public void SetPosition(int x, int y)
    {
        row = x;
        col = y;
    }
    public void OnMouseDown()
    {
        if (board.IsDoubleJump()) 
        {
            Debug.Log(board.IsDoubleJump() + "\n");
            if (isSelected)
            {
                ToggleSelected();
                board.EndTurn();
            }
        }
        else if (!IsEmpty()) //space is not empty
        {
            if (!board.IsSelected() || isSelected)
                ToggleSelected();
        }
        else
        {
            if (board.IsSelected())
            {
                board.HandleSelect(row, col);
            }
        }
    }
    public void ToggleSelected()
    {
        isSelected = !isSelected;
        circle.GetComponent<Renderer>().enabled = isSelected;
        if (isSelected)
        {
            board.Select();
            board.SetFirstSelected(this.row, this.col);
        }
        else
        {
            board.Unselect();
        }
     }
    public void Unselect()
    {
        if (isSelected)
            ToggleSelected();
    }
    public void ChangeState(int state)
    {
        if (state >= 0 && state <= 4)
            currentState = state;

        this.GetComponent<SpriteRenderer>().sprite = sprites[state];
    }
    public int GetState()
    {
        return currentState;
    }
    public bool IsEmpty()
    {
        return currentState == (int)states.Empty;
    }
    public bool IsRed()
    {
        return currentState == (int)states.Red || currentState == (int)states.RedKing;
    }
    public bool IsBlack()
    {
        return currentState == (int)states.Black || currentState == (int)states.BlackKing; 
    }
    public bool IsKing()
    {
        return currentState == (int)states.RedKing || currentState == (int)states.BlackKing; 
    }
    public void Empty()
    {
        ChangeState(0); 
    }
    public int GetX()
    {
        return row; 
    }
    public int GetY()
    {
        return col; 
    }

}
