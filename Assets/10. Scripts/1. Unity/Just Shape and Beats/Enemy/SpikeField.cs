using System.Collections;
using UnityEngine;

/// <summary>
/// 화면 하강과 동시에 아래쪽에서 가시(스파이크)가 올라옴.
/// 플레이어가 안전 지대(원형 신전 주변)를 벗어나면 즉사 처리하는 콜라이더 포함.
/// </summary>
public class SpikeField : MonoBehaviour
{
    public Transform spikeRoot;
    public float riseAmount = 1.5f;
    public float riseDuration = 1f;
    public AnimationCurve riseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public void RiseSpikes()
    {
        StartCoroutine(RiseRoutine());
    }

    IEnumerator RiseRoutine()
    {
        Vector3 start = spikeRoot.position;
        Vector3 end = start + Vector3.up * riseAmount;

        float elapsed = 0f;
        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;
            float pct = riseCurve.Evaluate(Mathf.Clamp01(elapsed / riseDuration));
            spikeRoot.position = Vector3.Lerp(start, end, pct);
            yield return null;
        }
        spikeRoot.position = end;
    }
}
