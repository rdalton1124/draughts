using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject space; 
    private List<GameObject> spaces; 
    void Start()
    {
        spaces = new List<GameObject>();
        for(int y = 0; y < 8; y++)
        {
            for(int x = 0; x < 4; x++)
            {
                Instantiate(space, transform);
                float newX = (float) (y % 2 == 0 ? (2 * x) - 3.5 : (2 * x) - 2.5);
                transform.GetChild((y * 4) + x + 1).transform.position = new Vector2(newX, (float)(y - 3.5)); 
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
