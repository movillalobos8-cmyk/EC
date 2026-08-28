using UnityEngine;

public class ProyectilEnemigo : MonoBehaviour
{
    public int danio = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Jugador"))
        {
            VidaJugador vida = collision.GetComponent<VidaJugador>();
            if (vida != null)
            {
                vida.RecibirDanio(danio, transform.position);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Piso"))
        {
            Destroy(gameObject);
        }
    }
}