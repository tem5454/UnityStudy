using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShootingUI : UIBase
{
    public Text timerText;
    public Text scoreText;

    private void OnEnable()
    {
        timerText.text = string.Format("PlayTime : {0:0.00}", GameManager.Instance.PlayTime);

        int score = 0;
        scoreText.text = string.Format($"Score : {score}");
    }

    private void Update()
    {        
        timerText.text = string.Format("PlayTime : {0:0.00}", GameManager.Instance.PlayTime);
    }

    public void RefreshScore()
    {
        int score = GameManager.Instance.PlayScore;
        scoreText.text = string.Format($"Score : {score}");
    }
}
