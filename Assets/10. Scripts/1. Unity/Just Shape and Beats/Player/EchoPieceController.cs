using UnityEngine;

public class EchoPieceController : MonoBehaviour
{
    [Header("소멸 시간")]
    public float lifetime = 0.3f;          // 잔상이 유지되는 총 시간 (대시 속도에 맞춰 조정)

    private Vector3 startScale;
    private SpriteRenderer spriteRenderer;
    private Color startColor;
    private float timeActive = 0f;

    void Start()
    {
        // 생성된 순간의 절대 크기와 색상을 고정 기록 (위치는 고정이므로 따로 건드리지 않음)
        startScale = transform.localScale;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) startColor = spriteRenderer.color;

        // 수명이 다하면 자동 삭제
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        timeActive += Time.deltaTime;
        float progress = timeActive / lifetime; // 0(생성) ~ 1(소멸)

        //  1. [크기 연출] 위아래 요동 없이, 제자리에서 시간이 지날수록 크기가 소수점 단위(0.05배)까지 쇽 줄어듦
        transform.localScale = Vector3.Lerp(startScale, startScale * 0.05f, progress);

        //  2. [투명도 연출] 줄어들면서 동시에 서서히 투명해지며 소멸
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0f, progress));
        }
    }
}