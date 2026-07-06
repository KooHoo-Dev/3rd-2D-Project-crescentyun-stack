using System.Collections;
using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class TriangleStack : MonoBehaviour
    {
        [Header("삼각형 루트")]
        public Transform triangleRoot;

        [Header("1페이즈 설정 (회전 및 탄막/레이저)")]
        public float rotateSpeed = 180f;        // 초당 회전 각도
        public float ascendDuration = 2.0f;     // 위로 상승하는 시간
        public Transform topScreenPoint;        // 화면 위쪽 목적지 위치

        // ===== 기존 탄막 설정 =====
        public GameObject bulletPrefab;         // 탄막 프리팹
        public Transform bulletSpawnPoint;      // 탄막 발사 위치
        public float bulletInterval = 0.15f;    // 탄막 발사 간격 (초)
        public float bulletSpeedMin = 3f;
        public float bulletSpeedMax = 6f;

        // ===== 기존 레이저 설정 완벽 복구 =====
        public GameObject laserPrefab;          // 단일 레이저 프리팹 (LaserBeam 컴포넌트 포함)
        public Transform laserSpawnPoint;       // 레이저 발사 위치
        public float laserInterval = 1.0f;      // 레이저 발사 간격 (초)

        [Header("3페이즈 설정")]
        public float sinkDuration = 3.0f;       // 땅으로 가라앉는 시간
        public float sinkDistance = 2.0f;       // 가라앉는 깊이
        public LaserBeam fanLaserLeft;          // 부채꼴 레이저 좌측 기준
        public LaserBeam fanLaserRight;         // 부채꼴 레이저 우측 기준

        [Header("5페이즈 설정")]
        public Transform[] splitTriangles;      // 분열될 3개의 삼각형 오브젝트들
        public Vector3[] splitDirections;       // 각 삼각형이 흩어질 방향
        public GameObject smallBulletPrefab;     // 살포할 소형 삼각형 자탄 프리팹
        public float scatterDuration = 5.0f;    // 자탄을 뿌리는 총 시간

        // 상태 체크용 변수들
        public bool IsRotationFinished { get; private set; } = false;
        private bool isRotating = false;
        private float currentTotalRotation = 0f;

        void Update()
        {
            // 1페이즈 회전 로직
            if (!isRotating || triangleRoot == null) return;

            // 매 프레임 회전량을 계산하여 더해줍니다 (오른쪽으로 회전)
            float angleThisFrame = rotateSpeed * Time.deltaTime;
            triangleRoot.Rotate(Vector3.forward, angleThisFrame);
            currentTotalRotation += angleThisFrame;

            // 2바퀴 반 = 360 * 2 + 180 = 900도에 도달하면 정지 신호를 보냅니다.
            if (currentTotalRotation >= 900f)
            {
                // 정확히 180도 뒤집힌 상태를 맞추기 위해 보정 후 종료합니다.
                triangleRoot.rotation = Quaternion.Euler(0, 0, 180);
                IsRotationFinished = true;
                isRotating = false;
            }
        }

        public void SetRotating(bool value)
        {
            isRotating = value;
            if (value)
            {
                IsRotationFinished = false;
                currentTotalRotation = 0f;
            }
        }

        // ==========================================
        // 1페이즈: 무작위 탄막 및 레이저 루프 코루틴
        // ==========================================

        /// <summary>
        /// 1페이즈 동안 bulletInterval 간격으로 무작위 방향 탄막을 생성합니다.
        /// </summary>
        public IEnumerator RandomBulletLoop()
        {
            while (true)
            {
                if (bulletPrefab != null && bulletSpawnPoint != null)
                {
                    // 탄막 생성
                    GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);

                    // 360도 무작위 방향 계산
                    float angle = Random.Range(0f, 360f);
                    Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                    float speed = Random.Range(bulletSpeedMin, bulletSpeedMax);

                    // Barrage 컴포넌트를 가져와 초기화 및 발사
                    var barrage = bullet.GetComponent<Barrage>();
                    if (barrage != null)
                    {
                        barrage.Init(dir, speed);
                    }
                }
                yield return new WaitForSeconds(bulletInterval);
            }
        }

        /// <summary>
        /// 원래 코드 방식 복구: laserInterval 간격으로 레이저 프리팹을 가져와 무작위 방향으로 발사합니다.
        /// </summary>
        public IEnumerator RandomLaserLoop()
        {
            while (true)
            {
                if (laserPrefab != null && laserSpawnPoint != null)
                {
                    // 360도 무작위 각도 계산
                    float randomAngle = Random.Range(0f, 360f);
                    Quaternion rotation = Quaternion.Euler(0, 0, randomAngle);

                    // 레이저 프리팹을 지정된 위치와 무작위 회전값으로 생성
                    GameObject laserObj = Instantiate(laserPrefab, laserSpawnPoint.position, rotation);

                    // 생성된 프리팹에서 LaserBeam 컴포넌트를 가져와 발사 처리
                    LaserBeam beam = laserObj.GetComponent<LaserBeam>();
                    if (beam != null)
                    {
                        // 무작위 각도를 방향 벡터(Vector2)로 변환하여 주입
                        float rad = randomAngle * Mathf.Deg2Rad;
                        beam.direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

                        // 기존 레이저 스크립트에 탑재된 Fire 함수 실행
                        beam.Fire();
                    }
                }
                yield return new WaitForSeconds(laserInterval);
            }
        }


        // ==========================================
        // 1페이즈 종료 이동 및 이후 페이즈 로직들
        // ==========================================

        /// <summary>
        /// 1페이즈 종료 시: 뒤집어진 상태로 화면 위쪽으로 서서히 상승합니다.
        /// </summary>
        public IEnumerator AscendToTop()
        {
            Vector3 startPos = triangleRoot.position;
            Vector3 endPos = topScreenPoint.position;
            float elapsed = 0f;

            while (elapsed < ascendDuration)
            {
                elapsed += Time.deltaTime;
                triangleRoot.position = Vector3.Lerp(startPos, endPos, elapsed / ascendDuration);
                yield return null;
            }
            triangleRoot.position = endPos;
        }

        
        
    }
}