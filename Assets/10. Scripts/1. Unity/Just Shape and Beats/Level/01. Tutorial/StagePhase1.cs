using System.Collections;
using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class StagePahse1 : MonoBehaviour
    {
        [Header("장애물 프리팹")]
        public GameObject obstaclePrefab;

        [Header("스폰 위치")]
        public float spawnX = 12f;          // 오른쪽 스폰 X 위치
        public float spawnYMin = -4f;       // 스폰 Y 최솟값
        public float spawnYMax = 4f;        // 스폰 Y 최댓값

        [Header("이동")]
        public float moveSpeed = 5f;        // 왼쪽으로 이동 속도
        public float rotateSpeed = 200f;    // 회전 속도

        [Header("스폰 패턴")]
        public int totalSpawnCount = 20;    // 총 스폰 개수
        public float spawnInterval = 0.4f;  // 스폰 간격

        void OnEnable()
        {
            StartCoroutine(SpawnRoutine());
            Invoke("SpawnClearTriangle", 10f);


        }

        void OnDisable()
        {
            StopAllCoroutines();
            CancelInvoke();
        }

        IEnumerator SpawnRoutine()
        {
            while (true) // 무한 반복
            {
                float randomY = Random.Range(spawnYMin, spawnYMax);
                Vector3 spawnPos = new Vector3(spawnX, randomY, 0f);

                GameObject obj = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);

                BulletMover mover = obj.AddComponent<BulletMover>();
                mover.direction = Vector2.left;
                mover.moveSpeed = moveSpeed;
                mover.rotateSpeed = rotateSpeed;

                yield return new WaitForSeconds(spawnInterval);
            }
            // OnStagePatternFinished 호출 제거 (삼각형을 먹으면 MusicLoopManager가 알아서 처리)
        }


        void SpawnClearTriangle()
        {
            if (MusicLoopManager.Instance != null)
                MusicLoopManager.Instance.OnStagePatternFinished();
        }


    }

}