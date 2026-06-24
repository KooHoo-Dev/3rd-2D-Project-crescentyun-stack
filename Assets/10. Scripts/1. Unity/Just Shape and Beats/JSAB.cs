using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class JSAB : MonoBehaviour
    {
        [Header("--- 이동 및 대시 설정 ---")]
        public float moveSpeed = 8f;
        public float dashSpeed = 24f;
        public float dashDuration = 0.15f;
        public float dashCooldown = 0.5f;

        [Header("--- 스쿼시 & 스트레치 (형태 변형) ---")]
        public float stretchFactor = 0.04f; // 속도에 따라 늘어나는 강도
        public float maxStretch = 1.6f;       // 최대 늘어남 제한
        public float lerpSpeed = 15f;         // 원래 크기로 돌아오는 부드러움 정도

        [Header("--- 잔상 파티클 시스템 ---")]
        public ParticleSystem trailParticle;  // 사각형 파티클 시스템 연결
        public float normalEmissionRate = 10f; // 평소 이동 시 잔상 개수
        public float dashEmissionRate = 80f;   // 대시 시 잔상 개수

        // 내부 변수
        private Rigidbody2D rb;
        private Vector2 moveInput;
        private Vector3 originalScale;

        private bool isDashing = false;
        private bool canDash = true;
        private float dashTimeLeft;
        private float dashCooldownLeft;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            originalScale = transform.localScale;

            // 초기 파티클 설정 꺼두기
            if (trailParticle != null)
            {
                var emission = trailParticle.emission;
                emission.rateOverTime = 0f;
            }
        }

        void Update()
        {
            // 1. 입력 받기 (대시 중에는 입력 무시)
            if (!isDashing)
            {
                moveInput.x = Input.GetAxisRaw("Horizontal");
                moveInput.y = Input.GetAxisRaw("Vertical");
                moveInput = moveInput.normalized; // 대각선 이동 속도 보정

                // 대시 키 입력 (Space바 또는 Shift)
                if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift)) && canDash && moveInput != Vector2.zero)
                {
                    StartDash();
                }
            }

            // 2. 쿨다운 계산
            if (!canDash)
            {
                dashCooldownLeft -= Time.deltaTime;
                if (dashCooldownLeft <= 0) canDash = true;
            }

            // 3. 비틀기 애니메이션 및 잔상 제어 호출
            HandleSquashAndStretch();
            HandleParticles();
        }

        void FixedUpdate()
        {
            // 4. 물리 이동 처리
            if (isDashing)
            {
                dashTimeLeft -= Time.fixedDeltaTime;
                if (dashTimeLeft <= 0)
                {
                    StopDash();
                }
            }
            else
            {
                rb.linearVelocity = moveInput * moveSpeed; // Unity 2026 기준 velocity -> linearVelocity
            }
        }

        // --- 대시 로직 ---
        void StartDash()
        {
            isDashing = true;
            canDash = false;
            dashTimeLeft = dashDuration;
            dashCooldownLeft = dashCooldown;

            // 대시 방향으로 강한 속도 부여
            rb.linearVelocity = moveInput * dashSpeed;
        }

        void StopDash()
        {
            isDashing = false;
            rb.linearVelocity = Vector2.zero;
        }

        // --- 애니메이션: 속도 비례 가로/세로 늘리기 ---
        void HandleSquashAndStretch()
        {
            // 현재 x, y 속도의 절대값
            float speedX = Mathf.Abs(rb.linearVelocity.x);
            float speedY = Mathf.Abs(rb.linearVelocity.y);

            // 이동 방향으로 늘어나고, 반대 방향은 홀쭉해지도록 계산
            float targetX = originalScale.x + (speedX * stretchFactor) - (speedY * stretchFactor * 0.5f);
            float targetY = originalScale.y + (speedY * stretchFactor) - (speedX * stretchFactor * 0.5f);

            // 원작 느낌을 위해 최소/최대 크기 제한 (너무 찌그러지면 안 예쁨)
            targetX = Mathf.Clamp(targetX, originalScale.x * 0.6f, originalScale.x * maxStretch);
            targetY = Mathf.Clamp(targetY, originalScale.y * 0.6f, originalScale.y * maxStretch);

            // 계산된 크기로 부드럽게 보간
            Vector3 targetScale = new Vector3(targetX, targetY, originalScale.z);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * lerpSpeed);
        }

        // --- 애니메이션: 속도 및 상태에 따른 잔상 파티클 제어 ---
        void HandleParticles()
        {
            if (trailParticle == null) return;

            var emission = trailParticle.emission;
            float currentSpeed = rb.linearVelocity.magnitude;

            if (isDashing)
            {
                // 대시 중일 때는 잔상이 엄청 많이 남음
                emission.rateOverTime = dashEmissionRate;
            }
            else if (currentSpeed > 0.1f)
            {
                // 그냥 움직일 때는 적당히 뽀시래기 사각형들이 떨어짐
                emission.rateOverTime = normalEmissionRate;
            }
            else
            {
                // 멈추면 잔상 안 나옴
                emission.rateOverTime = 0f;
            }
        }
    }

}
