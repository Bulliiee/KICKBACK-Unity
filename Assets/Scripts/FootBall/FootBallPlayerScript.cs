using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FootBallPlayerScript : MonoBehaviour
{
    public Rigidbody rb;
    public Animator animator;

    [Header("Scripts")]
    [SerializeField] private FootBallCameraFollowing cameraFollowing;
    [SerializeField] private Timer timer;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 15f; // 최대 속도
    [SerializeField] private float boostSpeed = 40f;
    [SerializeField] private float forceMultiplier = 10000f; // 힘의 배수
    [SerializeField] private float decelerationForce = 5f; // 감속력
    [SerializeField] private float reverseForceMultiplier = 0.1f; // 후진 힘 배수
    public LayerMask roadLayerMask; // Road LayerMask 설정

    [Header("Jump")]
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float fallMultiplier = 10f; // 점프 후 떨어지는 중력 가속도 계수
    private bool isJumping = false;
    private int jumpCount = 0; // 점프 횟수 관리 변수
    private int maxJump = 1; // 최대 점프 횟수

    [Header("Steering & Drift")]
    [SerializeField] private float rotateSpeed = 40f;
    [SerializeField] private float driftPower = 1000f;
    private float originalRotateSpeed;
    private float currentRotationSpeed;
    public float tiltAngle = 10f;

    [Header("Booster")]
    public Slider driftSlider; // 드리프트 게이지 슬라이더
    [SerializeField] private float driftFillRate = 0.2f; // 게이지 충전률
    private float driftDecreaseRate = 1.5f;
    public int maxBoost = 2; // 최대 부스터 개수
    public int currentBoost = 0; // 현재 부스터 개수
    public float boostDuration = 5f; // 부스터 활성화 유지 시간
    private bool isBoosting = false;

    [Header("Effects")]
    public ParticleSystem speedLineParticleSystem;
    public ParticleSystem LeftboostFire;
    public ParticleSystem RightboostFire;
    public TrailRenderer LeftSkid;
    public TrailRenderer RightSkid;

    [Header("Audio")]
    public AudioSource movingAudioSource; // 움직임 소리용 AudioSource
    public AudioSource boostingAudioSource; // 부스터 소리용 AudioSource
    public AudioSource boostWindAudioSource; // 부스터 사용 시 바람 소리 AudioSource
    public AudioSource driftAudioSource; // 드리프트 소리용 AudioSource
    public AudioClip[] audioClips;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Rigidbody의 중력 설정
        rb.useGravity = true; // Rigidbody의 기본 중력 사용

        originalRotateSpeed = rotateSpeed;
    }

    private void FixedUpdate()
    {
        if (!cameraFollowing.isStarting && !timer.isFinish)
        {
            Move();
            Steer();
            Jump();
            Boosts();

            // LeftShift 키 입력 감지하여 Drift 동작 실행
            if (Input.GetKey(KeyCode.LeftShift) && IsGrounded())
            {
                if (!driftAudioSource.isPlaying) // 오디오가 현재 재생 중이지 않을 때만
                {
                    driftAudioSource.clip = audioClips[3]; // 배열 1번 클립 (부스터 소리)
                    driftAudioSource.Play(); // 오디오 클립 재생 시작
                }

                Drift();
                LeftSkid.emitting = true;
                RightSkid.emitting = true;
            }
            else
            {
                if (driftAudioSource.isPlaying)
                {
                    driftAudioSource.Stop();
                }

                LeftSkid.emitting = false;
                RightSkid.emitting = false;
            }
        }
        else if (timer.isFinish)
        {
            animator.SetBool("Cute", true);
        }
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        bool isOnRoad = Physics.Raycast(transform.position, -transform.up, 1f, roadLayerMask);

        float speedMultiplier = isOnRoad ? 1.0f : 0.7f; // Road일 경우 1.0, NotRoad일 경우 0.6 배속

        if (Mathf.Abs(verticalInput) > 0.01f)
        {
            if (!movingAudioSource.isPlaying)
            {
                movingAudioSource.clip = audioClips[0];
                movingAudioSource.Play();
            }
            movingAudioSource.pitch = isBoosting ? 1.3f : 1.0f;
        }
        else if (movingAudioSource.isPlaying)
        {
            movingAudioSource.Stop();
        }

        Vector3 forceDirection = transform.forward * verticalInput * speedMultiplier;
        float forceMultiplierAdjusted = verticalInput < 0 ? forceMultiplier * reverseForceMultiplier : forceMultiplier;
        Vector3 force = forceDirection * forceMultiplierAdjusted * speedMultiplier;

        if (!isBoosting)
        {
            if (rb.velocity.magnitude > maxSpeed * speedMultiplier)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed * speedMultiplier;
            }

            if (Mathf.Approximately(verticalInput, 0) && rb.velocity.magnitude > 0.1f)
            {
                rb.AddForce(-rb.velocity.normalized * decelerationForce, ForceMode.Force);
            }
            else
            {
                rb.AddForce(force);
            }

            if (verticalInput < 0 && rb.velocity.magnitude > (maxSpeed * reverseForceMultiplier * speedMultiplier))
            {
                rb.velocity = rb.velocity.normalized * (maxSpeed * reverseForceMultiplier * speedMultiplier);
            }
        }
        else
        {
            // 부스트 활성화 시
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit, 1.5f, roadLayerMask))
            {
                // 지면과의 충돌을 감지하고, 지면의 노멀 벡터를 사용하여 이동 방향을 조정
                Vector3 groundNormal = hit.normal;
                Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward * boostSpeed * speedMultiplier, groundNormal).normalized;

                // 지면에 평행하게 부스트 적용
                Vector3 boostForce = projectedForward * boostSpeed * speedMultiplier - rb.velocity;
                rb.AddForce(boostForce, ForceMode.VelocityChange);
            }
            else
            {
                // 공중에 떠있을 때(즉, 지면과 충돌하지 않을 때) 기존의 로직 적용
                Vector3 boostForce = transform.forward * boostSpeed * speedMultiplier - rb.velocity;
                rb.AddForce(boostForce, ForceMode.VelocityChange);
            }
        }

        animator.SetBool("Move", Mathf.Abs(verticalInput) > 0.01f);
    }


    private void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && (IsGrounded() || jumpCount < maxJump))
        {
            jumpCount++; // 점프 횟수 증가
            isJumping = true;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            animator.SetBool("Jump", true);
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            animator.SetBool("Jump", false);
        }
    }

    private void Boosts()
    {
        if (Input.GetKey(KeyCode.LeftControl) && rb.velocity.magnitude < boostSpeed && currentBoost > 0 && !isBoosting)
        {
            StartCoroutine(BoostRoutine());
        }
    }

    private IEnumerator BoostRoutine()
    {
        isBoosting = true;
        currentBoost--;

        // 공중에 떠있는지 여부를 확인
        bool isInAir = !Physics.Raycast(transform.position, -transform.up, 1.5f, roadLayerMask);
        float currentBoostDuration = isInAir ? 0.3f : boostDuration; // 공중에 떠있으면 1초, 아니면 기본 부스터 시간 사용

        BoostEffect(true);

        boostingAudioSource.clip = audioClips[1]; // 배열 2번 클립 (부스터 소리)
        boostingAudioSource.Play(); // 오디오 클립 재생 시작
        boostWindAudioSource.clip = audioClips[2];
        boostWindAudioSource.Play();

        // 현재 부스터 유지 시간 대기
        yield return new WaitForSeconds(currentBoostDuration);

        // Boost 종료
        isBoosting = false;
        BoostEffect(false);
    }

    public IEnumerator BoostPadRoutine()
    {
        // Boost 시작
        isBoosting = true;
        BoostEffect(true);

        // 오디오 클립을 매번 설정하고 재생하도록 변경
        boostingAudioSource.clip = audioClips[1]; // 배열 2번 클립 (부스터 소리)
        boostingAudioSource.Play(); // 오디오 클립 재생 시작

        boostWindAudioSource.clip = audioClips[2];
        boostWindAudioSource.Play();

        // Boost 유지 시간
        yield return new WaitForSeconds(boostDuration);

        // Boost 종료
        isBoosting = false;
        BoostEffect(false);
    }

    private void BoostEffect(bool shouldPlay)
    {
        if (speedLineParticleSystem != null)
        {
            if (shouldPlay) speedLineParticleSystem.Play();
            else speedLineParticleSystem.Stop();
        }
        if (LeftboostFire != null && RightboostFire != null)
        {
            if (shouldPlay)
            {
                LeftboostFire.Play();
                RightboostFire.Play();
            }
            else
            {
                LeftboostFire.Stop();
                RightboostFire.Stop();
            }
        }
        animator.SetBool("Run", shouldPlay);
    }


    private void Steer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        // 현재 속도를 기반으로 한 회전 속도 조절
        float speedFactor = rb.velocity.magnitude / maxSpeed;
        speedFactor = Mathf.Pow(speedFactor, 2);
        float rotateAmount = horizontalInput * rotateSpeed * Mathf.Max(speedFactor, 1) * Time.deltaTime;

        if (Mathf.Abs(horizontalInput) > 0)
        {
            transform.Rotate(Vector3.up, rotateAmount, Space.World);
        }
        else
        {
            // 드리프트가 끝나면 차량을 원래 y축 기준으로 회전 상태로 복원
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, 0), Time.deltaTime * 5);

            // 여기서도 드리프트가 끝나면 rotateSpeed를 복원
            rotateSpeed = Mathf.Lerp(rotateSpeed, originalRotateSpeed, Time.deltaTime * 5);
        }
    }

    private void Drift()
    {
        float driftDirection = Input.GetAxis("Horizontal") > 0 ? 1f : -1f;
        float verticalInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(driftDirection) > 0.1f)
        {
            // 드리프트 중 회전속도를 증가
            currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, originalRotateSpeed * 5f, Time.deltaTime * 2);

            if (verticalInput > 0 && driftDirection != 0)
            {
                // 미끄러짐 효과 추가
                Vector3 driftForce = transform.right * driftDirection * driftPower;
                rb.AddForce(driftForce, ForceMode.Force);
                FillDriftGauge();
            }
        }
        else
        {
            // 드리프트가 끝나면 회전속도를 점차 복원
            currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, originalRotateSpeed, Time.deltaTime * 20);
        }
        // 회전속도 업데이트
        rotateSpeed = currentRotationSpeed;

        // 드리프트 중 속도를 조절
        rb.velocity *= Mathf.Lerp(0.9f, 0.7f, rb.velocity.magnitude / maxSpeed);
    }

    private void FillDriftGauge()
    {
        // 드리프트 감지를 위해 차량의 이동 방향과 현재 바라보고 있는 방향의 차이를 계산
        float forwardDotProduct = Vector3.Dot(transform.forward, rb.velocity.normalized);
        float driftStrength = 1 - Mathf.Abs(forwardDotProduct); // 차이가 클수록 드리프트 상태로 간주.

        // 드리프트 강도에 따라 게이지를 충전. 강도가 강할수록 더 많이 충전
        if (driftSlider.value < 1)
        {
            driftSlider.value += driftStrength * driftFillRate * Time.deltaTime;
        }
        else if (currentBoost < maxBoost)
        {
            currentBoost++;
            StartCoroutine(DecreaseDriftGauge());
        }
    }

    private IEnumerator DecreaseDriftGauge()
    {
        while (driftSlider.value > 0)
        {
            driftSlider.value -= driftDecreaseRate * Time.deltaTime;
            yield return null;
        }
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        float distance = 0.1f;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, distance))
        {
            if (!hit.collider.isTrigger)
            {
                isJumping = false;
                jumpCount = 0; // 땅에 닿았을 때 점프 횟수 초기화
                return true;
            }
        }

        return false;
    }

    public IEnumerator PreventCollision()
    {
        // Player 태그를 가진 모든 게임 오브젝트를 탐색
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            if (player != this.gameObject)
            {
                // 잠시 동안 다른 플레이어와의 충돌을 무시
                Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>(), true);
            }
        }

        // 설정한 시간(예: 2초) 동안 대기
        yield return new WaitForSeconds(2f);

        // 충돌 무시를 해제합니다.
        foreach (var player in players)
        {
            if (player != this.gameObject)
            {
                Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>(), false);
            }
        }
    }
}
