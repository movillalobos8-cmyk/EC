using UnityEngine;

public class MiddleHeadDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TOCÓ: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("ES EL PLAYER");

            PlayerHealth health =
                other.GetComponentInParent<PlayerHealth>();

            if (health != null)
            {
                Debug.Log("PLAYERHEALTH ENCONTRADO");
                health.TakeDamage();
            }
            else
            {
                Debug.Log("NO ENCONTRÓ PLAYERHEALTH");
            }
        }
    }
}