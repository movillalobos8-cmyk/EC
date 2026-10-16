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

    [Header("Secretaria")]
    public Image secretariaImage;
    public Sprite idleSprite;
    public Sprite talkingSprite;

    [Header("Configuración Imagen 1 (Esófago)")]
    [Tooltip("Marcar si se desea mostrar la imagen 1 durante el diálogo")]
    public bool mostrarImagenEsofago = true;
    public Image imagenEsofago; // Imagen de referencia del esófago
    public string fraseTriggerImagenEsofago = "en el esófago";

    [Header("Configuración Imagen 2 (Extra)")]
    [Tooltip("Marcar si se desea mostrar la imagen extra durante el diálogo")]
    public bool mostrarImagenExtra = false;
    public Image imagenExtra; // Nueva imagen extra opcional
    public string fraseTriggerImagenExtra = "comida";

    [Header("Texto del Tutorial")]
    [TextArea(8, 15)]
    public string tutorialText = "Ahora mismo estás dentro del cuerpo de Maitena, específicamente en el esófago viajando junto al agua que acaba de tomarse. Te diriges al estómago…\n\nPero tal y como dijo el doctor, no noto nada extraño en esta parte. De lo único que van a tener que cuidarse es de los restos de comida que pueden estar viajando junto con ustedes. Si chocan con la nave pueden llegar a hacer daño al motor. Pero no se preocupen, les mandaré unos avisos antes de que aparezcan.\n\nEl viaje hasta el estómago no es largo. Aguanten sólo un poco y estén atentos.";

    [Header("Spawners a Activar al Finalizar Todo")]
    public ObstacleSpawner[] obstacleSpawners;

    [Header("Configuración")]
    public float textSpeed = 0.03f;

    // Control interno de párrafos
    private string[] parrafostutorial;
    private int currentParagraphIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    // Banderas de control de estado
    private bool leyendoDialogos = true;
    private bool juegoIniciado = false;

    private void Start()
    {
        // 1. Estado inicial de paneles
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

        // Desactivar spawners al inicio
        foreach (ObstacleSpawner spawner in obstacleSpawners)
        {
            if (spawner != null) spawner.enabled = false;
        }

        // Pausar tiempo de juego
        Time.timeScale = 0f;

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
        // Trigger para la Imagen 1 (Esófago)
        if (mostrarImagenEsofago && imagenEsofago != null)
        {
            if (!string.IsNullOrEmpty(fraseTriggerImagenEsofago) && textoActual.Contains(fraseTriggerImagenEsofago))
            {
                imagenEsofago.gameObject.SetActive(true);
            }
        }

        // Trigger para la Imagen 2 (Extra)
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

        if (uiControles != null) uiControles.SetActive(false);

        // Activar Spawners
        foreach (ObstacleSpawner spawner in obstacleSpawners)
        {
            if (spawner != null) spawner.enabled = true;
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.StartLevel();
        }

        Time.timeScale = 1f;
        this.enabled = false;
    }
}