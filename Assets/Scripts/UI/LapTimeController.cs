using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UnityEngine.UI 네임스페이스 추가
using TMPro; // TMPro 네임스페이스 추가

public class LapTimeController : MonoBehaviour
{
    [SerializeField] private LapController controller;
    public TextMeshProUGUI timerText; // 타이머 텍스트
    public TextMeshProUGUI resultTimerText; // 결과창의 타이머 텍스트 참조를 위한 필드
    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (!controller.isFinish) // 경기가 끝나지 않았을 때만 타이머 업데이트
        {
            float time = Time.time - startTime;

            int minutes = GetMinute(time);
            int seconds = GetSecond(time);
            string milliseconds = GetMilliseconds(time);

            timerText.text = string.Format("{0:00}:{1:00}:{2}", minutes, seconds, milliseconds);
        }
    }

    public int GetMinute(float time)
    {
        return (int)((time / 60) % 60);
    }

    public int GetSecond(float time)
    {
        return (int)(time % 60);
    }

    public string GetMilliseconds(float time)
    {
        string milliseconds = string.Format("{0:00}", (time % 1) * 100);
        return milliseconds;
    }
}
