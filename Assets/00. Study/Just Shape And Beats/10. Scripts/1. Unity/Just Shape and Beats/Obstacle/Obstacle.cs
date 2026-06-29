using UnityEngine;
using System.Collections;


namespace Just_Shape_And_Beats
{

    public class Obstacle : MonoBehaviour
    {
        public float warningDuration = 1.0f; // 경고 지속 시간
        public float activeDuration = 2.0f;  // 공격 지속 시간

        private SpriteRenderer spriteRenderer;
        private Collider2D collider2D;
        private bool isDeadly = false;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            collider2D = GetComponent<Collider2D>();
            collider2D.enabled = false; // 처음엔 충돌을 끄기

            StartCoroutine(ObstacleRoutine());
        }

        IEnumerator ObstacleRoutine()
        {
            // 1. 경고 단계 (반투명하게 깜빡임)
            float time = 0f;
            Color originalColor = spriteRenderer.color;
            originalColor.a = 1f; // 완전 불투명 기준

            while (time < warningDuration)
            {
                // 알파값을 깜빡이게 만듦
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.PingPong(Time.time * 10, 0.8f) + 0.2f);
                time += Time.deltaTime;
                yield return null;
            }

            // 2. 실체화 단계 (완전한 핑크/적색으로 변하고 충돌체 활성화)
            spriteRenderer.color = originalColor; // 기본 네온 핑크색
            collider2D.enabled = true;
            isDeadly = true;

            

            //// 3. 소멸 단계 (점점 작아지며 파괴)
            //elapsed = 0f;
            //Vector3 originalScale = transform.localScale;
            //while (elapsed < 0.2f)
            //{
            //    transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsed / 0.2f);
            //    elapsed += Time.deltaTime;
            //    yield return null;
            //}

            //Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 충돌한 오브젝트가 플레이어고, 장애물이 실체화된(Deadly) 상태일 때
            if (collision.CompareTag("Player") && isDeadly)
            {
                PlayerHP playerHp = collision.GetComponent<PlayerHP>();
                
                playerHp.TakeDamage();
            }
        }
    }
}
