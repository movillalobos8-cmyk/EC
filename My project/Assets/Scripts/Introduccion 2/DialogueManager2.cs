using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 
using UnityEngine.Video;          

public class DialogueManager2 : MonoBehaviour
{
    [Header("UI General")]
    public TMP_Text leftNameText;
    public TMP_Text rightNameText;
    public GameObject leftNameBackground;
    public GameObject rightNameBackground;
    public TMP_Text dialogueText;

    [Header("Audio")]
    public AudioSource musicaFondo;             

    [Header("Characters UI Images")]
    public Image leftCharacterImage;
    public Image rightCharacterImage;

    [Header("Character Sizes")]
    public Vector2 doctorSize = new Vector2(650, 950);
    public Vector2 denisSize = new Vector2(700, 1000);
    public Vector2 secretariaSize = new Vector2(600, 900);
    public Vector2 daliaSize = new Vector2(650, 950);

    [Header("Doctor (Izquierda)")]
    public Sprite doctorTalking;
    public Sprite doctorIdle;

    [Header("Secretaria (Derecha)")]
    public Sprite secretariaTalking;
    public Sprite secretariaIdle;

    [Header("Denis (Derecha)")]
    public Sprite denisTalking;
    public Sprite denisIdle;

    [Header("Dalia (Derecha)")]
    public Sprite daliaTalking;
    public Sprite daliaIdle;

    [Header("Jerarquía de Preguntas (UI)")]
    public GameObject panelPreguntas;            
    
    [Header("Botones Principales")]
    public GameObject panelBotones;             
    public Button btnPreg1;                     
    public Button btnPreg2;                     
    public Button btnPreg3;                     

    [Header("SubPreguntas Pregunta 1")]
    public GameObject panelPreg1SubPreguntas;   
    public Button btnSubPreg1_1;                
    public Button btnSubPreg1_2;                

    [Header("SubPregunta Pregunta 3")]
    public GameObject panelPreg3SubPregunta;    
    public Button btnSubPreg3_1;                

    [Header("Panel Diagnóstico Final")]
    public GameObject panelDiagnostico;         
    public Button btnBronquitis;
    public Button btnNeumonia;
    public Button btnTuberculosis;

    [Header("Video Final y Siguiente Escena")]
    public GameObject videoDisplayObject;       
    public VideoPlayer videoPlayer;             
    public int numeroEscenaSiguiente = 5;       

    [Header("Config")]
    public float textSpeed = 0.05f;

    private string playerName = "Jugador";
    private bool isTyping = false;
    private bool isWaitingForChoice = false;
    private int currentLine = 0;

    private WaitForSeconds textSpeedWait;
    public List<DialogueLine> linesList = new List<DialogueLine>();

    // Control de preguntas completadas
    private bool askedHeadache = false;
    private bool askedAntecedents = false;
    private bool askedCough = false;

    void Start()
    {
        textSpeedWait = new WaitForSeconds(textSpeed);
        
        // Carga y valida el nombre del jugador
        LoadPlayerName();

        if (leftNameBackground != null) leftNameBackground.SetActive(false);
        if (rightNameBackground != null) rightNameBackground.SetActive(false);

        HideAllPanels();
        SetupButtonListeners();

        BuildInitialDialogue();
        StartCoroutine(TypeLine());
    }

