using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene1 : MonoBehaviour
{
    public void load1()                                                                                                      //Function for the scene loading
    {
        SceneManager.LoadScene("Kai Scene");                                                                                //Loads the main game scene
    }
}