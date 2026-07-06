using System.Collections;
using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class BossController : MonoBehaviour
    {
        [Header("참조 패턴 스크립트")]
        public TriangleStack triangleStack;     // 1, 3, 5페이즈 연출 및 기존 삼각형 관리
        public FistPunch fistPunch;             // 2~3페이즈 (보스가 삼각형 잡고 찍기)
        public TempleBuilder temple;            // 4페이즈 (신전 연쇄 상승 패턴)
        public CameraForcedDescent camDescent;  // 카메라 제어

        [Header("보스 퇴장 연출 설정 (4페이즈 진입 전)")]
        public Transform bossTransform;         // 보스 본체의 Transform
        public GameObject triangleObject;       // 1~3페이즈 동안 사용된 그 '기존 삼각형' 오브젝트
        public float bossAscendSpeed = 0.15f;    // 보스가 위로 번개처럼 사라지는 시간 (짧을수록 빠름)
        public float delayBeforePhase4 = 0.5f;   // 보스가 사라진 후 4페이즈 시작 전까지의 숨고르기 시간

        



        private void Start()
        {
            // 게임이 시작되면 전체 보스전 시퀀스를 가동합니다.
            StartCoroutine(ExecuteBossPatternSequence());
        }

        /// <summary>
        /// 1페이즈부터 5페이즈까지 누락 없이 순서대로 쭉 이어지는 전체 흐름 제어 함수입니다.
        /// </summary>
        public IEnumerator ExecuteBossPatternSequence()
        {
            if (triangleStack == null)
            {
                Debug.LogError("TriangleStack 참조가 비어있습니다!");
                yield break;
            }

            // ============================================================
            // 1페이즈: 회전 + 무작위 탄막 및 레이저 발사
            // ============================================================
            Debug.Log("--- 1페이즈 시작 ---");

            // TriangleStack에 public으로 만들어 두신 탄막/레이저 루프 코루틴을 안전하게 바로 실행합니다.
            Coroutine bulletLoop = StartCoroutine(triangleStack.RandomBulletLoop());
            Coroutine laserLoop = StartCoroutine(triangleStack.RandomLaserLoop());

            // 삼각형 회전 시작 (오른쪽으로 2바퀴 반 회전)
            triangleStack.SetRotating(true);

            // 2바퀴 반 회전이 완전히 끝날 때까지 대기
            yield return new WaitUntil(() => triangleStack.IsRotationFinished);

            // 회전이 끝나면 가동 중이던 1페이즈 루프들을 안전하게 멈춥니다.
            if (bulletLoop != null) StopCoroutine(bulletLoop);
            if (laserLoop != null) StopCoroutine(laserLoop);

            // 1페이즈 종료 연출: 삼각형이 위쪽으로 상승
            yield return StartCoroutine(triangleStack.AscendToTop());
            yield return new WaitForSeconds(0.5f);


            // ============================================================
            // 2~3페이즈: 보스가 나타나 삼각형을 잡고 바닥으로 강하게 찍음
            // ============================================================
            Debug.Log("--- 2~3페이즈 시작 ---");
            if (fistPunch != null)
            {
                yield return StartCoroutine(fistPunch.SpawnAndStabSequence());
            }


            // ============================================================
            // 전환 연출: 찍은 직후 기존 기믹 정리 및 보스 화면 밖으로 초고속 퇴장
            // ============================================================
            if (triangleObject != null) triangleObject.SetActive(false);

            if (bossTransform != null)
            {
                Vector3 exitPosition = bossTransform.position + Vector3.up * 15f;
                yield return StartCoroutine(MoveBossExit(bossTransform, bossTransform.position, exitPosition, bossAscendSpeed));
                bossTransform.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(delayBeforePhase4);


            // ============================================================
            // 4페이즈: 새로운 삼각형 -> 신전 순차 연쇄 상승 패턴 (화면 무빙)
            // ============================================================
            Debug.Log("--- 4페이즈 시작 ---");
            if (temple != null)
            {
                yield return StartCoroutine(temple.BuildTempleChainSequence());
            }

            yield return new WaitForSeconds(0.5f);
            
        }

        /// <summary>
        /// 보스를 화면 밖으로 빠르게 이동시키기 위한 전용 코루틴 함수입니다.
        /// </summary>
        private IEnumerator MoveBossExit(Transform target, Vector3 from, Vector3 to, float duration)
        {
            if (target == null) yield break;

            float elapsed = 0f;
            // duration이 0이거나 음수일 때 예외 처리
            if (duration <= 0)
            {
                target.position = to;
                yield break;
            }

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration); // 0~1 사이로 안전하게 고정
                target.position = Vector3.Lerp(from, to, t);
                yield return null;
            }
            target.position = to;
        }

        /// <summary>
        /// 보스가 등장할 때 크기가 0에서 서서히 커지도록 만드는 코루틴
        /// </summary>
        private IEnumerator ScaleUpBoss(Transform target, Vector3 targetScale, float duration)
        {
            if (target == null) yield break;

            float elapsed = 0f;
            if (duration <= 0)
            {
                target.localScale = targetScale;
                yield break;
            }

            // 조건식을 덜 정밀한 구조에서 탈출이 확실한 구조로 변경
            while (elapsed <= duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                target.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);

                // 시간이 다 채워졌다면 강제로 루프 탈출
                if (elapsed >= duration) break;

                yield return null;
            }
            target.localScale = targetScale;
        }

    }
}