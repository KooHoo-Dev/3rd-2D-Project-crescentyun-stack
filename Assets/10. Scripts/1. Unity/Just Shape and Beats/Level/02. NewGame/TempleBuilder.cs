using System.Collections;
using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class TempleBuilder : MonoBehaviour
    {
        [Header("참조 스크립트")]
        public CameraForcedDescent camDescent;

        [Header("씬에 배치된 기믹 순서대로 넣기")]
        [Tooltip("[삼각형1 -> 신전1 -> 삼각형2 -> 신전2] 순서 배열")]
        public GameObject[] patternObjects;

        [Header("순차 생성 및 카메라 동기화 타이밍")]
        public float triangleToTempleDelay = 0.25f;
        public float templeToCameraDelay = 0.2f;
        public float cameraMoveDuration = 0.4f;
        public float nextStepDelay = 0.3f;


        [Header("첫 신전 생성 시 제거할 스프라이트")]
        [Tooltip("첫 번째 신전이 켜질 때 화면에서 지워버릴 스프라이트 오브젝트를 여기에 넣으세요.")]
        public GameObject spriteToRemove;

        // ============================================================
        // ↙ [추가된 부분] 마지막 사원 완성 후 활성화할 새로운 보스 등록칸
        // ============================================================
        [Header("마지막 사원 완성 후 등장할 보스")]
        [Tooltip("모든 사원이 완성되고 카메라 무빙이 끝났을 때 활성화할 NEW BOSS 오브젝트를 넣으세요.")]
        public GameObject newBossObject;

        public IEnumerator BuildTempleChainSequence()
        {
            if (patternObjects == null || patternObjects.Length == 0 || camDescent == null) yield break;

            for (int i = 0; i < patternObjects.Length; i += 2)
            {
                // 최적화: 새로운 기믹이 시작될 때 이전 세트를 완전히 끕니다.
                if (i >= 2)
                {
                    if (patternObjects[i - 2] != null) patternObjects[i - 2].SetActive(false);
                    if (patternObjects[i - 1] != null) patternObjects[i - 1].SetActive(false);
                }

                // 1단계: 이번 박자의 삼각형 활성화
                GameObject currentTriangle = patternObjects[i];
                if (currentTriangle != null)
                {
                    currentTriangle.SetActive(true);
                    StartCoroutine(ScaleUp(currentTriangle.transform, 0.15f));
                }

                yield return new WaitForSeconds(triangleToTempleDelay);

                // 2단계: 이번 박자의 신전 활성화
                if (i + 1 >= patternObjects.Length) yield break;
                GameObject currentTemple = patternObjects[i + 1];
                if (currentTemple != null)
                {
                    currentTemple.SetActive(true);
                    yield return StartCoroutine(ScaleUp(currentTemple.transform, 0.15f));

                    // 루프가 처음 돌 때 (i == 0 이면 첫 번째 세트) 스프라이트 제거
                    if (i == 0 && spriteToRemove != null)
                    {
                        spriteToRemove.SetActive(false);
                    }
                }

                // 신전 켜지고 카메라 출발 전 비트 대기
                yield return new WaitForSeconds(templeToCameraDelay);

                // 3단계: 완성된 신전의 위치로 카메라를 정밀 이동 (첫 사원 제외)
                if (i != 0 && currentTemple != null)
                {
                    float targetCameraY = currentTemple.transform.position.y;
                    yield return StartCoroutine(camDescent.MoveToYPosition(targetCameraY, cameraMoveDuration));
                }

                // 카메라가 한 칸 도착한 후 다음 삼각형이 켜지기 전 비트 대기
                if (i < patternObjects.Length - 2)
                {
                    yield return new WaitForSeconds(nextStepDelay);
                }
            }

            // 마무리 정리 (마지막 세트 직전의 기믹들 오프)
            int lastIndex = patternObjects.Length;
            if (lastIndex >= 2)
            {
                if (patternObjects[lastIndex - 2] != null) patternObjects[lastIndex - 2].SetActive(false);
                if (patternObjects[lastIndex - 1] != null) patternObjects[lastIndex - 1].SetActive(false);
            }

            // 모든 사원이 지어지고 카메라도 마지막 꼭대기에 완전히 안착한 뒤 숨고르기 대기
            yield return new WaitForSeconds(0.5f);

            // ============================================================
            // .
            // ============================================================
            if (newBossObject != null)
            {
                newBossObject.SetActive(true);
                Debug.Log("마지막 사원 안착 완료! 새로운 보스 등장!");
                yield return StartCoroutine(ScaleUpBoss(newBossObject.transform, 18f));

            }

            yield return new WaitForSeconds(0.5f);
        }

        IEnumerator ScaleUp(Transform t, float duration)
        {
            if (t == null) yield break;
            Vector3 endScale = t.localScale;
            t.localScale = Vector3.zero;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                t.localScale = Vector3.Lerp(Vector3.zero, endScale, elapsed / duration);
                yield return null;
            }
            t.localScale = endScale;
        }

        IEnumerator ScaleUpBoss(Transform t, float duration)
        {
            if (t == null) yield break;

            Vector3 endScale = t.localScale;
            t.localScale = Vector3.zero;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float pct = elapsed / duration;
                float easedPct = 1f - Mathf.Pow(1f - pct, 3f);
                t.localScale = Vector3.Lerp(Vector3.zero, endScale, easedPct);
                yield return null;
            }

            t.localScale = endScale;

            // 보스 소멸
            Destroy(t.gameObject);

            yield return new WaitForSeconds(0.5f);

            // 음악 종료
            AudioSource audio = FindFirstObjectByType<AudioSource>();
            if (audio != null) audio.Stop();

            yield return new WaitForSeconds(0.5f);

            // 스테이지 클리어 씬 전환
            TransitionManager.GoTo("100. Credit", TransitionManager.TransitionType.NewGameClear);

        }



    }
}