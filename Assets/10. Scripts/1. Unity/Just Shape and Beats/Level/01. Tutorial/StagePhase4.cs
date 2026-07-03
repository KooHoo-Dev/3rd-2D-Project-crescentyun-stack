using System.Collections;
using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class StagePhase4 : StagePhase2
    {
        [Header("세로 레이저")]
        public GameObject verticalLaserPrefab;

        [Header("레이저 위치")]
        public float centerX = 0f;          // 중앙 X
        public float sideOffset = 2f;       // 중앙에서 좌우로 얼마나 떨어질지

        [Header("레이저 지속 시간")]
        public bool overrideLaserDuration = true;
        public float laserWarningDuration = 0.5f;
        public float laserActiveDuration = 0.95f;
        public float laserShrinkDuration = 0.25f;
        public float laserFlashInterval = 0.2f;

        [Header("세트 간격")]
        public float setBetweenInterval = 0.5f; // 2개 세트 → 3개 세트 사이 간격
        public float loopInterval = 1f;         // 한 사이클 끝나고 다시 시작까지 대기

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(LaserRoutine());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        IEnumerator LaserRoutine()
        {
            if (verticalLaserPrefab == null)
            {
                Debug.LogError("verticalLaserPrefab이 null입니다!");
                yield break;
            }

            while (true)
            {
                // ===== 1세트: 좌우 2개 동시 생성 =====
                SpawnLasers(new float[]
                {
                    centerX - sideOffset,   // 왼쪽
                    centerX + sideOffset    // 오른쪽
                });

                // 이 세트가 활성화될 때까지 대기
                yield return new WaitForSeconds(laserWarningDuration + laserFlashInterval + laserActiveDuration + laserShrinkDuration);

                yield return new WaitForSeconds(setBetweenInterval);

                // ===== 2세트: 좌/중앙/우 3개 동시 생성 =====
                SpawnLasers(new float[]
                {
                    centerX - sideOffset,   // 왼쪽
                    centerX,                // 중앙
                    centerX + sideOffset    // 오른쪽
                });

                yield return new WaitForSeconds(laserWarningDuration + laserFlashInterval + laserActiveDuration + laserShrinkDuration);

                yield return new WaitForSeconds(loopInterval);
            }
        }

        void SpawnLasers(float[] xPositions)
        {
            foreach (float xPos in xPositions)
            {
                Vector3 spawnPos = new Vector3(xPos, 0f, 0f);
                GameObject laser = Instantiate(
                    verticalLaserPrefab,
                    spawnPos,
                    verticalLaserPrefab.transform.rotation);

                Obstacle ob = laser.GetComponent<Obstacle>();
                if (ob != null && overrideLaserDuration)
                {
                    ob.warningDuration = laserWarningDuration;
                    ob.activeDuration = laserActiveDuration;
                    ob.shrinkDuration = laserShrinkDuration;
                    ob.flashInterval = laserFlashInterval;
                }
            }
        }
    }

}

