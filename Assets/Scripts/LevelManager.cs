using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int levelsCompleted = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        Debug.Log(levelsCompleted);
        if(Input.GetKeyDown(KeyCode.J))
        {
            levelsCompleted++;
            SceneManager.LoadScene(0);
        }
    }

    public static void LevelComplete(int lvlNum)
    {
        if(lvlNum > levelsCompleted)
        {
            levelsCompleted++;
        }
        
    }
}
