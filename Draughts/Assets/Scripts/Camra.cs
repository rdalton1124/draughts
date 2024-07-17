using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camra : MonoBehaviour
{
    void Start()
    {
        //thanks to thealexyguy1 and his 2016 post to the Unity forums for this solution.
        Screen.SetResolution(Screen.width, Screen.height, true);
    }

}
