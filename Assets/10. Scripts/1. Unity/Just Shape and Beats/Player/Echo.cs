using UnityEngine;


namespace Just_Shape_And_Beats
{
    public class EchoSprite : MonoBehaviour
    {
        public float fadeDuration = 0.2f; // 잔상이 사라지는 시간
        private SpriteRenderer spriteRenderer;
        private Color alphaColor;
        private float timeActive;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            // 잔상 색상을 반투명한 하늘색이나 흰색으로 지정
            alphaColor = new Color(0f, 0.75f, 1f, 0.5f);
            spriteRenderer.color = alphaColor;
           
            Destroy(gameObject, fadeDuration); // 시간 지나면 파괴
        }

        void Update()
        {
            timeActive += Time.deltaTime;
            // 시간에 따라 알파값(투명도)을 0으로 보냄
            alphaColor.a = Mathf.Lerp(0.5f, 0f, timeActive / fadeDuration);
            sr.color = alphaColor;
        }
    }
}
