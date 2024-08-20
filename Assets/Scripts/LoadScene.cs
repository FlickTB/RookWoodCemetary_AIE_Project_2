using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void load() //Variable for the scene loading
    {
        SceneManager.LoadScene("Playground"); //Loads the main game scene
    }
}