    private void LoadPlayerName()
    {
        // Se añade la clave exacta guardada en la Escena 2 ("NombreJugador")
        if (PlayerPrefs.HasKey("NombreJugador") && !string.IsNullOrEmpty(PlayerPrefs.GetString("NombreJugador")))
            playerName = PlayerPrefs.GetString("NombreJugador");
        else if (PlayerPrefs.HasKey("PlayerName") && !string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerName")))
            playerName = PlayerPrefs.GetString("PlayerName");
        else if (PlayerPrefs.HasKey("Nombre") && !string.IsNullOrEmpty(PlayerPrefs.GetString("Nombre")))
            playerName = PlayerPrefs.GetString("Nombre");
        else if (PlayerPrefs.HasKey("UserName") && !string.IsNullOrEmpty(PlayerPrefs.GetString("UserName")))
            playerName = PlayerPrefs.GetString("UserName");
        else if (PlayerPrefs.HasKey("Name") && !string.IsNullOrEmpty(PlayerPrefs.GetString("Name")))
            playerName = PlayerPrefs.GetString("Name");
        
        Debug.Log("Nombre de jugador cargado en DialogueManager2: " + playerName);
    }

    // Devuelve si un nombre de personaje corresponde al jugador
    private bool IsPlayerCharacter(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        string lower = name.ToLower().Trim();
        return lower == "player_name" || lower == "nombre" || lower == "jugador" || lower == playerName.ToLower().Trim();
    }

    void HideAllPanels()
    {
        if (panelPreguntas != null) panelPreguntas.SetActive(false);
        if (panelBotones != null) panelBotones.SetActive(false);
        if (panelPreg1SubPreguntas != null) panelPreg1SubPreguntas.SetActive(false);
        if (panelPreg3SubPregunta != null) panelPreg3SubPregunta.SetActive(false);
        if (panelDiagnostico != null) panelDiagnostico.SetActive(false);
        if (videoDisplayObject != null) videoDisplayObject.SetActive(false);
    }

    void SetupButtonListeners()
    {
        if (btnPreg1 != null) btnPreg1.onClick.AddListener(SelectPregunta1);
        if (btnPreg2 != null) btnPreg2.onClick.AddListener(SelectPregunta2);
        if (btnPreg3 != null) btnPreg3.onClick.AddListener(SelectPregunta3);

        if (btnSubPreg1_1 != null) btnSubPreg1_1.onClick.AddListener(SelectSubPregunta1_1);
        if (btnSubPreg1_2 != null) btnSubPreg1_2.onClick.AddListener(SelectSubPregunta1_2);

        if (btnSubPreg3_1 != null) btnSubPreg3_1.onClick.AddListener(SelectSubPregunta3_1);

        if (btnBronquitis != null) btnBronquitis.onClick.AddListener(() => SelectDiagnosis(false));
        if (btnNeumonia != null) btnNeumonia.onClick.AddListener(() => SelectDiagnosis(true));
        if (btnTuberculosis != null) btnTuberculosis.onClick.AddListener(() => SelectDiagnosis(false));
    }

    void ApplyCharacterSize(Image image, Vector2 size)
    {
        if (image != null && image.rectTransform != null)
        {
            image.rectTransform.sizeDelta = size;
        }
    }

    void Update()
    {
        if (isWaitingForChoice) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
                CompleteLine();
            else
                NextLine();
        }
    }

