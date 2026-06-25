using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace Just_Shape_And_Beats
{
    public class PlayerController : MonoBehaviour
    {
          
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D rb2D;      

        private float moveSpeed = 5.0f;

        public float dashSpeed = 5f;
        public float dashTime = 0.2f;
        public float dashCooltime = 0.2f;

        private bool isDashing = false;
        private bool canDash = true;
        private bool isColliding = false;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            rb2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            Dash();
            Move();
        }
        private void Move()
        {
            if (Keyboard.current.leftArrowKey.isPressed == true)
            {
                transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            }
            if (Keyboard.current.rightArrowKey.isPressed == true)
            {
                transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            }
            if (Keyboard.current.upArrowKey.isPressed == true)
            {
                transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            }
            if (Keyboard.current.downArrowKey.isPressed == true)
            {
                transform.position += Vector3.down * moveSpeed * Time.deltaTime;
            }
        }
        private void Dash()
        {
            if (isDashing == true) return;
            

            if (Keyboard.current.spaceKey.wasPressedThisFrame && canDash == true)
            {
                // 대시 & 대시중에 무적
                Debug.Log("대쉬");
                //rb2D.linearVelocity = new Vector2(5, 0);
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");

                Vector2 dsahDirection = new Vector2(h, v).normalized;

                // 방향이 입력이 없다면 자동적으로 오른쪽으로 대시
                if (dsahDirection == Vector2.zero)
                {
                    dsahDirection = transform.right;
                }

                StartCoroutine(DashCoroutine(dsahDirection));
                
            }

            else if (Keyboard.current.spaceKey.wasReleasedThisFrame == true)
            {
                // 대시종료 & 무적 종료
                Debug.Log("대쉬 종료");
                //rb2D.linearVelocity = Vector2.zero;
            }

        }


        public IEnumerator DashCoroutine(Vector2 direction)
        {
            canDash = false;
            isDashing = true;

            // 대시 시작
            rb2D.linearVelocity = direction * dashSpeed;

            yield return new WaitForSeconds(dashTime);

            // 대시 끝: 속도를 다시 0
            rb2D.linearVelocity = Vector2.zero;
            isDashing = false;

            yield return new WaitForSeconds(dashCooltime);
            canDash = true;
        }
              

        private void OnTriggerEnter2D(Collider2D collider)
        {                                    
            if ((isColliding == false) && (collider.gameObject.CompareTag("Obstacle")))
            {
                // 장해물과의 충돌 로직
                Debug.Log("123");
                isColliding = true;
                StartCoroutine(CollisionEffect()); 
            }
        }

        public IEnumerator CollisionEffect()
        {
            Color origin = spriteRenderer.color;
            Color effect = spriteRenderer.color;
            effect.a = 0.2f; // 투명도를 20%로 낮춰줍니다.

            float term = 0.1f;

            for (int i = 0; i < 10; ++i)
            {
                spriteRenderer.color = effect;
                yield return new WaitForSeconds(term);
                spriteRenderer.color = origin;
                yield return new WaitForSeconds(term);
            }

            isColliding = false;
        }



    }
}

