using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] TextMeshProUGUI scoreText;
    public float killPoints;
    public float finalscore;
    public float timeLeft;
    public float timeMultiplier;
    public bool levelComplete;
    public bool reachedEnd;

    void OnTriggerEnter(Collider other)
    {
        levelComplete = true;
        Score();
        reachedEnd = true;
    }

    void Update()
    {
        if ((timeLeft > 0) && (levelComplete == false))
        {
            timeLeft -= Time.deltaTime;
            Timer(timeLeft);
            Score();
        }
        else if (timeLeft <= 0)
        {
            timeLeft = 0;
            finalscore = 0;
        }
    }

    void Start()
    {
        levelComplete = false;
        reachedEnd = false;
    }

    void Timer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        countdownText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    void Score()
    {
        if (levelComplete == false)
        {
            scoreText.text = string.Format("Score: {0000:0}", finalscore);
        }
        else if (levelComplete == true && reachedEnd == false)
        {
            finalscore += (timeLeft * timeMultiplier);
            scoreText.text = string.Format("Score: {0000:0}", finalscore);
        }
    }

    //void End()
    //{
    //    reachedEnd = true;
    //}
}