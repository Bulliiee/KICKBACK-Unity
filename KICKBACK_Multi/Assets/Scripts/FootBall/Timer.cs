using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private FootBallCameraFollowing cameraFollowing;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float remainingTime;
    public bool isFinish;

    void Update()
    {
        if (remainingTime > 0 && !cameraFollowing.isStarting)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime <= 0)
        {
            remainingTime = 0;
            timerText.color = Color.yellow;
            isFinish = true;
            StartCoroutine(scoreManager.Finish());
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
