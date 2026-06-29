using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class WaveTrailEffect : MonoBehaviour
    {
        [Header("프리팹 설정")]
        public GameObject echoPrefab;          // EchoPiece 프리팹 연결

        [Header("잔상 생성 간격")]
        public float walkTimeBetweenSpawns = 0.06f; // 평소 이동할 때 잔상 간격
        public float dashTimeBetweenSpawns = 0.02f; // 대시할 때는 더 빠르게 촘촘히 생성

        private float timeLeft;
        private PlayerController playerController;
        private Vector3 lastPosition; // 💡 이전 프레임의 위치를 기록할 변수

        void Start()
        {
            playerController = GetComponent<PlayerController>();
            lastPosition = transform.position; // 초기 위치 기록
        }

        void Update()
        {
            // 💡 [핵심 변경] 물리 속도 대신, 지난 프레임의 위치와 현재 위치의 거리를 계산합니다.
            // 플레이어가 어떤 방식으로 이동하든(Translate, MovePosition, Velocity 등) 움직이기만 하면 무조건 감지됩니다.
            float moveDistance = Vector3.Distance(transform.position, lastPosition);

            // 아주 미세한 떨림을 제외하고 실제로 부드럽게 움직이고 있는지 체크 (프레임당 이동 거리 기준)
            bool isMoving = moveDistance > 0.001f;

            // 💡 대시 중이거나, 실제로 좌표가 이동 중일 때 잔상 생성 가동!
            if ((playerController != null && playerController.isDashing) || isMoving)
            {
                if (timeLeft <= 0)
                {
                    SpawnEchoPiece();

                    if (playerController != null && playerController.isDashing)
                    {
                        timeLeft = dashTimeBetweenSpawns;
                    }
                    else
                    {
                        timeLeft = walkTimeBetweenSpawns;
                    }
                }
                else
                {
                    timeLeft -= Time.deltaTime;
                }
            }

            // 💡 현재 위치를 다음 프레임 비교용으로 기록
            lastPosition = transform.position;
        }

        void SpawnEchoPiece()
        {
            if (echoPrefab == null) return;

            // 플레이어의 현재 위치와 회전값 복사 생성
            GameObject instance = Instantiate(echoPrefab, transform.position, transform.rotation);

            // 크기 버그 원천 차단 (플레이어의 실제 월드 크기 강제 주입)
            instance.transform.localScale = transform.lossyScale;

            // 플레이어 바로 뒤 레이어에 정렬
            SpriteRenderer playerSpriteRenderer = GetComponent<SpriteRenderer>();
            SpriteRenderer echoSpriteRenderer = instance.GetComponent<SpriteRenderer>();
            if (playerSpriteRenderer != null && echoSpriteRenderer != null)
            {
                echoSpriteRenderer.sortingLayerID = playerSpriteRenderer.sortingLayerID;
                echoSpriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder - 1;
            }
        }
    }
}