    void BuildInitialDialogue()
    {
        linesList = new List<DialogueLine>
        {
            new DialogueLine { characterName = "Doctor", text = "Muy bien, ya conseguimos eliminar el coágulo de sangre con éxito.", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "¿Cómo sientes el brazo, Denis?", isLeftCharacter = true },
            new DialogueLine { characterName = "Denis", text = "¡Wow! El dolor desapareció y ya no lo siento pesado. Muchas gracias, Doctor, PLAYER_NAME.", isLeftCharacter = false },
            new DialogueLine { characterName = "Denis", text = "Han eliminado el dolor de mi brazo por completo. He hecho una buena elección al venir a esta clínica maravillosa.", isLeftCharacter = false },
            new DialogueLine { characterName = "", text = "Denis se va.", isLeftCharacter = false },
            new DialogueLine { characterName = "Secretaria", text = "Por suerte, este ha sido un problema sencillo.", isLeftCharacter = false },
            new DialogueLine { characterName = "Secretaria", text = "Ya terminamos con el primer paciente, el siguiente está en la sala de espera.", isLeftCharacter = false },
            new DialogueLine { characterName = "Secretaria", text = "Lo haré pasar.", isLeftCharacter = false },
            new DialogueLine { characterName = "Doctor", text = "Lo estás haciendo bien. Un coágulo es un buen enemigo para entrar en calor.", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "Esto te puede ayudar para tu informe. No todos los días puedes viajar a través de las venas y combatir una bola de sangre estacionada desde adentro.", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "¡Oh!", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "Ahí está la siguiente paciente. ¡Adelante!", isLeftCharacter = true },
            new DialogueLine { characterName = "Dalia", text = "Hola, buenas. Mi nombre es Dalia.", isLeftCharacter = false },
            new DialogueLine { characterName = "Dalia", text = "¿Cómo están? Espero que de maravilla, pues yo no.", isLeftCharacter = false },
            new DialogueLine { characterName = "Dalia", text = "Ja, Ja, Ja.", isLeftCharacter = false },
            new DialogueLine { characterName = "Dalia", text = "Bueno, la verdad es que no me he estado sintiendo muy bien. Traigo una congestión que me dificulta respirar y…", isLeftCharacter = false },
            new DialogueLine { characterName = "Dalia", text = "Tengo una migraña insoportable.", isLeftCharacter = false },
            new DialogueLine { characterName = "Doctor", text = "La mayor parte de trabajar en una clínica es averiguar cuál es el problema a través de diagnósticos.", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "A ver, PLAYER_NAME ¿Qué preguntas le puedes hacer a Dalia para determinar su problema?", isLeftCharacter = true }
        };
    }

    private string CleanText(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";

        return text.Replace("PLAYER_NAME", playerName)
                   .Replace("PlayerName", playerName)
                   .Replace("PLAYERNAME", playerName);
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        DialogueLine line = linesList[currentLine];

        leftNameText.gameObject.SetActive(false);
        rightNameText.gameObject.SetActive(false);

        if (leftNameBackground != null) leftNameBackground.SetActive(false);
        if (rightNameBackground != null) rightNameBackground.SetActive(false);

        // Procesamiento en directo del nombre del personaje
        string charName = line.characterName;
        if (IsPlayerCharacter(charName))
        {
            charName = playerName;
        }

        if (!string.IsNullOrEmpty(charName))
        {
            if (line.isLeftCharacter)
            {
                leftNameText.gameObject.SetActive(true);
                if (leftNameBackground != null) leftNameBackground.SetActive(true);
                leftNameText.text = charName;
            }
            else
            {
                rightNameText.gameObject.SetActive(true);
                if (rightNameBackground != null) rightNameBackground.SetActive(true);
                rightNameText.text = charName;
            }
        }

        leftCharacterImage.gameObject.SetActive(true);
        ApplyCharacterSize(leftCharacterImage, doctorSize);

        UpdateRightCharacter(line);

        if (line.isLeftCharacter)
        {
            leftCharacterImage.sprite = doctorTalking;
            SetRightCharacterIdle(GetActiveRightCharacterName());
        }
        else
        {
            leftCharacterImage.sprite = doctorIdle;

            switch (line.characterName)
            {
                case "Denis": rightCharacterImage.sprite = denisTalking; break;
                case "Secretaria": rightCharacterImage.sprite = secretariaTalking; break;
                case "Dalia": rightCharacterImage.sprite = daliaTalking; break;
            }
        }

        // Limpia el texto de la frase en vivo antes de escribir carácter por carácter
        string processedText = CleanText(line.text);

        foreach (char c in processedText)
        {
            dialogueText.text += c;
            yield return textSpeedWait;
        }

        isTyping = false;
        SetIdleSprites();

        CheckForQuestionTriggers();
    }

    private void CheckForQuestionTriggers()
    {
        DialogueLine current = linesList[currentLine];

        if (current.characterName == "TRIGGER_PANEL" || 
           (current.characterName == "Doctor" && current.text.Contains("¿Qué preguntas le puedes hacer a Dalia")))
        {
            if (askedHeadache && askedAntecedents && askedCough)
            {
                TriggerFinalDoctorDiagnosis();
            }
            else
            {
                ShowMainQuestions();
            }
        }
        else if (current.characterName == "TRIGGER_SUB_PREG_1")
        {
            ShowSubPreguntas1();
        }
        else if (current.characterName == "TRIGGER_SUB_PREG_3")
        {
            ShowSubPregunta3();
        }
        else if (current.characterName == "Doctor" && current.text.Contains("¿tú qué crees que sea?"))
        {
            ShowDiagnosisPanel();
        }
    }

