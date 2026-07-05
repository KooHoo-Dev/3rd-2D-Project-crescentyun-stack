using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class GearWallMover : MonoBehaviour
    {
        public float moveSpeed = 3f;
        public float destroyX = -12f;

        void Update()
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;

            if (transform.position.x < destroyX)
                Destroy(gameObject);
        }
    }
}