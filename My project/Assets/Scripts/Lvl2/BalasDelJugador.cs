using UnityEngine;

public class BalasDelJugador : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 15f;
    public float tiempoDeVida = 3f;

    [Header("Orientación del Sprite")]
    public float anguloSprite = 90f;

    private bool yaPego = false;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, anguloSprite);
        Destroy(gameObject, tiempoDeVida);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * velocidad * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (yaPego) return;

        // 1. Ignorar al jugador y a sus disparos
        if (collision.CompareTag("Player") || collision.CompareTag("Jugador")) return;

        // 2. Buscar al Jefe (GusanoBoss) en el objeto golpeado o en sus padres
        GusanoBoss jefe = collision.GetComponent<GusanoBoss>();
        if (jefe == null) jefe = collision.GetComponentInParent<GusanoBoss>();

        if (jefe != null)
        {
            yaPego = true;
            jefe.RecibirDano(1); // Aplica 1 de daño por cada bala
            Destroy(gameObject);
            return;
        }

        // 3. Destruir si choca contra el escenario (Piso o Pared)
        if (collision.CompareTag("Piso") || collision.CompareTag("Pared"))
        {
            yaPego = true;
            Destroy(gameObject);
        }
    }
}