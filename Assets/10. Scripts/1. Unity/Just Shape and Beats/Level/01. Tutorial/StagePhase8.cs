using System.Collections;
using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class StagePhase8 : StagePhase7
    {
        [Header("원 패턴")]
        public GameObject circlePrefab;
        public int circleCount = 6;
        public float circleSpacing = 2f;
        public float topRowY = 4f;
        public float bottomRowY = -4f;
        public float rowStartX = -5f;
        public float circleInterval = 0.5f;
        public float loopInterval = 1f;

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(CircleRowRoutine());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        IEnumerator CircleRowRoutine()
        {
            if (circlePrefab == null)
            {
                Debug.LogError("circlePrefab이 null입니다!");
                yield break;
            }

            while (true)
            {
                // 왼쪽부터 순서대로 생성
                // circleInterval 간격으로 생성하면 터지는 것도 순서대로
                for (int i = 0; i < circleCount; i++)
                {
                    float xPos = rowStartX + (i * circleSpacing);

                    // 상단 + 하단 동시 생성
                    Instantiate(circlePrefab, new Vector3(xPos, topRowY, 0f), Quaternion.identity);
                    Instantiate(circlePrefab, new Vector3(xPos, bottomRowY, 0f), Quaternion.identity);

                    yield return new WaitForSeconds(circleInterval);
                }

                // 마지막 원이 다 끝날 때까지 대기 후 반복
                CircleObstacle sample = circlePrefab.GetComponent<CircleObstacle>();
                float totalDuration = sample != null
                    ? sample.warningDuration + sample.activeDuration + sample.shrinkDuration
                    : 2f;

                yield return new WaitForSeconds(totalDuration + loopInterval);
            }
        }
    }
}