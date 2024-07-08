using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
    enum states {Empty, Red, RedKing, Black, BlackKing};
    bool isSelected = false;
    int currentState = 0;

    public Sprite[] sprites; 
    void Start()
    {
        
    }
    public void OnMouseDown()
    {
    }
    public void changeState(int state)
    {
        if (state >= 0 && state <= 4)
            currentState = state;

        this.GetComponent<SpriteRenderer>().sprite = sprites[state]; 
    }
}
