using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeStarting : MonoBehaviour
{
    [SerializeField] private CameraFollowing cameraFollowing;
    [SerializeField] private GameObject inGameUI;

    void Start()
    {
        inGameUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraFollowing.isStarting)
        {
            inGameUI.SetActive(false);
        }
        else
        {
            inGameUI.SetActive(true);
        }
    }
}
