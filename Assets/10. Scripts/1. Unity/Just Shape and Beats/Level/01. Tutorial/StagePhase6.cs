using UnityEngine;

namespace Just_Shape_And_Beats
{
    public class StagePhase6 : StagePhase2
    {
        [Header("톱니바퀴 기둥")]
        public GameObject gearWall; // 씬에 미리 만들어둔 톱니바퀴 기둥 오브젝트
        public GameObject gearWall2;
        protected override void OnEnable()
        {
            base.OnEnable();

            // 페이즈 6 진입 시 톱니바퀴 켜기
            if (gearWall != null)
                gearWall.SetActive(true);
            if (gearWall2 != null)
                gearWall2.SetActive(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // 페이즈 7 진입 시 톱니바퀴 끄기
            if (gearWall != null)
                gearWall.SetActive(false);
            if (gearWall2 != null)
                gearWall2.SetActive(true);
        }
    }
}