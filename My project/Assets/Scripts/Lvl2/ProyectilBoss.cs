using UnityEngine;

public class ProyectilBoss : MonoBehaviour
{
    [Header("Ajustes del Proyectil")]
    public int dano = 1; // Resta 1 corazón exacto
    public float tiempoDeVida = 5f;

    [Header("Ajuste del Sprite")]
    [Tooltip("Márcalo si la imagen de tu bala apunta hacia ARRIBA por defecto")]
    public bool elSpriteApuntaHaciaArriba = false;

    private Vector2 direccionMovimiento;
    private float velocidadMovimiento;
    private bool destruyendose = false;

    public void Inicializar(Vector2 direccion, float velocidad, GameObject creador)
    {
        direccionMovimiento = direccion.normalized;
        velocidadMovimiento = velocidad;

        // Rotación mirando al jugador
        float angulo = Mathf.Atan2(direccionMovimiento.y, direccionMovimiento.x) * Mathf.Rad2Deg;
        if (elSpriteApuntaHaciaArriba)
        {
            transform.rotation = Quaternion.Euler(0, 0, angulo - 90f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, angulo);
        }

        Destroy(gameObject, tiempoDeVida);
    }

    private void Update()
    {
        transform.Translate(direccionMovimiento * velocidadMovimiento * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (destruyendose) return;

        // Ignora al propio Boss, otros disparos o la hitbox extra
        if (collision.CompareTag("Boss") || 
            collision.GetComponent<GusanoBoss>() != null || 
            collision.GetComponent<ProyectilBoss>() != null ||
            collision.GetComponent<HitboxSueloBoss>() != null) 
        {
            return;
        }

        // Toca al jugador
        if (collision.CompareTag("Player"))
        {
            destruyendose = true;

            VidaJugador vida = collision.GetComponent<VidaJugador>();
            if (vida != null)
            {
                vida.RecibirDanio(1, transform.position); // Fuerza enviar 1 de daño siempre
            }

            Destroy(gameObject);
            return;
        }

        // Toca pared/suelo
        destruyendose = true;
        Destroy(gameObject);
    }
}