using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // 씬제어를 위한 기능들이 담겨있는 네임스페이스
using Jay;

namespace Study_SceneManagement
{
    // # 동기적 씬 로딩
    // - 가장 간단한 방법으로 유니티에서 가장 많이 사용합니다.
    // - 씬의 모든 요소들을 불러오며, 불러오자마자 현재 씬을 바꿔줍니다.

    public class Study_SceneManagement : SingletonBase<Study_SceneManagement>
    {
        private Canvas canvas;

        // Awake가 아닌 SingletonBase의 OnInitialize 함수를 재정의하여 초기화 합니다
        protected override void OnInitialize()
        {
            base.OnInitialize();
            canvas = GetComponent<Canvas>();
        }

        // Update is called once per frame
        private void Update()
        {
            // 탭을 눌렀을때만 Canvas가 출력되게 해줍니다.
            bool isPressed = SimpleInput.GetKey(Key.Tab);
            canvas.enabled = isPressed;
            if (isPressed == false) return;

            if(SimpleInput.GetKeyDown(Key.Digit1))
            {
                // 유니티 엔진에서 제공하는 SceneManager를 이용해서
                // 씬 변경을 요청해볼 겁니다.
                SceneManager.LoadScene(0);
            }
            if (SimpleInput.GetKeyDown(Key.Digit2))
                SceneManager.LoadScene(1);

            if (SimpleInput.GetKeyDown(Key.Digit3))
                SceneManager.LoadScene("UFO Game"); 
            // string으로도 사용가능합니다. 다만, 씬이름이 정확하게 동일해야함
            
            if (SimpleInput.GetKeyDown(Key.Digit4))
                SceneManager.LoadScene("Merge Game");

            if (SimpleInput.GetKeyDown(Key.F5))
                SceneManager.LoadScene(4);
        }


        // 보통 씬을 사용하는 구조는 아래와 같습니다.

        public enum GameScene
        {
            CardSelector = 0,
            PairMatching,
            UFOGame,
            MergeGame,
            Home
        }

        private GameScene PrevScene { get; set; }
        private GameScene CurrentScene { get; set; } = GameScene.CardSelector;

        public void ChangeScene(int sceneIndex)
        {
            // 비효율적이지만 빠르게 버튼을 이용하기위해 함수를 제작함
            ChangeScene((GameScene)sceneIndex);
        }

        public void ChangeScene(GameScene targetScene)
        {
            PrevScene = CurrentScene;
            CurrentScene = targetScene;
            SceneManager.LoadScene((int)CurrentScene);
        }

        public void MovePrevScene()
        {
            ChangeScene(PrevScene);
        }
    }
}


