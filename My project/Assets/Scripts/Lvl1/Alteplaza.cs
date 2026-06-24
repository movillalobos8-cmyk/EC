using UnityEngine;

public class Alteplaza : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignorar proyectiles de sangre
        if (collision.GetComponent<BloodProjectile>() != null)
            return;

        if (collision.CompareTag("Player"))
        {
            SecretariaEndManager manager = FindObjectOfType<SecretariaEndManager>();

            if (manager != null)
            {
                manager.StartEndDialogue();
            }

            Destroy(gameObject);
        }
    }
}