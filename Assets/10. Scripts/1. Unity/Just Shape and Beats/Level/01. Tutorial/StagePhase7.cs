using System.Collections;
using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class StagePhase7 : MonoBehaviour
    {
        [Header("톱니바퀴 기둥")]
        public GameObject gearColumnPrefab;
        public float spawnX = 12f;
        public float destroyX = -12f;
        public float moveSpeed = 3f;
        public float spawnInterval = 3f;
        public float spawnY = 0f;

        protected virtual void OnEnable()
        {
            StartCoroutine(SpawnColumnRoutine());
            Invoke("SpawnClearTriangle", 30f);
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
            CancelInvoke();
        }

        void SpawnClearTriangle()
        {
            if (MusicLoopManager.Instance != null)
                MusicLoopManager.Instance.OnStagePatternFinished();
        }

        IEnumerator SpawnColumnRoutine()
        {
            while (true)
            {
                Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);
                GameObject column = Instantiate(gearColumnPrefab, spawnPos, Quaternion.identity);

                GearWallMover mover = column.AddComponent<GearWallMover>();
                mover.moveSpeed = moveSpeed;
                mover.destroyX = destroyX;

                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}