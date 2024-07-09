using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
    public enum states {Empty, Red, RedKing, Black, BlackKing};
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
        if (currentState == 1 || currentState == 3)
            ChangeState(currentState + 1); 
    }
    public void SetPosition(int x, int y)
    {
        row = x;
        col = y; 
    }
    public void OnMouseDown()
    {
        if (currentState != 0) //space is not empty
        {
            if(isSelected || !board.IsSelected())
                ToggleSelected();
        }
        else
        {
            if(board.IsSelected())
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
            board.Unselect();
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
}
