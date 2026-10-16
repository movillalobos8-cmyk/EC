using UnityEngine;

public class ParallaxLoop3Imagenes : MonoBehaviour
{
    [Header("Referencias")]
    public Transform camara;
    public SpriteRenderer[] fondos = new SpriteRenderer[3]; // Arrastra tus 3 Sprites aquí

    [Header("Efecto Parallax Horizontal (Eje X)")]
    [Range(0f, 1f)]
    public float efectoParallaxX = 0.5f; // 0 = pegado a la cámara, 1 = estático en el nivel

    [Header("Efecto Parallax Vertical (Eje Y)")]
    [Range(0f, 1f)]
    public float intensidadParallaxY = 0.3f; // Sensación de profundidad al subir/bajar (Ej: 0.2 a 0.5)
    public bool noBajarDelSueloInicial = true; // Bloquea que el fondo baje de su posición original

    [Header("Profundidad Absoluta")]
    public float posicionZ = 10f; // Detrás del personaje (Z=0)

    private float longitudSprite;
    private Vector3 ultimaPosicionCamara;
    private float[] alturasInicialesSpritesY;
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

        // 1. Obtener la anchura exacta del sprite
        if (fondos.Length > 0 && fondos[0] != null)
        {
            longitudSprite = fondos[0].bounds.size.x;
        }

        alturasInicialesSpritesY = new float[fondos.Length];

        // 2. Posicionar los 3 fondos y memorizar su altura inicial en el mapa
        for (int i = 0; i < fondos.Length; i++)
        {
            if (fondos[i] != null)
            {
                fondos[i].sortingOrder = -100;
                
                // Guardar la altura Y original del inspector
                alturasInicialesSpritesY[i] = fondos[i].transform.position.y;

                float posX = camara.position.x + ((i - 1) * longitudSprite);
                fondos[i].transform.position = new Vector3(posX, alturasInicialesSpritesY[i], posicionZ);
            }
        }
    }

    private void LateUpdate()
    {
        if (camara == null || longitudSprite <= 0) return;

        // --- CÁLCULO DE MOVIMIENTO DE LA CÁMARA ---
        
        // Desplazamiento X de este frame
        float deltaCamaraX = camara.position.x - ultimaPosicionCamara.x;
        ultimaPosicionCamara = camara.position;

        // Cuánto ha subido o bajado la cámara en TOTAL desde que inició el juego
        float camaraRecorridoTotalY = camara.position.y - alturaInicialCamaraY;


        // --- MOVER LOS FONDOS ---
        for (int i = 0; i < fondos.Length; i++)
        {
            if (fondos[i] == null) continue;

            Vector3 posActual = fondos[i].transform.position;

            // 1. Movimiento Horizontal (Eje X) con Parallax
            posActual.x += deltaCamaraX * efectoParallaxX;

            // 2. Movimiento Vertical (Eje Y):
            // La cámara se mueve Y -> El fondo debe moverse Y (100%) MENOS la diferencia de Parallax.
            // Esto garantiza que el fondo SIEMPRE SIGA a la cámara suba o baje.
            float nuevaY = alturasInicialesSpritesY[i] + (camaraRecorridoTotalY * (1f - intensidadParallaxY));

            // Restricción: Si la cámara baja al suelo o más abajo, el fondo nunca baja de su Y inicial
            if (noBajarDelSueloInicial)
            {
                nuevaY = Mathf.Max(alturasInicialesSpritesY[i], nuevaY);
            }

            posActual.y = nuevaY;
            posActual.z = posicionZ;

            fondos[i].transform.position = posActual;


            // 3. Loop infinito Horizontal en X
            float diferenciaX = camara.position.x - fondos[i].transform.position.x;

            if (diferenciaX > longitudSprite * 1.5f)
            {
                fondos[i].transform.position += Vector3.right * (longitudSprite * fondos.Length);
            }
            else if (diferenciaX < -longitudSprite * 1.5f)
            {
                fondos[i].transform.position -= Vector3.right * (longitudSprite * fondos.Length);
            }
        }
    }
}