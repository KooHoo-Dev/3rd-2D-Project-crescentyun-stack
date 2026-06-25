using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Just_Shape_And_Beats
{
    public class MusicLoopManager : MonoBehaviour
    {
        public static MusicLoopManager Instance;

        [Header("Audio Settings")]
        public AudioSource audioSource;
        public List<AudioStageSegment> stageSegments;

        [Header("Prefabs")]
        public GameObject trianglePrefab;
        public Transform triangleSpawnPoint;

        private int currentStageIndex = 0;
        private bool isMusicStarted = false;
        private bool isTriangleSpawned = false; // 💡 현재 화면에 삼각형이 떠 있는 상태인가?

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
            audioSource.loop = false; // 우리가 수동 제어하므로 무조건 false

            // 게임 시작하자마자 첫 번째 시작용 삼각형 스폰
            isTriangleSpawned = true;
            SpawnTriangle();
            Debug.Log("[시스템] 준비 완료. 첫 번째 삼각형을 먹으면 음악이 시작됩니다.");
        }

        void Update()
        {
            // 음악이 안 켜졌거나, 모든 스테이지가 끝났다면 루프 검사를 안 함
            if (!isMusicStarted || currentStageIndex >= stageSegments.Count) return;

            AudioStageSegment currentSegment = stageSegments[currentStageIndex];

            // 💡 플레이어가 삼각형을 먹어서 '다음 단계'로 넘어가기 전까지는 
            // 화면에 삼각형이 떠 있든 말든 무조건 설정된 구간 안에서 루프를 돕니다.
            if (audioSource.time >= currentSegment.loopEndTime)
            {
                audioSource.time = currentSegment.loopStartTime;
                Debug.Log($"[오디오 루프] {currentStageIndex + 1}단계 끝자락 도달 -> 시작점({currentSegment.loopStartTime}초)으로 워프!");
            }
        }

        // 각 단계의 스포너(또는 타이머)가 "패턴 끝났다"고 신호를 주면 호출
        public void OnStagePatternFinished()
        {
            // 이미 화면에 삼각형이 있다면 중복 생성 금지
            if (isTriangleSpawned) return;

            isTriangleSpawned = true;
            SpawnTriangle();
            Debug.Log($"[시스템] {currentStageIndex + 1}단계 패턴 종료! 다음 단계로 가기 위한 삼각형이 생성되었습니다.");
        }

        // 플레이어가 삼각형을 먹었을 때 호출되는 함수
        public void AdvanceToNextStage()
        {
            // 1. 최초로 시작 삼각형을 먹었을 때
            if (!isMusicStarted)
            {
                isMusicStarted = true;
                isTriangleSpawned = false; // 삼각형 먹었으니 플래그 해제
                audioSource.Play();

                ApplyStageSettings();
                Debug.Log("[시스템] 게임 시작! 1단계 음악 구간 진행 중...");
                return;
            }

            // 2. 2~5단계 진행 중 삼각형을 먹었을 때
            isTriangleSpawned = false; // 삼각형 먹었으니 플래그 해제
            currentStageIndex++;

            // 5단계까지 다 깨고 마지막 삼각형을 먹었다면 보스 씬으로!
            if (currentStageIndex >= stageSegments.Count)
            {
                Debug.Log("[시스템] 모든 튜토리얼 통과! 보스 씬으로 씬을 전환합니다.");
                SceneManager.LoadScene("Scene_Boss");
                return;
            }

            // 다음 단계 스포너 켜기
            ApplyStageSettings();

            // 음악 시간을 다음 단계의 시작 영역으로 강제 강제 이동
            AudioStageSegment nextSegment = stageSegments[currentStageIndex];
            audioSource.time = nextSegment.loopStartTime;

            Debug.Log($"[시스템] {currentStageIndex + 1}단계 진입 성공! 음악 구간 설정: {nextSegment.loopStartTime}초 ~ {nextSegment.loopEndTime}초");
        }

        void ApplyStageSettings()
        {
            // 모든 스포너 일단 끄기
            foreach (var segment in stageSegments)
            {
                if (segment.spawner != null) segment.spawner.SetActive(false);
            }

            // 현재 단계 스포너만 켜기
            AudioStageSegment currentSegment = stageSegments[currentStageIndex];
            if (currentSegment.spawner != null)
            {
                currentSegment.spawner.SetActive(true);
                Debug.Log($"[스포너] {currentStageIndex + 1}단계 스포너({currentSegment.spawner.name})가 활성화되었습니다.");
            }
            else
            {
                // 1단계처럼 스포너가 없는 평화로운 구간이라면, 10초 버틴 후 자동으로 삼각형이 나오게 설정
                Debug.Log($"[타이머] {currentStageIndex + 1}단계는 평화 구간입니다. 10초 후 삼각형이 나옵니다.");
                Invoke("OnStagePatternFinished", 10f);
            }
        }

        public void SpawnTriangle()
        {
            Instantiate(trianglePrefab, triangleSpawnPoint.position, Quaternion.identity);
        }
    }
}
