using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(RectTransform))]
public class ControlNaveLvl3 : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Velocidad de movimiento de la nave.")]
    public float velocidad = 500f;

    [Header("Límites de Pantalla (Opcional)")]
    public bool limitarMovimiento = true;
    public float limiteX = 400f;

    private Rigidbody2D rb;
    private RectTransform rectTransform;
    private float entradaX;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rectTransform = GetComponent<RectTransform>();

        // Configuración requerida para que no rote al chocar
        rb.freezeRotation = true;
        rb.gravityScale = 0f; // Evitar que caiga por gravedad
    }

    private void Update()
    {
        // Capturar la entrada del teclado en Update para no perder pulsaciones
        entradaX = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        if (rb == null || rectTransform == null) return;

        // 1. Calcular el desplazamiento basado en físicas
        Vector2 posicionActual = rb.position;
        Vector2 nuevaPosicion = posicionActual + new Vector2(entradaX * velocidad * Time.fixedDeltaTime, 0f);

        // 2. Aplicar límites de pantalla en X
        if (limitarMovimiento)
        {
            nuevaPosicion.x = Mathf.Clamp(nuevaPosicion.x, -limiteX, limiteX);
        }

        // 3. Mover con el motor de físicas (respetando Colliders y colisiones sólidas)
        rb.MovePosition(nuevaPosicion);
    }
}