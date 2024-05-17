using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FootBallCountDownController : MonoBehaviour
{
    [SerializeField] FootBallCameraFollowing CameraFollowing;

    public Sprite[] sprites;
    public GameObject countDown;
    public Image countdownImage;

    public AudioClip[] audioClips;
    public AudioSource audioSources;

    public bool isCountDown;

    public IEnumerator StartGame()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            countdownImage.sprite = sprites[i];
            // 페이드인 시작
            PlayCountdownSound(i); // 인덱스에 해당하는 소리 재생

            // 마지막 스프라이트를 제외하고 0.5초 기다림 (페이드아웃 동안)
            if (i < sprites.Length - 1)
            {
                StartCoroutine(FadeImage(true, 1f));
                // 페이드아웃 시작
                StartCoroutine(FadeImage(false, 1f));
                yield return new WaitForSecondsRealtime(1);
            }
            else
            {
                StartCoroutine(FadeImage(true, 0.3f));

                // 마지막 스프라이트에서는 페이드아웃 없이 바로 전환
                yield return new WaitForSecondsRealtime(0.3f); // 페이드인과 함께 0.5초 기다림
            }
        }

        FinishCountdown(); // 게임 시작 처리
    }


    private IEnumerator FadeImage(bool fadeIn, float duration)
    {
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < startTime + duration)
        {
            float t = (Time.realtimeSinceStartup - startTime) / duration;
            Color color = countdownImage.color;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            countdownImage.color = color;
            yield return null;
        }

        // 최종 알파값 설정
        Color finalColor = countdownImage.color;
        finalColor.a = endAlpha;
        countdownImage.color = finalColor;
    }

    public void PlayCountdownSound(int index)
    {
        if (index < audioClips.Length)
        {
            audioSources.clip = audioClips[index];
            audioSources.Play();
        }
    }

    private void FinishCountdown()
    {
        countdownImage.gameObject.SetActive(false);
        isCountDown = false;
        CameraFollowing.isStarting = false; // 인트로씬 종료
    }
}
