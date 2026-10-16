using UnityEngine;

public class ParallaxLoopDinamico : MonoBehaviour
{
    [Header("Referencias de Fondos")]
    [Tooltip("Fondo único del inicio del nivel.")]
    public SpriteRenderer fondoInicial;

    [Tooltip("Los 2 fondos que rotarán continuamente.")]
    public SpriteRenderer[] fondosBucle = new SpriteRenderer[2];

    [Header("Referencias de Cámara")]
    public Transform camara;

    [Header("Efecto Parallax")]
    [Range(0f, 1f)]
    public float efectoParallax = 0.5f;

    [Header("Profundidad Absoluta")]
    public float posicionZ = 10f;
    public int sortingOrder = -100;

    private float longitudSprite;
    private float ultimaPosicionCamaraX;

    private void Start()
    {
        if (camara == null && Camera.main != null)
        {
            camara = Camera.main.transform;
        }

        if (camara != null)
        {
            ultimaPosicionCamaraX = camara.position.x;
        }

        // Obtener la anchura del sprite
        if (fondosBucle.Length > 0 && fondosBucle[0] != null)
        {
            longitudSprite = fondosBucle[0].bounds.size.x;
        }
        else if (fondoInicial != null)
        {
            longitudSprite = fondoInicial.bounds.size.x;
        }

        // 1. Configurar Fondo Inicial
        if (fondoInicial != null)
        {
            fondoInicial.sortingOrder = sortingOrder;
            Vector3 posInicial = fondoInicial.transform.position;
            posInicial.z = posicionZ;
            fondoInicial.transform.position = posInicial;
        }

        // 2. Alinear los fondos de bucle inmediatamente a la derecha del fondo inicial
        float puntoPartidaX = (fondoInicial != null) 
            ? fondoInicial.transform.position.x + longitudSprite 
            : camara.position.x;

        for (int i = 0; i < fondosBucle.Length; i++)
        {
            if (fondosBucle[i] != null)
            {
                fondosBucle[i].sortingOrder = sortingOrder;
                float posX = puntoPartidaX + (i * longitudSprite);
                fondosBucle[i].transform.position = new Vector3(posX, fondosBucle[i].transform.position.y, posicionZ);
            }
        }
    }

    private void FixedUpdate()
    {
        if (camara == null || longitudSprite <= 0) return;

        float deltaCamaraX = camara.position.x - ultimaPosicionCamaraX;
        ultimaPosicionCamaraX = camara.position.x;

        // Mover Fondo Inicial
        if (fondoInicial != null)
        {
            Vector3 posInicial = fondoInicial.transform.position;
            posInicial.x += deltaCamaraX * efectoParallax;
            posInicial.z = posicionZ;
            fondoInicial.transform.position = posInicial;
        }

        // Mover y Reciclar Fondos en Bucle
        for (int i = 0; i < fondosBucle.Length; i++)
        {
            if (fondosBucle[i] == null) continue;

            Vector3 pos = fondosBucle[i].transform.position;
            pos.x += deltaCamaraX * efectoParallax;
            pos.z = posicionZ;
            fondosBucle[i].transform.position = pos;

            float diferencia = camara.position.x - fondosBucle[i].transform.position.x;

            // Al avanzar a la derecha: se teletransporta en cuanto sale la mitad exacta del sprite en pantalla
            if (diferencia > longitudSprite)
            {
                fondosBucle[i].transform.position += Vector3.right * (longitudSprite * fondosBucle.Length);
            }
            // Al retroceder a la izquierda
            else if (diferencia < -longitudSprite)
            {
                float limiteIzquierdo = (fondoInicial != null) 
                    ? fondoInicial.transform.position.x + (longitudSprite * 0.5f) 
                    : float.MinValue;

                if (fondosBucle[i].transform.position.x - (longitudSprite * fondosBucle.Length) >= limiteIzquierdo)
                {
                    fondosBucle[i].transform.position -= Vector3.right * (longitudSprite * fondosBucle.Length);
                }
            }
        }
    }
}