using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Just_Shape_And_Beats
{
    /// <summary>
    /// 스포너에서 Instantiate 후 동적으로 붙이는 이동/회전 컴포넌트.
    /// Obstacle.cs의 경고/공격 단계는 그대로 유지하면서
    /// 추가로 이동과 회전만 담당.
    /// </summary>
    public class BulletMover : MonoBehaviour
    {
        public Vector2 direction = Vector2.left;
        public float moveSpeed = 5f;
        public float rotateSpeed = 200f;

        private bool canMove = false;
        
        void Start()
        {
            // Obstacle의 warningDuration + flashInterval 만큼 기다린 후 이동 시작
            Obstacle obstacle = GetComponent<Obstacle>();
            if (obstacle != null)
            {
                float delay = obstacle.warningDuration + obstacle.flashInterval;
                StartCoroutine(WaitAndMove(delay));
            }
            else
            {
                canMove = true;
            }
        }

        IEnumerator WaitAndMove(float delay)
        {
            yield return new WaitForSeconds(delay);
            canMove = true;
        }

        void Update()
        {
            if (!canMove) return;

            transform.position += (Vector3)(direction.normalized * moveSpeed * Time.deltaTime);
            transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);

            if (transform.position.x < -15f)
                Destroy(gameObject);
        }
    }
}