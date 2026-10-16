using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 
using UnityEngine.Video;          

public class DialogueManager3 : MonoBehaviour
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
    public Vector2 secretariaSize = new Vector2(600, 900);
    public Vector2 daliaSize = new Vector2(650, 950);
    public Vector2 maiteSize = new Vector2(600, 900);
    public Vector2 ferSize = new Vector2(650, 950);

    [Header("Doctor (Izquierda)")]
    public Sprite doctorTalking;
    public Sprite doctorIdle;

    [Header("Secretaria (Derecha)")]
    public Sprite secretariaTalking;
    public Sprite secretariaIdle;

    [Header("Dalia (Derecha)")]
    public Sprite daliaTalking;
    public Sprite daliaIdle;

    [Header("Maite (Derecha)")]
    public Sprite maiteTalking;
    public Sprite maiteIdle;

    [Header("Fer (Derecha)")]
    public Sprite ferTalking;
    public Sprite ferIdle;

    [Header("Video Final y Siguiente Escena")]
    public GameObject videoDisplayObject;       
    public VideoPlayer videoPlayer;             
    public int numeroEscenaSiguiente = 6;       

    [Header("Config")]
    public float textSpeed = 0.05f;

    private string playerName = "Jugador";
    private bool isTyping = false;
    private int currentLine = 0;

    private WaitForSeconds textSpeedWait;
    public List<DialogueLine> linesList = new List<DialogueLine>();

    void Start()
    {
        textSpeedWait = new WaitForSeconds(textSpeed);
        
        // Carga y valida el nombre del jugador
        LoadPlayerName();

        if (leftNameBackground != null) leftNameBackground.SetActive(false);
        if (rightNameBackground != null) rightNameBackground.SetActive(false);
        if (videoDisplayObject != null) videoDisplayObject.SetActive(false);

        BuildInitialDialogue();
        StartCoroutine(TypeLine());
    }

    private void LoadPlayerName()
    {
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
        
        Debug.Log("Nombre de jugador cargado en DialogueManager3: " + playerName);
    }

    private bool IsPlayerCharacter(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        string lower = name.ToLower().Trim();
        return lower == "player_name" || lower == "nombre" || lower == "jugador" || lower == playerName.ToLower().Trim();
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
        // Se añade detección para Espacio, Enter (principal y teclado numérico) y Clic Izquierdo del mouse
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0))
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
            new DialogueLine { characterName = "Doctor", text = "Ufff… eso fue complicado…", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "Lo bueno es que ya terminamos y pudimos ayudar a Dalia.", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "Lo hiciste muy bien, PLAYER_NAME. Cada vez entiendes mejor cómo funciona nuestra clínica.", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "Te voy a recetar unos analgésicos para aliviar las molestias que todavía pueda sentir. En una semanita ya deberías sentirte perfectamente.", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "Lo importante ahora es que descanses y sigas las indicaciones.", isLeftCharacter = true },
            
            new DialogueLine { characterName = "Dalia", text = "No sé qué habrán hecho, pero me siento un poco mejor.", isLeftCharacter = false },
            new DialogueLine { characterName = "Dalia", text = "Sinceramente, no le tenía mucha fe a este tratamiento, pero la verdad es que me sorprendieron.", isLeftCharacter = false },
            new DialogueLine { characterName = "Dalia", text = "Principalmente vos, PLAYER_NAME.", isLeftCharacter = false },
            new DialogueLine { characterName = "Dalia", text = "Creo que después de todo hice bien en venir.", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Doctor", text = "Me alegra escuchar eso.", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "Ahora lo mejor es que descanses y dejés que tu cuerpo se recupere.", isLeftCharacter = true },
            
            new DialogueLine { characterName = "Dalia", text = "Lo voy a hacer, doctor.", isLeftCharacter = false },
            new DialogueLine { characterName = "Dalia", text = "Muchas gracias a los dos.", isLeftCharacter = false },
            new DialogueLine { characterName = "", text = "Dalia se va.", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Secretaria", text = "Este ha sido un problema más complicado, pero por suerte contamos con vos, PLAYER_NAME.", isLeftCharacter = false },
            new DialogueLine { characterName = "Secretaria", text = "Ya has aprendido bastante en estos últimos casos.", isLeftCharacter = false },
            new DialogueLine { characterName = "Secretaria", text = "Y ahora ha llegado la última paciente del turno.", isLeftCharacter = false },
            new DialogueLine { characterName = "Secretaria", text = "Mucha suerte.", isLeftCharacter = false },
            new DialogueLine { characterName = "", text = "Maitena entra corriendo al consultorio.", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Maite", text = "¡Doctoooor!", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Secretaria", text = "Maitena, te había dicho que esperaras a que te llamara…", isLeftCharacter = false },
            new DialogueLine { characterName = "Secretaria", text = "Además, no podés entrar sola al consultorio. ¿Dónde está tu hermano?", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Maite", text = "Está afuera…", isLeftCharacter = false },
            new DialogueLine { characterName = "Maite", text = "Pero me duele la panza y quería que el doctor me revisara.", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Secretaria", text = "Sí, entiendo, pero tenés que esperar a que venga tu hermano. El doctor no puede atenderte si estás sola.", isLeftCharacter = false },
            new DialogueLine { characterName = "", text = "La secretaria se dirige hacia la puerta, pero antes de salir, Y entra rápidamente.", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Fer", text = "¡Maitena!", isLeftCharacter = false },
            new DialogueLine { characterName = "Fer", text = "¡Acá estabas! Me preocupé mucho cuando no te vi en la sala de espera.", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Maite", text = "Perdón…", isLeftCharacter = false },
            new DialogueLine { characterName = "Maite", text = "Es que me dolía mucho la panza.", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Fer", text = "Ya sé, pero no puedes irte sola así. Me asustaste.", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Secretaria", text = "Tranquilo, Fernando. Maitena entró porque quería que el doctor la revisara.", isLeftCharacter = false },
            new DialogueLine { characterName = "Secretaria", text = "Ahora que estás acá, podemos continuar.", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Fer", text = "Sí, claro.", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Doctor", text = "Está bien. Maitena, ven conmigo y vamos a revisar dónde te duele.", isLeftCharacter = true },
            
            new DialogueLine { characterName = "Maite", text = "¿Me va a doler?", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Doctor", text = "No debería. Voy a hacerlo con cuidado y, si algo te molesta, me avisás.", isLeftCharacter = true },
            
            new DialogueLine { characterName = "Maite", text = "Bueno…", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Fer", text = "Yo me quedo acá con vos.", isLeftCharacter = false },
            new DialogueLine { characterName = "", text = "El doctor revisa a Maitena, pero no encuentra nada evidente.", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Doctor", text = "Mmm…", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "No veo nada fuera de lo normal por ahora.", isLeftCharacter = true },
            
            new DialogueLine { characterName = "Maite", text = "Pero me sigue doliendo…", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Doctor", text = "¿Todavía?", isLeftCharacter = true },
            
            new DialogueLine { characterName = "Maite", text = "Sí…", isLeftCharacter = false },
            new DialogueLine { characterName = "Maite", text = "No quiero irme así.", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Fer", text = "Maite, quizás cuando lleguemos a la casa se te pase.", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Maite", text = "Pero me duele ahora.", isLeftCharacter = false },
            new DialogueLine { characterName = "Maite", text = "No quiero irme hasta que me deje de doler.", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Doctor", text = "Está bien. No vamos a apresurarnos.", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "Si seguís teniendo dolor, tenemos que averiguar qué lo está provocando.", isLeftCharacter = true },
            
            new DialogueLine { characterName = "Secretaria", text = "¿Cree que puede ser algo más?", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Doctor", text = "Y.. no lo tengo claro todavía.", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "A simple vista no encuentro la causa, así que tendremos que investigar un poco más.", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "De hecho, PLAYER_NAME, podríamos aprovechar este caso para que aprendas algo que puede ser muy útil para tu tesis.", isLeftCharacter = true },
            
            new DialogueLine { characterName = "Fer", text = "¿Van a hacerle más estudios?", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Doctor", text = "Algo parecido.", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "Nuestra clínica cuenta con una tecnología bastante particular que nos permite estudiar el cuerpo desde adentro.", isLeftCharacter = true },
            
            new DialogueLine { characterName = "Maite", text = "¿Y eso sirve para saber por qué me duele?", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Doctor", text = "Eso es justamente lo que queremos averiguar.", isLeftCharacter = true },
            
            new DialogueLine { characterName = "Fer", text = "Bueno… Mientras ayude a Maite, está bien.", isLeftCharacter = false },
            
            new DialogueLine { characterName = "Doctor", text = "Perfecto.", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "Entonces, PLAYER_NAME, prepárate.", isLeftCharacter = true },
            new DialogueLine { characterName = "Doctor", text = "Nuestro último caso del día está a punto de comenzar.", isLeftCharacter = true }
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
                case "Secretaria": rightCharacterImage.sprite = secretariaTalking; break;
                case "Dalia": rightCharacterImage.sprite = daliaTalking; break;
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

        UpdateRightCharacter(line);
        SetIdleSprites();
    }

    void NextLine()
    {
        currentLine++;

        if (currentLine >= linesList.Count)
        {
            ReproducirVideoFinal();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(TypeLine());
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
            case "Secretaria": ApplyCharacterSize(rightCharacterImage, secretariaSize); break;
            case "Dalia": ApplyCharacterSize(rightCharacterImage, daliaSize); break;
            case "Maite": ApplyCharacterSize(rightCharacterImage, maiteSize); break;
            case "Fer": ApplyCharacterSize(rightCharacterImage, ferSize); break;
            default: rightCharacterImage.gameObject.SetActive(false); break;
        }
    }

    private string GetActiveRightCharacterName()
    {
        DialogueLine line = linesList[currentLine];

        if (line.characterName == "Secretaria" || line.characterName == "Dalia" || line.characterName == "Maite" || line.characterName == "Fer")
            return line.characterName;

        if (string.IsNullOrEmpty(line.characterName))
            return "";

        for (int i = currentLine - 1; i >= 0; i--)
        {
            string prevName = linesList[i].characterName;
            if (prevName == "Secretaria" || prevName == "Dalia" || prevName == "Maite" || prevName == "Fer") return prevName;
            if (string.IsNullOrEmpty(prevName)) return "";
        }

        return "";
    }

    private void SetRightCharacterIdle(string characterName)
    {
        switch (characterName)
        {
            case "Secretaria": rightCharacterImage.sprite = secretariaIdle; break;
            case "Dalia": rightCharacterImage.sprite = daliaIdle; break;
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