    // ==========================================
    // FLUJO DE PANELES Y BOTONES
    // ==========================================

    void ShowMainQuestions()
    {
        isWaitingForChoice = true;
        panelPreguntas.SetActive(true);
        panelBotones.SetActive(true);

        panelPreg1SubPreguntas.SetActive(false);
        panelPreg3SubPregunta.SetActive(false);

        if (btnPreg1 != null) btnPreg1.gameObject.SetActive(!askedHeadache);
        if (btnPreg2 != null) btnPreg2.gameObject.SetActive(!askedAntecedents);
        if (btnPreg3 != null) btnPreg3.gameObject.SetActive(!askedCough);
    }

    void SelectPregunta1()
    {
        HideAllPanels();
        isWaitingForChoice = false;

        List<DialogueLine> lines = new List<DialogueLine>
        {
            new DialogueLine { characterName = playerName, text = "¿Has sentido algún dolor de cabeza?", isLeftCharacter = true },
            new DialogueLine { characterName = "Dalia", text = "No puntualmente, siento dolores de cabeza seguidos, no creo que tengan algo que ver. Aunque últimamente he sentido el cuerpo decaído como si tuviera fiebre.", isLeftCharacter = false },
            new DialogueLine { characterName = "TRIGGER_SUB_PREG_1", text = "", isLeftCharacter = false }
        };
        InsertDialogueSequence(lines);
    }

    void ShowSubPreguntas1()
    {
        isWaitingForChoice = true;
        panelPreguntas.SetActive(true);
        panelBotones.SetActive(false);
        panelPreg1SubPreguntas.SetActive(true);
    }

    void SelectSubPregunta1_1()
    {
        HideAllPanels();
        isWaitingForChoice = false;

        List<DialogueLine> lines = new List<DialogueLine>
        {
            new DialogueLine { characterName = playerName, text = "¿Los dolores de cabeza son normales?", isLeftCharacter = true },
            new DialogueLine { characterName = "Dalia", text = "Bueno… Sí. Y más dolor de cabeza tengo los fines de semana.", isLeftCharacter = false },
            new DialogueLine { characterName = "TRIGGER_SUB_PREG_1", text = "", isLeftCharacter = false }
        };
        
        if (btnSubPreg1_2 != null && !btnSubPreg1_2.gameObject.activeSelf)
        {
            askedHeadache = true;
            lines[lines.Count - 1] = new DialogueLine { characterName = "TRIGGER_PANEL", text = "", isLeftCharacter = false };
        }

        if (btnSubPreg1_1 != null) btnSubPreg1_1.gameObject.SetActive(false);
        InsertDialogueSequence(lines);
    }

    void SelectSubPregunta1_2()
    {
        HideAllPanels();
        isWaitingForChoice = false;

        List<DialogueLine> lines = new List<DialogueLine>
        {
            new DialogueLine { characterName = playerName, text = "¿Fumás o consumís alcohol seguido?", isLeftCharacter = true },
            new DialogueLine { characterName = "Dalia", text = "Sí, suelo tomar y fumo todos los días desde hace ya cinco años. Por eso decía que tengo dolores de cabeza…", isLeftCharacter = false },
            new DialogueLine { characterName = "TRIGGER_SUB_PREG_1", text = "", isLeftCharacter = false }
        };

        if (btnSubPreg1_1 != null && !btnSubPreg1_1.gameObject.activeSelf)
        {
            askedHeadache = true;
            lines[lines.Count - 1] = new DialogueLine { characterName = "TRIGGER_PANEL", text = "", isLeftCharacter = false };
        }

        if (btnSubPreg1_2 != null) btnSubPreg1_2.gameObject.SetActive(false);
        InsertDialogueSequence(lines);
    }

