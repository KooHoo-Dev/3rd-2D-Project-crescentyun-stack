using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Just_Shape_And_Beats
{
    public class PlayerHP : MonoBehaviour
    {
        public int maxHp = 4;
        private int currentHP;

        public float knockbackPower = 10f;
        public float invincibilityDuration = 1.5f;

        private bool isHurt = false;

        public Sprite[] HpSprites;

        private PlayerController playerController;
        private Rigidbody2D rb2D;
        private SpriteRenderer spriteRenderer;


        public void Start()
        {
            currentHP = maxHp;
            playerController = GetComponent<PlayerController>();
            rb2D = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }


        public void TakeDamage()
        {
            if ((playerController != null && playerController.isDashing) || isHurt) return;

            currentHP--;
            Debug.Log($"플레이어 피격! 남은 체력: {currentHP}");


            UpdatePlayerSprite();


        }


        public void UpdatePlayerSprite()
        {

        }

        private void Die()
        {

        }
                    
        

    }

}
