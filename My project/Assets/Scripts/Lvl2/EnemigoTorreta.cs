using UnityEngine;

public class EnemigoTorreta : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject prefabProyectil;
    public Transform puntoDisparo;

    [Header("Ajustes de Disparo")]
    public float velocidadProyectil = 10f;
    public float tiempoEntreDisparos = 2f;
    public float rangoDeteccion = 8f;

    private Transform jugador;
    private float cronometro;

    private void Start()
    {
        // Buscar al jugador por Tag
        GameObject objJugador = GameObject.FindGameObjectWithTag("Jugador");
        if (objJugador != null)
        {
            jugador = objJugador.transform;
        }
    }

    private void Update()
    {
        if (jugador == null) return;

        // Calcular distancia hacia el jugador
        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia <= rangoDeteccion)
        {
            cronometro += Time.deltaTime;

            if (cronometro >= tiempoEntreDisparos)
            {
                Disparar();
                cronometro = 0f;
            }
        }
    }

    private void Disparar()
    {
        if (prefabProyectil == null || puntoDisparo == null) return;

        // Crear la bala en el punto de disparo
        GameObject bala = Instantiate(prefabProyectil, puntoDisparo.position, Quaternion.identity);
        Rigidbody2D rbBala = bala.GetComponent<Rigidbody2D>();

        if (rbBala != null)
        {
            // Calcular la dirección HACIA el jugador
            Vector2 direccion = (jugador.position - puntoDisparo.position).normalized;
            rbBala.linearVelocity = direccion * velocidadProyectil;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja un círculo rojo en la ventana Scene para ver el rango
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}