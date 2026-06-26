using UnityEngine;

namespace Just_Shape_And_Beats
{
    using UnityEngine;

    public class DashEcho : MonoBehaviour
    {
        [Header("Echo Settings")]
        public float timeBetweenSpawns = 0.05f; // 잔상이 생기는 간격 (평소 이동 시)
        public float dashTimeBetweenSpawns = 0.02f; // 대시 중에는 더 빠르게 잔상이 생기도록 설정
        public GameObject echoPrefab;          // 잔상 프리팹 (EchoSprite 스크립트가 붙은 것)

        private float timeLeft;
        private PlayerController playerController;
        private Rigidbody2D rb2D;

        void Start()
        {
            playerController = GetComponent<PlayerController>();
            rb2D = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            // 💡 1. 플레이어가 현재 움직이고 있는지 체크 (물리 속도의 크기가 0.1보다 큰지 확인)
            bool isMoving = rb2D.linearVelocity.magnitude > 0.1f;

            // 💡 2. 대시 중이거나, 평소에 이동 중일 때 잔상을 만듭니다.
            if (playerController.isDashing || isMoving)
            {
                if (timeLeft <= 0)
                {
                    // 잔상 생성
                    GameObject instance = Instantiate(echoPrefab, transform.position, transform.rotation);
                    instance.transform.localScale = transform.localScale;

                    // 💡 3. 대시 중일 때는 잔상이 더 촘촘하게 생기고, 평소 이동 시에는 적당한 간격으로 생기게 조절
                    if (playerController.isDashing)
                    {
                        timeLeft = dashTimeBetweenSpawns;
                    }
                    else
                    {
                        timeLeft = timeBetweenSpawns;
                    }
                }
                else
                {
                    timeLeft -= Time.deltaTime;
                }
            }
        }
    }
}
