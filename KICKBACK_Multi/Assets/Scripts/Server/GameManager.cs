using System.Collections;
using System.Collections.Generic;
using Modules;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    void Start()
    {
        LoadSceneAdditive();
    }

    // 매니저 생성 및 관리 위해 씬 중첩
    void LoadSceneAdditive()
    {
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
    }
}