    void SelectPregunta2()
    {
        HideAllPanels();
        isWaitingForChoice = false;
        askedAntecedents = true;

        List<DialogueLine> lines = new List<DialogueLine>
        {
            new DialogueLine { characterName = playerName, text = "¿Has tenido antecedentes de enfermedades como el asma o diabetes?", isLeftCharacter = true },
            new DialogueLine { characterName = "Dalia", text = "Asma. Desde chica.", isLeftCharacter = false },
            new DialogueLine { characterName = "TRIGGER_PANEL", text = "", isLeftCharacter = false }
        };
        InsertDialogueSequence(lines);
    }

    void SelectPregunta3()
    {
        HideAllPanels();
        isWaitingForChoice = false;

        List<DialogueLine> lines = new List<DialogueLine>
        {
            new DialogueLine { characterName = playerName, text = "¿Tienes tos?", isLeftCharacter = true },
            new DialogueLine { characterName = "Dalia", text = "Sí. ¿No lo había mencionado ya?", isLeftCharacter = false },
            new DialogueLine { characterName = "TRIGGER_SUB_PREG_3", text = "", isLeftCharacter = false }
        };
        InsertDialogueSequence(lines);
    }

    void ShowSubPregunta3()
    {
        isWaitingForChoice = true;
        panelPreguntas.SetActive(true);
        panelBotones.SetActive(false);
        panelPreg3SubPregunta.SetActive(true);
    }

    void SelectSubPregunta3_1()
    {
        HideAllPanels();
        isWaitingForChoice = false;
        askedCough = true;

        List<DialogueLine> lines = new List<DialogueLine>
        {
            new DialogueLine { characterName = playerName, text = "¿Expulsa flema y de qué color o aspecto es?", isLeftCharacter = true },
            new DialogueLine { characterName = "Dalia", text = "Eh… Sí, cuando toso expulso flema o algo parecido…", isLeftCharacter = false },
            new DialogueLine { characterName = "Dalia", text = "Es un algo rojizo amarronado… ¿Es normal?", isLeftCharacter = false },
            new DialogueLine { characterName = "TRIGGER_PANEL", text = "", isLeftCharacter = false }
        };
        InsertDialogueSequence(lines);
    }

    void TriggerFinalDoctorDiagnosis()
    {
        List<DialogueLine> finalLines = new List<DialogueLine>
        {
            new DialogueLine { characterName = playerName, text = "Está bien.", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "Bueno, yo creo que ya sé qué es… PLAYER_NAME, ¿tú qué crees que sea?", isLeftCharacter = true }
        };
        InsertDialogueSequence(finalLines);
    }

    void ShowDiagnosisPanel()
    {
        isWaitingForChoice = true;
        if (panelDiagnostico != null) panelDiagnostico.SetActive(true);
    }

    void SelectDiagnosis(bool isCorrect)
    {
        if (panelDiagnostico != null) panelDiagnostico.SetActive(false);
        isWaitingForChoice = false;

        List<DialogueLine> diagnosisResult = new List<DialogueLine>();

        if (isCorrect)
        {
            diagnosisResult.Add(new DialogueLine { characterName = "Doctor", text = "Yo también creo que es neumonía. Ahora revisemos adentro…", isLeftCharacter = true });
            diagnosisResult.Add(new DialogueLine { characterName = "Doctor", text = "¡A buscar el problema!", isLeftCharacter = true });
        }
        else
        {
            diagnosisResult.Add(new DialogueLine { characterName = "Doctor", text = "Mmm..", isLeftCharacter = true });
            diagnosisResult.Add(new DialogueLine { characterName = "Doctor", text = "No lo sé. Yo creo que es neumonía.", isLeftCharacter = true });
            diagnosisResult.Add(new DialogueLine { characterName = "Doctor", text = "Aún así, hay que revisar el problema desde adentro.", isLeftCharacter = true });
            diagnosisResult.Add(new DialogueLine { characterName = "Doctor", text = "¡Viajemos por los bronquios!", isLeftCharacter = true });
        }

        InsertDialogueSequence(diagnosisResult);
    }

