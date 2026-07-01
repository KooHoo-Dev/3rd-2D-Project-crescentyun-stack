using System.Collections;
using UnityEngine;

/// <summary>
/// 보스가 삼각형을 잡고 찍은 뒤, 주먹질을 반복해서 삼각형을 언덕 안으로 꽂아넣는 패턴.
///
/// 흐름:
/// 1. 보스가 삼각형 위치로 내려와 잡음 (GrabTriangle)
/// 2. 보스 + 삼각형 같이 아래로 찍음, 언덕 상승 (StabDown)
/// 3. 보스가 위로 올라가며 주먹 준비 + 레이저 발사 (WindUp)
/// 4. 보스가 빠르게 내려찍음, 삼각형 한 단계 더 들어감 (PunchDown)
/// 5. punchCount만큼 3~4 반복
/// 6. 보스가 위로 복귀
/// </summary>
public class FistPunch : MonoBehaviour
{
    [Header("보스 오브젝트")]
    public Transform bossTransform;         // 보스 전체 (보스 스프라이트)
    public Transform triangleTransform;     // 삼각형 스프라이트

    [Header("위치 포인트")]
    public Transform bossRestPoint;         // 보스 평상시 위치 (위)
    public Transform grabPoint;             // 보스가 삼각형 잡는 위치
    public Transform stabPoint;             // 처음 찍는 위치 (언덕 위)
    public Transform windUpPoint;           // 주먹 준비 위치 (stabPoint보다 위)

    [Header("삼각형 꽂힘")]
    public int punchCount = 3;              // 총 주먹질 횟수
    public float sinkPerPunch = 0.5f;       // 주먹질 1번당 삼각형이 내려가는 거리

    [Header("타이밍")]
    public float grabDuration = 0.5f;       // 보스가 삼각형 위치로 내려오는 시간
    public float stabDuration = 0.3f;       // 처음 찍는 시간
    public float windUpDuration = 0.4f;     // 주먹 준비(위로 올라가는) 시간
    public float punchDuration = 0.15f;     // 내려찍는 시간 (빠를수록 타격감)
    public float punchHoldDuration = 0.1f;  // 찍고 잠깐 유지하는 시간
    public float returnDuration = 0.6f;     // 마지막에 보스가 복귀하는 시간

    [Header("레이저 (주먹 준비 시 발사)")]
    public LaserBeam[] lasers;

    [Header("언덕")]
    public TempleBuilder templeBuilder;

    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public IEnumerator PunchSequence()
    {
        // ===== 1. 보스가 삼각형 위치로 내려와 잡음 =====
        yield return MoveObject(bossTransform, bossTransform.position, grabPoint.position, grabDuration);

        // 삼각형을 보스 자식으로 붙여서 같이 움직이게 함
        triangleTransform.SetParent(bossTransform);

        // ===== 2. 보스 + 삼각형 같이 아래로 찍음 =====
        yield return MoveObject(bossTransform, bossTransform.position, stabPoint.position, stabDuration);

        // 언덕 상승
        if (templeBuilder != null)
            StartCoroutine(templeBuilder.RaiseHill());

        yield return new WaitForSeconds(0.2f);

        // 삼각형을 보스에서 분리 (삼각형은 찍힌 자리에 고정)
        triangleTransform.SetParent(null);

        // ===== 3~4. 주먹질 반복 =====
        Vector3 currentPunchTarget = stabPoint.position;

        for (int i = 0; i < punchCount; i++)
        {
            // 주먹 준비: 보스가 위로 올라감 + 레이저 발사
            yield return MoveObject(bossTransform, bossTransform.position, windUpPoint.position, windUpDuration);

            FireLasers();

            yield return new WaitForSeconds(0.1f);

            // 주먹질: 보스가 빠르게 내려찍음
            yield return MoveObject(bossTransform, bossTransform.position, currentPunchTarget, punchDuration);

            // 삼각형 한 단계 더 아래로 꽂힘
            currentPunchTarget += Vector3.down * sinkPerPunch;
            triangleTransform.position += Vector3.down * sinkPerPunch;

            yield return new WaitForSeconds(punchHoldDuration);
        }

        // ===== 5. 보스 복귀 =====
        yield return MoveObject(bossTransform, bossTransform.position, bossRestPoint.position, returnDuration);
    }

    void FireLasers()
    {
        if (lasers == null) return;
        foreach (var laser in lasers)
        {
            if (laser != null) laser.Fire();
        }
    }

    IEnumerator MoveObject(Transform target, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float pct = moveCurve.Evaluate(Mathf.Clamp01(elapsed / duration));
            target.position = Vector3.Lerp(from, to, pct);
            yield return null;
        }
        target.position = to;
    }
}