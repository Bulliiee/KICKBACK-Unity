using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LapController : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerScript script;
    [SerializeField] private LapTimeController timeController;

    [Header("CheckPoints")]
    public Vector3 respawnPointPosition; // 리스폰 위치
    public Quaternion respawnPointRotation; // 리스폰 Rotation
    public GameObject startPoint; // 시작 지점

    [Header("Laps")]
    public int currentIndex; // 인덱스 체킹
    public int checkPointsCnt; // 체크 포인트 개수
    public int currentLap; // 현재 바퀴수

    [Header("Results")]
    public TMP_Text currentLapTxt;
    public TMP_Text nickName;
    public GameObject Result;
    public bool isFinish;

    void Start()
    {
        respawnPointPosition = startPoint.transform.position;
        respawnPointRotation = startPoint.transform.rotation;

        currentLap = 1;

        // 'Respawn' 태그를 가진 모든 오브젝트 찾기
        GameObject[] respawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        // 체크포인트 개수 설정
        checkPointsCnt = respawnPoints.Length;

        isFinish = false;
        Result.SetActive(false);
    }

    // 체크포인트 정보를 갱신하는 메서드
    public void UpdateCheckPoint(int index, Vector3 checkpointPosition, Quaternion checkpointRotation)
    {
        respawnPointPosition = checkpointPosition;
        respawnPointRotation = checkpointRotation;
        currentIndex = index;
    }

    public void UpdateLap()
    {
        if (SceneManager.GetActiveScene().name == "Cebu Track")
        {
            if (currentIndex >= checkPointsCnt && currentLap <= 3)
            {
                currentLap++;

                if (currentLap == 1)
                {
                    currentLapTxt.text = currentLap.ToString();
                    currentLapTxt.color = Color.white;
                    currentIndex = -1;
                }
                else if (currentLap == 2)
                {
                    currentLapTxt.text = currentLap.ToString();
                    currentLapTxt.color = Color.yellow;
                    currentIndex = -1;
                }
                else if (currentLap == 3)
                {
                    currentLapTxt.text = currentLap.ToString();
                    currentLapTxt.color = Color.red;
                    currentIndex = -1;
                }
                else if (currentLap == 4)
                {
                    currentLapTxt.text = "3";
                    currentLapTxt.color = Color.red;
                }
            }
        }
        else if (SceneManager.GetActiveScene().name == "Mexico Track")
        {
            if (currentIndex >= checkPointsCnt && currentLap <= 2)
            {
                currentLap++;

                if (currentLap == 1)
                {
                    currentLapTxt.text = currentLap.ToString();
                    currentLapTxt.color = Color.white;
                    currentIndex = -1;
                }
                else if (currentLap == 2)
                {
                    currentLapTxt.text = currentLap.ToString();
                    currentLapTxt.color = Color.red;
                    currentIndex = -1;
                }
                else if (currentLap == 3)
                {
                    currentLapTxt.text = "2";
                    currentLapTxt.color = Color.red;
                }
            }
        }
        else if (SceneManager.GetActiveScene().name == "Downhill Track")
        {
            if (currentIndex >= checkPointsCnt && currentLap <= 1)
            {
                currentLap++;

                if (currentLap == 1)
                {
                    currentLapTxt.text = "1";
                    currentLapTxt.color = Color.red;
                }
            }
        }
    }

    public IEnumerator Finish()
    {
        yield return new WaitForSeconds(0.2f);

        if (SceneManager.GetActiveScene().name == "Cebu Track")
        {
            if (currentIndex >= checkPointsCnt && currentLap > 3)
            {
                // Player 태그를 가진 모든 게임 오브젝트를 탐색
                var players = GameObject.FindGameObjectsWithTag("Player");
                foreach (var player in players)
                {
                    if (player != this.gameObject)
                    {
                        // 다른 플레이어와의 충돌을 무시
                        Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>(), true);
                    }
                }

                isFinish = true;

                if (timeController.resultTimerText != null)
                {
                    timeController.resultTimerText.text = timeController.timerText.text; // 결과창 타이머에 현재 타이머 텍스트를 복사
                }

                // Result가 CanvasGroup 컴포넌트를 가지고 있다고 가정
                CanvasGroup resultCanvasGroup = Result.GetComponent<CanvasGroup>();
                if (resultCanvasGroup != null)
                {
                    float duration = 1.0f; // 페이드 인하는 데 걸리는 시간(초)
                    float elapsedTime = 0;

                    // CanvasGroup의 alpha 값을 0에서 1로 서서히 증가
                    while (elapsedTime < duration)
                    {
                        elapsedTime += Time.deltaTime;
                        resultCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
                        yield return null;
                    }
                    resultCanvasGroup.alpha = 1; // 마지막으로 alpha 값을 완전히 1로 설정하여 확실히 보이게 함
                }

                Result.SetActive(true);
            }
        }
        else if (SceneManager.GetActiveScene().name == "Mexico Track")
        {
            if (currentIndex >= checkPointsCnt && currentLap > 2)
            {
                // Player 태그를 가진 모든 게임 오브젝트를 탐색
                var players = GameObject.FindGameObjectsWithTag("Player");
                foreach (var player in players)
                {
                    if (player != this.gameObject)
                    {
                        // 다른 플레이어와의 충돌을 무시
                        Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>(), true);
                    }
                }

                isFinish = true;

                if (timeController.resultTimerText != null)
                {
                    timeController.resultTimerText.text = timeController.timerText.text; // 결과창 타이머에 현재 타이머 텍스트를 복사
                }

                // Result가 CanvasGroup 컴포넌트를 가지고 있다고 가정
                CanvasGroup resultCanvasGroup = Result.GetComponent<CanvasGroup>();
                if (resultCanvasGroup != null)
                {
                    float duration = 1.0f; // 페이드 인하는 데 걸리는 시간(초)
                    float elapsedTime = 0;

                    // CanvasGroup의 alpha 값을 0에서 1로 서서히 증가
                    while (elapsedTime < duration)
                    {
                        elapsedTime += Time.deltaTime;
                        resultCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
                        yield return null;
                    }
                    resultCanvasGroup.alpha = 1; // 마지막으로 alpha 값을 완전히 1로 설정하여 확실히 보이게 함
                }

                Result.SetActive(true);
            }
        }
        else if (SceneManager.GetActiveScene().name == "Downhill Track")
        {
            if (currentIndex >= checkPointsCnt && currentLap > 1)
            {
                // Player 태그를 가진 모든 게임 오브젝트를 탐색
                var players = GameObject.FindGameObjectsWithTag("Player");
                foreach (var player in players)
                {
                    if (player != this.gameObject)
                    {
                        // 다른 플레이어와의 충돌을 무시
                        Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>(), true);
                    }
                }

                isFinish = true;

                if (timeController.resultTimerText != null)
                {
                    timeController.resultTimerText.text = timeController.timerText.text; // 결과창 타이머에 현재 타이머 텍스트를 복사
                }

                // Result가 CanvasGroup 컴포넌트를 가지고 있다고 가정
                CanvasGroup resultCanvasGroup = Result.GetComponent<CanvasGroup>();
                if (resultCanvasGroup != null)
                {
                    float duration = 1.0f; // 페이드 인하는 데 걸리는 시간(초)
                    float elapsedTime = 0;

                    // CanvasGroup의 alpha 값을 0에서 1로 서서히 증가
                    while (elapsedTime < duration)
                    {
                        elapsedTime += Time.deltaTime;
                        resultCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
                        yield return null;
                    }
                    resultCanvasGroup.alpha = 1; // 마지막으로 alpha 값을 완전히 1로 설정하여 확실히 보이게 함
                }

                Result.SetActive(true);
            }
        }
        nickName.text = DataManager.Instance.loginUserInfo.dataBody.nickname;
    }
}