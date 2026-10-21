using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SecretariaTutorialManagerNivel3 : MonoBehaviour
{
    [Header("1. UI del Tutorial (Secretaria y Diálogos)")]
    public GameObject uiTutorial;
    public TMP_Text dialogueText;
    public Button entendidoButton; // Botón "Entendido" que aparece al final del diálogo

    [Header("2. UI de Controles (Siguiente Panel)")]
    public GameObject uiControles;
    public Button estoyPreparadoButton; // Botón "Estoy Preparado" en el panel de controles

    [Header("3. Objetos a Congelar Estrictamente")]
    [Tooltip("Arrastra aquí el GameObject de la Nave / Jugador")]
    public GameObject jugador;
    [Tooltip("Arrastra cualquier script de movimiento (Cámara, Nave, Enemigos) que deba apagarse durante el tutorial")]
    public MonoBehaviour[] scriptsAMoverOApagar;

    [Header("Secretaria")]
    public Image secretariaImage;
    public Sprite idleSprite;
    public Sprite talkingSprite;

    [Header("Configuración Imagen 1 (Esófago)")]
    public bool mostrarImagenEsofago = true;
    public Image imagenEsofago;
    public string fraseTriggerImagenEsofago = "en el esófago";

    [Header("Configuración Imagen 2 (Extra)")]
    public bool mostrarImagenExtra = false;
    public Image imagenExtra;
    public string fraseTriggerImagenExtra = "comida";

    [Header("Texto del Tutorial")]
    [TextArea(8, 15)]
    public string tutorialText = "Ahora mismo estás dentro del cuerpo de Maitena, específicamente en el esófago viajando junto al agua que acaba de tomarse. Te diriges al estómago…\n\nPero tal y como dijo el doctor, no noto nada extraño en esta parte. De lo único que van a tener que cuidarse es de los restos de comida que pueden estar viajando junto con ustedes. Si chocan con la nave pueden llegar a hacer daño al motor. Pero no se preocupen, les mandaré unos avisos antes de que aparezcan.\n\nEl viaje hasta el estómago no es largo. Aguanten sólo un poco y estén atentos.";

    [Header("Spawners a Activar al Finalizar Todo")]
    public ObstacleSpawner[] obstacleSpawners;

    [Header("Configuración")]
    public float textSpeed = 0.03f;

    [Header("Música")]
    [Tooltip("Arrastra el GameMusicManager. Si lo dejas vacío, lo busca solo.")]
    public GameMusicManager musicManager;
    private bool musicaDetenida = false;

    // Control interno de párrafos
    private string[] parrafostutorial;
    private int currentParagraphIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    // Banderas de control de estado
    private bool leyendoDialogos = true;
    private bool juegoIniciado = false;

    // Los spawners leen esto para saber cuándo empezar a contar su delay inicial
    public bool TutorialTerminado { get; private set; }

    private void Start()
    {
        if (musicManager == null)
            musicManager = FindFirstObjectByType<GameMusicManager>();

        StartCoroutine(PausarMusicaTrasUnFrame());

        // --- CONGELAMIENTO ABSOLUTO AL INICIAR ---
        AplicarPausaAbsoluta(true);

        // Estado inicial de paneles UI
        if (uiTutorial != null) uiTutorial.SetActive(true);
        if (uiControles != null) uiControles.SetActive(false);

        if (entendidoButton != null)
        {
            entendidoButton.gameObject.SetActive(false);
            entendidoButton.onClick.RemoveAllListeners();
            entendidoButton.onClick.AddListener(BotonEntendido);
        }

        if (estoyPreparadoButton != null)
        {
            estoyPreparadoButton.onClick.RemoveAllListeners();
            estoyPreparadoButton.onClick.AddListener(BotonEstoyPreparado);
        }

        // Ocultar imágenes de referencia al inicio
        if (imagenEsofago != null) imagenEsofago.gameObject.SetActive(false);
        if (imagenExtra != null) imagenExtra.gameObject.SetActive(false);

        // Desactivar spawners
        foreach (ObstacleSpawner spawner in obstacleSpawners)
        {
            if (spawner != null) spawner.enabled = false;
        }

        // Cargar párrafos
        if (!string.IsNullOrEmpty(tutorialText))
        {
            parrafostutorial = tutorialText.Split(new string[] { "\n\n", "\r\n\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            if (parrafostutorial.Length > 0)
            {
                typingCoroutine = StartCoroutine(EscribirTextoTutorial(parrafostutorial[currentParagraphIndex]));
            }
        }
    }

    private void Update()
    {
        if (!leyendoDialogos || juegoIniciado) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            AvanzarOCompletarTexto();
        }
    }

    public void AvanzarOCompletarTexto()
    {
        if (parrafostutorial == null || parrafostutorial.Length == 0) return;

        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = parrafostutorial[currentParagraphIndex];

            ComprobarTriggers(parrafostutorial[currentParagraphIndex]);

            if (secretariaImage != null) secretariaImage.sprite = idleSprite;
            isTyping = false;

            if (currentParagraphIndex >= parrafostutorial.Length - 1 && entendidoButton != null)
            {
                entendidoButton.gameObject.SetActive(true);
            }
        }
        else if (currentParagraphIndex < parrafostutorial.Length - 1)
        {
            currentParagraphIndex++;
            typingCoroutine = StartCoroutine(EscribirTextoTutorial(parrafostutorial[currentParagraphIndex]));
        }
    }

    private IEnumerator EscribirTextoTutorial(string parrafo)
    {
        isTyping = true;
        dialogueText.text = "";

        if (secretariaImage != null) secretariaImage.sprite = talkingSprite;
        if (entendidoButton != null) entendidoButton.gameObject.SetActive(false);

        for (int i = 0; i < parrafo.Length; i++)
        {
            dialogueText.text += parrafo[i];

            ComprobarTriggers(dialogueText.text);

            yield return new WaitForSecondsRealtime(textSpeed);
        }

        if (secretariaImage != null) secretariaImage.sprite = idleSprite;
        isTyping = false;

        if (currentParagraphIndex >= parrafostutorial.Length - 1 && entendidoButton != null)
        {
            entendidoButton.gameObject.SetActive(true);
        }
    }

    private void ComprobarTriggers(string textoActual)
    {
        if (mostrarImagenEsofago && imagenEsofago != null)
        {
            if (!string.IsNullOrEmpty(fraseTriggerImagenEsofago) && textoActual.Contains(fraseTriggerImagenEsofago))
            {
                imagenEsofago.gameObject.SetActive(true);
            }
        }

        if (mostrarImagenExtra && imagenExtra != null)
        {
            if (!string.IsNullOrEmpty(fraseTriggerImagenExtra) && textoActual.Contains(fraseTriggerImagenExtra))
            {
                imagenExtra.gameObject.SetActive(true);
            }
        }
    }

    public void BotonEntendido()
    {
        leyendoDialogos = false;

        if (uiTutorial != null) uiTutorial.SetActive(false);

        if (uiControles != null)
        {
            uiControles.SetActive(true);
        }
        else
        {
            BotonEstoyPreparado();
        }
    }

    public void BotonEstoyPreparado()
    {
        juegoIniciado = true;
        TutorialTerminado = true; // Los spawners empiezan a contar su delay desde aquí

        if (uiControles != null) uiControles.SetActive(false);

        ReanudarMusica(); // La música vuelve al salir de los controles

        // --- DESCONGELAR TODO STRICTAMENTE ---
        AplicarPausaAbsoluta(false);

        // Activar Spawners
        foreach (ObstacleSpawner spawner in obstacleSpawners)
        {
            if (spawner != null) spawner.enabled = true;
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.StartLevel();
        }

        this.enabled = false;
    }

    private IEnumerator PausarMusicaTrasUnFrame()
    {
        yield return null; // Espera un frame para que el GameMusicManager ya haya arrancado la música
        DetenerMusica();   // La música queda en pausa mientras habla la secretaria
    }

    private void DetenerMusica()
    {
        if (musicManager == null)
        {
            Debug.LogWarning("[Tutorial] No se encontró el GameMusicManager en la escena.");
            return;
        }
        musicManager.PauseMusic();
        musicaDetenida = true;
    }

    private void ReanudarMusica()
    {
        if (musicManager == null || !musicaDetenida) return;
        musicManager.ResumeMusic();
        musicaDetenida = false;
    }

    /// <summary>
    /// Forzado estricto de pausa / reanudación total de la escena.
    /// </summary>
    private void AplicarPausaAbsoluta(bool pausar)
    {
        // 1. Tiempo global de Unity
        Time.timeScale = pausar ? 0f : 1f;

        // 2. Físicas e inercia del jugador
        if (jugador != null)
        {
            Rigidbody2D rb2d = jugador.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                if (pausar)
                {
                    rb2d.linearVelocity = Vector2.zero;
                    rb2d.simulated = false; // Desactiva el motor de físicas
                }
                else
                {
                    rb2d.simulated = true;
                }
            }

            Rigidbody rb3d = jugador.GetComponent<Rigidbody>();
            if (rb3d != null)
            {
                if (pausar)
                {
                    rb3d.linearVelocity = Vector3.zero;
                    rb3d.isKinematic = true;
                }
                else
                {
                    rb3d.isKinematic = false;
                }
            }
        }

        // 3. Desactivar / Activar manualmente los scripts de control
        if (scriptsAMoverOApagar != null)
        {
            foreach (MonoBehaviour script in scriptsAMoverOApagar)
            {
                if (script != null)
                {
                    script.enabled = !pausar;
                }
            }
        }
    }
}