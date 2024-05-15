using System.Collections;
using System.Collections.Generic;
using Modules;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    void Start()
    {
        // MasterScene에서 필요한 Manager 생성 후 MainStage 이동
        LoadingSceneManager.LoadScene("MainStage");
    }


}
