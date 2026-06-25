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
        private bool isWaitingForNextStage = false;

        //  음악이 시작되었는지 체크
        private bool isMusicStarted = false;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
            audioSource.loop = false;

            
            // 게임 시작 시 첫 번째 대기용 삼각형을 바로 화면에 띄워줍니다.
            SpawnTriangle();
        }

        void Update()
        {
            //  음악이 아직 시작되지 않았다면 루프 체크를 하지 않고 리턴
            if (!isMusicStarted) return;
            if (currentStageIndex >= stageSegments.Count) return;

            AudioStageSegment currentSegment = stageSegments[currentStageIndex];

            if (!isWaitingForNextStage)
            {
                if (audioSource.time >= currentSegment.loopEndTime)
                {
                    audioSource.time = currentSegment.loopStartTime;
                }
            }
        }

        public void OnStagePatternFinished()
        {
            if (isWaitingForNextStage) return;
            isWaitingForNextStage = true; // 다음 단계를 기다리는 상태로 변경
            SpawnTriangle();
        }

        // 플레이어가 삼각형을 먹었을 때 호출되는 함수
        public void AdvanceToNextStage()
        {
            // 최초로 삼각형을 먹었을 때의 처리
            if (!isMusicStarted)
            {
                isMusicStarted = true;
                audioSource.Play(); // 
                ApplyStageSettings(); // 1단계 설정 적용
                Debug.Log("[시스템] 첫 삼각형 획득! 음악과 1단계가 시작됩니다.");
                return; // 첫 삼각형은 단계 인덱스를 올리지 않고 여기서 끝냅니다.
            }

            // 두 번째 삼각형부터는 정상적으로 다음 단계로 진행합니다.
            currentStageIndex++;

            if (currentStageIndex >= stageSegments.Count)
            {
                Debug.Log("모든 튜토리얼 완료! 보스 씬으로 이동합니다.");
                SceneManager.LoadScene("Scene_Boss");
                return;
            }

            isWaitingForNextStage = false;
            ApplyStageSettings();

            AudioStageSegment nextSegment = stageSegments[currentStageIndex];
            if (audioSource.time < nextSegment.loopStartTime || audioSource.time > nextSegment.loopEndTime)
            {
                audioSource.time = nextSegment.loopStartTime;
            }

            Debug.Log($"[시스템] {currentStageIndex + 1}단계 시작! 음악 구간: {nextSegment.loopStartTime}초 ~ {nextSegment.loopEndTime}초");
        }

        void ApplyStageSettings()
        {
            foreach (var segment in stageSegments)
            {
                if (segment.spawner != null) segment.spawner.SetActive(false);
            }

            AudioStageSegment currentSegment = stageSegments[currentStageIndex];
            if (currentSegment.spawner != null)
            {
                currentSegment.spawner.SetActive(true);
            }
            else
            {
                // 스포너가 없는 단계(예: 1단계)라면 설정된 시간 뒤에 자동으로 다음 삼각형 스폰
                // 예를 들어 1단계를 10초 동안 유지하고 싶다면 10f를 적어줍니다.
                Invoke("OnStagePatternFinished", 10f);
            }
        }

        // 삼각형 생성을 깔끔하게 처리하기 위한 공용 함수
        public void SpawnTriangle()
        {
            Instantiate(trianglePrefab, triangleSpawnPoint.position, Quaternion.identity);
        }
    }
}

