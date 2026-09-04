using UnityEngine;

public class HitboxSueloBoss : MonoBehaviour
{
    [Header("Configuración de Daño")]
    public int danoEmbestida = 1;

    private Collider2D miCollider;

    private void Awake()
    {
        miCollider = GetComponent<Collider2D>();
        DesactivarHitbox();
    }

    public void ActivarHitbox()
    {
        if (miCollider != null) miCollider.enabled = true;
    }

    public void DesactivarHitbox()
    {
        if (miCollider != null) miCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo hace daño al Jugador
        if (collision.CompareTag("Player"))
        {
            VidaJugador vida = collision.GetComponent<VidaJugador>();
            if (vida != null)
            {
                vida.RecibirDanio(danoEmbestida, transform.position);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            VidaJugador vida = collision.gameObject.GetComponent<VidaJugador>();
            if (vida != null)
            {
                vida.RecibirDanio(danoEmbestida, transform.position);
            }
        }
    }
}