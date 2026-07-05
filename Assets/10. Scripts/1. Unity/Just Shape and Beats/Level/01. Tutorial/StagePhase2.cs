using System.Collections;
using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class StagePhase2 : MonoBehaviour
    {
        [Header("메인 탄막 (커졌다가 터지는)")]
        public GameObject mainBulletPrefab;

        [Header("고정 스폰 위치")]
        public float topTargetY = 3f;       // 위쪽 탄막이 멈출 Y 위치
        public float bottomTargetY = -3f;   // 아래쪽 탄막이 멈출 Y 위치
        public float targetX = 4f;          // 탄막이 멈출 X 위치 (화면 중앙)
        public float offScreenOffset = 8f;  // 화면 밖 시작 거리

        [Header("슬라이드 진입")]
        public float slideDuration = 0.5f;  // 화면 밖에서 진입하는 시간

        [Header("성장/폭발 설정")]
        public float growDuration = 1f;
        public float startScale = 0.2f;
        public float endScale = 0.5f;

        [Header("8방향 탄막")]
        public GameObject subBulletPrefab;
        public float subBulletSpeed = 4f;
        public float subBulletScale = 0.1f;

        [Header("스폰 패턴")]
        public float spawnInterval = 2f;    // 위/아래 각각 발동 간격

        protected virtual void OnEnable()
        {
            StartCoroutine(SpawnRoutine());
            Invoke("SpawnClearTriangle", 10f);
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

        IEnumerator SpawnRoutine()
        {
            bool spawnTop = true; // true=위, false=아래 번갈아가며

            while (true)
            {
                if (spawnTop)
                {
                    // 위: 화면 위 밖에서 떨어지면서 진입
                    Vector3 startPos = new Vector3(targetX, topTargetY + offScreenOffset, 0f);
                    Vector3 endPos = new Vector3(targetX, topTargetY, 0f);
                    StartCoroutine(SlideAndGrow(startPos, endPos));
                }
                else
                {
                    // 아래: 화면 아래 밖에서 솟아오르면서 진입
                    Vector3 startPos = new Vector3(targetX, bottomTargetY - offScreenOffset, 0f);
                    Vector3 endPos = new Vector3(targetX, bottomTargetY, 0f);
                    StartCoroutine(SlideAndGrow(startPos, endPos));
                }

                spawnTop = !spawnTop; // 위아래 번갈아
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        IEnumerator SlideAndGrow(Vector3 startPos, Vector3 endPos)
        {
            GameObject main = Instantiate(mainBulletPrefab, startPos, Quaternion.identity);
            main.transform.localScale = Vector3.one * startScale;

            // 콜라이더 끄기 (진입/성장 동안 무적)
            Collider2D col = main.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // ===== 1. 화면 밖에서 목표 위치로 슬라이드 진입 =====
            float elapsed = 0f;
            while (elapsed < slideDuration)
            {
                elapsed += Time.deltaTime;
                if (main == null) yield break;
                float t = elapsed / slideDuration;
                // EaseOut: 빠르게 들어오다가 부드럽게 멈춤
                float easedT = 1f - Mathf.Pow(1f - t, 3f);
                main.transform.position = Vector3.Lerp(startPos, endPos, easedT);
                yield return null;
            }
            if (main == null) yield break;
            main.transform.position = endPos;

            // ===== 2. 목표 위치에서 점점 커짐 =====
            elapsed = 0f;
            while (elapsed < growDuration)
            {
                elapsed += Time.deltaTime;
                if (main == null) yield break;
                float t = elapsed / growDuration;
                main.transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, t);
                yield return null;
            }

            if (main == null) yield break;

            Vector3 explodePos = main.transform.position;
            Destroy(main);

            // ===== 3. 8방향 폭발 =====
            Explode(explodePos);
        }

        void Explode(Vector3 pos)
        {
            Vector2[] directions = new Vector2[]
            {
                Vector2.up,
                Vector2.down,
                Vector2.left,
                Vector2.right,
                new Vector2(1, 1).normalized,
                new Vector2(1, -1).normalized,
                new Vector2(-1, 1).normalized,
                new Vector2(-1, -1).normalized
            };

            foreach (Vector2 dir in directions)
            {
                GameObject sub = Instantiate(subBulletPrefab, pos, Quaternion.identity);
                sub.transform.localScale = Vector3.one * subBulletScale;

                BulletMover mover = sub.AddComponent<BulletMover>();
                Obstacle obstacle = sub.GetComponent<Obstacle>();
                float delay = obstacle != null ? obstacle.warningDuration + obstacle.flashInterval : 0f;

                mover.direction = dir;
                mover.moveSpeed = subBulletSpeed;
                mover.rotateSpeed = 0f;
            }
        }
    }
}