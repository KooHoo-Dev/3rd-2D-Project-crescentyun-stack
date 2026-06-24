using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Study_SilderBar
{
    public class Study_SilderBar : MonoBehaviour
    {
        private Slider[] Sliders { get; set; }
        // private 프로퍼티는 나만 접근하고 나만 수정할 수 있는 필드 입니다
        [field: SerializeField] private TMP_Text[] SliderTexts { get; set; }
        // 이 녀석은 직접 할당 합니다. 이유는 그냥입니다. 연습하세여


        private void Awake()
        {
            Sliders = GetComponentsInChildren<Slider>();
        }

        private void Update()
        {
            //UpdateSliderText();
        }

        private void UpdateSliderText()
        {
            // 모든 슬라이더의 value들을 Text에 표현합니다.

            for(int i = 0; i < SliderTexts.Length; ++i)
            {
                // 인덱스에 알맞는 Slider 객체를 가져옵니다.
                Slider targetSlider = Sliders[i];
                // 인덱스에 알맞는 TMP_Text 객체를 가져옵니다.
                TMP_Text targetText = SliderTexts[i];

                // targetText에 targetSlider의 value를 넣어 줍니다

                // 아래 두개는 같은 표현 입니다.
                // string text = targetSlider.value.ToString("F2");
                string text = $"{targetSlider.value:F2}";

                targetText.SetText(text);

                //targetText.text = targetSlider.value.ToString();
                // => 이렇게 해도 되지만, TMP의 SetText 함수를 호출
                //  하는게 더 좋습니다.
            }
        }


        // 매 순간 바꾸는 게 아니라 수치가 변경될때만 Update를 해봅시다
        public void OnChangedValue(float value)
        {
            Debug.Log($"{value:F2}");
            UpdateSliderText();
        }
    }
}


