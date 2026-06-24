using UnityEngine;

public class ObstacleKill : MonoBehaviour
{
    public GameObject explosionPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // explosión
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // obtener vida del player
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            // hacer daño
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }

            // destruir obstáculo
            Destroy(gameObject);
        }
    }
}