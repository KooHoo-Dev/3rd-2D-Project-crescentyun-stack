using System.Collections;
using UnityEngine;

namespace Just_Shape_And_Beats
{
    /// <summary>
    /// 2페이즈: 멈춰 있는 삼각형 위치에 보스를 정렬하여 활성화하고,
    /// 오차 없이 한 몸으로 묶어서 땅으로 강하게 내려찍는 패턴을 제어합니다.
    /// </summary>
    public class FistPunch : MonoBehaviour
    {
        [Header("보스 및 삼각형 오브젝트")]
        public GameObject bossObject;           // 활성화/비활성화할 보스 게임오브젝트
        public Transform bossTransform;         // 보스의 위치 제어용 Transform
        public Transform triangleTransform;     // 1페이즈 종료 후 화면 위에 뒤집힌 채 대기 중인 삼각형

        [Header("위치 및 오프셋 세팅")]
        public Transform stabPoint;             // 삼각형을 잡고 땅으로 강하게 찍어내릴 목표 위치 (바닥 땅 지점)
        [Tooltip("보스가 삼각형 정중앙에 겹치지 않도록 위로 띄워줄 높이 오프셋입니다.")]
        public float bossYOffset = 2.0f;        // 인스펙터에서 보스 생김새에 맞춰 조절 (예: 1.5 ~ 2.5)

        [Header("타이밍 설정")]
        public float stabDuration = 0.15f;      // 땅으로 초고속 내려찍는 시간 (타격감 강조)
        public float holdDuration = 0.5f;       // 내려찍은 후 바닥에서 멈춰있는 대기 시간

        public AnimationCurve stabCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        /// <summary>
        /// 2페이즈 전체 연출 시퀀스입니다. BossController에서 호출.
        /// </summary>
        public IEnumerator SpawnAndStabSequence()
        {
            if (bossObject == null || bossTransform == null || triangleTransform == null) yield break;

           
            Vector3 spawnPosition = triangleTransform.position;
            spawnPosition.y += bossYOffset;
            bossTransform.position = spawnPosition;

           
            bossObject.SetActive(true);
            yield return new WaitForSeconds(0.15f); 

           
            triangleTransform.SetParent(bossTransform);
            yield return new WaitForSeconds(0.1f); 

          
            yield return StartCoroutine(MoveObject(bossTransform, bossTransform.position, stabPoint.position, stabDuration));

           
            triangleTransform.SetParent(null);

           
            yield return new WaitForSeconds(holdDuration);
        }

        /// <summary>
        /// 오브젝트를 부드럽고 정밀하게 이동시키는 공용 함수입니다.
        /// </summary>
        IEnumerator MoveObject(Transform target, Vector3 from, Vector3 to, float duration)
        {
            if (target == null) yield break;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float pct = stabCurve.Evaluate(Mathf.Clamp01(elapsed / duration));
                target.position = Vector3.Lerp(from, to, pct);
                yield return null;
            }
            target.position = to;
        }
    }
}