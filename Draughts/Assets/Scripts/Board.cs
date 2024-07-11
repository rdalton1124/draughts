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

            if ((firstSpace.IsRed() && y2 == 7)
                || (firstSpace.IsBlack() && y2 == 0))
                firstSpace.KingMe();

            if (isDoubleJump())
            {
                if(CanCaptureHere(firstSelected, secondSelected))
                {
                    Capture(firstSelected, secondSelected); 
                }

                if(!CanDoubleJump(secondSelected))
                {
                    currentState = (int)state.NotSelected;
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
                    ChangeSpaces(firstSpace, secondSpace);
                    ToggleTurn();
                    Unselect();
                }
            }
        }
    }

    bool CanDoubleJump(Vector2 pos)
    {
        int x1 = (int) pos.x - 1, x2 = (int) pos.x + 1;
        int y1 = (int) pos.y + 2, y2 = (int) pos.y - 2;

        return (CanCaptureHere(pos, new Vector2(x1, y1)) ||
                    CanCaptureHere(pos, new Vector2(x1, y2)) ||
                    CanCaptureHere(pos, new Vector2(x2, y1)) ||
                    CanCaptureHere(pos, new Vector2(x2, y2))); 
    }
    bool CanCaptureHere(Vector2 start, Vector2 dest)
    {
        Space destSpace = transform.GetChild(GetIndex((int)dest.x, (int)dest.y)).GetComponent<Space>();
        Space startSpace = transform.GetChild(GetIndex((int)start.x, (int)start.y)).GetComponent<Space>();

        Vector2 pieceCoord = FindMindPointBlock(start, dest);
        Space midSpace = transform.GetChild(GetIndex((int)pieceCoord.x, (int)pieceCoord.y)).GetComponent<Space>();

        if (dest.x < 0 || dest.y < 0 || dest.x > 7 || dest.y > 7) //if destination is out of bounds somehow
            return false;
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
        else if(startSpace.IsBlack())
        {
            return (startSpace.IsKing() || dest.y < start.y); 
        }
        return false; 
    }

    bool CanMoveHere(Vector2 start, Vector2 dest)
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
        if (GetSpace(start).IsKing())
            return true;
        if (GetSpace(start).IsRed() && dest.y <= start.y)
            return false;
        if (GetSpace(start).IsBlack()&& dest.y >= start.y)
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
    }
    public void Select() 
    {
        currentState = 1; 
    }
    public void SetFirstSelected(int x, int y)
    {
        firstSelected = new Vector2(x, y); 
    }
    public void Unselect()
    {
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