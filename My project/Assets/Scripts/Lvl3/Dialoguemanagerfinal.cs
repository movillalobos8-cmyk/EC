using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class DialogueManagerFinal : MonoBehaviour
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
    public Vector2 maiteSize = new Vector2(600, 900);
    public Vector2 ferSize = new Vector2(650, 950);

    [Header("Doctor (Izquierda)")]
    public Sprite doctorTalking;
    public Sprite doctorIdle;

    [Header("Maite (Derecha)")]
    public Sprite maiteTalking;
    public Sprite maiteIdle;

    [Header("Fer (Derecha)")]
    public Sprite ferTalking;
    public Sprite ferIdle;

    [Header("Final (Video opcional y Escena)")]
    [Tooltip("Opcional: si lo dejas vacío, al terminar los diálogos cambia de escena directamente")]
    public GameObject videoDisplayObject;
    public VideoPlayer videoPlayer;
    [Tooltip("Build Index de la escena a cargar al terminar (ej: 0 = menú / créditos)")]
    public int numeroEscenaSiguiente = 0;

    [Header("Config")]
    public float textSpeed = 0.05f;

    [Header("Forzar tamaños")]
    [Tooltip("Solo si lo activas, se fuerza Preserve Aspect = false. Por defecto se respeta lo que tengas en el Inspector")]
    public bool desactivarPreserveAspect = false;

    private string playerName = "Jugador";
    private bool isTyping = false;
    private bool terminado = false;
    private int currentLine = 0;

    private WaitForSeconds textSpeedWait;
    public List<DialogueLine> linesList = new List<DialogueLine>();

    void Start()
    {
        textSpeedWait = new WaitForSeconds(textSpeed);

        LoadPlayerName();

        PrepararImagen(leftCharacterImage);
        PrepararImagen(rightCharacterImage);

        Debug.Log("[DialogueManagerFinal] ACTIVO en '" + gameObject.name + "' | Doctor=" + doctorSize + " Maite=" + maiteSize + " Fer=" + ferSize);

        if (leftNameBackground != null) leftNameBackground.SetActive(false);
        if (rightNameBackground != null) rightNameBackground.SetActive(false);
        if (videoDisplayObject != null) videoDisplayObject.SetActive(false);

        BuildInitialDialogue();
        StartCoroutine(TypeLine());
    }

    private void LoadPlayerName()
    {
        string[] claves = { "NombreJugador", "PlayerName", "Nombre", "UserName", "Name" };
        foreach (string k in claves)
        {
            if (PlayerPrefs.HasKey(k) && !string.IsNullOrEmpty(PlayerPrefs.GetString(k)))
            {
                playerName = PlayerPrefs.GetString(k);
                break;
            }
        }
        Debug.Log("Nombre de jugador cargado en DialogueManagerFinal: " + playerName);
    }

    private bool IsPlayerCharacter(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        string lower = name.ToLower().Trim();
        return lower == "player_name" || lower == "nombre" || lower == "jugador" || lower == playerName.ToLower().Trim();
    }

    // Elimina todo lo que pueda pisar el tamaño de la imagen
    void PrepararImagen(Image image)
    {
        if (image == null) return;

        if (desactivarPreserveAspect) image.preserveAspect = false;

        // Un Content Size Fitter pisa el tamaño en cada fotograma
        ContentSizeFitter fitter = image.GetComponent<ContentSizeFitter>();
        if (fitter != null)
        {
            Debug.LogWarning("[DialogueManagerFinal] '" + image.name + "' tenía ContentSizeFitter: se desactiva.");
            fitter.enabled = false;
        }

        // Hace que un Layout Group del padre ignore esta imagen
        LayoutElement le = image.GetComponent<LayoutElement>();
        if (le == null) le = image.gameObject.AddComponent<LayoutElement>();
        le.ignoreLayout = true;

        // Un Animator con Width/Height/Scale sobrescribe los valores
        Animator anim = image.GetComponent<Animator>();
        if (anim != null)
            Debug.LogWarning("[DialogueManagerFinal] '" + image.name + "' tiene un Animator: puede estar sobrescribiendo el tamaño.");
    }

    // Igual que en DialogueManager3: se asigna sizeDelta y NO se toca la escala del objeto
    void ApplyCharacterSize(Image image, Vector2 size)
    {
        if (image == null || image.rectTransform == null) return;
        image.rectTransform.sizeDelta = size;
    }

    // Reaplica los tamaños en cada fotograma para que ningún Layout/Animator los pise
    void LateUpdate()
    {
        if (terminado || linesList == null || linesList.Count == 0) return;

        ApplyCharacterSize(leftCharacterImage, doctorSize);

        switch (GetActiveRightCharacterName())
        {
            case "Maite": ApplyCharacterSize(rightCharacterImage, maiteSize); break;
            case "Fer":   ApplyCharacterSize(rightCharacterImage, ferSize);   break;
        }
    }

    void Update()
    {
        if (terminado) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0))
        {
            if (isTyping) CompleteLine();
            else NextLine();
        }
    }

    void BuildInitialDialogue()
    {
        linesList = new List<DialogueLine>
        {
            new DialogueLine { characterName = "Maite", text = "¿Y bien? ¿Qué es lo que tengo?", isLeftCharacter = false },

            new DialogueLine { characterName = "Doctor", text = "Malas noticias… La verdad me decepciona mucho decir esto pero… ¡No tenés nada fuera de lo común!", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "Tu cuerpo está funcionando perfectamente bien, así que no hay nada que preocuparse.", isLeftCharacter = true },

            new DialogueLine { characterName = "Fer", text = "Ya sabía yo…", isLeftCharacter = false },

            new DialogueLine { characterName = "Maite", text = "Uh…", isLeftCharacter = false },
            new DialogueLine { characterName = "Maite", text = "Bueno…", isLeftCharacter = false },
            new DialogueLine { characterName = "Maite", text = "Entonces…", isLeftCharacter = false },

            new DialogueLine { characterName = "Fer", text = "Entonces debemos ir a casa. No te pasa absolutamente nada.", isLeftCharacter = false },

            new DialogueLine { characterName = "Maite", text = "Gracias Doctor…", isLeftCharacter = false },
            new DialogueLine { characterName = "Maite", text = "Y gracias a ti también, PLAYER_NAME.", isLeftCharacter = false },
            new DialogueLine { characterName = "Maite", text = "Ahora me siento mucho mejor.", isLeftCharacter = false },

            new DialogueLine { characterName = "Doctor", text = "Vos siempre estuviste bien, Maite.", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "Y no hay ningún tipo de problema. Estamos acá para resolver dudas y asegurarnos de que no pase nada malo.", isLeftCharacter = true },

            new DialogueLine { characterName = "Fer", text = "Adiós, Doctor.", isLeftCharacter = false },

            new DialogueLine { characterName = "Maite", text = "¡Chauuuuu!", isLeftCharacter = false }
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

        string charName = line.characterName;
        if (IsPlayerCharacter(charName)) charName = playerName;

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

        UpdateRightCharacter();

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
                case "Maite": rightCharacterImage.sprite = maiteTalking; break;
                case "Fer": rightCharacterImage.sprite = ferTalking; break;
            }
        }

        string processedText = CleanText(line.text);

        foreach (char c in processedText)
        {
            dialogueText.text += c;
            yield return textSpeedWait;
        }

        isTyping = false;
        SetIdleSprites();
    }

    void CompleteLine()
    {
        StopAllCoroutines();
        DialogueLine line = linesList[currentLine];
        dialogueText.text = CleanText(line.text);
        isTyping = false;

        UpdateRightCharacter();
        SetIdleSprites();
    }

    void NextLine()
    {
        currentLine++;

        if (currentLine >= linesList.Count)
        {
            Finalizar();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    // ==========================================
    // FINAL: VIDEO OPCIONAL Y CAMBIO DE ESCENA
    // ==========================================

    void Finalizar()
    {
        terminado = true;

        if (musicaFondo != null) musicaFondo.mute = true;

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
    // CONTROL DE PERSONAJES
    // ==========================================

    private void UpdateRightCharacter()
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
            case "Maite": ApplyCharacterSize(rightCharacterImage, maiteSize); break;
            case "Fer": ApplyCharacterSize(rightCharacterImage, ferSize); break;
            default: rightCharacterImage.gameObject.SetActive(false); break;
        }
    }

    private bool IsRightCharacter(string n)
    {
        return n == "Maite" || n == "Fer";
    }

    // Devuelve quién está en pantalla a la derecha (el que habla, o el último que habló)
    private string GetActiveRightCharacterName()
    {
        DialogueLine line = linesList[currentLine];

        if (IsRightCharacter(line.characterName)) return line.characterName;
        if (string.IsNullOrEmpty(line.characterName)) return "";

        for (int i = currentLine - 1; i >= 0; i--)
        {
            string prevName = linesList[i].characterName;
            if (IsRightCharacter(prevName)) return prevName;
            if (string.IsNullOrEmpty(prevName)) return "";
        }

        return "";
    }

    private void SetRightCharacterIdle(string characterName)
    {
        switch (characterName)
        {
            case "Maite": rightCharacterImage.sprite = maiteIdle; break;
            case "Fer": rightCharacterImage.sprite = ferIdle; break;
        }
    }

    private void SetIdleSprites()
    {
        leftCharacterImage.sprite = doctorIdle;
        SetRightCharacterIdle(GetActiveRightCharacterName());
    }
}