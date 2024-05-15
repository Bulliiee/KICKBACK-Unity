using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineController : MonoBehaviour
{
    [SerializeField] private LapController controller;
    public AudioSource[] audioSources;
    public AudioClip[] audioClips;
    private void OnTriggerExit(Collider other)
    {
        controller.UpdateLap();
        StartCoroutine(controller.Finish());

        if (controller.currentLap == 2)
        {
            audioSources[1].clip = audioClips[1];
            audioSources[1].Play();
        }
        else if ( controller.currentLap == 3)
        {
            audioSources[2].clip = audioClips[2];
            audioSources[2].Play();
        }
        else if (controller.currentLap == 4)
        {
            audioSources[3].clip = audioClips[3];
            audioSources[3].Play();
        }
    }
}
