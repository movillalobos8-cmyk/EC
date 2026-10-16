using UnityEngine;

public class SeguimientoVerticalParallax : MonoBehaviour
{
    [Header("Referencia a la Cámara")]
    public Transform camara;

    [Header("Efecto Parallax Vertical (Eje Y)")]
    [Range(0f, 1f)]
    public float efectoParallaxY = 0.5f; // 0 = Pega el fondo a la cámara | 1 = Se queda totalmente estático en el mapa

    [Header("Restricción")]
    public bool noBajarDelSueloInicial = true;

    private float alturaInicialContenedorY;
    private float alturaInicialCamaraY;

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

        // Guardamos la altura inicial donde colocaste el contenedor en el editor
        alturaInicialContenedorY = transform.position.y;
    }

    private void LateUpdate()
    {
        if (camara == null) return;

        // Cuánto ha subido o bajado la cámara desde el inicio de la escena
        float deltaCamaraY = camara.position.y - alturaInicialCamaraY;

        // Calculamos la nueva posición Y
        float nuevaY = alturaInicialContenedorY + (deltaCamaraY * efectoParallaxY);

        // Regla: NUNCA baja de la altura con la que inició
        if (noBajarDelSueloInicial)
        {
            nuevaY = Mathf.Max(alturaInicialContenedorY, nuevaY);
        }

        // Aplicamos el movimiento en Y únicamente
        Vector3 pos = transform.position;
        pos.y = nuevaY;
        transform.position = pos;
    }
}