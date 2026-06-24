using UnityEngine;

public class BloodProjectile : MonoBehaviour
{
    public float velocidad = 8f;

    private Vector2 direccion;

    public void SetDirection(Vector2 nuevaDireccion)
    {
        direccion = nuevaDireccion.normalized;

        float angle = Mathf.Atan2(
            direccion.y,
            direccion.x
        ) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(
            0,
            0,
            angle + 180f
        );
    }

    void Update()
    {
        transform.position +=
            (Vector3)(direccion * velocidad * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(
            "Choqué con: " +
            other.name +
            " | Root: " +
            other.transform.root.name +
            " | Tag: " +
            other.tag
        );

        // IGNORAR ALTEPLAZA
        if (other.GetComponent<Alteplaza>() != null)
        {
            Debug.Log("Ignorando Alteplaza");
            return;
        }

        // IGNORAR CUALQUIER PARTE DEL JEFE
        if (other.transform.root.CompareTag("Boss"))
        {
            Debug.Log("Ignorando jefe");
            return;
        }

        // DAÑO AL PLAYER
        if (other.CompareTag("Player"))
        {
            PlayerHealth health =
                other.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage();
            }
        }

        Destroy(gameObject);
    }
}