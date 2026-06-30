using Study_ActionPlatformer;
using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class StageClear : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            
            if (collision.CompareTag("Player"))
            {
                
               

                // 음악 루프 매니저에게 다음 단계로 봉인 해제하라고 신호 전달
                if (MusicLoopManager.Instance != null)
                {

                    MusicLoopManager.Instance.AdvanceToNextStage();
                }

                Destroy(gameObject);
            }
        }
    }
}
