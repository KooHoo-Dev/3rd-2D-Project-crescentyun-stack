using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Just_Shape_And_Beats
{
    [System.Serializable]
    public class AudioStageSegment
    {
        public int stageNumber;       // 스테이지 번호 (1~5)
        public float loopStartTime;   // 루프 시작 시간 (초)
        public float loopEndTime;     // 루프 끝 시간 (초)
        public GameObject spawner;    // 이 단계에서 켜질 장애물 스포너 오브젝트
    }

    public class Music : MonoBehaviour
    {
        public static Music Instance;

        [Header("Audio Settings")]
        public AudioSource audioSource;
        public List<AudioStageSegment> stageSegments; // 인스펙터에서 5개 단계를 세팅할 리스트

        [Header("Prefabs")]
        public GameObject trianglePrefab;   
        public Transform triangleSpawnPoint;

        private int currentStageIndex = 0; 
        private bool isWaitingForNextStage = false;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
            audioSource.loop = false; // 수동 제어하므로 유니티 자체 루프는 끕니다.

            
            ApplyStageSettings();
        }

        void Update()
        {
            if (currentStageIndex >= stageSegments.Count) return;

            AudioStageSegment currentSegment = stageSegments[currentStageIndex];

            // 플레이어가 삼각형을 '먹기 전' 상태라면 설정된 구간 안에서 무한 루프
            if (!isWaitingForNextStage)
            {
                if (audioSource.time >= currentSegment.loopEndTime)
                {
                    audioSource.time = currentSegment.loopStartTime;
                }
            }
        }

        // [중요] 각 단계의 스포너가 "미션 완료(시간 버티기 끝)"를 선언하면 호출됩니다.
        public void OnStagePatternFinished()
        {
            // 이미 대기 중이면 중복 실행 방지
            if (isWaitingForNextStage) return;

            // 삼각형을 화면에 띄워줍니다.
            Instantiate(trianglePrefab, triangleSpawnPoint.position, Quaternion.identity);
            Debug.Log($"[시스템] {currentStageIndex + 1}단계 완료! 삼각형을 획득하세요.");
        }

        // 플레이어가 삼각형을 먹었을 때 호출되는 함수
        public void AdvanceToNextStage()
        {
            currentStageIndex++;

            // 5단계까지 다 깨고 마지막 삼각형을 먹었다면 보스 씬으로!
            if (currentStageIndex >= stageSegments.Count)
            {
                Debug.Log("모든 튜토리얼 완료! 보스 씬으로 이동합니다.");
                SceneManager.LoadScene("Scene_Boss");
                return;
            }

            // 루프 제한을 풀고 다음 단계 세팅 적용
            isWaitingForNextStage = false;
            ApplyStageSettings();

            // ⚠️ 삼각형을 먹은 시점이 루프 끝 시간보다 뒤에 있다면, 자연스러운 진행을 위해 다음 단계 시작 시간으로 점프시킵니다.
            AudioStageSegment nextSegment = stageSegments[currentStageIndex];
            if (audioSource.time < nextSegment.loopStartTime || audioSource.time > nextSegment.loopEndTime)
            {
                audioSource.time = nextSegment.loopStartTime;
            }

            Debug.Log($"[시스템] {currentStageIndex + 1}단계 시작! 음악 구간: {nextSegment.loopStartTime}초 ~ {nextSegment.loopEndTime}초");
        }

        // 단계가 바뀔 때 스포너를 교체하고 세팅하는 함수
        void ApplyStageSettings()
        {
            // 1. 모든 스포너 원격 종료
            foreach (var segment in stageSegments)
            {
                if (segment.spawner != null) segment.spawner.SetActive(false);
            }

            // 2. 현재 단계의 스포너만 활성화
            AudioStageSegment currentSegment = stageSegments[currentStageIndex];
            if (currentSegment.spawner != null)
            {
                currentSegment.spawner.SetActive(true);
            }
            else
            {
                // 1단계처럼 스포너가 없는 평화로운 구간이라면, 일정 시간(예: 5초) 후 자동으로 삼각형이 나오게 처리
                Invoke("OnStagePatternFinished", 5f);
            }
        }
    }
}
