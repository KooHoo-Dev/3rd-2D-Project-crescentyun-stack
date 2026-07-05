using System.Collections;
using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class StagePhase5 : StagePhase4
    {
        [Header("원 패턴")]
        public GameObject circlePrefab;         // CircleObstacle 프리팹
        public float circleOffset = 3f;         // (0,0,0) 기준 원 위치 간격
        public float circleInterval = 0.8f;     // 원 하나씩 생성 간격
        public float circleLoopInterval = 1f;   // Z자 패턴 한 사이클 끝나고 다시 시작까지

        // Z자 순서: 왼쪽위 -> 오른쪽위 -> 왼쪽아래 -> 오른쪽아래
        Vector3[] GetCirclePositions()
        {
            return new Vector3[]
            {
                new Vector3(-circleOffset,  circleOffset, 0f),  // 왼쪽 위
                new Vector3( circleOffset,  circleOffset, 0f),  // 오른쪽 위
                new Vector3(-circleOffset, -circleOffset, 0f),  // 왼쪽 아래
                new Vector3( circleOffset, -circleOffset, 0f),  // 오른쪽 아래
            };
        }

        protected override void OnEnable()
        {
            base.OnEnable(); // Phase2 + Phase4 패턴 시작
            StartCoroutine(CircleRoutine());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        IEnumerator CircleRoutine()
        {
            if (circlePrefab == null)
            {
                Debug.LogError("circlePrefab이 null입니다!");
                yield break;
            }

            while (true)
            {
                Vector3[] positions = GetCirclePositions();

                // Z자 순서로 하나씩 생성
                foreach (Vector3 pos in positions)
                {
                    Instantiate(circlePrefab, pos, Quaternion.identity);
                    yield return new WaitForSeconds(circleInterval);
                }

                // 마지막 원이 다 끝날 때까지 대기 후 다시 시작
                CircleObstacle sample = circlePrefab.GetComponent<CircleObstacle>();
                float totalDuration = sample != null
                    ? sample.warningDuration + sample.activeDuration + sample.shrinkDuration
                    : 2f;

                yield return new WaitForSeconds(totalDuration + circleLoopInterval);
            }
        }
    }
}