using System.Collections;
using System.Collections.Generic;
using Modules;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public User user;
    
    string[] canvases = {
        "Login Canvas",
        "Lobby Canvas",
        "Channel Canvas"
    };
    
    private void Start()
    {
        // MasterScene에서 필요한 Manager 생성 후 MainStage의 로그인으로 이동
        ChangeMainStageCanvas("Login Canvas");
    }

    public void ChangeMainStageCanvas(string canvasName)
    {
        StartCoroutine(ChangeMainStageCanvasCoroutine(canvasName));
    }

    private IEnumerator ChangeMainStageCanvasCoroutine(string canvasName)
    {
        // 현재 씬이 MainStage가 아니라면 씬을 로드합니다.
        if (SceneManager.GetActiveScene().name != "MainStage")
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainStage");

            // 씬 로드가 완료될 때까지 대기합니다.
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        // 씬 로드가 완료된 후, 캔버스 상태를 변경합니다.
        for (int i = 0; i < 3; i++)
        {
            GameObject canvas = GameObject.Find(canvases[i]);
            if (canvas != null)
            {
                canvas.SetActive(canvasName.Equals(canvases[i]));
            }
        }
    }

    public void GoToIngame()
    {
        
    }

}
