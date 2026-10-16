using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SecretariaEndManager : MonoBehaviour
{
    public static SecretariaEndManager Instance;

    [Header("1. Panel de Diálogo")]
    public GameObject panel;
    public TMP_Text dialogueText;
    public Button continueButton; // Botón final para cargar la siguiente escena

    [Header("2. Panel de Pregunta (Multiple Choice)")]
    public GameObject choicePanel;
    public Button option1Button;
    public Button option2Button;
    public Button option3Button;
    public Button option4Button;
    public int correctOption = 3;

    [Header("Secretaria")]
    public Image secretariaImage;
    public Sprite idleSprite;
    public Sprite talkingSprite;

    [Header("Textos del Diálogo")]
    [TextArea(6, 12)]
    public string finalText = "¡Excelente trabajo! Has llegado al final del recorrido dentro de la vena.";

    [TextArea(4, 10)]
    public string correctAnswerText = "¡Respuesta correcta!\n\nLa Alteplasa disuelve los coágulos de sangre de forma efectiva.";

    [TextArea(4, 10)]
    public string wrongAnswerText = "Respuesta incorrecta.\n\nPero no te preocupes, debes tener más cuidado la próxima vez.";

    [Header("Configuración de Texto")]
    public float textSpeed = 0.03f;

    [Header("Escena Siguiente")]
    public int siguienteEscenaIndex = 3;

    // Control interno de diálogos y estados
    private string[] parrafosTexto;
    private int currentParagraphIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private string textoResultadoCompleto = "";

    private enum EstadoEnd { Inactivo, TextoInicial, EsperandoOpciones, MostrandoResultado, ListoParaContinuar }
    private EstadoEnd estadoActual = EstadoEnd.Inactivo;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (panel == null || dialogueText == null || secretariaImage == null)
        {
            Debug.LogError("❌ [SecretariaEndManager] Faltan referencias clave en el Inspector.");
            return;
        }

        // Ocultar elementos al inicio
        panel.SetActive(false);
        if (continueButton != null) continueButton.gameObject.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);
    }

    private void Update()
    {
        if (estadoActual == EstadoEnd.Inactivo) return;

        // Avanzar texto con Espacio, Enter o Clic del ratón
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            if (estadoActual == EstadoEnd.TextoInicial)
            {
                AvanzarOCompletarTextoInicial();
            }
            else if (estadoActual == EstadoEnd.MostrandoResultado)
            {
                AvanzarOCompletarTextoResultado();
            }
        }
    }

    // --- PUNTO DE ENTRADA ---
    public void StartEndDialogue()
    {
        Debug.Log("✔ [SecretariaEndManager] Iniciando secuencia final de nivel.");

        panel.SetActive(true);
        if (choicePanel != null) choicePanel.SetActive(false);
        if (continueButton != null) continueButton.gameObject.SetActive(false);

        Time.timeScale = 0f; // Pausa el juego para el diálogo final

        parrafosTexto = SepararTextoEnParrafos(finalText);
        currentParagraphIndex = 0;
        estadoActual = EstadoEnd.TextoInicial;

        if (parrafosTexto.Length > 0)
        {
            IniciarTipeado(parrafosTexto[0], false);
        }
        else
        {
            MostrarOpciones();
        }
    }

    private string[] SepararTextoEnParrafos(string textoBruto)
    {
        if (string.IsNullOrEmpty(textoBruto)) return new string[0];

        // Normaliza saltos de línea sin achicar el texto
        string textoLimpio = textoBruto.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
        
        // Separa por párrafos en dobles saltos de línea
        string[] parrafos = textoLimpio.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        if (parrafos.Length == 0)
        {
            parrafos = new string[] { textoLimpio };
        }

        return parrafos;
    }

    private void AvanzarOCompletarTextoInicial()
    {
        if (parrafosTexto == null || parrafosTexto.Length == 0) return;

        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);

            dialogueText.text = parrafosTexto[currentParagraphIndex];
            if (secretariaImage != null && idleSprite != null) secretariaImage.sprite = idleSprite;
            isTyping = false;

            if (currentParagraphIndex >= parrafosTexto.Length - 1)
            {
                MostrarOpciones();
            }
        }
        else if (currentParagraphIndex < parrafosTexto.Length - 1)
        {
            currentParagraphIndex++;
            IniciarTipeado(parrafosTexto[currentParagraphIndex], false);
        }
        else
        {
            MostrarOpciones();
        }
    }

    private void IniciarTipeado(string texto, bool esResultado)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(EscribirTexto(texto, esResultado));
    }

    private IEnumerator EscribirTexto(string texto, bool esResultado)
    {
        isTyping = true;
        dialogueText.text = "";

        if (secretariaImage != null && talkingSprite != null) secretariaImage.sprite = talkingSprite;

        for (int i = 0; i < texto.Length; i++)
        {
            dialogueText.text += texto[i];
            yield return new WaitForSecondsRealtime(textSpeed);
        }

        if (secretariaImage != null && idleSprite != null) secretariaImage.sprite = idleSprite;
        isTyping = false;

        if (!esResultado && currentParagraphIndex >= parrafosTexto.Length - 1)
        {
            MostrarOpciones();
        }
        else if (esResultado)
        {
            // Al terminar de escribir el resultado actual
            if (currentParagraphIndex >= parrafosTexto.Length - 1)
            {
                estadoActual = EstadoEnd.ListoParaContinuar;
                if (continueButton != null) continueButton.gameObject.SetActive(true);
            }
        }
    }

    // --- FASE 2: MOSTRAR OPCIONES ---
    private void MostrarOpciones()
    {
        estadoActual = EstadoEnd.EsperandoOpciones;

        if (choicePanel == null || option1Button == null || option2Button == null || option3Button == null || option4Button == null)
        {
            if (continueButton != null) continueButton.gameObject.SetActive(true);
            return;
        }

        choicePanel.SetActive(true);

        option1Button.onClick.RemoveAllListeners();
        option2Button.onClick.RemoveAllListeners();
        option3Button.onClick.RemoveAllListeners();
        option4Button.onClick.RemoveAllListeners();

        option1Button.onClick.AddListener(() => ResponderOpcion(1));
        option2Button.onClick.AddListener(() => ResponderOpcion(2));
        option3Button.onClick.AddListener(() => ResponderOpcion(3));
        option4Button.onClick.AddListener(() => ResponderOpcion(4));
    }

    // --- FASE 3: RESPUESTA Y FEEDBACK POR PÁRRAFOS ---
    private void ResponderOpcion(int opcionSeleccionada)
    {
        if (choicePanel != null) choicePanel.SetActive(false);

        estadoActual = EstadoEnd.MostrandoResultado;

        textoResultadoCompleto = (opcionSeleccionada == correctOption) ? correctAnswerText : wrongAnswerText;
        
        // Separa la respuesta en párrafos para que si es larga no se corte
        parrafosTexto = SepararTextoEnParrafos(textoResultadoCompleto);
        currentParagraphIndex = 0;

        if (parrafosTexto.Length > 0)
        {
            IniciarTipeado(parrafosTexto[0], true);
        }
        else
        {
            estadoActual = EstadoEnd.ListoParaContinuar;
            if (continueButton != null) continueButton.gameObject.SetActive(true);
        }
    }

    private void AvanzarOCompletarTextoResultado()
    {
        if (parrafosTexto == null || parrafosTexto.Length == 0) return;

        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);

            dialogueText.text = parrafosTexto[currentParagraphIndex];
            if (secretariaImage != null && idleSprite != null) secretariaImage.sprite = idleSprite;
            isTyping = false;

            if (currentParagraphIndex >= parrafosTexto.Length - 1)
            {
                estadoActual = EstadoEnd.ListoParaContinuar;
                if (continueButton != null) continueButton.gameObject.SetActive(true);
            }
        }
        else if (currentParagraphIndex < parrafosTexto.Length - 1)
        {
            currentParagraphIndex++;
            IniciarTipeado(parrafosTexto[currentParagraphIndex], true);
        }
        else
        {
            estadoActual = EstadoEnd.ListoParaContinuar;
            if (continueButton != null) continueButton.gameObject.SetActive(true);
        }
    }

    // --- FASE 4: CARGAR SIGUIENTE ESCENA ---
    public void ContinueToNextScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(siguienteEscenaIndex);
    }
}