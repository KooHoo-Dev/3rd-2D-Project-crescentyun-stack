using System.Collections;
using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class StagePhase9 : StagePhase2
    {
        [Header("1. 상단/하단 원 일렬")]
        public GameObject circlePrefab;
        public int rowCircleCount = 6;
        public float rowCircleSpacing = 2.5f;
        public float topRowY = 2.8f;
        public float bottomRowY = -2.8f;
        public float rowStartX = -5f;
        public float rowCircleInterval = 0.1f;
                     

        [Header("4. 랜덤 레이저")]
        public GameObject laserPrefab;
        public float laserSpawnInterval = 1.5f;
        public float laserLength = 20f;

        [Header("5. 절반 톱니바퀴 기둥")]
        public GameObject halfColumnPrefab;
        public float columnSpawnX = 12f;
        public float columnDestroyX = -12f;
        public float columnMoveSpeed = 3f;
        public float columnSpawnInterval = 2f;
        public float columnHalfY = 0f;          // 중앙 Y

        [Header("6. Y축 원 일렬")]
        public int colCircleCount = 6;
        public float colCircleSpacing = 1.5f;
        public float colCircleX = 0f;
        public float colStartY = -4f;
        public float colCircleInterval = 0.4f;
        public float colLoopInterval = 1f;

        [Header("타이밍")]
        public float clearTriangleDelay = 60f;

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(Phase9Sequence());
            StartCoroutine(WaitForMusicEnd());
        }

        protected override void OnDisable()
        {
            StopAllCoroutines();
            CancelInvoke();
        }

        IEnumerator WaitForMusicEnd()
        {
            AudioSource audio = MusicLoopManager.Instance?.audioSource;
            if (audio == null) yield break;

            // 노래가 끝날 때까지 대기
            yield return new WaitUntil(() => !audio.isPlaying);

            SpawnClearTriangle();
        }


        void SpawnClearTriangle()
        {
            if (MusicLoopManager.Instance != null)
                MusicLoopManager.Instance.OnStagePatternFinished();
        }

        IEnumerator Phase9Sequence()
        {
            // ===== 1. 상단/하단 원 일렬 =====
            yield return StartCoroutine(SpawnRowCircles());

            // 마지막 원 끝날 때까지 대기
            CircleObstacle sample = circlePrefab.GetComponent<CircleObstacle>();
            float rowWait = sample != null
                ? sample.warningDuration + sample.activeDuration + sample.shrinkDuration
                : 2f;
            yield return new WaitForSeconds(rowWait);

           

            // ===== 3~6 동시 진행 =====
            
            StartCoroutine(RandomLaserLoop());
            StartCoroutine(HalfColumnLoop());
            StartCoroutine(ColCircleLoop());
        }

        // ===== 1. 상단/하단 원 일렬 =====
        IEnumerator SpawnRowCircles()
        {
            for (int i = 0; i < rowCircleCount; i++)
            {
                float xPos = rowStartX + (i * rowCircleSpacing);
                Instantiate(circlePrefab, new Vector3(xPos, topRowY, 0f), Quaternion.identity);
                Instantiate(circlePrefab, new Vector3(xPos, bottomRowY, 0f), Quaternion.identity);
                yield return new WaitForSeconds(rowCircleInterval);
            }
        }
                      


        // ===== 4. 랜덤 레이저 =====
        IEnumerator RandomLaserLoop()
        {
            if (laserPrefab == null) yield break;

            while (true)
            {
                // X축 또는 Y축 랜덤 선택
                bool isHorizontal = Random.value > 0.5f;
                float angle = isHorizontal ? 0f : 90f;
                float randomPos = Random.Range(-4f, 4f);

                Vector3 spawnPos = isHorizontal
                    ? new Vector3(0f, randomPos, 0f)
                    : new Vector3(randomPos, 0f, 0f);

                GameObject laser = Instantiate(laserPrefab, spawnPos,
                    Quaternion.Euler(0f, 0f, angle));

                Obstacle ob = laser.GetComponent<Obstacle>();
                if (ob != null)
                {
                    float totalDuration = ob.warningDuration + ob.activeDuration + ob.shrinkDuration;
                    Destroy(laser, totalDuration);
                }

                yield return new WaitForSeconds(laserSpawnInterval);
            }
        }

        // ===== 5. 절반 톱니바퀴 기둥 =====
        IEnumerator HalfColumnLoop()
        {
            if (halfColumnPrefab == null) yield break;

            bool spawnBottom = true; // true=하단~중앙, false=상단~중앙

            while (true)
            {
                GameObject column = Instantiate(halfColumnPrefab,
                    new Vector3(columnSpawnX, columnHalfY, 0f), Quaternion.identity);

                // 하단~중앙 또는 상단~중앙 크기로 조절
                // halfColumnPrefab 자체를 절반 크기로 만들어두고
                // Y 방향으로 플립해서 번갈아 사용
                if (!spawnBottom)
                {
                    Vector3 scale = column.transform.localScale;
                    column.transform.localScale = new Vector3(scale.x, -scale.y, scale.z);
                }

                GearWallMover mover = column.AddComponent<GearWallMover>();
                mover.moveSpeed = columnMoveSpeed;
                mover.destroyX = columnDestroyX;

                spawnBottom = !spawnBottom;
                yield return new WaitForSeconds(columnSpawnInterval);
            }
        }

        // ===== 6. Y축 원 일렬 =====
        IEnumerator ColCircleLoop()
        {
            if (circlePrefab == null) yield break;

            while (true)
            {
                for (int i = 0; i < colCircleCount; i++)
                {
                    float yPos = colStartY + (i * colCircleSpacing);
                    Instantiate(circlePrefab, new Vector3(colCircleX, yPos, 0f), Quaternion.identity);
                    yield return new WaitForSeconds(colCircleInterval);
                }

                CircleObstacle sample = circlePrefab.GetComponent<CircleObstacle>();
                float totalDuration = sample != null
                    ? sample.warningDuration + sample.activeDuration + sample.shrinkDuration
                    : 2f;

                yield return new WaitForSeconds(totalDuration + colLoopInterval);
            }
        }



    }
}