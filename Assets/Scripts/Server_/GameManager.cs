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
        if (SceneManager.GetActiveScene().name != "MainStage")
        {
            LoadingSceneManager.LoadScene("MainStage");
        }

        for (int i = 0; i < 3; i++)
        {
            if (canvasName.Equals(canvases[i]))
            {
                GameObject.Find(canvasName).SetActive(true);
            }
            else
            {
                GameObject.Find(canvasName).SetActive(false);
            }
        }
    }

    public void GoToIngame()
    {
        
    }

}