    void InsertDialogueSequence(List<DialogueLine> newLines)
    {
        for (int i = 0; i < newLines.Count; i++)
        {
            linesList.Insert(currentLine + 1 + i, newLines[i]);
        }
        NextLine();
    }

    // ==========================================
    // CONTROL DE VIDEO, AUDIO Y CAMBIO DE ESCENA
    // ==========================================

    void ReproducirVideoFinal()
    {
        if (musicaFondo != null)
        {
            musicaFondo.mute = true;
        }

        dialogueText.text = "";
        leftNameText.text = "";
        rightNameText.text = "";

        leftCharacterImage.gameObject.SetActive(false);
        rightCharacterImage.gameObject.SetActive(false);

        if (leftNameBackground != null) leftNameBackground.SetActive(false);
        if (rightNameBackground != null) rightNameBackground.SetActive(false);

        if (videoDisplayObject != null && videoPlayer != null)
        {
            videoDisplayObject.SetActive(true);
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.Play();
        }
        else
        {
            CambiarDeEscena();
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        vp.loopPointReached -= OnVideoEnd;
        CambiarDeEscena();
    }

    void CambiarDeEscena()
    {
        SceneManager.LoadScene(numeroEscenaSiguiente);
    }

    // ==========================================
    // CONTROL DE PERSONAJES Y VISUALIZACIÓN
    // ==========================================

    private void UpdateRightCharacter(DialogueLine currentLineData)
    {
        string character = GetActiveRightCharacterName();

        if (string.IsNullOrEmpty(character))
        {
            rightCharacterImage.gameObject.SetActive(false);
            return;
        }

        rightCharacterImage.gameObject.SetActive(true);

        switch (character)
        {
            case "Denis": ApplyCharacterSize(rightCharacterImage, denisSize); break;
            case "Secretaria": ApplyCharacterSize(rightCharacterImage, secretariaSize); break;
            case "Dalia": ApplyCharacterSize(rightCharacterImage, daliaSize); break;
            default: rightCharacterImage.gameObject.SetActive(false); break;
        }
    }

    private string GetActiveRightCharacterName()
    {
        DialogueLine line = linesList[currentLine];

        if (line.characterName == "Denis" || line.characterName == "Secretaria" || line.characterName == "Dalia")
            return line.characterName;

        if (string.IsNullOrEmpty(line.characterName) || line.characterName.StartsWith("TRIGGER_"))
            return "";

        for (int i = currentLine - 1; i >= 0; i--)
        {
            string prevName = linesList[i].characterName;
            if (prevName == "Denis" || prevName == "Secretaria" || prevName == "Dalia") return prevName;
            if (string.IsNullOrEmpty(prevName)) return "";
        }

        return "";
    }

    private void SetRightCharacterIdle(string characterName)
    {
        switch (characterName)
        {
            case "Denis": rightCharacterImage.sprite = denisIdle; break;
            case "Secretaria": rightCharacterImage.sprite = secretariaIdle; break;
            case "Dalia": rightCharacterImage.sprite = daliaIdle; break;
        }
    }

    private void SetIdleSprites()
    {
        leftCharacterImage.sprite = doctorIdle;
        SetRightCharacterIdle(GetActiveRightCharacterName());
    }

    void CompleteLine()
    {
        StopAllCoroutines();
        DialogueLine line = linesList[currentLine];
        dialogueText.text = CleanText(line.text);
        isTyping = false;

        UpdateRightCharacter(line);
        SetIdleSprites();
        CheckForQuestionTriggers();
    }

    void NextLine()
    {
        currentLine++;

        if (currentLine >= linesList.Count)
        {
            ReproducirVideoFinal();
            return;
        }

        if (linesList[currentLine].characterName.StartsWith("TRIGGER_"))
        {
            CheckForQuestionTriggers();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }
}