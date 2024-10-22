using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    void Update() // Update is called once per frame
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //Looks for the pressing of the escape key
        {
           Application.Quit(); //Quits the application
        }
    }

    public void Leave() //Varible foring leave (used with a mouse click)
    {
        Application.Quit(); //Quits the application
    }    
}