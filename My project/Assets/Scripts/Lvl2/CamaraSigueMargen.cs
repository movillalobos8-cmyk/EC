using UnityEngine;



public class CamaraSigueMargen : MonoBehaviour

{

    [Header("Objetivo")]

    public Transform objetivo; // Arrastra a tu Personaje aquí



    [Header("Márgenes (Deadzone en X)")]

    public float margenHorizontal = 3f;

    public float velocidadSuavizado = 12f;



    [Header("Límites del Nivel (Límites en X)")]

    public bool usarLimites = true;

    public float limiteIzquierdo = -10f; // Punto X mínimo

    public float limiteDerecho = 50f;    // Punto X máximo



    private float centroInicialY;

    private float centroInicialZ;



    private void Start()

    {

        centroInicialY = transform.position.y;

        centroInicialZ = transform.position.z;

    }



    private void FixedUpdate()

    {

        if (objetivo == null) return;



        float posXCamara = transform.position.x;

        float posXObjetivo = objetivo.position.x;

        float distanciaX = posXObjetivo - posXCamara;



        // 1. Calcular a dónde debería ir la cámara según la Deadzone

        if (distanciaX > margenHorizontal)

        {

            posXCamara = posXObjetivo - margenHorizontal;

        }

        else if (distanciaX < -margenHorizontal)

        {

            posXCamara = posXObjetivo + margenHorizontal;

        }



        // 2. Mover la cámara hacia esa posición primero (Suavizado)

        Vector3 posicionDeseada = new Vector3(posXCamara, centroInicialY, centroInicialZ);

        Vector3 posicionSuave = Vector3.Lerp(transform.position, posicionDeseada, velocidadSuavizado * Time.fixedDeltaTime);



        // 3. 🔒 RESTRICCIÓN ABSOLUTA (Frena la cámara exacto en el límite)

        if (usarLimites)

        {

            posicionSuave.x = Mathf.Clamp(posicionSuave.x, limiteIzquierdo, limiteDerecho);

        }



        // 4. Asignar posición final garantizada

        transform.position = posicionSuave;

    }



    private void OnDrawGizmosSelected()

    {

        Gizmos.color = Color.cyan;

        Vector3 centro = new Vector3(transform.position.x, transform.position.y, 0f);

        Gizmos.DrawWireCube(centro, new Vector3(margenHorizontal * 2f, 6f, 0.1f));



        if (usarLimites)

        {

            Gizmos.color = Color.red;

            Gizmos.DrawLine(new Vector3(limiteIzquierdo, transform.position.y - 10, 0), new Vector3(limiteIzquierdo, transform.position.y + 10, 0));

            Gizmos.DrawLine(new Vector3(limiteDerecho, transform.position.y - 10, 0), new Vector3(limiteDerecho, transform.position.y + 10, 0));

        }

    }

}