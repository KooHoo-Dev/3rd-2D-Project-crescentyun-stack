using System.Collections;
using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class CircleObstacle : MonoBehaviour
    {
        [Header("스프라이트")]
        public SpriteRenderer outlineCircle;  // 점선 원
        public SpriteRenderer filledCircle;   // 채워진 원

        [Header("타이밍")]
        public float warningDuration = 1f;    // 점선 원 표시 시간
        public float activeDuration = 0.5f;   // 채워진 원 표시 시간
        public float shrinkDuration = 0.3f;   // 소멸 시간

        private Collider2D col;

        void Start()
        {
            col = GetComponent<Collider2D>();
            
            if (col != null) col.enabled = false;

            filledCircle.gameObject.SetActive(false);
            outlineCircle.gameObject.SetActive(true);

            StartCoroutine(CircleRoutine());
        }

        IEnumerator CircleRoutine()
        {
            // 경고: 점선 원
            yield return new WaitForSeconds(warningDuration);

            // 공격: 채워진 원으로 교체
            outlineCircle.gameObject.SetActive(false);
            filledCircle.gameObject.SetActive(true);
            if (col != null) col.enabled = true;

            yield return new WaitForSeconds(activeDuration);

            // 소멸: 작아지면서 사라짐
            if (col != null) col.enabled = false;
            Vector3 originalScale = transform.localScale;
            float elapsed = 0f;
            while (elapsed < shrinkDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / shrinkDuration;
                transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
                yield return null;
            }

            Destroy(gameObject);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log($"충돌 감지: {collision.gameObject.tag}");
            if (collision.CompareTag("Player"))
            {
                Debug.Log("플레이어 충돌!");
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                PlayerController pc = collision.GetComponent<PlayerController>();
                if (pc != null)
                {
                    pc.StartCoroutine(pc.KnockbackCoroutine(knockbackDir));
                    pc.StartCoroutine(pc.CollisionEffect()); 
                }

                collision.GetComponent<PlayerHP>()?.TakeDamage();
            }
        }
    }
}