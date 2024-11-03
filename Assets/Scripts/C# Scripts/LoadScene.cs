using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void load1()                                                                                                     //Function for loading Level 1
    {
        SceneManager.LoadScene("Kai Scene");                                                                                //Loads Level 1
    }

    public void load2()                                                                                                     //Function for loading Level 2
    {
        SceneManager.LoadScene("Heloise Scene");                                                                            //Loads Level 2
    }
}