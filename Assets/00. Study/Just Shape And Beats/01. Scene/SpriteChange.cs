using UnityEngine;

public class SpriteChange : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite[] sprites; // 1, 2, 3, 4 순서대로 넣기

    [SerializeField] private float changeInterval = 1f;

    private int currentIndex = 0;

    private void Start()
    {
        if (sprites.Length == 0) return;

        spriteRenderer.sprite = sprites[currentIndex];

        InvokeRepeating(nameof(ChangeSprite), changeInterval, changeInterval);
    }

    private void ChangeSprite()
    {
        currentIndex++;

        if (currentIndex >= sprites.Length)
            currentIndex = 0;

        spriteRenderer.sprite = sprites[currentIndex];
    }
}