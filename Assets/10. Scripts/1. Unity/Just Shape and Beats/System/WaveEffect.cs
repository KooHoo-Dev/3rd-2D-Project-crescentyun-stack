using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Just_Shape_And_Beats
{
    /// <summary>
    /// 파란색 사각형들이 플레이어 위치에서 원형으로 퍼지면서 화면을 덮는 효과.
    /// 사각형 프리팹(파란색 SpriteRenderer)만 있으면 됨.
    /// </summary>
    public class WaveEffect : MonoBehaviour
    {
        [Header("사각형 프리팹")]
        public GameObject squarePrefab;         // 파란색 사각형 스프라이트 프리팹

        [Header("파티클 설정")]
        public int squareCount = 40;            // 사각형 개수
        public float spreadRadius = 10f;        // 최대 퍼지는 반경
        public float spreadDuration = 0.6f;     // 퍼지는 시간
        public float squareSize = 0.8f;         // 사각형 크기
        
        public float rotateSpeed = 180f;        // 회전 속도

        [Header("후광 (중심 빛)")]
        public float glowRadius = 2f;           // 중심에서 빽빽하게 채울 반경

        List<SquareParticle> particles = new List<SquareParticle>();

        class SquareParticle
        {
            public Transform transform;
            public Vector2 direction;
            public float speed;
            public float rotSpeed;
            public float size;
        }

        public void Play(Vector3 center)
        {
            StartCoroutine(PlayRoutine(center));
        }

        IEnumerator PlayRoutine(Vector3 center)
        {
            particles.Clear();

            // 사각형 생성 - 원형으로 고르게 배치
            for (int i = 0; i < squareCount; i++)
            {
                float angle = (360f / squareCount) * i + Random.Range(-3f, 3f);
                Vector2 dir = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad));

                GameObject obj = Instantiate(squarePrefab, center, Quaternion.identity);

                float size = squareSize;
                obj.transform.localScale = Vector3.one * size;
                obj.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

                var sp = new SquareParticle
                {
                    transform = obj.transform,
                    direction = dir,
                    speed = spreadRadius / spreadDuration * Random.Range(0.95f, 1.05f),
                    rotSpeed = Random.Range(-rotateSpeed, rotateSpeed),
                    size = size
                };

                particles.Add(sp);
            }

            // 퍼져나가는 애니메이션
            float elapsed = 0f;
            while (elapsed < spreadDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / spreadDuration;

                foreach (var sp in particles)
                {
                    if (sp.transform == null) continue;

                    // 위치: 중심에서 바깥으로
                    sp.transform.position += (Vector3)(sp.direction * sp.speed * Time.deltaTime);

                    // 회전
                    sp.transform.Rotate(Vector3.forward, sp.rotSpeed * Time.deltaTime);

                    // 뒤쪽 사각형은 페이드 아웃
                    SpriteRenderer sr = sp.transform.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        Color c = sr.color;
                        c.a = Mathf.Lerp(1f, 0f, (t - 0.5f) * 2f); // 후반부에 사라짐
                        sr.color = c;
                    }
                }

                yield return null;
            }

            // 다 끝나면 파괴
            foreach (var sp in particles)
            {
                if (sp.transform != null)
                    Destroy(sp.transform.gameObject);
            }
            particles.Clear();
        }
    }
}