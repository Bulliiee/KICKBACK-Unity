using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointsController : MonoBehaviour
{
    [SerializeField] private LapController lapController;
    [SerializeField] private int index;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // LapController에 현재 체크포인트 정보를 갱신하라고 알립니다.
            lapController.UpdateCheckPoint(index, other.transform.position, other.transform.rotation);
        }
    }
}