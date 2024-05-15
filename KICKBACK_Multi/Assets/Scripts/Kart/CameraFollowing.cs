using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollowing : MonoBehaviour
{
    [SerializeField] private LapController controller;
    [SerializeField] private CountDownController countDownController;

    public Vector3 offset;

    [SerializeField] private Vector3 introCamOffset1; // 인트로 씬을 위한 첫번째 카메라 offset
    [SerializeField] private Vector3 introCamOffset2; // 인트로 씬을 위한 두번째 카메라 offset

    [SerializeField] private Vector3 finishCamOffset; // Inspector에서 설정 가능한 최종 카메라 offset

    public Transform player;

    public Vector3 originCamPos;

    public bool isStarting = false; // 인트로 씬 시작 여부

    void Awake()
    {
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        if (SceneManager.GetActiveScene().name == "Downhill Track")
        {
            isStarting = true; // 인트로 씬 시작

            // 카메라를 introCamOffset1 위치로 즉시 이동
            transform.position = player.position + introCamOffset1;
            // 카메라의 회전을 Y축 기준 180도로 설정
            transform.rotation = Quaternion.Euler(0, 180, 0);

            // introCamOffset1에서 introCamOffset2로 서서히 이동
            float startTime = Time.time;
            float journeyLength = Vector3.Distance(introCamOffset1, introCamOffset2);
            float journeyTime = 5.0f; // 이동에 걸리는 시간
            float fractionOfJourney = 0;

            while (fractionOfJourney < 1)
            {
                float distCovered = (Time.time - startTime) * journeyLength / journeyTime;
                fractionOfJourney = distCovered / journeyLength;
                transform.position = player.position + Vector3.Lerp(introCamOffset1, introCamOffset2, fractionOfJourney);
                yield return null;
            }

            // introCamOffset2에서 오리진 캠 위치로 이동하기 전에 3초간 대기
            yield return new WaitForSeconds(2.0f);

            // introCamOffset2에서 originCamPos로 즉시 이동
            transform.position = player.position + originCamPos;

            // 카메라의 회전을 다시 0도로 설정
            transform.rotation = Quaternion.Euler(0, 180, 0);

            countDownController.gameObject.SetActive(true);
            StartCoroutine(countDownController.StartGame());
        }
        else
        {
            isStarting = true; // 인트로 씬 시작

            // 카메라를 introCamOffset1 위치로 즉시 이동
            transform.position = player.position + introCamOffset1;
            // 카메라의 회전을 Y축 기준 180도로 설정
            transform.rotation = Quaternion.Euler(0, 180, 0);

            // introCamOffset1에서 introCamOffset2로 서서히 이동
            float startTime = Time.time;
            float journeyLength = Vector3.Distance(introCamOffset1, introCamOffset2);
            float journeyTime = 5.0f; // 이동에 걸리는 시간
            float fractionOfJourney = 0;

            while (fractionOfJourney < 1)
            {
                float distCovered = (Time.time - startTime) * journeyLength / journeyTime;
                fractionOfJourney = distCovered / journeyLength;
                transform.position = player.position + Vector3.Lerp(introCamOffset1, introCamOffset2, fractionOfJourney);
                yield return null;
            }

            // introCamOffset2에서 오리진 캠 위치로 이동하기 전에 3초간 대기
            yield return new WaitForSeconds(2.0f);

            // introCamOffset2에서 originCamPos로 즉시 이동
            transform.position = player.position + originCamPos;

            // 카메라의 회전을 다시 0도로 설정
            transform.rotation = Quaternion.Euler(0, 0, 0);

            countDownController.gameObject.SetActive(true);
            StartCoroutine(countDownController.StartGame());
        }

    }


    void LateUpdate()
    {
        if (!controller.isFinish && countDownController.gameObject.activeSelf)
        {
            transform.position = player.position + offset;
            transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, originCamPos, 3 * Time.deltaTime);
        }
        else if (controller.isFinish)
        {
            // 경기가 끝났을 때, offset을 finishCamOffset으로 서서히 변경
            offset = Vector3.Lerp(offset, finishCamOffset, Time.deltaTime);

            // 카메라 위치를 변경된 offset을 사용하여 업데이트
            transform.position = player.position + offset;

            // 카메라의 Y축을 기준으로 180도 회전을 서서히 적용
            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0, 180, 0); // Y축을 기준으로 180도 회전

            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime);
        }
    }
}
