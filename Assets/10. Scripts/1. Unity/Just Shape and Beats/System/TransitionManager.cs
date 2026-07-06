using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Just_Shape_And_Beats
{
    /// <summary>
    /// 씬 전환 화면 관리.
    /// 전환 전에 TransitionManager.GoTo("씬이름", TransitionType.XXX) 호출.
    /// </summary>
    public class TransitionManager : MonoBehaviour
    {
        public static TransitionManager Instance;

        // 전환 타입
        public enum TransitionType
        {
            MainToStage,        // 메인 → 스테이지 (Let's GO!)
            StageToTutorial,    // 스테이지 → Tutorial (곡명 로딩)
            StageToNewGame,     // 스테이지 → NewGame (곡명 로딩)
            TutorialClear,      // Tutorial 클리어 → 스테이지
            NewGameClear,       // NewGame 클리어 → 엔딩
        }

        [Header("UI 오브젝트")]
        public GameObject letsGoPanel;          // "Let's GO!" 패널
        public GameObject loadingPanel;         // 곡명 로딩 패널
        public GameObject levelClearPanel;      // Level Completed 패널

        [Header("텍스트")]
        public TextMeshProUGUI songNameText;    // 우측 하단 곡명
        public TextMeshProUGUI letsGoText;      // Let's GO 텍스트

        [Header("곡명 설정")]
        public string tutorialSongName = " Corrupted ";
        public string newGameSongName = " New Game ";

        [Header("사운드")]
        public AudioSource audioSource;
        public AudioClip loadingBGM;            // 로딩 중 BGM
        public AudioClip clearSound;            // 클리어 사운드

        [Header("타이밍")]
        public float displayDuration = 2.5f;    // 화면 표시 시간

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            // PlayerPrefs에서 전환 타입과 목적지 씬 읽기
            int typeIndex = PlayerPrefs.GetInt("TransitionType", 0);
            string targetScene = PlayerPrefs.GetString("TargetScene", "");

            TransitionType type = (TransitionType)typeIndex;
            StartCoroutine(PlayTransition(type, targetScene));
        }

        /// <summary>
        /// 외부에서 호출: 전환 정보 저장 후 전환 씬으로 이동
        /// </summary>
        public static void GoTo(string targetScene, TransitionType type)
        {
            PlayerPrefs.SetInt("TransitionType", (int)type);
            PlayerPrefs.SetString("TargetScene", targetScene);
            PlayerPrefs.Save();
            SceneManager.LoadScene("98. Transition"); // 전환 씬 이름
        }

        IEnumerator PlayTransition(TransitionType type, string targetScene)
        {
            // 모든 패널 숨기기
            if (letsGoPanel != null) letsGoPanel.SetActive(false);
            if (loadingPanel != null) loadingPanel.SetActive(false);
            if (levelClearPanel != null) levelClearPanel.SetActive(false);

            switch (type)
            {
                case TransitionType.MainToStage:
                    yield return StartCoroutine(ShowLetsGo());
                    break;

                case TransitionType.StageToTutorial:
                    yield return StartCoroutine(ShowLoading(tutorialSongName));
                    break;

                case TransitionType.StageToNewGame:
                    yield return StartCoroutine(ShowLoading(newGameSongName));
                    break;

                case TransitionType.TutorialClear:
                case TransitionType.NewGameClear:
                    yield return StartCoroutine(ShowLevelClear());
                    break;
            }

            // 목적지 씬으로 전환
            //if (FadeManager.Instance != null)
            //    FadeManager.Instance.LoadScene(targetScene);
            //else
                SceneManager.LoadScene(targetScene);
        }

        // ===== 메인 → 스테이지: Let's GO! =====
        IEnumerator ShowLetsGo()
        {
            if (letsGoPanel != null) letsGoPanel.SetActive(true);

            // 로딩 BGM 재생
            if (audioSource != null && loadingBGM != null)
                audioSource.PlayOneShot(loadingBGM);

            yield return new WaitForSeconds(displayDuration);
        }

        // ===== 스테이지 → Tutorial/NewGame: 곡명 로딩 =====
        IEnumerator ShowLoading(string songName)
        {
            if (loadingPanel != null) loadingPanel.SetActive(true);
            if (songNameText != null) songNameText.text = songName;

            

            yield return new WaitForSeconds(displayDuration);
        }

        // ===== 클리어: Level Completed =====
        IEnumerator ShowLevelClear()
        {
            if (levelClearPanel != null) levelClearPanel.SetActive(true);

            // 클리어 사운드 재생
            if (audioSource != null && clearSound != null)
                audioSource.PlayOneShot(clearSound);

            yield return new WaitForSeconds(displayDuration);
        }
    }
}