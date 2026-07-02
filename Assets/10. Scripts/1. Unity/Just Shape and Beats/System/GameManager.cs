using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 추가
using UnityEngine.UI;           // UI 컴포넌트 접근을 위해 추가

namespace Just_Shape_And_Beats
{
    public class GameManager : MonoBehaviour
    {
        [Header("UI Buttons")]
        [SerializeField] private Button gameStartButton;
        [SerializeField] private Button endButton;

        private void Start()
        {
            // 게임 시작 시 'Game Start' 버튼이 자동으로 키보드 포커스(선택)를 받도록 설정
            if (gameStartButton != null)
            {
                gameStartButton.Select();
            }

            // 마우스 클릭 시 실행될 함수 연결
            if (gameStartButton != null) gameStartButton.onClick.AddListener(OnGameStart);
            if (endButton != null) endButton.onClick.AddListener(OnGameEnd);
        }

        /// <summary>
        /// Game Start 버튼을 누르거나 선택 후 Enter를 치면 실행되는 함수
        /// </summary>
        public void OnGameStart()
        {
            Debug.Log("Game Start: 01.stage 씬으로 이동합니다.");
            SceneManager.LoadScene("01. Stage");
        }

        /// <summary>
        /// END 버튼을 누르거나 선택 후 Enter를 치면 실행되는 함수
        /// </summary>
        public void OnGameEnd()
        {
            Debug.Log("END: 게임을 종료합니다.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 테스트할 때 종료
#else
            Application.Quit(); // 빌드된 게임에서 종료
#endif
        }
    }
}