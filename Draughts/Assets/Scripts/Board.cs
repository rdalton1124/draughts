using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject space;
    private List<Space> spaces;
    private bool isSelected = false; //is at least one space selected.
    Vector2 firstSelected, secondSelected;
    int existingChildren;
    bool isRedTurn = true; 
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

        if (isRedTurn && !(firstSpace.GetState() == 1 || firstSpace.GetState() == 2))
        {
            Unselect();
            return; 
        }
        if(!isRedTurn && !(firstSpace.GetState() == 3 || firstSpace.GetState() == 4))
        {
            Unselect();
            return; 
        }
        if(CanCaptureHere(firstSelected, secondSelected, firstSpace.GetState()) ||
            CanMoveHere(firstSelected, secondSelected, firstSpace.GetState()))
        {
            if ((firstSpace.GetState() == 1 && y2 == 7)
                || (firstSpace.GetState() == 3 && y2 == 0))
                firstSpace.KingMe();
            secondSpace.ChangeState(firstSpace.GetState()); 
            firstSpace.ChangeState(0);
            ToggleTurn(); 
        }
        Unselect(); 
    }
    bool CanCaptureHere(Vector2 start, Vector2 dest, int state)
    {
        if (Mathf.Abs(start.y - dest.y) != 2)
            return false;
        if (Mathf.Abs(start.x - dest.x) != 1)
            return false;
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

        Space piece = transform.GetChild(getIndex((int)start.x + dx, (int)start.y + dy)).GetComponent<Space>();

        if (state == 1 || state == 2)
        {
            if (state == 1 && dy == -1)
                return false;

            if (piece.GetState() == 3 || piece.GetState() == 4)
            {
                piece.ChangeState(0); 
                return true;
            }
        }
        else if(state == 3 || state == 4)
        {
            if (state == 3 && dy == 1)
                return false; 
            if (piece.GetState() == 1 || piece.GetState() == 2)
            {
                piece.ChangeState(0);
                return true;
            }
        }
        return false; 
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
        for (int i = existingChildren; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Space>().Unselect();
        }
    }
    public bool IsSelected()
    {
        return isSelected; 
    }
}