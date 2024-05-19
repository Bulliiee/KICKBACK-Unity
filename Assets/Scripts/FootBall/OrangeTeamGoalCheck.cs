using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrangeTeamGoalCheck : MonoBehaviour
{
    [Header("Script")]
    [SerializeField] private FootBallCountDownController countDownController;
    [SerializeField] private FootBallCameraFollowing cameraFollowing;

    [Header("Trigger")]
    public TextMeshProUGUI OrangeScore;
    private int score;
    public AudioSource audioSource;
    public AudioSource whistleAudio;
    public ParticleSystem[] particleSystems; // 파티클 시스템 배열 추가

    [Header("Reset")]
    [SerializeField] private Transform[] orangeTeamPosition;
    [SerializeField] private Transform[] blueTeamPosition;
    public GameObject ball;
    public GameObject[] players;
    public Image CountDownImage;

    void Start()
    {
        score = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            score++;
            OrangeScore.text = score.ToString();
            audioSource.Play();
            PlayAllParticles(); // 파티클 재생 함수 호출

            Invoke("ResetPositions", 2.0f);
        }
    }

    private void PlayAllParticles()
    {
        foreach (var ps in particleSystems)
        {
            ps.Play();
        }
    }

    private void StopAllParticles()
    {
        foreach (var ps in particleSystems)
        {
            ps.Stop();
        }
    }

    private void ResetPositions()
    {
        whistleAudio.Play();

        cameraFollowing.isStarting = true;
        CountDownImage.gameObject.SetActive(true);

        // 공 위치 복원
        ball.transform.position = Vector3.zero;
        ball.transform.rotation = Quaternion.identity;

        // 공의 Rigidbody 컴포넌트 찾기
        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
        if (ballRigidbody != null)
        {
            // 공의 속도와 각속도 초기화
            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;
        }

        // 플레이어 위치 및 회전 복원
        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = orangeTeamPosition[i].position;
            players[i].transform.rotation = Quaternion.identity; // 플레이어의 회전을 0으로 설정
        }

        StopAllParticles();

        StartCoroutine(countDownController.StartGame());
    }
}
