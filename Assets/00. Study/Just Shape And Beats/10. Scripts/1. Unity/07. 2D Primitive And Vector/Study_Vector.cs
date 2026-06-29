using UnityEngine;
using UnityEngine.InputSystem;

namespace Study.PrimitiveAndVector
{
    // ! 주의사항 :  현재는 2D프로젝트를 진행하지만, 가르칠때는 3D를 가르칩니다.
    //              2D Vector는 3D Vector의 하위 개념이여서 Vector3만 사용하면
    //              2D도 자동적으로 사용 가능합니다.

    // # Vector? 
    //  3차원 공간에서 위치, 방향(or 크기)를 나타내는 데 사용되는 구조체(Struct)
    // 마치, 지도에서 특정 지점의 좌료플 표시하거나, 어떤 방향으로 어떤 속도 만큼
    // 이동하는지를 표현하는데에 사용됩니다.
    //  Vector는 게임 엔진의 모든 3D 계산의 기본이 되며, GameObject의 위치, 이동 방향
    // 속도, 힘, 작용 반작용 등등을 표현하는데에 필수적으로 사용됩니다.

    public class Study_Vector : MonoBehaviour
    {
        // # 3D 그래픽스(게임) 엔진에서 자주 사용하는
        // Vector3 관련 정적 필드와 메서드

        // # 기본 단위
        // Vector3.zero : (0,0,0)을 담아둔 변수(원점, 크기가 없는 상태)
        // Vector3.one : (1,1,1)을 담아둔 변수(유니티 게임오브젝트의 기본 Scale값)

        // # 방향
        // Vector3.forward : (0,0,1)을 나타냅니다 (Z축의 양의 방향)
        // Vector3.back : (0,0,-1)을 나타냅니다   (Z축의 음의 방향)
        // Vector3.up : (0,1,0)을 나타냅니다      (Y축의 양의 방향)
        // Vector3.down : (0,-1,0)을 나타냅니다   (Y축의 음의 방향)
        // Vector3.right : (1,0,0)을 나타냅니다   (X축의 양의 방향)
        // Vector3.left : (-1,0,0)을 나타냅니다   (X축의 음의 방향)

        // # 길이 (vector 소문자인 이유는 해당 객체를 통해 호출하라는거)
        // vector.magnitude : 벡터의 길이(크기)를 반환합니다
        //                   (원점으로부터 얼마나 떨어졌는지)
        // vector.sqrMagnitude : 벡터의 길이의 제곱을 반환합니다.
        //                   magnitude보다 연산 비용이 적어서 단순 비교시 사용함
        // vector.normalized : 길이가 1인 단위 벡터(Unit Vector)를 반환합니다.
        //                    방향을 나타낼 때 사용.
        //                    

        private void Update()
        {
            Vector();
        }

        public void Vector()
        {
            // # Vector의 합연산
            // - Vector 끼리의 합연산은 가능,
            //  - 의미 : 방향을 변환시키고자 할때
            // - 단일 값과의 합연산은 불가능 하다

            // # Vector의 곱연산
            // - Vector 끼리의 곱연산 나중에 배움(스칼라, 내적, 외적 등),
            // - 단일 값과의 곱연산은 가능
            //  -  의미 : 길이를 변환시키고자 할때

            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                // transform은 모든 GameObject에 존재함
                transform.position += Vector3.one;
            }
            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                Vector3 angle = new Vector3(0, 0, 30);
                transform.rotation *= Quaternion.Euler(angle);
            }
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Vector3 applyVector = Vector3.one * 20;
                transform.localScale += applyVector;
            }
        }



    }

}