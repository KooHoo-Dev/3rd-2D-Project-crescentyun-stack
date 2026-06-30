using System.Collections;
using UnityEngine;

/// <summary>
/// - RaiseHill: 펀치할 때마다 바닥(언덕)이 솟아오름
/// - BuildTemple: 주먹이 완전히 들어가면 신전 구조물 생성
/// - SeparateRedTriangleAndRebuild: 신전에서 빨간 트라이앵글이 분리되어 위로 떠오르고,
///   다시 새로운 신전을 만들며 레이저를 쏨
/// </summary>
public class TempleBuilder : MonoBehaviour
{
    [Header("언덕")]
    public Transform hill;          // 바닥 언덕 스프라이트/메쉬
    public float hillRiseAmount = 1f;
    public float hillRiseDuration = 0.4f;

    [Header("신전")]
    public GameObject templePrefab;     // 흰색 신전 구조물
    public Transform templeSpawnPoint;

    [Header("빨간 트라이앵글")]
    public GameObject redTrianglePrefab;
    public Transform redTriangleStartPoint; // 신전 위치
    public Transform redTriangleEndPoint;   // 분리되어 떠오를 위치 (화면 위쪽)
    public float separateDuration = 1f;

    [Header("새 신전(분리 후)")]
    public GameObject newTemplePrefab;
    public Transform newTempleSpawnPoint;

    [Header("레이저")]
    public LaserBeam redTriangleLaser;

    GameObject currentTemple;
    GameObject currentRedTriangle;

    public IEnumerator RaiseHill()
    {
        if (hill == null) yield break;

        Vector3 start = hill.position;
        Vector3 end = start + Vector3.up * hillRiseAmount;
        float elapsed = 0f;

        while (elapsed < hillRiseDuration)
        {
            elapsed += Time.deltaTime;
            hill.position = Vector3.Lerp(start, end, elapsed / hillRiseDuration);
            yield return null;
        }
        hill.position = end;
    }

    public IEnumerator BuildTemple()
    {
        if (templePrefab != null && templeSpawnPoint != null)
        {
            currentTemple = Instantiate(templePrefab, templeSpawnPoint.position, Quaternion.identity);
            // 신전이 솟아오르는 연출 (선택)
            yield return ScaleUp(currentTemple.transform, 0.5f);
        }
    }

    public IEnumerator SeparateRedTriangleAndRebuild()
    {
        // 1) 신전에서 빨간 트라이앵글 분리
        if (redTrianglePrefab != null)
        {
            currentRedTriangle = Instantiate(
                redTrianglePrefab,
                redTriangleStartPoint.position,
                Quaternion.identity);

            yield return MoveTransform(
                currentRedTriangle.transform,
                redTriangleStartPoint.position,
                redTriangleEndPoint.position,
                separateDuration);
        }

        // 2) 새로운 신전 생성
        if (newTemplePrefab != null && newTempleSpawnPoint != null)
        {
            Instantiate(newTemplePrefab, newTempleSpawnPoint.position, Quaternion.identity);
        }

        // 3) 레이저 발사
        if (redTriangleLaser != null)
        {
            redTriangleLaser.Fire();
        }

        yield return null;
    }

    IEnumerator MoveTransform(Transform t, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            t.position = Vector3.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        t.position = to;
    }

    IEnumerator ScaleUp(Transform t, float duration)
    {
        Vector3 endScale = t.localScale;
        t.localScale = Vector3.zero;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            t.localScale = Vector3.Lerp(Vector3.zero, endScale, elapsed / duration);
            yield return null;
        }
        t.localScale = endScale;
    }
}
