using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private TMP_Text ResultScore;
    public int Score = 0;

    [Header("Result")]
    [SerializeField] private Timer timer;
    public GameObject Result;

    public IEnumerator Finish()
    {
     
        yield return new WaitForSeconds(0.2f);

        if (timer.isFinish)
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

            if (ResultScore != null)
            {
                ResultScore.text = Score.ToString(); // 결과창 타이머에 현재 타이머 텍스트를 복사
            }

            CanvasGroup resultCanvasGroup = Result.GetComponent<CanvasGroup>();
            if (resultCanvasGroup != null)
            {
                float duration = 1.0f; // 페이드 인하는 데 걸리는 시간(초)
                float elapsedTime = 0;

                // CanvasGroup의 alpha 값을 0에서 1로 서서히 증가
                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    resultCanvasGroup.alpha = Mathf.Lerp(resultCanvasGroup.alpha, 1, Time.deltaTime);
                    yield return null;
                }
                resultCanvasGroup.alpha = 1; // 마지막으로 alpha 값을 완전히 1로 설정하여 확실히 보이게 함
            }
        }
    }

}
