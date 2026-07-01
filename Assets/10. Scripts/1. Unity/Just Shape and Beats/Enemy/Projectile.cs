using Just_Shape_And_Beats;
using UnityEngine;

/// <summary>
/// 무작위 방향 탄막. 일정 시간 후 자동 파괴.
/// Rigidbody2D가 있으면 물리 기반(velocity)으로 이동,
/// 없으면 transform 직접 이동.
/// </summary>
public class Barrage : MonoBehaviour
{
    public float lifeTime = 6f;

    Vector2 direction;
    float speed;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = direction * speed;
        }

        // 일정 시간 후 자동 삭제
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 태그와 충돌 시 처리 (Player 스크립트에서 사망/리스폰 처리)
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHP>()?.TakeDamage();
        }
    }
}