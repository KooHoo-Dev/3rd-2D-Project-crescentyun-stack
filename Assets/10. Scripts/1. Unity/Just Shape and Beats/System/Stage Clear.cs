using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class StageClear : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                // 씬에서 StageTransition을 직접 찾아옴
                StageTransition stageTransition = FindFirstObjectByType<StageTransition>();
                
                if (stageTransition != null)
                {
                    stageTransition.StartTransition(transform);
                }
                else
                {
                    // StageTransition이 없으면 기존 방식으로 처리
                    if (MusicLoopManager.Instance != null)
                        MusicLoopManager.Instance.AdvanceToNextStage();
                }

            }
        }
    }
}