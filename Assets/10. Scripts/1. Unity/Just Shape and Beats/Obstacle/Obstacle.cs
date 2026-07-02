using UnityEngine;
using System.Collections;

namespace Just_Shape_And_Beats
{
    public class Obstacle : MonoBehaviour
    {
        public float warningDuration = 1.0f; // 경고 지속 시간
        public float activeDuration = 2.0f;  // 공격 지속 시간
        public float lifeTime = 6f;

        [Header("번쩍임 설정")]
        public int flashCount = 1;           // 공격 진입 시 번쩍이는 횟수
        public float flashInterval = 0.2f;   // 번쩍임 간격

        [Header("축소 소멸 설정")]
        public float shrinkDuration = 0.25f; // 작아지며 사라지는 시간

        Vector2 direction;
        float speed;

        private SpriteRenderer spriteRenderer;
        private Collider2D collider2d;
        private bool isDeadly = false;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            collider2d = GetComponent<Collider2D>();
            collider2d.enabled = false;

            
            StartCoroutine(ObstacleRoutine());
        }

        public void Init(Vector2 dir, float spd)
        {
            direction = dir.normalized;
            speed = spd;
        }

        IEnumerator ObstacleRoutine()
        {
            Color originalColor = spriteRenderer.color; // 원본 색 저장
            Vector3 originalScale = transform.localScale; // 원본 크기 저장

            //  1. 경고 - 반투명
            Color color = spriteRenderer.color;
            color.a = 0.4f; // 반투명 
            spriteRenderer.color = color;

            yield return new WaitForSeconds(warningDuration);


            //  2. 생성: 흰색으로 변하면서 세로로 넓어짐

            
            float elapsed = 0f;
            while (elapsed < flashInterval)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / flashInterval;

                // 색: 원본 → 흰색
                spriteRenderer.color = new Color(255, 255, 255);

                // 세로: 0 → 원본 크기
                transform.localScale = new Vector3(originalScale.x, Mathf.Lerp(0f, originalScale.y, t), originalScale.z);

                yield return null;
            }


            ////  2. 번쩍

            //    spriteRenderer.color = new Color(255, 255, 255);
            //    yield return new WaitForSeconds(flashInterval);
            //transform.localScale = new Vector3(
            //        originalScale.x,
            //        Mathf.Lerp(0f, originalScale.y, t),
            //        originalScale.z);

            //spriteRenderer.color = originalColor;
            //    yield return new WaitForSeconds(flashInterval);


            // 3. 공격 
            originalColor.a = 1f;
            spriteRenderer.color = originalColor;
            this.collider2d.enabled = true;
            isDeadly = true;

            yield return new WaitForSeconds(activeDuration);

            //  4. 소멸: 작아지면서 사라짐
            this.collider2d.enabled = false;
            isDeadly = false;
            
            while (elapsed < shrinkDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / shrinkDuration;
                transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
                yield return null;
            }

            Destroy(gameObject);
        }
              

        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.CompareTag("Player") && isDeadly)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                PlayerController playerController = collision.GetComponent<PlayerController>();

                playerController.StartCoroutine(playerController.KnockbackCoroutine(knockbackDir));
                playerController.StartCoroutine(playerController.CollisionEffect());


                collision.GetComponent<PlayerHP>().TakeDamage();

            }
        }
    }
}