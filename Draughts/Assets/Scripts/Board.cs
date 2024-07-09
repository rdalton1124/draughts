using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    enum state {NotSelected, Selected, DoubleJump};
    int currentState = 0; 
    public GameObject space;
    private List<Space> spaces;
    private bool isSelected = false; //is at least one space selected.
    Vector2 firstSelected, secondSelected;
    int existingChildren;
    bool isRedTurn = true;
    public GameObject text; 
    void Start()
    {
        existingChildren = transform.childCount; //number of children other than the spaces. 
        spaces = new List<Space>();

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                Instantiate(space, transform);
                float newX = (float)(y % 2 == 0 ? (2 * x) - 3.5 : (2 * x) - 2.5);
                transform.GetChild((y * 4) + x + existingChildren).transform.position = new Vector2(newX, (float)(y - 3.5));
                transform.GetChild((y * 4) + x + existingChildren).GetComponent<Space>().SetPosition(x, y);
                if (y < 3)
                {
                    transform.GetChild((y * 4) + x + existingChildren).GetComponent<Space>().ChangeState(1);

                }
                else if (y > 4)
                {
                    transform.GetChild((y * 4) + x + existingChildren).GetComponent<Space>().ChangeState(3);
                }
            }
        }
    }
    int getIndex(int x, int y)
    {
        return (y * 4) + x + existingChildren;
    }
    void ToggleTurn()
    {
        isRedTurn = !isRedTurn;

    }
    public void HandleSelect(int x2, int y2)
    {
        secondSelected = new Vector2(x2, y2);
        Space firstSpace = transform.GetChild(getIndex((int)firstSelected.x, (int)firstSelected.y)).GetComponent<Space>();
        Space secondSpace = transform.GetChild(getIndex(x2, y2)).GetComponent<Space>();

        if ((isRedTurn && !(firstSpace.GetState() == 1 || firstSpace.GetState() == 2))
        || (!isRedTurn && !(firstSpace.GetState() == 3 || firstSpace.GetState() == 4)))
        {
            Unselect();
            return; 
        }
   
        if (CanCaptureHere(firstSelected, secondSelected, firstSpace.GetState()) ||
            CanMoveHere(firstSelected, secondSelected, firstSpace.GetState()))
        {
            if ((firstSpace.GetState() == 1 && y2 == 7)
                || (firstSpace.GetState() == 3 && y2 == 0))
                firstSpace.KingMe();
            if(isDoubleJump())
            {
                if(CanCaptureHere(firstSelected, secondSelected, firstSpace.GetState()))
                {
                    secondSpace.ChangeState(firstSpace.GetState());
                    firstSpace.ChangeState(0); 
                }
                if(!CanDoubleJump(secondSelected, secondSpace.GetState()))
                {
                    ToggleTurn();
                    Unselect(); 
                }
            }
            else if (CanCaptureHere(firstSelected, secondSelected, firstSpace.GetState()))
            {
                Vector2 midpoint = FindMindPointBlock(firstSelected, secondSelected);
                transform.GetChild(getIndex((int)midpoint.x, (int)midpoint.y)).GetComponent<Space>().ChangeState(0);

                secondSpace.ChangeState(firstSpace.GetState());
                firstSpace.ChangeState(0);
                if (CanDoubleJump(secondSelected, secondSpace.GetState()))
                {
                    currentState = (int)state.DoubleJump;
                    firstSelected = secondSelected; 
                    secondSelected = new Vector2(-1, -1);

                }
                else
                {
                    ToggleTurn();
                    Unselect();

                }
             }
            else
            {
                if (!isDoubleJump())
                {
                    secondSpace.ChangeState(firstSpace.GetState());
                    firstSpace.ChangeState(0);
                }
                ToggleTurn();
                Unselect();
            }
        }
    }
    bool CanDoubleJump(Vector2 pos, int state)
    {
        int x1 = (int) pos.x - 1, x2 = (int) pos.x + 1;
        int y1 = (int) pos.y + 2, y2 = (int) pos.y - 2;

        return (CanCaptureHere(pos, new Vector2(x1, y1), state) ||
                    CanCaptureHere(pos, new Vector2(x1, y2), state) ||
                    CanCaptureHere(pos, new Vector2(x2, y1), state) ||
                    CanCaptureHere(pos, new Vector2(x2, y2), state)); 
    }
    bool CanCaptureHere(Vector2 start, Vector2 dest, int state)
    {
        if (dest.x < 0 || dest.y < 0 || dest.x > 7 || dest.y > 7)
            return false;
        if (Mathf.Abs(start.y - dest.y) != 2)
            return false;
        if (Mathf.Abs(start.x - dest.x) != 1)
            return false;

        Vector2 pieceCoord = FindMindPointBlock(start, dest); 
        int pieceState = transform.GetChild(getIndex((int) pieceCoord.x, (int) pieceCoord.y)).GetComponent<Space>().GetState();
        if (state == 1 || state == 2)
        {
            if (state == 1 && dest.y < start.y)
                return false;

            if (pieceState  == 3 || pieceState == 4)
            {
                return true;
            }
        }
        else if(state == 3 || state == 4)
        {
            if (state == 3 && dest.y > start.y)
                return false; 
            if (pieceState == 1 || pieceState == 2)
            {
                return true;
            }
        }
        return false; 
    }
    Vector2 FindMindPointBlock(Vector2 start, Vector2 dest)
    {
        int dx = 0; 
        if (start.y % 2 == 0)
        {
            if (dest.x == start.x - 1)
                dx = -1;
        }
        else
        {
            if (dest.x == start.x + 1)
                dx = 1;
        }
        int dy = (dest.y > start.y) ? 1 : -1;

        return new Vector2((int) start.x + dx, start.y + dy); 
    }
    bool CanMoveHere(Vector2 start, Vector2 dest, int state)
    {


        if (Mathf.Abs(dest.y - start.y) != 1)
            return false;

        if(start.y % 2 == 0)
        {
            if (start.x == 0 && dest.x != 0)
                return false;
            if (start.x != dest.x && start.x - 1 != dest.x)
                return false; 
        }
        else
        {
            if (start.x == 3 && dest.x != 3)
                return false;
            if (start.x != dest.x && start.x + 1 != dest.x)
                return false; 
        }
        if (state == 2 || state == 4)
            return true;
        if (state == 1 && dest.y <= start.y)
            return false;
        if (state == 3 && dest.y >= start.y)
            return false;
        return true;
    }
    public void Select() 
    {
        isSelected = true;
        currentState = 1; 
    }
    public void SetFirstSelected(int x, int y)
    {
        firstSelected = new Vector2(x, y); 
    }
    public void Unselect()
    {
        isSelected = false;
        firstSelected = new Vector2(-1, -1);
        secondSelected = new Vector2(-1, -1);
        currentState = 0; 
        for (int i = existingChildren; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Space>().Unselect();
        }
    }
    public bool isDoubleJump()
    {
        return currentState == (int) state.DoubleJump; 
    }
    public bool IsSelected()
    {
        return currentState == (int) state.Selected; 
    }
}