using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootBallBeforeStarting : MonoBehaviour
{
    [SerializeField] private FootBallCameraFollowing cameraFollowing;
    [SerializeField] private Timer timer;

    [SerializeField] private GameObject inGameUI;
    [SerializeField] private CanvasGroup scoreCanvasGroup;

    void Start()
    {
        inGameUI.SetActive(false);
        scoreCanvasGroup.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraFollowing.isStarting || timer.isFinish)
        {
            inGameUI.SetActive(false);
            scoreCanvasGroup.alpha = 0f;
        }
        else
        {
            inGameUI.SetActive(true);
            scoreCanvasGroup.alpha = 1f;
        }
    }
}
