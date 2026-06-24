using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Study.UGUI_BasicComponent
{
    public class Study_UIController : MonoBehaviour
    {
        // 스크립트를 이용해서 여러 캔버스를 제어하는 방법을 알아봅시다
        // 1. 키입력으로 제어하기
        // 2. Button을 사용해서 UI 입력을 통해 제어하기

        [Serializable]
        public enum CanvasType
        {
            CanvasA = 1,
            CanvasB,
            CanvasC,
            CanvasD,
        }

        private Canvas[] canvases;
        private Canvas menuCanvas;

        private void Awake()
        {
            canvases = GetComponentsInChildren<Canvas>();
            
            // 여러 캔버스 중 MenuCanvas 게임오브젝트를 골라 골라서
            // menuCanvas에 할당을 해줍니다. 해당 Canvas는 언제나 활성화 해둘겁니다.

            for(int i = 0; i < canvases.Length; ++i)
            {
                // MenuCanvas의 게임오브젝트를 검색했으면 종료
                if (canvases[i].gameObject.name.Equals("MenuCanvas"))
                {
                    menuCanvas = canvases[i];
                    break;
                }
            }

            SetActiveCanvas(CanvasType.CanvasA);
            SetButtons();
        }

        private void Update()
        {
            if(Keyboard.current.qKey.wasPressedThisFrame)
            {
                SetActiveCanvas(CanvasType.CanvasA);
            }

            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                SetActiveCanvas(CanvasType.CanvasB);
            }

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                SetActiveCanvas(CanvasType.CanvasC);
            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                SetActiveCanvas(CanvasType.CanvasD);
            }
        }

        public void SetActiveCanvas(CanvasType canvasType)
        {
            // 다 껏다가
            for (int i = 0; i < canvases.Length; ++i)
            {
                canvases[i].enabled = false;
            }

            // 켜줘여할 녀석들만 켜준다
            menuCanvas.enabled = true;
            canvases[(int)canvasType].enabled = true;
        }

        public void SetActiveCanvas(int canvasType)
        {
            SetActiveCanvas((CanvasType)canvasType);
        }

        public void EmptyFunction()
        {

        }


        private Button[] buttons;

        private void SetButtons()
        {
            buttons = menuCanvas.transform.GetComponentsInChildren<Button>();
            // menuCanvas.GetComponentsInChildren<Button>(); => 이렇게 해도 됨

            for (int i = 0; i < buttons.Length; ++i)
            {
                //buttons[i].onClick.AddListener(() => SetActiveCanvas(i));

                int index = i + 1;
                buttons[i].onClick.AddListener(() => SetActiveCanvas(index));
                // 람다 표현식 : (매개변수) => 지칭 함수
                // : 프로그래밍에서 함수를 하나의 식으로 간결하게 표현하는 방법
                //  익명함수, 무명함수(이름이 없는 함수)라고도 부르고, 코드의
                //  가독성을 높이지만, 비용(메모리를 사용해서 가비지)이 소모됩니다

                // 캡처
                // : 람다가 선언된 범위 밖의 외부 변수를 람다 내부로 가져와서 사용하느
                //  동작을 의미합니다. 값 캡처와 참조 캡처가 있어서 캡처가 일어날 경우
                //  의도치 않은 버그가 발생할 수 있습니다
            }

            for (int i = 0; i < buttons.Length; ++i)
            {
                // buttons[i].interactable = (i % 2 == 0);
                // 아래와 내용은 같지만 위가 맞는 표현

                if (i % 2 == 0)
                {
                    buttons[i].interactable = true;
                }
                else
                {
                    buttons[i].interactable = false;
                }
                
            }
        }
    }
}
