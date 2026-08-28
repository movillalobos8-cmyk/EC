using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    public string text;

    public string characterName;

    public Sprite talkingSprite;

    public Sprite idleSprite;

    public bool isLeftCharacter;

    public bool showBothCharacters = false;

    public bool importantLine = false;
}

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text leftNameText;
    public TMP_Text rightNameText;
    public GameObject leftNameBackground;
    public GameObject rightNameBackground;
    public TMP_Text dialogueText;

    [Header("Ending")]
    public GameObject videoPanel;
    public VideoPlayer endingVideo;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip secretariaMusic;
    public AudioClip doctorMusic;

    [Header("Characters")]
    public Image leftCharacterImage;
    public Image rightCharacterImage;

    [Header("Character Sizes")]
    public Vector2 secretariaSize = new Vector2(600, 900);
    public Vector2 denisSize = new Vector2(700, 1000);
    public Vector2 doctorSize = new Vector2(650, 950);

    [Header("Background")]
    public Image backgroundImage;

    public Sprite secretariaBackground;
    public Sprite doctorBackground;

    [Header("Secretaria")]
    public Sprite secretariaTalking;
    public Sprite secretariaIdle;

    [Header("Doctor")]
    public Sprite doctorTalking;
    public Sprite doctorIdle;

    [Header("Denis")]
    public Sprite denisTalking;
    public Sprite denisIdle;

    [Header("Input Nombre")]
    public TMP_InputField nameInput;
    public GameObject inputPanel;

    [Header("Multiple Choice")]
    public GameObject choicePanel;

    public Button option1Button;
    public Button option2Button;
    public Button option3Button;

    [Header("Definitions")]
    public GameObject definition1;
    public GameObject definition2;
    public GameObject definition3;

    [Header("Medical Panels")]
    public GameObject hinchazonPanel1;
    public GameObject hinchazonPanel2;

    public GameObject diagnosticoPanel1;
    public GameObject diagnosticoPanel2;

    [Header("Config")]
    public float textSpeed = 0.05f;

    string playerName = "";

    bool askedName = false;
    bool isTyping = false;
    bool waitingChoice = false;
    bool answeredQuestion = false;

    bool endingReady = false;

    int currentLine = 0;

    public DialogueLine[] lines;

    private Coroutine typeRoutine;

    void Awake()
    {
        VideoPlayer[] vps = FindObjectsOfType<VideoPlayer>();

        foreach (var vp in vps)
        {
            Debug.Log("VideoPlayer encontrado: " + vp.gameObject.name);
            vp.Stop();
            vp.enabled = false;
        }
    }

    void Start()
    {
        Debug.Log("START");

        if (endingVideo != null)
        {
            endingVideo.Stop();
            endingVideo.frame = 0;
            endingVideo.time = 0;
            endingVideo.enabled = false;
        }

        Debug.Log("1");
        hinchazonPanel1.SetActive(false);

        Debug.Log("2");
        hinchazonPanel2.SetActive(false);

        Debug.Log("3");
        diagnosticoPanel1.SetActive(false);

        Debug.Log("4");
        diagnosticoPanel2.SetActive(false);

        Debug.Log("5");
        definition1.SetActive(false);

        Debug.Log("6");
        definition2.SetActive(false);

        Debug.Log("7");
        definition3.SetActive(false);

        Debug.Log("8");
        videoPanel.SetActive(false);

        leftNameText.gameObject.SetActive(false);
        rightNameText.gameObject.SetActive(false);
        leftNameBackground.SetActive(false);
        rightNameBackground.SetActive(false);

        lines = new DialogueLine[]
        {
            // SECRETARIA
            new DialogueLine
            {
                characterName = "Secretaria",
                text = "Buenos días, joven.",
                talkingSprite = secretariaTalking,
                idleSprite = secretariaIdle,
                isLeftCharacter = true
            },

            new DialogueLine
            {
                characterName = "Secretaria",
                text = "Soy la secretaria del doctor Jacinto, seré tu guía hoy.",
                talkingSprite = secretariaTalking,
                idleSprite = secretariaIdle,
                isLeftCharacter = true
            },

            new DialogueLine
            {
                characterName = "Secretaria",
                text = "Recuérdame algo, ¿Cuál es tu nombre?",
                talkingSprite = secretariaTalking,
                idleSprite = secretariaIdle,
                isLeftCharacter = true,
                importantLine = true
            },

            // SECRETARIA CONTINÚA
            new DialogueLine
            {
                characterName = "Secretaria",
                text = "Oh! PLAYER_NAME, sí, claro.",
                talkingSprite = secretariaTalking,
                idleSprite = secretariaIdle,
                isLeftCharacter = true
            },

            new DialogueLine
            {
                characterName = "Secretaria",
                text = "Lo había leído esta mañana solo que no me acordaba. Entre tantos documentos y archivos es difícil acordarme de todos los nombres.",
                talkingSprite = secretariaTalking,
                idleSprite = secretariaIdle,
                isLeftCharacter = true
            },

            new DialogueLine
            {
                characterName = "Secretaria",
                text = "Vení, pasá. El doctor te está esperando.",
                talkingSprite = secretariaTalking,
                idleSprite = secretariaIdle,
                isLeftCharacter = true
            },

            // DOCTOR SOLO
            new DialogueLine
            {
                characterName = "Doctor Jacinto",
                text = "Hola, soy el doctor Jacinto.",
                talkingSprite = doctorTalking,
                idleSprite = doctorIdle,
                isLeftCharacter = false
            },

            new DialogueLine
            {
                characterName = "Doctor Jacinto",
                text = "Un placer conocerte PLAYER_NAME.",
                talkingSprite = doctorTalking,
                idleSprite = doctorIdle,
                isLeftCharacter = false
            },

            new DialogueLine
            {
                characterName = "Doctor Jacinto",
                text = "¿Estás preparando tu tesis, no? Sé que vas a ser un/a gran doctor/a algún día.",
                talkingSprite = doctorTalking,
                idleSprite = doctorIdle,
                isLeftCharacter = false
            },

            new DialogueLine
            {
                characterName = "Doctor Jacinto",
                text = "Para empezar, nuestra clínica cuenta con una particularidad.",
                talkingSprite = doctorTalking,
                idleSprite = doctorIdle,
                isLeftCharacter = false
            },

            // AMBOS
            new DialogueLine
            {
                characterName = "Secretaria",
                text = "Sí, tenemos una máquina que nos reduce al tamaño de un átomo. Bueno, un milímetro más grande.",
                talkingSprite = secretariaTalking,
                idleSprite = secretariaIdle,
                isLeftCharacter = true,
                showBothCharacters = true
            },

            new DialogueLine
            {
                characterName = "Secretaria",
                text = "Con esta máquina podemos sanar enfermedades desde adentro del cuerpo.",
                talkingSprite = secretariaTalking,
                idleSprite = secretariaIdle,
                isLeftCharacter = true,
                showBothCharacters = true
            },

            new DialogueLine
            {
                characterName = "Doctor Jacinto",
                text = "Ahora sí, ya entendiste así que dejemos pasar al primer paciente.",
                talkingSprite = doctorTalking,
                idleSprite = doctorIdle,
                isLeftCharacter = false,
                showBothCharacters = true
            },

            // SECRETARIA SALE
            new DialogueLine
            {
                characterName = "Secretaria",
                text = "El paciente del turno de las 09:00 a.m. ya está aquí. Lo haré pasar. Un momento.",
                talkingSprite = secretariaTalking,
                idleSprite = secretariaIdle,
                isLeftCharacter = true,
                showBothCharacters = true
            },

            // DOCTOR SOLO
            new DialogueLine
            {
                characterName = "Doctor Jacinto",
                text = "Me honra muchísimo que nuestra clínica sea parte de tu informe para la universidad.",
                talkingSprite = doctorTalking,
                idleSprite = doctorIdle,
                isLeftCharacter = false
            },

            new DialogueLine
            {
                characterName = "Doctor Jacinto",
                text = "Estoy muy seguro que tu presencia me será de gran ayuda hoy.",
                talkingSprite = doctorTalking,
                idleSprite = doctorIdle,
                isLeftCharacter = false
            },

            new DialogueLine
            {
                characterName = "Doctor Jacinto",
                text = "Ya estoy viendo al primer paciente. Dejemos que pase.",
                talkingSprite = doctorTalking,
                idleSprite = doctorIdle,
                isLeftCharacter = false
            },

            // DENIS ENTRA
            new DialogueLine
            {
                characterName = "Denis",
                text = "Buenos días doctor. Vine hoy porque desde ayer siento el brazo muy pesado.",
                talkingSprite = denisTalking,
                idleSprite = denisIdle,
                isLeftCharacter = true,
                showBothCharacters = true
            },

            new DialogueLine
            {
                characterName = "Denis",
                text = "Primero pensé que era cansancio, pero empezó a dolerme incluso cuando no hago fuerza.",
                talkingSprite = denisTalking,
                idleSprite = denisIdle,
                isLeftCharacter = true,
                showBothCharacters = true
            },

            new DialogueLine
            {
                characterName = "Denis",
                text = "Y mire… esta parte está más hinchada que la otra.",
                talkingSprite = denisTalking,
                idleSprite = denisIdle,
                isLeftCharacter = true,
                showBothCharacters = true
            },

            // ÚLTIMO DIÁLOGO
            new DialogueLine
            {
                characterName = "Doctor Jacinto",
                text = "Está bien, me imagino que puede ser. Tiene que ver con la sangre. PLAYER_NAME: ¿Tú qué crees que sea?",
                talkingSprite = doctorTalking,
                idleSprite = doctorIdle,
                isLeftCharacter = false,
                showBothCharacters = true,
                importantLine = true
            }
        };

        Debug.Log("ANTES DE TYPELINE");

        Debug.Log("9");
        if (musicSource != null && secretariaMusic != null)
        {
            musicSource.clip = secretariaMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        StartNextTypeLine();
    }

    void ApplyCharacterSize(Image image, Vector2 size)
    {
        image.rectTransform.sizeDelta = size;
    }

    void Update()
    {
        if (inputPanel.activeSelf)
            return;

        if (waitingChoice)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                CompleteLine();
            }
            else
            {
                if (endingReady)
                {
                    endingReady = false;
                    PlayEndingVideo();
                    return;
                }

                if (!answeredQuestion && currentLine == lines.Length - 1)
                {
                    ShowChoices();
                    return;
                }

                NextLine();
            }
        }
    }

    void StartNextTypeLine()
    {
        if (typeRoutine != null)
        {
            StopCoroutine(typeRoutine);
        }
        typeRoutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;

        dialogueText.text = "";

        DialogueLine line = lines[currentLine];

        // Ocultar paneles de hinchazón por defecto
        hinchazonPanel1.SetActive(false);
        hinchazonPanel2.SetActive(false);

        // Mostrar paneles de hinchazón sólo en esta frase específica
        if (line.text.Contains("esta parte está más hinchada que la otra"))
        {
            hinchazonPanel1.SetActive(true);
            hinchazonPanel2.SetActive(true);
        }

        // Mostrar paneles de diagnóstico en la última pregunta
        if (currentLine == lines.Length - 1)
        {
            diagnosticoPanel1.SetActive(true);
            diagnosticoPanel2.SetActive(true);
        }

        leftNameText.gameObject.SetActive(false);
        rightNameText.gameObject.SetActive(false);
        leftNameText.text = "";
        rightNameText.text = "";

        if (line.isLeftCharacter)
        {
            leftNameText.gameObject.SetActive(true);
            leftNameText.text = line.characterName;
        }
        else
        {
            rightNameText.gameObject.SetActive(true);
            rightNameText.text = line.characterName;
        }

        leftNameBackground.SetActive(false);
        rightNameBackground.SetActive(false);

        if (line.isLeftCharacter)
        {
            leftNameBackground.SetActive(true);
        }
        else
        {
            rightNameBackground.SetActive(true);
        }

        // FONDO
        if (line.showBothCharacters)
        {
            backgroundImage.sprite = doctorBackground;
        }
        else
        {
            if (line.characterName == "Secretaria")
            {
                backgroundImage.sprite = secretariaBackground;
            }
            else
            {
                backgroundImage.sprite = doctorBackground;
            }
        }

        // PERSONAJES
        if (line.showBothCharacters)
        {
            leftCharacterImage.gameObject.SetActive(true);
            rightCharacterImage.gameObject.SetActive(true);

            if (line.characterName == "Secretaria")
            {
                ApplyCharacterSize(leftCharacterImage, secretariaSize);
                ApplyCharacterSize(rightCharacterImage, doctorSize);

                leftCharacterImage.sprite = secretariaTalking;
                rightCharacterImage.sprite = doctorIdle;
            }
            else if (line.characterName == "Denis")
            {
                ApplyCharacterSize(leftCharacterImage, denisSize);
                ApplyCharacterSize(rightCharacterImage, doctorSize);

                leftCharacterImage.sprite = denisTalking;
                rightCharacterImage.sprite = doctorIdle;
            }
            else
            {
                ApplyCharacterSize(rightCharacterImage, doctorSize);

                if (currentLine >= 17)
                {
                    ApplyCharacterSize(leftCharacterImage, denisSize);
                    leftCharacterImage.sprite = denisIdle;
                }
                else
                {
                    ApplyCharacterSize(leftCharacterImage, secretariaSize);
                    leftCharacterImage.sprite = secretariaIdle;
                }

                rightCharacterImage.sprite = doctorTalking;
            }
        }
        else
        {
            if (line.isLeftCharacter)
            {
                leftCharacterImage.gameObject.SetActive(true);
                rightCharacterImage.gameObject.SetActive(false);

                if (line.characterName == "Secretaria")
                {
                    ApplyCharacterSize(leftCharacterImage, secretariaSize);
                }

                if (line.characterName == "Denis")
                {
                    ApplyCharacterSize(leftCharacterImage, denisSize);
                }

                leftCharacterImage.sprite = line.talkingSprite;
            }
            else
            {
                leftCharacterImage.gameObject.SetActive(false);
                rightCharacterImage.gameObject.SetActive(true);

                ApplyCharacterSize(rightCharacterImage, doctorSize);

                rightCharacterImage.sprite = line.talkingSprite;
            }
        }

        // TEXTO
        foreach (char c in line.text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;

        // IDLE
        SetCharacterIdle(line);
    }

    void CompleteLine()
    {
        if (typeRoutine != null)
        {
            StopCoroutine(typeRoutine);
        }

        DialogueLine line = lines[currentLine];
        dialogueText.text = line.text;
        isTyping = false;

        SetCharacterIdle(line);
    }

    void SetCharacterIdle(DialogueLine line)
    {
        if (line.showBothCharacters)
        {
            if (line.characterName == "Secretaria")
            {
                leftCharacterImage.sprite = secretariaIdle;
            }
            else if (line.characterName == "Denis")
            {
                leftCharacterImage.sprite = denisIdle;
            }
            else
            {
                rightCharacterImage.sprite = doctorIdle;
            }
        }
        else
        {
            if (line.isLeftCharacter)
            {
                leftCharacterImage.sprite = line.idleSprite;
            }
            else
            {
                rightCharacterImage.sprite = line.idleSprite;
            }
        }
    }

    void NextLine()
    {
        currentLine++;

        if (currentLine == 6 && musicSource != null && doctorMusic != null)
        {
            musicSource.Stop();
            musicSource.clip = doctorMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        // PEDIR NOMBRE
        if (!askedName && currentLine == 3)
        {
            askedName = true;
            inputPanel.SetActive(true);
            return;
        }

        // FIN
        if (currentLine >= lines.Length)
        {
            dialogueText.text = "";
            return;
        }

        StartNextTypeLine();
    }

    public void ConfirmName()
    {
        string input = nameInput.text;
        input = input.Trim();

        if (input.Length < 1)
        {
            dialogueText.text = "Debes escribir tu nombre o no podremos continuar";
            return;
        }

        playerName = input;

        PlayerPrefs.SetString("NombreJugador", playerName);
        PlayerPrefs.Save();

        inputPanel.SetActive(false);

        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].text = lines[i].text.Replace("PLAYER_NAME", playerName);
        }

        StartNextTypeLine();
    }

    void ShowChoices()
    {
        waitingChoice = true;
        choicePanel.SetActive(true);

        option1Button.onClick.RemoveAllListeners();
        option2Button.onClick.RemoveAllListeners();
        option3Button.onClick.RemoveAllListeners();

        option1Button.onClick.AddListener(() =>
        {
            WrongAnswer("fractura muscular");
        });

        option2Button.onClick.AddListener(() =>
        {
            WrongAnswer("infección respiratoria");
        });

        option3Button.onClick.AddListener(() =>
        {
            CorrectAnswer();
        });
    }

    void WrongAnswer(string wrongCondition)
    {
        answeredQuestion = true;
        choicePanel.SetActive(false);
        waitingChoice = false;

        diagnosticoPanel1.SetActive(false);
        diagnosticoPanel2.SetActive(false);

        dialogueText.text =
        "Equivocarse es parte del aprendizaje. No es " +
        wrongCondition +
        ". Se trata de una trombosis. De todas maneras, hay que analizar el problema desde dentro. Vamos.";

        endingReady = true;
    }

    void CorrectAnswer()
    {
        answeredQuestion = true;
        choicePanel.SetActive(false);
        waitingChoice = false;

        diagnosticoPanel1.SetActive(false);
        diagnosticoPanel2.SetActive(false);

        dialogueText.text =
        "Correcto, hay que analizar el problema desde dentro. Ven.";

        endingReady = true;
    }

    public void PlayEndingVideo()
    {
        StartCoroutine(EndingSequence());
    }

    public void ShowDefinition1() { definition1.SetActive(true); }
    public void HideDefinition1() { definition1.SetActive(false); }

    public void ShowDefinition2() { definition2.SetActive(true); }
    public void HideDefinition2() { definition2.SetActive(false); }

    public void ShowDefinition3() { definition3.SetActive(true); }
    public void HideDefinition3() { definition3.SetActive(false); }

    IEnumerator EndingSequence()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        videoPanel.SetActive(true);

        endingVideo.enabled = true;
        endingVideo.Stop();

        endingVideo.Prepare();
        yield return new WaitUntil(() => endingVideo.isPrepared);

        endingVideo.Play();

        yield return new WaitUntil(() => !endingVideo.isPlaying);

        SceneManager.LoadScene(0);
    }
}