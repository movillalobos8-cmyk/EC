using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class GeneradorProyectiles : MonoBehaviour
{
    [System.Serializable]
    public class PuntoSpawn
    {
        [Tooltip("Objeto de referencia donde aparecerá el proyectil")]
        public Transform puntoTransform;

        [Tooltip("Objeto de alerta asociado a este carril")]
        public GameObject alertaObject;
    }

    [Header("Puntos de Spawn (Izquierda, Centro, Derecha)")]
    public PuntoSpawn[] carriles = new PuntoSpawn[3];

    [Header("Variantes de Proyectil")]
    public GameObject proyectilVarianteA;
    public GameObject proyectilVarianteB;

    [Tooltip("Si está activo alterna A -> B -> A -> B. Si está desactivado, elige uno al azar.")]
    public bool alternarSecuencial = true;

    [Header("Movimiento Vertical de Caída")]
    public float velocidadCaida = 400f;
    public float limiteInferiorY = -800f;

    [Header("Tiempos")]
    public float tiempoEntreApariciones = 2.5f;
    public float duracionAlerta = 1.0f;
    public float intervaloParpadeo = 0.15f;

    [Header("Configuración de Oleadas y Victoria")]
    [Tooltip("Cantidad total de oleadas requeridas para ganar y activar el video")]
    public int cantidadOleadasParaVideo = 16;

    [Tooltip("Número de índice de la siguiente escena a cargar (según Build Settings)")]
    public int indiceSiguienteEscena = 1;

    [Header("Ending & Video")]
    public GameObject videoPanel;
    public VideoPlayer endingVideo;

    [Header("Music")]
    public AudioSource musicSource;

    private bool usarVarianteA = true;
    private int contadorIndividuales = 0;
    private int oleadasSuperadas = 0;
    private bool nivelCompletado = false;

    private void Start()
    {
        foreach (var carril in carriles)
        {
            if (carril.alertaObject != null)
            {
                carril.alertaObject.SetActive(false);
            }
        }

        if (endingVideo != null)
        {
            endingVideo.playOnAwake = false;
            endingVideo.enabled = false;
            // Precargar el video en memoria en segundo plano para evitar tirones
            endingVideo.Prepare(); 
        }

        if (videoPanel != null)
        {
            videoPanel.SetActive(false);
        }

        StartCoroutine(RutinaAparicion());
    }

    private IEnumerator RutinaAparicion()
    {
        while (!nivelCompletado)
        {
            yield return new WaitForSeconds(tiempoEntreApariciones);

            oleadasSuperadas++;

            int cantidadApariciones = 1;
            if (contadorIndividuales >= 3)
            {
                cantidadApariciones = 2;
                contadorIndividuales = 0;
            }
            else
            {
                contadorIndividuales++;
            }

            List<int> indicesCarrilesElegidos = ObtenerCarrilesUnicos(cantidadApariciones);

            foreach (int idx in indicesCarrilesElegidos)
            {
                if (carriles[idx].alertaObject != null)
                {
                    StartCoroutine(TitilarAlerta(carriles[idx].alertaObject));
                }
            }

            yield return new WaitForSeconds(duracionAlerta);

            foreach (int idx in indicesCarrilesElegidos)
            {
                PuntoSpawn carril = carriles[idx];
                GameObject prefabElegido = ObtenerProyectilActual();

                if (prefabElegido != null && carril.puntoTransform != null)
                {
                    GameObject proyectil = Instantiate(prefabElegido, carril.puntoTransform.position, Quaternion.identity, transform);
                    proyectil.tag = "Enemigo";

                    StartCoroutine(MoverProyectilHaciaAbajo(proyectil));
                }
            }

            if (oleadasSuperadas >= cantidadOleadasParaVideo)
            {
                nivelCompletado = true;
                yield return new WaitForSeconds(1.0f);
                PlayEndingVideo();
                yield break;
            }
        }
    }

    public void PlayEndingVideo()
    {
        StartCoroutine(EndingSequence());
    }

    private IEnumerator EndingSequence()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        if (videoPanel != null)
        {
            videoPanel.transform.SetAsLastSibling();
            videoPanel.SetActive(true);

            Canvas canvasVideo = videoPanel.GetComponent<Canvas>();
            if (canvasVideo != null)
            {
                canvasVideo.overrideSorting = true;
                canvasVideo.sortingOrder = 999;
            }
        }

        if (endingVideo != null)
        {
            endingVideo.enabled = true;

            // Esperar a que el buffer esté 100% preparado
            if (!endingVideo.isPrepared)
            {
                endingVideo.Prepare();
                while (!endingVideo.isPrepared)
                {
                    yield return null;
                }
            }

            endingVideo.Play();
            yield return new WaitForSeconds(0.2f);

            float tiempoEspera = 5f;
            if (endingVideo.clip != null)
            {
                tiempoEspera = (float)endingVideo.clip.length - 0.2f;
            }

            yield return new WaitForSeconds(tiempoEspera);
        }

        SceneManager.LoadScene(indiceSiguienteEscena);
    }

    private List<int> ObtenerCarrilesUnicos(int cantidad)
    {
        List<int> disponibles = new List<int> { 0, 1, 2 };
        List<int> seleccionados = new List<int>();

        for (int i = 0; i < cantidad; i++)
        {
            int idxAleatorio = Random.Range(0, disponibles.Count);
            seleccionados.Add(disponibles[idxAleatorio]);
            disponibles.RemoveAt(idxAleatorio);
        }

        return seleccionados;
    }

    private IEnumerator MoverProyectilHaciaAbajo(GameObject proyectil)
    {
        float posXFijaWorld = proyectil.transform.position.x;

        while (proyectil != null)
        {
            Vector3 pos = proyectil.transform.position;
            pos.x = posXFijaWorld;
            pos.y -= velocidadCaida * Time.deltaTime;
            proyectil.transform.position = pos;

            if (proyectil.transform.position.y < limiteInferiorY)
            {
                Destroy(proyectil);
                yield break;
            }

            yield return null;
        }
    }

    private GameObject ObtenerProyectilActual()
    {
        GameObject seleccionado;

        if (alternarSecuencial)
        {
            seleccionado = usarVarianteA ? proyectilVarianteA : proyectilVarianteB;
            usarVarianteA = !usarVarianteA;
        }
        else
        {
            seleccionado = (Random.value > 0.5f) ? proyectilVarianteA : proyectilVarianteB;
        }

        return seleccionado;
    }

    private IEnumerator TitilarAlerta(GameObject alerta)
    {
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracionAlerta)
        {
            alerta.SetActive(!alerta.activeSelf);
            yield return new WaitForSeconds(intervaloParpadeo);
            tiempoTranscurrido += intervaloParpadeo;
        }

        alerta.SetActive(false);
    }
}