using UnityEngine;

namespace Just_Shape_And_Beats
{
    /// <summary>
    /// 톱니바퀴가 제자리에서 회전하는 컴포넌트.
    /// StagePhase6에서 동적으로 붙임.
    /// </summary>
    public class GearRotator : MonoBehaviour
    {
        public float rotateSpeed = -90f; // 양수=반시계, 음수=시계

        void Update()
        {
            transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
        }
    }
}