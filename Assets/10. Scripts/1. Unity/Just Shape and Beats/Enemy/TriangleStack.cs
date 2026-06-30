using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Just_Shape_And_Beats
{

    /// <summary>
    /// 겹쳐있는 삼각형 3개를 관리.
    /// - 회전하며 무작위 탄막 발사
    /// - 레이저 발사 (FireLaserBurst)
    /// - 펀치할 때마다 한 단계씩 안으로 들어감 (SinkOneLevel)
    /// </summary>
    public class TriangleStack : MonoBehaviour
    {
        [Header("삼각형")]
        public Transform triangleRoot;

        [Header("회전")]
        public float rotateSpeed = 60f; // deg/sec

        [Header("탄막")]
        public GameObject bulletPrefab;
        public Transform bulletSpawnPoint;
        public float bulletInterval = 0.15f;
        public float bulletSpeedMin = 3f;
        public float bulletSpeedMax = 6f;

        [Header("레이저")]
        public GameObject layserPrefab;
        public Transform layserSpawnPoint;
        public LaserBeam[] lasers; // 


        [Header("들어가는 애니메이션")]
        public float sinkDistance = 0.6f;
        public float sinkDuration = 0.2f;

        bool isRotating = false;
        int sunkLevels = 0;

        void Update()
        {
            if (!isRotating || triangleRoot == null) return;
            triangleRoot.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
        }

        public void SetRotating(bool value)
        {
            isRotating = value;
        }

        /// <summary>
        /// 회전하면서 무작위 방향/속도로 탄막을 계속 뿌림.
        /// BossController에서 StartCoroutine("RandomBulletLoop")로 호출.
        /// </summary>
        IEnumerator RandomBulletLoop()
        {
            while (true)
            {
                SpawnRandomBullet();
                yield return new WaitForSeconds(bulletInterval);
            }
        }

        IEnumerator RandomLaserLoop()
        {
            while (true)
            {
                SpawnRandomLaser();
                yield return new WaitForSeconds(bulletInterval);
            }
        }
        
        void SpawnRandomLaser()
        {
            if (layserPrefab == null || layserSpawnPoint == null) return;
            float angle = Random.Range(0f, 360f);
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);
            GameObject layser = Instantiate(layserPrefab, layserSpawnPoint.position, rot);

        }


        void SpawnRandomBullet()
        {
            if (bulletPrefab == null || bulletSpawnPoint == null) return;

            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            float angle = Random.Range(0f, 360f);
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            float speed = Random.Range(bulletSpeedMin, bulletSpeedMax);

            var barrage = bullet.GetComponent<Barrage>();
            if (barrage != null)
            {
                barrage.Init(dir, speed);
            }
        }

        

        /// <summary>
        /// 주먹이 꽂힐 때마다 삼각형들이 한 단계 안으로(아래로) 들어가는 연출.
        /// </summary>
        public void SinkOneLevel()
        {
            sunkLevels++;
            StartCoroutine(SinkAnimation());
        }

        IEnumerator SinkAnimation()
        {
            if (triangleRoot == null) yield break;

            Vector3 start = triangleRoot.position;
            Vector3 end = start + Vector3.down * sinkDistance;

            float elapsed = 0f;
            while (elapsed < sinkDuration)
            {
                elapsed += Time.deltaTime;
                float pct = elapsed / sinkDuration;
                triangleRoot.position = Vector3.Lerp(start, end, pct);
                yield return null;
            }

            triangleRoot.position = end;
        }
    }
}