using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Just_Shape_And_Beats
{
    public class GameOverManager : MonoBehaviour
    {
        [Header("IT's Over 스프라이트")]
        public SpriteRenderer itsSprite;        // IT's 스프라이트
        public SpriteRenderer overSprite;       // Over 스프라이트
        public SpriteRenderer notSprite;        // NOT 스프라이트 (처음엔 숨김)

        [Header("플레이어 / 조각")]
        public SpriteRenderer playerRenderer;   // 플레이어 SpriteRenderer
        public Sprite wholeSprite;              // 완전한 사각형 스프라이트
        public Sprite brokenSprite;             // 쪼개진 조각 스프라이트
        public List<Transform> pieces;          // 쪼개진 조각 오브젝트들

        [Header("조각 바닥 위치")]
        public float floorY = -4f;              // 바닥 Y 위치
        public float fallDuration = 0.6f;       // 떨어지는 시간
        public float centerY = 0f;              // 중앙 Y 위치
        public float centerX = 0f;              // 중앙 X 위치

        [Header("튀어오르는 높이")]
        public float bounceHeight3 = 1f;        // 3번째 Enter 튀어오르는 높이
        public float bounceHeight4 = 2.5f;      // 4번째 Enter 튀어오르는 높이
        public float bounceDuration = 0.4f;     // 튀어오르는 시간

        [Header("합쳐진 후 연출")]
        public float rotateDuration = 1f;       // 회전하며 올라가는 시간
        public float riseHeight = 10f;          // 올라가는 높이
        public float rotateSpeed = 360f;        // 회전 속도

        private int enterCount = 0;
        private bool isAnimating = false;
        private bool isGathered = false;
        private List<Vector3> originalPiecePositions = new List<Vector3>();

        void Start()
        {
            // NOT 스프라이트 숨기기
            if (notSprite != null) notSprite.gameObject.SetActive(false);

            // 조각 오브젝트 숨기기 (처음엔 완전한 사각형)
            foreach (var piece in pieces)
                if (piece != null) piece.gameObject.SetActive(false);

            // 원본 위치 저장
            foreach (var piece in pieces)
                if (piece != null) originalPiecePositions.Add(piece.position);

            StartCoroutine(IntroSequence());
        }

        void Update()
        {
            if (isAnimating || isGathered) return;

            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                enterCount++;

                if (enterCount == 3)
                    StartCoroutine(BounceAnimation(bounceHeight3));
                else if (enterCount == 4)
                    StartCoroutine(BounceAnimation(bounceHeight4));
                else if (enterCount >= 5)
                    StartCoroutine(GatherSequence());
            }
        }

        // ===== 1~3. 인트로: IT's Over 표시 + 쪼개짐 + 낙하 =====
        IEnumerator IntroSequence()
        {
            // IT's Over 표시
            yield return new WaitForSeconds(0.5f);

            // 플레이어 → 쪼개진 조각으로 교체
            if (playerRenderer != null) playerRenderer.gameObject.SetActive(false);
            foreach (var piece in pieces)
                if (piece != null) piece.gameObject.SetActive(true);

            // 조각들 바닥으로 떨어짐
            yield return StartCoroutine(FallToFloor());
        }

        IEnumerator FallToFloor()
        {
            List<Vector3> startPositions = new List<Vector3>();
            List<Vector3> endPositions = new List<Vector3>();

            foreach (var piece in pieces)
            {
                if (piece == null) continue;
                startPositions.Add(piece.position);
                endPositions.Add(new Vector3(piece.position.x, floorY, piece.position.z));
            }

            float elapsed = 0f;
            while (elapsed < fallDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fallDuration;
                // EaseIn: 중력감
                float easedT = t * t;

                for (int i = 0; i < pieces.Count; i++)
                {
                    if (pieces[i] == null) continue;
                    pieces[i].position = Vector3.Lerp(startPositions[i], endPositions[i], easedT);
                }
                yield return null;
            }
        }

        // ===== 4. Enter 3~4번: 튀어오르는 연출 =====
        IEnumerator BounceAnimation(float height)
        {
            isAnimating = true;

            List<Vector3> floorPositions = new List<Vector3>();
            foreach (var piece in pieces)
                floorPositions.Add(piece != null ? piece.position : Vector3.zero);

            // 위로 튀어오름
            float elapsed = 0f;
            while (elapsed < bounceDuration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (bounceDuration * 0.5f);
                float easedT = 1f - (1f - t) * (1f - t); // EaseOut

                for (int i = 0; i < pieces.Count; i++)
                {
                    if (pieces[i] == null) continue;
                    pieces[i].position = Vector3.Lerp(
                        floorPositions[i],
                        new Vector3(floorPositions[i].x, floorPositions[i].y + height, floorPositions[i].z),
                        easedT);
                }
                yield return null;
            }

            // 다시 떨어짐
            List<Vector3> topPositions = new List<Vector3>();
            foreach (var piece in pieces)
                topPositions.Add(piece != null ? piece.position : Vector3.zero);

            elapsed = 0f;
            while (elapsed < bounceDuration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (bounceDuration * 0.5f);
                float easedT = t * t; // EaseIn

                for (int i = 0; i < pieces.Count; i++)
                {
                    if (pieces[i] == null) continue;
                    pieces[i].position = Vector3.Lerp(
                        topPositions[i],
                        floorPositions[i],
                        easedT);
                }
                yield return null;
            }

            // 바닥 위치로 고정
            for (int i = 0; i < pieces.Count; i++)
                if (pieces[i] != null)
                    pieces[i].position = floorPositions[i];

            isAnimating = false;
        }

        // ===== 5~7. Enter 5번: 모임 + NOT 등장 + 합쳐짐 + 회전 상승 =====
        IEnumerator GatherSequence()
        {
            isGathered = true;
            isAnimating = true;

            // 조각들이 중앙으로 모임
            List<Vector3> fromPositions = new List<Vector3>();
            foreach (var piece in pieces)
                fromPositions.Add(piece != null ? piece.position : Vector3.zero);

            float elapsed = 0f;
            float gatherDuration = 0.5f;
            while (elapsed < gatherDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / gatherDuration;
                float easedT = 1f - (1f - t) * (1f - t);

                for (int i = 0; i < pieces.Count; i++)
                {
                    if (pieces[i] == null) continue;
                    pieces[i].position = Vector3.Lerp(
                        fromPositions[i],
                        new Vector3(centerX, centerY, 0f),
                        easedT);
                }
                yield return null;
            }

            // NOT 스프라이트 등장
            if (notSprite != null) notSprite.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.3f);

            // 조각 → 완전한 사각형으로 교체
            foreach (var piece in pieces)
                if (piece != null) piece.gameObject.SetActive(false);

            if (playerRenderer != null)
            {
                playerRenderer.sprite = wholeSprite;
                playerRenderer.transform.position = new Vector3(centerX, centerY, 0f);
                playerRenderer.gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(0.2f);

            // 회전하면서 위로 올라감
            yield return StartCoroutine(RiseAndRotate());

            isAnimating = false;
        }

        IEnumerator RiseAndRotate()
        {
            if (playerRenderer == null) yield break;

            Vector3 startPos = playerRenderer.transform.position;
            Vector3 endPos = startPos + Vector3.up * riseHeight;

            float elapsed = 0f;
            while (elapsed < rotateDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / rotateDuration;

                playerRenderer.transform.position = Vector3.Lerp(startPos, endPos, t);
                playerRenderer.transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
                yield return null;
            }

            // 씬 전환
            SceneManager.LoadScene("01. Stage");
        }
    }
}