using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject space;
    private List<Space> spaces;
    void Start()
    {
        int existingChildren = transform.childCount;
        spaces = new List<Space>();
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                Instantiate(space, transform);
                float newX = (float)(y % 2 == 0 ? (2 * x) - 3.5 : (2 * x) - 2.5);
                transform.GetChild((y * 4) + x + existingChildren).transform.position = new Vector2(newX, (float)(y - 3.5));
                if (y < 2)
                {
                    transform.GetChild((y * 4) + x + existingChildren).GetComponent<Space>().changeState(1);
                }
                else if (y > 5)
                {
                    transform.GetChild((y * 4) + x + existingChildren).GetComponent<Space>().changeState(3);
                }
            }
        }
    }
}