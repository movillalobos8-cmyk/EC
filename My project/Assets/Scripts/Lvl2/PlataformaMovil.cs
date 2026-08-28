using UnityEngine;

public class PlataformaMovil : MonoBehaviour
{
    [Header("Límites de Movimiento (Relativos)")]
    [Tooltip("Distancia hacia la izquierda desde la posición inicial")]
    public float limiteIzquierdo = 5f;
    [Tooltip("Distancia hacia la derecha desde la posición inicial")]
    public float limiteDerecho = 5f;

    [Header("Configuración de Movimiento")]
    public float velocidad = 3f;

    private Vector3 puntoInicio;
    private Vector3 posIzquierda;
    private Vector3 posDerecha;
    private Vector3 objetivoActual;

    private void Start()
    {
        puntoInicio = transform.position;
        posIzquierda = puntoInicio + Vector3.left * limiteIzquierdo;
        posDerecha = puntoInicio + Vector3.right * limiteDerecho;

        objetivoActual = posDerecha;
    }

    private void Update()
    {
        // Mueve la plataforma hacia el objetivo
        transform.position = Vector3.MoveTowards(transform.position, objetivoActual, velocidad * Time.deltaTime);

        // Si llega al límite derecho, cambia a la izquierda
        if (Vector3.Distance(transform.position, posDerecha) < 0.05f)
        {
            objetivoActual = posIzquierda;
        }
        // Si llega al límite izquierdo, cambia a la derecha
        else if (Vector3.Distance(transform.position, posIzquierda) < 0.05f)
        {
            objetivoActual = posDerecha;
        }
    }

    // Dibujar las líneas rojas visuales en la vista Scene (Igual a la Cámara)
    private void OnDrawGizmosSelected()
    {
        Vector3 centro = Application.isPlaying ? puntoInicio : transform.position;

        Vector3 pIzq = centro + Vector3.left * limiteIzquierdo;
        Vector3 pDer = centro + Vector3.right * limiteDerecho;

        // Línea roja horizontal que une ambos límites
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pIzq, pDer);

        // Líneas verticales rojas (indicadores de tope igual que la cámara)
        Gizmos.DrawLine(pIzq + Vector3.up * 1f, pIzq + Vector3.down * 1f);
        Gizmos.DrawLine(pDer + Vector3.up * 1f, pDer + Vector3.down * 1f);
    }

    // --- EMPARENTAR AL JUGADOR PARA QUE NO SE RESBALE ---

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}