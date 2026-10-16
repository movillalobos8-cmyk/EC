using UnityEngine;

public class FondoInfinitoUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public RectTransform[] fondos; // Tus 3 o 4 RectTransforms/Images
    public Transform camara;

    [Header("Configuración de Caída Continua")]
    [Tooltip("Velocidad en Y. Positivo = hacia arriba (caída), Negativo = hacia abajo.")]
    public float velocidadCaida = 200f;

    [Header("Ajuste Exacto de Cámara (Eje Y)")]
    [Tooltip("Si es 1.0, el fondo está 100% fijo a la pantalla (cero margen). Si es 0.8, tiene un leve desfasaje.")]
    [Range(0f, 1f)]
    public float factorSeguimientoCamaraY = 1.0f; 
    public bool noBajarDelSueloInicial = true;

    [Header("Prioridad de Capa")]
    public bool forzarFrenteDePantalla = true;

    private float alturaSprite;
    private float alturaInicialCamaraY;
    private float acumuladoCaidaY = 0f;

    private void Start()
    {
        if (camara == null && Camera.main != null)
        {
            camara = Camera.main.transform;
        }

        if (camara != null)
        {
            alturaInicialCamaraY = camara.position.y;
        }

        if (forzarFrenteDePantalla)
        {
            transform.SetAsLastSibling();
        }

        if (fondos != null && fondos.Length > 0 && fondos[0] != null)
        {
            alturaSprite = fondos[0].rect.height;
        }
    }

    private void LateUpdate()
    {
        if (alturaSprite <= 0 || fondos == null || fondos.Length == 0) return;

        // 1. Desplazamiento por velocidad de caída propia
        acumuladoCaidaY += velocidadCaida * Time.deltaTime;

        // 2. Calcular exacto el movimiento de la cámara desde el inicio
        float deltaCamaraY = 0f;
        if (camara != null)
        {
            deltaCamaraY = camara.position.y - alturaInicialCamaraY;

            if (noBajarDelSueloInicial)
            {
                deltaCamaraY = Mathf.Max(0f, deltaCamaraY);
            }
        }

        // Convertir la distancia de la cámara a unidades locales de UI
        float offsetCamaraEnUI = deltaCamaraY * factorSeguimientoCamaraY * 100f;

        // Posición global base para el grupo
        float posBaseY = offsetCamaraEnUI + acumuladoCaidaY;
        float tamañoTotalColumna = alturaSprite * fondos.Length;

        // 3. Posicionar cada imagen de forma modular exacta
        for (int i = 0; i < fondos.Length; i++)
        {
            if (fondos[i] == null) continue;

            Quaternion rotOriginal = fondos[i].localRotation;

            // Alineación modular continua
            float yPos = (posBaseY + (i * alturaSprite)) % tamañoTotalColumna;

            // Mantener la cobertura en la parte superior del canvas
            if (yPos > alturaSprite * (fondos.Length - 1.5f))
            {
                yPos -= tamañoTotalColumna;
            }

            Vector3 localPos = fondos[i].localPosition;
            localPos.y = yPos;

            fondos[i].localPosition = localPos;
            fondos[i].localRotation = rotOriginal;
        }
    }
}