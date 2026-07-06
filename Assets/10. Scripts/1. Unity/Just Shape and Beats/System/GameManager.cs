using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Just_Shape_And_Beats
{
    public class GameManager : MonoBehaviour
    {
        [Header("버튼 오브젝트")]
        public Button gameStartButton;
        public Button endButton;

        [Header("버튼 SpriteRenderer")]
        public SpriteRenderer gameStartRenderer;
        public SpriteRenderer endRenderer;

        [Header("스프라이트")]
        public Sprite normalSprite;     
        public Sprite selectedSprite;   
               
        private int selectedIndex = 0;
        private Button[] buttons;
        private SpriteRenderer[] renderers;

        void Start()
        {
            buttons = new Button[] { gameStartButton, endButton };
            renderers = new SpriteRenderer[] { gameStartRenderer, endRenderer };

            gameStartButton.onClick.AddListener(OnGameStart);
            endButton.onClick.AddListener(OnEnd);

            // 마우스 호버 이벤트 연결
            AddHoverEvent(gameStartButton, 0);
            AddHoverEvent(endButton, 1);

            UpdateButtonVisual();
        }

        void Update()
        {
            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                selectedIndex = (selectedIndex - 1 + buttons.Length) % buttons.Length;
                UpdateButtonVisual();
            }
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                selectedIndex = (selectedIndex + 1) % buttons.Length;
                UpdateButtonVisual();
            }

            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                buttons[selectedIndex].onClick.Invoke();
            }
        }

        void AddHoverEvent(Button button, int index)
        {
            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            int capturedIndex = index;
            enterEntry.callback.AddListener((data) =>
            {
                selectedIndex = capturedIndex;
                UpdateButtonVisual();
            });
            trigger.triggers.Add(enterEntry);
        }

        void UpdateButtonVisual()
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                    renderers[i].sprite = (i == selectedIndex) ? selectedSprite : normalSprite;
            }
        }

        public void OnGameStart()
        {
            TransitionManager.GoTo("01. Stage", TransitionManager.TransitionType.MainToStage);
            
        }

        public void OnEnd()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}