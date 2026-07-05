using UnityEngine;

public class TriggerTest : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"TriggerTest 충돌: {collision.gameObject.name}");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"TriggerTest 물리충돌: {collision.gameObject.name}");
    }
}