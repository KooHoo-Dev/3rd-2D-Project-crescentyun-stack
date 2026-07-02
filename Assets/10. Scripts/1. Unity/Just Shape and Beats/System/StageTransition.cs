
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Just_Shape_And_Beats
{
    /// <summary>
    /// 스테이지 전환 연출 시퀀스:
    /// 1. 삼각형이 회전하며 위로 올라갔다가 플레이어 위치로 떨어짐
    /// 2. 파란색 물결이 화면을 덮으며 플레이어 사라짐
    /// 3. 왼쪽에서 플레이어 새로 등장
    /// 4. MusicLoopManager.AdvanceToNextStage() 호출
    /// </summary>
    public class StageTransition : MonoBehaviour
    {
        [Header("삼각형 연출")]
        public Transform triangleTransform;       // 먹은 삼각형
        public float riseHeight = 3f;             // 위로 올라가는 높이
        public float riseDuration = 0.4f;         // 올라가는 시간
        public float fallDuration = 0.3f;         // 떨어지는 시간
        public float rotateSpeed = 360f;          // 회전 속도 (deg/sec)

        [Header("플레이어")]
        public Transform playerTransform;         // 현재 플레이어
        public GameObject playerObject;           // 플레이어 오브젝트 (사라짐/등장용)
        public Vector3 respawnPosition;           // 왼쪽 재등장 위치

        [Header("파란색 물결 UI")]
        public WaveEffect waveEffect;

        [Header("타이밍")]
        public float playerHideDuration = 0.3f;   // 플레이어 사라지는 시간
        public float respawnDelay = 0.3f;         // 왼쪽에서 등장하기 전 딜레이



        void Start()
        {
           
        }

        public void StartTransition(Transform triangle)
        {
            triangleTransform = triangle;
            StartCoroutine(TransitionSequence());
        }

        IEnumerator TransitionSequence()
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null) player.isTransitioning = true;


            // ===== 1. 삼각형이 회전하며 위로 올라갔다가 떨어짐 =====
            yield return StartCoroutine(TriangleEffect());

            // ===== 2. 파란색 물결이 화면을 덮음 + 플레이어 사라짐 =====
            yield return StartCoroutine(Wave());

            // ===== 3. 왼쪽에서 플레이어 새로 등장 =====
            yield return StartCoroutine(RespawnPlayer());

            // ===== 4. 다음 스테이지로 =====
            if (MusicLoopManager.Instance != null)
            {
                MusicLoopManager.Instance.AdvanceToNextStage();
            }
        }

        IEnumerator TriangleEffect()
        {
            if (triangleTransform == null || playerTransform == null) yield break;

            Vector3 playerPos = playerTransform.position;
            Vector3 topPos = playerPos + Vector3.up * riseHeight;

            // 위로 올라가며 회전
            float elapsed = 0f;
            while (elapsed < riseDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / riseDuration;
                triangleTransform.position = Vector3.Lerp(playerPos, topPos, t);
                triangleTransform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
                yield return null;
            }

            // 떨어지며 회전
            elapsed = 0f;
            while (elapsed < fallDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fallDuration;
                // 중력감 있게 떨어지도록 EaseIn
                triangleTransform.position = Vector3.Lerp(topPos, playerPos, t * t);
                triangleTransform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
                yield return null;
            }

            // 삼각형 사라짐
            triangleTransform.gameObject.SetActive(false);
            Destroy(triangleTransform.gameObject);

        }

        IEnumerator Wave()
        {
            // 플레이어 위치에서 사각형 파티클 퍼짐
            if (waveEffect != null)
                waveEffect.Play(playerTransform.position);

            // 잠깐 기다렸다가 플레이어 숨기기
            yield return new WaitForSeconds(0.2f);
            playerObject.SetActive(false);

            // 파티클이 퍼지는 시간만큼 대기
            yield return new WaitForSeconds(0.4f);
        }

        IEnumerator RespawnPlayer()
        {
            yield return new WaitForSeconds(respawnDelay);

            // 플레이어 왼쪽에 재등장
            playerTransform.position = respawnPosition;
            playerObject.SetActive(true);

            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null) player.isTransitioning = false;

        }
    }
}