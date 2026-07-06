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
        private Collider2D plyaerCollider2D;


        private float moveSpeed = 5.0f;

        public float dashSpeed = 5f;
        public float dashTime = 0.2f;
        public float dashCooltime = 0.2f;

        public bool isDashing = false;
        private bool canDash = true;
        private bool isColliding = false;
        public bool isTransitioning = false;


        [Header("넉백 설정")]
        public float knockbackForce = 8f;      // 넉백 세기
        public float knockbackDuration = 0.3f; // 이동 잠금 시간

        private bool isKnockedBack = false;    // 넉백 중이면 이동 불가

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            rb2D = GetComponent<Rigidbody2D>();
            plyaerCollider2D = GetComponent<Collider2D>();
        }

        private void Update()
        {
            Dash();
            Move();
        }
        private void Move()
        {
            if (isKnockedBack || isTransitioning) return;

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

            if (isKnockedBack || isTransitioning) return;

            if (Keyboard.current.spaceKey.wasPressedThisFrame && canDash == true)
            {
                // 대시 
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

            // 끝나고 지울거

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

            // 무적판정
            plyaerCollider2D.enabled = false;

            // 대시 시작
            rb2D.linearVelocity = direction * dashSpeed;

            yield return new WaitForSeconds(dashTime);


            // 대시 끝: 속도를 다시 0
            rb2D.linearVelocity = Vector2.zero;
            isDashing = false;

            yield return new WaitForSeconds(dashCooltime);
            canDash = true;

            // 무적 끝
            plyaerCollider2D.enabled = true;


        }
              
        // obstacle로 옴겼음
        //private void OnTriggerEnter2D(Collider2D collider)
        //{                                    
        //    if ((isColliding == false) && (collider.gameObject.CompareTag("Obstacle")))
        //    {
        //        // 장해물과의 충돌 로직
        //        Debug.Log("충돌");
        //        isColliding = true;
                
        //        // 장애물 중심 → 플레이어 방향으로 넉백
        //        Vector2 knockbackDir = (transform.position - collider.transform.position).normalized;

        //        StartCoroutine(KnockbackCoroutine(knockbackDir));

        //        StartCoroutine(CollisionEffect());

        //    }
        //}

    
        public IEnumerator KnockbackCoroutine(Vector2 direction)
        {
            Debug.Log("넉백");
            isKnockedBack = true;

            float elapsed = 0f;
            while (elapsed < knockbackDuration)
            {
                elapsed += Time.deltaTime;

                // 시간이 지날수록 점점 느려지는 넉백 (자연스러운 감속)
                float speed = Mathf.Lerp(knockbackForce, 0f, elapsed / knockbackDuration);
                transform.position += (Vector3)(direction * speed * Time.deltaTime);

                yield return null;
            }

            isKnockedBack = false;
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

