using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Just_Shape_And_Beats
{
    public class PlayerController : MonoBehaviour
    {
          
        private SpriteRenderer spriteRenderer;
              

        private float moveSpeed = 5.0f;
        private float dashSpeed = 20f;
        private float dashcooltime = 0.5f;

        private bool isDashing = false;
        private bool canDash = true;
        private bool isColliding = false;


        private Vector3 moveDirection;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
           
        }

        private void FixedUpdate()
        {
            Dash();
            Move();
        }

        private void Dash()
        {
            

            if (Keyboard.current.spaceKey.wasPressedThisFrame == true)
            {
                // 대시 & 대시중에 무적

                Debug.Log("Dash");
                //Rigidbody2D.linearVelocity = new Vector2(5, 0);
                //Rigidbody2D.AddForce(Vector2.right * 10f, ForceMode2D.Impulse);
            }
           
                        
            else if (Keyboard.current.spaceKey.wasReleasedThisFrame == true)
            {
                // 대시종료 & 무적 종료
                Debug.Log("321");
            }

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

        private IEnumerator CollisionEffect()
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

