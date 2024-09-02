using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void load()                                                                                                      //Function for the scene loading
    {
        SceneManager.LoadScene("Kai Scene");                                                                               //Loads the main game scene
    }
}