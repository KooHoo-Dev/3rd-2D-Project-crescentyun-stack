using System.Collections;
using UnityEngine;

/// <summary>
/// 신전 패턴 등에 대응하여 카메라(화면)를 정밀하게 이동시키는 연출용 스크립트입니다.
/// </summary>
public class CameraForcedDescent : MonoBehaviour
{
    public Transform cameraTransform;
    public AnimationCurve descendCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    /// <summary>
    /// [핵심 수정] 타겟의 목표 Y축 좌표로 카메라를 정밀하고 부드럽게 올립니다.
    /// </summary>
    /// <param name="targetY">목표 Y 좌표</param>
    /// <param name="duration">이동에 걸리는 시간</param>
    public IEnumerator MoveToYPosition(float targetY, float duration)
    {
        if (cameraTransform == null) yield break;

        Vector3 start = cameraTransform.position;
        // X, Z 좌표는 유지하고 Y 좌표만 다음 신전/삼각형 높이에 맞춥니다.
        Vector3 end = new Vector3(start.x, targetY, start.z);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 지정된 시간(duration) 동안 커브를 반영해 이동
            float pct = descendCurve.Evaluate(Mathf.Clamp01(elapsed / duration));
            cameraTransform.position = Vector3.Lerp(start, end, pct);
            yield return null;
        }
        cameraTransform.position = end;
    }
}
  