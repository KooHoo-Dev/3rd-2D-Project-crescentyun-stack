using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 추가
using UnityEngine.UI;           // UI 컴포넌트 접근을 위해 추가

namespace Just_Shape_And_Beats
{
    public class LevelManager : MonoBehaviour
    {
        [Header("UI Buttons")]
        [SerializeField] private Button tutorial;
        [SerializeField] private Button newGame;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ReturnToMainMenu();
            }
        }


        private void Start()
        {
            // 게임 시작 시 'Game Start' 버튼이 자동으로 키보드 포커스(선택)를 받도록 설정
            if (tutorial != null)
            {
                tutorial.Select();
            }

            // 마우스 클릭 시 실행될 함수 연결
            if (tutorial != null) tutorial.onClick.AddListener(Tutorial);
            if (newGame != null) newGame.onClick.AddListener(NewGame);
        }

        /// <summary>
        /// Game Start 버튼을 누르거나 선택 후 Enter를 치면 실행되는 함수
        /// </summary>
        public void Tutorial()
        {

            SceneManager.LoadScene("02. Tutorial");
        }

        /// <summary>
        /// END 버튼을 누르거나 선택 후 Enter를 치면 실행되는 함수
        /// </summary>
        public void NewGame()
        {


            SceneManager.LoadScene("04. New Game(Boss)");
        }


        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("00. Main");
        }
    }

}