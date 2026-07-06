using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Just_Shape_And_Beats
{
    public class PlayerHP : MonoBehaviour
    {

        public int maxHp = 5;
        private int currentHP;

        public float knockbackPower = 10f;
        public float invincibilityDuration = 1.5f;

        private bool isHurt = false;

        public Sprite[] HpSprites;

        private PlayerController playerController;
        private Rigidbody2D rb2D;
        private SpriteRenderer spriteRenderer;

        [Header("게임오버")]
        public string gameOverSceneName = "GameOver"; // 게임오버 씬 이름
        public float gameOverDelay = 1f; // 사망 후 씬 전환까지 딜레이


        public void Start()
        {
            currentHP = maxHp;
            playerController = GetComponent<PlayerController>();
            rb2D = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();


        }


        public void TakeDamage()
        {
            if (currentHP < 0) return;
            {
                if ((playerController != null && playerController.isDashing) || isHurt) return;

                currentHP--;
                Debug.Log($"플레이어 피격! 남은 체력: {currentHP}");


                UpdatePlayerSprite();
            }

            if (currentHP < 0)
            {
                Die();
            }

            StartCoroutine(InvincibilityRoutine());

        }

        IEnumerator InvincibilityRoutine()
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            float elapsed = 0f;
            while (elapsed < invincibilityDuration)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;
            }

            spriteRenderer.enabled = true;
            if (col != null) col.enabled = true;
        }



        public void UpdatePlayerSprite()
        {

            if (spriteRenderer == null || HpSprites == null) return;

            // HpSprites 배열을 HP 높은 순으로 할당
            // 예: HpSprites[0]=HP4, HpSprites[1]=HP3, HpSprites[2]=HP2, HpSprites[3]=HP1
            int index = maxHp - currentHP;
            if (index >= 0 && index < HpSprites.Length)
            {
                spriteRenderer.sprite = HpSprites[index];

            }
        }

        private void Die()
        {
            Debug.Log("플레이어 사망!");

            // 이동 잠금
            if (playerController != null)
                playerController.isTransitioning = true;

            StartCoroutine(GameOverRoutine());
        }




        IEnumerator GameOverRoutine()
        {
            // 사망 연출: 스프라이트 사라짐
            float elapsed = 0f;
            float shrinkDuration = 0.4f;
            Vector3 originalScale = transform.localScale;

            while (elapsed < shrinkDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / shrinkDuration;
                transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
                yield return null;
            }

            yield return new WaitForSeconds(gameOverDelay);

            // 게임오버 씬으로 전환
            SceneManager.LoadScene("99. GameOver");
        }

    }

}