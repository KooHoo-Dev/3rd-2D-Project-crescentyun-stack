using System.Collections;
using UnityEngine;

/// <summary>
/// 신전이 다 생기면 화면(카메라)이 강제로 아래로 내려감.
/// 플레이어를 압박하는 연출 (리듬에 맞춰 카메라 워크).
/// </summary>
public class CameraForcedDescent : MonoBehaviour
{
    public Transform cameraTransform;
    public float descendDistance = 4f;
    public float descendDuration = 1.5f;
    public AnimationCurve descendCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public IEnumerator ForceDescend()
    {
        Vector3 start = cameraTransform.position;
        Vector3 end = start + Vector3.down * descendDistance;

        float elapsed = 0f;
        while (elapsed < descendDuration)
        {
            elapsed += Time.deltaTime;
            float pct = descendCurve.Evaluate(Mathf.Clamp01(elapsed / descendDuration));
            cameraTransform.position = Vector3.Lerp(start, end, pct);
            yield return null;
        }
        cameraTransform.position = end;
    }
}
