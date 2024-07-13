using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    enum state { NotSelected, Selected, DoubleJump };
    int currentState = 0;
    public GameObject space;
    private List<Space> spaces;
    Vector2 firstSelected, secondSelected;
    int existingChildren;
    bool isRedTurn = true;
    public TMP_Text textbox;
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
                GetSpace(x, y).transform.position = new Vector2((float) transform.localScale.x * newX, (float) (transform.localScale.y * (y - 3.5)));
                GetSpace(x, y).SetPosition(x, y);

                if (y < 3)
                {
                    GetSpace(x, y).ChangeState(1);
                }
                else if (y > 4)
                {
                    GetSpace(x, y).ChangeState(3);
                }
            }
        }
    }

    public void HandleSelect(int x2, int y2)
    {
        secondSelected = new Vector2(x2, y2);
        Space firstSpace = GetSpace(firstSelected);
        Space secondSpace = GetSpace(secondSelected);
        if ((isRedTurn && !firstSpace.IsRed())
        || (!isRedTurn && !firstSpace.IsBlack()))
        {
            Unselect();
            return;
        }

        if (CanCaptureHere(firstSelected, secondSelected) ||
            CanMoveHere(firstSelected, secondSelected))
        {
            if (isDoubleJump())
            {
                if (CanCaptureHere(firstSelected, secondSelected))
                {
                    Capture(firstSelected, secondSelected);
                }

                if (!CanDoubleJump(secondSelected))
                {
                    ToggleTurn();
                    Unselect();
                }
            }
            else if (CanCaptureHere(firstSelected, secondSelected))
            {

                Capture(firstSelected, secondSelected);
                if (CanDoubleJump(secondSelected))
                {
                    currentState = (int)state.DoubleJump;
                    firstSelected = secondSelected;
                    secondSelected = new Vector2(-1, -1);
                    firstSpace.Unselect();
                }
                else
                {
                    ToggleTurn();
                    Unselect();
                }
            }
            else
            { 
                    ChangeSpaces(firstSpace, secondSpace);
                    ToggleTurn();
                    Unselect();
            }
        }
    }

    bool CanDoubleJump(Vector2 pos)
    {
        int x1 = (int)pos.x - 1, x2 = (int)pos.x + 1;
        int y1 = (int)pos.y + 2, y2 = (int)pos.y - 2;
        return (CanCaptureHere(pos, new Vector2(x1, y1)) ||
                    CanCaptureHere(pos, new Vector2(x1, y2)) ||
                    CanCaptureHere(pos, new Vector2(x2, y1)) ||
                    CanCaptureHere(pos, new Vector2(x2, y2)));
    }
    bool CanCaptureHere(Vector2 start, Vector2 dest)
    {
        if (dest.x < 0 || dest.y < 0 || dest.x > 7 || dest.y > 7) //if destination is out of bounds somehow
            return false;

        Space destSpace = transform.GetChild(GetIndex((int)dest.x, (int)dest.y)).GetComponent<Space>();
        Space startSpace = transform.GetChild(GetIndex((int)start.x, (int)start.y)).GetComponent<Space>();
        Vector2 pieceCoord = FindMindPointBlock(start, dest);
        Space midSpace = transform.GetChild(GetIndex((int)pieceCoord.x, (int)pieceCoord.y)).GetComponent<Space>();

        if (Mathf.Abs(start.y - dest.y) != 2) //if destination is not two rows higher or two rows lower. 
            return false;
        if (Mathf.Abs(start.x - dest.x) != 1)//if destination is 
            return false;

        if (!destSpace.IsEmpty() || midSpace.IsEmpty()) //if destination is not empty or piece to be capture is empty. 
            return false;
        if (startSpace.IsRed() == midSpace.IsRed()) //if the piece to be captured is the same color. 
            return false;

        if (startSpace.IsRed())
        {
            return (startSpace.IsKing() || dest.y > start.y);
        }
        else if (startSpace.IsBlack())
        {
            return (startSpace.IsKing() || dest.y < start.y);
        }
        return false;
    }

    bool CanMoveHere(Vector2 start, Vector2 dest)
    {
        if (Mathf.Abs(dest.y - start.y) != 1)
            return false;

        if (start.y % 2 == 0)
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
        if (GetSpace(start).IsKing())
            return true; 
        if (GetSpace(start).IsRed() && dest.y <= start.y)
            return false;
        if (GetSpace(start).IsBlack() && dest.y >= start.y)
            return false;

        return true;
    }
    int GetIndex(int x, int y)
    {
        return (y * 4) + x + existingChildren;
    }
    void ToggleTurn()
    {
        isRedTurn = !isRedTurn;
        textbox.text = isRedTurn ? "Red's Turn" : "Black's Turn";

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

        return new Vector2((int)start.x + dx, start.y + dy);
    }
    Space GetSpace(Vector2 coords)
    {
        int x = (int)coords.x, y = (int)coords.y;
        return transform.GetChild(GetIndex(x, y)).GetComponent<Space>();
    }
    Space GetSpace(int x, int y)
    {
        return GetSpace(new Vector2(x, y));
    }
    void Capture(Vector2 start, Vector2 dest)
    {
        ChangeSpaces(GetSpace(start), GetSpace(dest));
        Vector2 midpoint = FindMindPointBlock(start, dest);
        GetSpace(midpoint).Empty();

    }
    void ChangeSpaces(Space first, Space second)
    {
        second.ChangeState(first.GetState());
        first.Empty();
        if ((second.IsRed() && second.GetY() == 7)
            || (second.IsBlack() && second.GetY() == 0))
            second.KingMe();
    }
    public void Select()
    {
        currentState = (int)state.Selected; 
    }
    public void SetFirstSelected(int x, int y)
    {
        firstSelected = new Vector2(x, y);
    }
    public void Unselect()
    {
        firstSelected = new Vector2(-1, -1);
        secondSelected = new Vector2(-1, -1);
        currentState = (int)state.NotSelected;
        for (int i = existingChildren; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Space>().Unselect();
        }
    }
    public bool isDoubleJump()
    {
        return currentState == (int)state.DoubleJump;
    }
    public bool IsSelected()
    {
        return currentState == (int)state.Selected;
    }
    public void EndTurn()
    {
        Unselect();
        ToggleTurn();

        if(GameOver())
        {
            if (Count(true) == 0)
                textbox.text = "Black Wins!";
            else
                textbox.text = "Red Wins!"; 
        }
    }
    public bool GameOver()
    {
        if (Count(true) == 0 || Count(false) == 0)
            return true;
        return false; 
    }
    public int Count(bool reds)
    {
        int count = 0; 
        for(int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (reds && GetSpace(x, y).IsRed())
                    count++;
                else if (!reds && GetSpace(x, y).IsBlack())
                    count++; 
            }
        }
        return count; 
    }
}