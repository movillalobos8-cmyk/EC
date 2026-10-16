using UnityEngine;

/// <summary>
/// Controla el efecto parallax horizontal y vertical para un elemento o capa de fondo.
/// Permite loop infinito horizontal y restricción de altura mínima.
/// </summary>
public class ParallaxLayer : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Cámara principal que sigue al jugador. Si se deja nulo, buscará Camera.main.")]
    public Transform camara;

    [Header("Efecto Parallax Horizontal (Eje X)")]
    [Range(0f, 1f)]
    [Tooltip("0 = Se mueve junto a la cámara (muy lejano). 1 = Estático respecto al mundo.")]
    public float efectoParallaxX = 0.5f;

    [Header("Efecto Parallax Vertical (Eje Y)")]
    [Range(0f, 1f)]
    [Tooltip("Factor de profundidad al subir/bajar la cámara (0.2 a 0.5 recomendado).")]
    public float intensidadParallaxY = 0.3f;
    
    [Tooltip("Evita que la capa baje por debajo de su posición Y inicial cuando la cámara desciende.")]
    public bool noBajarDelSueloInicial = true;

    [Header("Configuración de Loop Infinito (Opcional)")]
    [Tooltip("Marcar si este objeto debe hacer loop infinito en el eje X.")]
    public bool usarLoopInfinito = true;
    [Tooltip("Cantidad total de capas idénticas alineadas en horizontal para el loop (ej: 3).")]
    public int cantidadCapasEnLoop = 3;

    private float longitudSprite;
    private Vector3 ultimaPosicionCamara;
    private float alturaInicialY;
    private float alturaInicialCamaraY;

    private void Start()
    {
        if (camara == null && Camera.main != null)
        {
            camara = Camera.main.transform;
        }

        if (camara != null)
        {
            ultimaPosicionCamara = camara.position;
            alturaInicialCamaraY = camara.position.y;
        }

        // Obtener el ancho exacto del Sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            longitudSprite = spriteRenderer.bounds.size.x;
        }

        // Registrar la altura inicial Y
        alturaInicialY = transform.position.y;
    }

    private void LateUpdate()
    {
        if (camara == null) return;

        // 1. Cálculo del delta de la cámara
        float deltaCamaraX = camara.position.x - ultimaPosicionCamara.x;
        float camaraRecorridoTotalY = camara.position.y - alturaInicialCamaraY;
        
        ultimaPosicionCamara = camara.position;

        // 2. Aplicar movimiento
        Vector3 posActual = transform.position;

        // Horizontal (Eje X)
        posActual.x += deltaCamaraX * efectoParallaxX;

        // Vertical (Eje Y)
        float nuevaY = alturaInicialY + (camaraRecorridoTotalY * (1f - intensidadParallaxY));
        if (noBajarDelSueloInicial)
        {
            nuevaY = Mathf.Max(alturaInicialY, nuevaY);
        }
        posActual.y = nuevaY;

        transform.position = posActual;

        // 3. Loop infinito en X
        if (usarLoopInfinito && longitudSprite > 0)
        {
            float diferenciaX = camara.position.x - transform.position.x;
            float umbralReposicion = longitudSprite * (cantidadCapasEnLoop / 2f);

            if (diferenciaX > umbralReposicion)
            {
                transform.position += Vector3.right * (longitudSprite * cantidadCapasEnLoop);
            }
            else if (diferenciaX < -umbralReposicion)
            {
                transform.position -= Vector3.right * (longitudSprite * cantidadCapasEnLoop);
            }
        }
    }
}