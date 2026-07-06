using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Just_Shape_And_Beats
{
    public class LevelManager : MonoBehaviour
    {
        [Header("원 트리거 오브젝트")]
        public Transform leftCircle;        // 왼쪽 원 중심
        public Transform rightCircle;       // 오른쪽 원 중심
        public float circleRadius = 1f;     // 원 반경

        [Header("플레이어")]
        public Transform playerTransform;
        public float moveSpeed = 5f;

        [Header("점선 이동 경로")]
        public Transform leftPoint;         // 왼쪽 끝 이동 포인트
        public Transform rightPoint;        // 오른쪽 끝 이동 포인트

        [Header("미리보기 스프라이트")]
        public GameObject leftPreviewImage;  // 왼쪽 원에 있을 때 표시할 이미지
        public GameObject rightPreviewImage; // 오른쪽 원에 있을 때 표시할 이미지


        public AudioSource leftAudioSource;
        public AudioSource rightAudioSource;


        private bool isInLeftCircle = false;
        private bool isInRightCircle = false;

        void Start()
        {
            // 처음엔 미리보기 이미지 전부 숨김
            if (leftPreviewImage != null) leftPreviewImage.SetActive(false);
            if (rightPreviewImage != null) rightPreviewImage.SetActive(false);
        }

        void Update()
        {
            MovePlayer();
            CheckCircleZone();
            HandleInput();
        }

        void MovePlayer()
        {
            if (playerTransform == null || leftPoint == null || rightPoint == null) return;

            // 좌우 키 입력으로 점선 경로 위에서만 이동
            float input = 0f;
            if (Keyboard.current.leftArrowKey.isPressed) input = -1f;
            if (Keyboard.current.rightArrowKey.isPressed) input = 1f;

            Vector3 newPos = playerTransform.position + Vector3.right * input * moveSpeed * Time.deltaTime;

            // 왼쪽/오른쪽 끝 클램프
            float minX = Mathf.Min(leftPoint.position.x, rightPoint.position.x);
            float maxX = Mathf.Max(leftPoint.position.x, rightPoint.position.x);
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            newPos.y = leftPoint.position.y; // Y는 고정 (점선 위)

            playerTransform.position = newPos;
        }

        void CheckCircleZone()
        {
            if (playerTransform == null) return;

            // 왼쪽 원 범위 체크
            bool inLeft = leftCircle != null &&
                Vector2.Distance(playerTransform.position, leftCircle.position) <= circleRadius;

            // 오른쪽 원 범위 체크
            bool inRight = rightCircle != null &&
                Vector2.Distance(playerTransform.position, rightCircle.position) <= circleRadius;

            // 왼쪽 원 진입/이탈
            if (inLeft != isInLeftCircle)
            {
                isInLeftCircle = inLeft;
                if (leftPreviewImage != null) leftPreviewImage.SetActive(inLeft);
                if (leftAudioSource != null)
                {
                    if (inLeft) leftAudioSource.Play();
                    else leftAudioSource.Stop();
                }


            }

            // 오른쪽 원 진입/이탈
            if (inRight != isInRightCircle)
            {
                isInRightCircle = inRight;
                if (rightPreviewImage != null) rightPreviewImage.SetActive(inRight);
                if (rightAudioSource != null)
                {
                    if (inRight) rightAudioSource.Play();
                    else rightAudioSource.Stop();
                }
            }
        }

        void HandleInput()
        {
            // ESC → 메인 씬
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene("00. Main");
                return;
            }

            // Enter → 씬 전환
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                if (isInLeftCircle)
                    TransitionManager.GoTo("02. Tutorial", TransitionManager.TransitionType.StageToTutorial);
                else if (isInRightCircle)
                    TransitionManager.GoTo("04. New Game(Boss)", TransitionManager.TransitionType.StageToNewGame);
            }
        }
    }
}