using UnityEngine;
using Just_Shape_And_Beats;

/// <summary>
/// 레이저: 이미 만들어둔 "Obstacle_Raser" 프리팹을 발사 위치/각도/길이에 맞게
/// Instantiate해서 쏨. 경고(깜빡임) -> 실체화(충돌 활성화) 로직은
/// 프리팹에 붙어있는 Obstacle.cs가 알아서 처리함.
/// </summary>
public class LaserBeam : MonoBehaviour
{
    [Header("발사 기준점 (보통 뒤쪽 오브젝트)")]
    public Transform origin;

    [Header("레이저 프리팹 (Obstacle 스크립트가 붙은 그 프리팹)")]
    public GameObject laserPrefab;

    [Header("프리팹 원본 가로 길이 (localScale=1일 때, 유닛 단위)")]
    public float baseSpriteLength = 1f;

    [Header("피벗 설정 (프리팹 피벗이 왼쪽 끝이면 false, 가운데면 true)")]
    public bool pivotIsCenter = false;

    [Header("방향/길이")]
    public Vector2 direction = Vector2.down;
    public float length = 20f;
    public float thickness = 1f; // localScale.y

    public void Fire()
    {
        if (laserPrefab == null) return;

        Vector3 start = origin != null ? origin.position : transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Vector3 spawnPos = start;
        if (pivotIsCenter)
        {
            spawnPos = start + (Vector3)(direction.normalized * (length * 0.5f));
        }

        GameObject instance = Instantiate(
            laserPrefab,
            spawnPos,
            Quaternion.Euler(0, 0, angle));

        float scaleX = baseSpriteLength > 0f ? length / baseSpriteLength : 1f;
        instance.transform.localScale = new Vector3(scaleX, thickness, 1f);

        // Obstacle 스크립트의 경고/활성 시간만큼 지나면 자동 파괴
        Obstacle ob = instance.GetComponent<Obstacle>();
        if (ob != null)
        {
            float totalDuration = ob.warningDuration + ob.activeDuration;
            Destroy(instance, totalDuration);
        }
    }
}