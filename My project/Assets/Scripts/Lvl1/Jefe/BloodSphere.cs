using UnityEngine;

public class SnakeProjectile : MonoBehaviour
{
    [Header("Velocidad horizontal")]
    public float velocidadX = 4f;

    [Header("Velocidad vertical")]
    public float velocidadY = 3f;

    [Header("Tiempo de vida")]
    public float vida = 10f;

    [HideInInspector]
    public SnakeAttackSpawner spawner;

    private float direccionVertical = 1f;

    private float limiteSuperior;
    private float limiteInferior;

    void Start()
    {
        Camera cam = Camera.main;

        limiteSuperior =
            cam.ViewportToWorldPoint(
                new Vector3(0f, 1f, 0f)
            ).y;

        limiteInferior =
            cam.ViewportToWorldPoint(
                new Vector3(0f, 0f, 0f)
            ).y;

        Destroy(gameObject, vida);
    }

    void Update()
    {
        transform.position += new Vector3(
            -velocidadX * Time.deltaTime,
            velocidadY * direccionVertical * Time.deltaTime,
            0f
        );

        if (transform.position.y >= limiteSuperior)
        {
            direccionVertical = -1f;
        }

        if (transform.position.y <= limiteInferior)
        {
            direccionVertical = 1f;
        }
    }

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.RemoverProyectil();
        }
    }

        private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.root.CompareTag("Boss"))
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            PlayerHealth health =
                other.GetComponentInParent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage();
            }

            Destroy(gameObject);
        }
    }
}