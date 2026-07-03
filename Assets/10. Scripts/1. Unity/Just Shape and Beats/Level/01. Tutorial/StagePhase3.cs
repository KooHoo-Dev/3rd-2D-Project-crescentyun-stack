using System.Collections;
using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class StagePhase3 : StagePhase2
    {
        [Header("세로 레이저")]
        public GameObject verticalLaserPrefab;  // 세로 레이저 프리팹
        public float laserStartX = 0f;          // 레이저 시작 X (중앙)
        public float laserSpacing = 1.5f;       // 레이저 간격
        public int laserCount = 9;              // 순서대로 생성할 레이저 개수
        public float laserSpawnInterval = 1f;   // 9개 다 끝난 후 다시 시작까지 대기 시간

        [Header("레이저 지속 시간 오버라이드")]
        public bool overrideLaserDuration = true;   // true면 아래 값으로 덮어씀
        public float laserWarningDuration = 0.3f;   // 경고 시간
        public float laserActiveDuration = 0.7f;    // 공격 지속 시간
        public float laserShrinkDuration = 0.2f;    // 소멸 시간

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
            Debug.Log($"LaserRoutine 시작 / 프리팹: {verticalLaserPrefab}");

            if (verticalLaserPrefab == null)
            {
                Debug.LogError("verticalLaserPrefab이 null입니다! Inspector에서 연결해주세요.");
                yield break;
            }

            while (true)
            {
                for (int i = 0; i < laserCount; i++)
                {
                    float xPos = laserStartX + (i * laserSpacing);
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
                    }

                    // warningDuration + flashInterval만큼 기다리면
                    // 현재 레이저가 활성화되는 순간 다음 레이저 경고 시작
                    float waitTime = ob != null
                        ? ob.warningDuration + ob.flashInterval
                        : 0.5f;

                    yield return new WaitForSeconds(waitTime);
                }

                yield return new WaitForSeconds(laserSpawnInterval);
            }
        }
    }
    
}