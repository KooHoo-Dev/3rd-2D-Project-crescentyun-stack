using Just_Shape_And_Beats;
using UnityEngine;

public class Gear : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
            PlayerController playerController = collision.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.StartCoroutine(playerController.KnockbackCoroutine(knockbackDir));
                playerController.StartCoroutine(playerController.CollisionEffect());
            }

            collision.GetComponent<PlayerHP>()?.TakeDamage();
        }
    }
}