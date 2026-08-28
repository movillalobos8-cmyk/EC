using UnityEngine;

public class ParallaxLoop3Imagenes : MonoBehaviour
{
    [Header("Referencias")]
    public Transform camara;
    public SpriteRenderer[] fondos = new SpriteRenderer[3]; // Arrastra los 3 Sprites

    [Header("Efecto Parallax")]
    [Range(0f, 1f)]
    public float efectoParallax = 0.5f; // 0 = pegado a la cámara (más lejano), 1 = estático en el nivel

    [Header("Profundidad Absoluta (Atrás de todo)")]
    public float posicionZ = 10f; // Asegura que quede atrás del jugador (Z=0) y delante de la cámara (Z=-10)

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

        // 1. Obtener la anchura exacta del sprite
        if (fondos.Length > 0 && fondos[0] != null)
        {
            longitudSprite = fondos[0].bounds.size.x;
        }

        // 2. Colocar los 3 fondos en fila alrededor de la cámara y mandar el eje Z al fondo absoluto
        for (int i = 0; i < fondos.Length; i++)
        {
            if (fondos[i] != null)
            {
                // Ordenar capas por código por si acaso
                fondos[i].sortingOrder = -100;

                float posX = camara.position.x + ((i - 1) * longitudSprite);
                fondos[i].transform.position = new Vector3(posX, fondos[i].transform.position.y, posicionZ);
            }
        }
    }

    private void FixedUpdate()
    {
        if (camara == null || longitudSprite <= 0) return;

        // 1. Calcular el desplazamiento de la cámara
        float deltaCamaraX = camara.position.x - ultimaPosicionCamaraX;
        ultimaPosicionCamaraX = camara.position.x;

        // 2. Mover cada sprite individualmente aplicando Parallax y teletransporte sin bug
        for (int i = 0; i < fondos.Length; i++)
        {
            if (fondos[i] == null) continue;

            // Mover en X con Parallax
            Vector3 pos = fondos[i].transform.position;
            pos.x += deltaCamaraX * efectoParallax;
            pos.z = posicionZ; // Forzar que NUNCA cambie su profundidad Z
            fondos[i].transform.position = pos;

            // 3. Teletransportar en bucle continuo al salir de vista
            float diferencia = camara.position.x - fondos[i].transform.position.x;

            // Si quedó atrás por la izquierda -> Pasa al extremo derecho
            if (diferencia > longitudSprite * 1.5f)
            {
                fondos[i].transform.position += Vector3.right * (longitudSprite * fondos.Length);
            }
            // Si quedó atrás por la derecha -> Pasa al extremo izquierdo
            else if (diferencia < -longitudSprite * 1.5f)
            {
                fondos[i].transform.position -= Vector3.right * (longitudSprite * fondos.Length);
            }
        }
    }
}