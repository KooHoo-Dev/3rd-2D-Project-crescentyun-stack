using UnityEngine;
using UnityEngine.UI; 
// UI에 있는 기능을 스크립트에서 사용하려면
// UnityEngine.UI를 사용한다고 선언해야 합니다

namespace Study_ProgressBar
{
    public class Study_ProgressBar : MonoBehaviour
    {
        // # SerializeField?
        // - C# 스크립트의 private(비공개) 변수나 프로퍼티를
        //  유니티 에디터 인스펙터 창에 노출 시켜, 에디터 상에서
        // 직접 값을 변경하거나 게임오브젝트 등의 컴포넌트를
        // 할당할 수 있게 해주는 속성 입니다.
        // - private 멤버변수 선언 앞에 "[SerializeField]" 키워드를 붙힙니다
        // - 프로퍼티의 경우에는 선언 앞에 "[field: SerializeField]" 키워드를
        //  붙힙니다

        [field: SerializeField] public Image ProgressBarA { get; private set; }
        [field: SerializeField] public Image ProgressBarB { get; private set; }
        [field: SerializeField] public Image ProgressBarC { get; private set; }
        [field: SerializeField] public Image ProgressBarD { get; private set; }

        private Image[] progressBars;

        private void Start()
        {
            progressBars = new Image[]
            { ProgressBarA, ProgressBarB, ProgressBarC , ProgressBarD };
        }

        private void Update()
        {
            UpdateProgressBar();
        }

        [field: SerializeField] public int MaxNumber { get; set; } = 100;
        [field: SerializeField] public int SumAmount { get; set; } = 1;
        
        private int currentNumber = 0;

        private void UpdateProgressBar()
        {
            if (currentNumber >= MaxNumber) currentNumber = 0;
            currentNumber += SumAmount;
            float fillAmount = (float)currentNumber / MaxNumber;
            // int / int 연산시에는 앞에 float를 붙혀줘서 백분율 형태로
            // 표현되도록 해줘야 합니다.

            // Image 배열을 돌면서 Image의 FillAmount를 수정해 줍니다.
            for(int i = 0; i < progressBars.Length; ++i)
            {
                progressBars[i].fillAmount = fillAmount;
            }

            // 위의 for문은 아래와 똑같습니다.
            // ProgressBarA.fillAmount = fillAmount;
            // ProgressBarB.fillAmount = fillAmount;
            // ProgressBarC.fillAmount = fillAmount;
            // ProgressBarD.fillAmount = fillAmount;
        }
    }

}

