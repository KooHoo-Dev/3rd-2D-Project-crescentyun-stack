using Just_Shape_And_Beats;
using System.Collections;
using UnityEngine;

/// <summary>
/// Just Shape & Beats - NewGame 보스 패턴 전체 흐름 관리.
/// 1. 트라이앵글 회전 + 탄막/레이저
/// 2. 삼각형 꽂기 (주먹 펀치 -> 언덕 상승, 레이저)
/// 3. 신전 생성 -> 빨간 트라이앵글 분리 -> 화면 강제 하강 + 가시
/// </summary>
public class BossController : MonoBehaviour
{
    [Header("참조")]
    public TriangleStack triangleStack;     // 삼각형 3개 스택 (회전/꽂기 공용)
    public FistPunch fist;                  // 보스 주먹
    public TempleBuilder temple;            // 신전 생성/분리
    public CameraForcedDescent camDescent;  // 화면 강제 하강
    public SpikeField spikeField;           // 가시밭

    [Header("타이밍 설정")]
    public float rotatePhaseDuration = 8f;
    public int punchCount = 3;
    public float punchInterval = 1.2f;

    void Start()
    {
        StartCoroutine(RunBossSequence());
    }

    IEnumerator RunBossSequence()
    {
        // ===== 1단계: 트라이앵글 회전 + 탄막/레이저 =====
        yield return StartCoroutine(Phase1_RotateAndShoot());

        // ===== 2단계: 삼각형 꽂기 (펀치 N회) =====
        yield return StartCoroutine(Phase2_PunchSequence());

        // ===== 3단계: 신전 생성 -> 분리 -> 강제 하강 + 가시 =====
        yield return StartCoroutine(Phase3_TempleAndDescent());

        Debug.Log("보스 패턴 1싸이클 종료");
        // 필요하면 다음 패턴/루프로 연결
    }

    IEnumerator Phase1_RotateAndShoot()
    {
        triangleStack.SetRotating(true);
        triangleStack.StartCoroutine("RandomBulletLoop"); // 무작위 탄막
        //triangleStack.FireLaserBurst();                   // 레이저 발사

        yield return new WaitForSeconds(rotatePhaseDuration);

        triangleStack.SetRotating(false);
        triangleStack.StopCoroutine("RandomBulletLoop");
    }

    IEnumerator Phase2_PunchSequence()
    {
        for (int i = 0; i < punchCount; i++)
        {
            yield return StartCoroutine(fist.PunchOnce());

            // 주먹이 꽂힐 때마다: 레이저 발사 + 삼각형이 안으로 들어감 + 언덕 상승
            //triangleStack.FireLaserBurst();
            triangleStack.SinkOneLevel();
            yield return StartCoroutine(temple.RaiseHill());

            yield return new WaitForSeconds(punchInterval);
        }
    }

    IEnumerator Phase3_TempleAndDescent()
    {
        // 주먹이 완전히 들어가면 신전 생성
        yield return StartCoroutine(temple.BuildTemple());

        // 신전에서 빨간 트라이앵글 분리, 새 신전을 만들고 레이저
        yield return StartCoroutine(temple.SeparateRedTriangleAndRebuild());

        // 신전이 다 생기면 화면이 강제로 내려가며 가시 상승
        yield return StartCoroutine(camDescent.ForceDescend());
        spikeField.RiseSpikes();
    }
}