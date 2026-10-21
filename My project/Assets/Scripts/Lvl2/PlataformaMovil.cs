using UnityEngine;

public class PlataformaMovil : MonoBehaviour
{
    [Header("Límites de Movimiento (Relativos)")]
    [Tooltip("Distancia hacia la izquierda desde la posición inicial")]
    public float limiteIzquierdo = 5f;
    [Tooltip("Distancia hacia la derecha desde la posición inicial")]
    public float limiteDerecho = 5f;

    [Header("Configuración de Movimiento")]
    public float velocidad = 6f;

    [Tooltip("OPCIONAL: segundos que tarda en cruzar de un límite a otro. Si es mayor que 0, ignora 'Velocidad' y así TODAS las plataformas llegan a los límites al mismo tiempo aunque tengan distancias distintas.")]
    public float duracionTramo = 0f;

    [Header("Pausa en los Límites")]
    [Tooltip("Segundos que se queda frenada al llegar a cada límite")]
    public float tiempoDePausa = 2f;

    private Vector3 puntoInicio;
    private Vector3 posIzquierda;
    private Vector3 posDerecha;
    private Vector3 objetivoActual;

    private float velocidadReal;
    private float temporizadorPausa = 0f;

    private void Start()
    {
        puntoInicio = transform.position;
        posIzquierda = puntoInicio + Vector3.left * limiteIzquierdo;
        posDerecha = puntoInicio + Vector3.right * limiteDerecho;

        // Si ya empieza pegada al límite derecho, su primer destino es el izquierdo.
        // Así NO cuenta como "llegada" el estar ya en un límite al inicio y todas arrancan a la vez.
        if (limiteDerecho <= 0.05f && limiteIzquierdo > 0.05f)
            objetivoActual = posIzquierda;
        else
            objetivoActual = posDerecha;

        // Velocidad: fija, o calculada para que todas tarden lo mismo en cada tramo
        float distanciaTramo = Vector3.Distance(posIzquierda, posDerecha);
        velocidadReal = (duracionTramo > 0f) ? distanciaTramo / duracionTramo : velocidad;
    }

    private void Update()
    {
        // Si está en pausa, solo cuenta el tiempo y no se mueve
        if (temporizadorPausa > 0f)
        {
            temporizadorPausa -= Time.deltaTime;
            return;
        }

        // Mueve la plataforma hacia el objetivo
        transform.position = Vector3.MoveTowards(transform.position, objetivoActual, velocidadReal * Time.deltaTime);

        // Al llegar al objetivo: se frena y luego va hacia el límite contrario
        if (Vector3.Distance(transform.position, objetivoActual) < 0.05f)
        {
            objetivoActual = (objetivoActual == posDerecha) ? posIzquierda : posDerecha;
            temporizadorPausa = tiempoDePausa;
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