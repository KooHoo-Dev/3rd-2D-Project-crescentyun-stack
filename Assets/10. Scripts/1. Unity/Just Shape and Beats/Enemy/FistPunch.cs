using System.Collections;
using UnityEngine;

/// <summary>
/// 보스 주먹이 내려와서 삼각형 스택을 꽂는 동작.
/// 한 번 펀치할 때마다 PunchOnce()를 호출.
/// </summary>
public class FistPunch : MonoBehaviour
{
    public Transform fistTransform;
    public Vector3 restPosition;   // 평상시 위치
    public Vector3 punchPosition;  // 꽂는 위치 (아래)

    public float windUpDuration = 0.25f;  // 들어올리는 시간(예고)
    public float punchDuration = 0.15f;   // 내려찍는 시간(빠르게)
    public float returnDuration = 0.3f;   // 복귀 시간

    public AnimationCurve punchEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public IEnumerator PunchOnce()
    {
        // 살짝 들어올리며 예고
        yield return Move(restPosition, restPosition + Vector3.up * 0.3f, windUpDuration);

        // 빠르게 내려찍기
        yield return Move(fistTransform.position, punchPosition, punchDuration);

        // 꽂힌 상태 잠깐 유지
        yield return new WaitForSeconds(0.1f);

        // 복귀
        yield return Move(punchPosition, restPosition, returnDuration);
    }

    IEnumerator Move(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float pct = punchEase.Evaluate(Mathf.Clamp01(elapsed / duration));
            fistTransform.position = Vector3.Lerp(from, to, pct);
            yield return null;
        }
        fistTransform.position = to;
    }
}
