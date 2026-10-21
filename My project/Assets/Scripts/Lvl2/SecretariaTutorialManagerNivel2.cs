using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SecretariaTutorialManagerNivel2 : MonoBehaviour
{
    [Header("UI del Tutorial (Objeto dentro del Canvas)")]
    public GameObject uiTutorial; // Arrastra el GameObject contenedor del tutorial
    public TMP_Text dialogueText;
    public Button continueButton;

    [Header("UI de Controles (Siguiente Canvas/Panel)")]
    public GameObject uiControles; // Arrastra el panel/canvas con las instrucciones de los controles
    public Button estoyPreparadoButton; // (Opcional) Asigna el botón "Estoy preparado" si deseas manejar su visibilidad por código

    [Header("Secretaria")]
    public Image secretariaImage;
    public Sprite idleSprite;
    public Sprite talkingSprite;

    [Header("Imágenes del Tutorial")]
    public Image imagenNeumococo; 
    public Image imagenBotas;     

    [Header("Texto del Tutorial")]
    [TextArea(8, 15)]
    public string tutorialText = "¡Estás dentro del cuerpo de Dalia! Ella básicamente te respiró a ti y al doctor!\nComo verás, estás controlando un muy pequeño robot. Esta herramienta te permitirá pasar por los bronquios de Dalia.\n\n¿Ves esto? Es el neumococo, un tipo de neumonía. Con el dr. Jacinto creemos que es esta bacteria la que está afectando a Dalia.\n\nSi te encuentras con algo como esto, no dudas en eliminarlo saltando por arriba de él, pues las botas del robot que manejas tienen un antibacterial que ayudará a Dalia. Ten cuidado, el sistema no es seguro.";

    [Header("Puntos de Activación de Imágenes")]
    public string fraseTriggerImagen1 = "¿Ves esto?";
    public string fraseTriggerImagen2 = "Si te encuentras con algo como esto";

    [Header("Spawners a Activar al Finalizar")]
    public ObstacleSpawner[] obstacleSpawners;

    [Header("Configuración")]
    public float textSpeed = 0.03f;

    [Header("Música")]
    [Tooltip("Arrastra el GameMusicManager. Si lo dejas vacío, lo busca solo.")]
    public GameMusicManager musicManager;
    private bool musicaDetenida = false;

    private void Start()
    {
        if (musicManager == null)
            musicManager = FindFirstObjectByType<GameMusicManager>();

        StartCoroutine(PausarMusicaTrasUnFrame());

        if (uiTutorial != null) uiTutorial.SetActive(true);
        if (uiControles != null) uiControles.SetActive(false); // Nos aseguramos de que empiece oculto
        if (continueButton != null) continueButton.gameObject.SetActive(false);

        if (imagenNeumococo != null) imagenNeumococo.gameObject.SetActive(false);
        if (imagenBotas != null) imagenBotas.gameObject.SetActive(false);

        foreach (ObstacleSpawner spawner in obstacleSpawners)
        {
            if (spawner != null) spawner.enabled = false;
        }

        Time.timeScale = 0f;

        StartCoroutine(EscribirTextoTutorial());
    }

    private IEnumerator EscribirTextoTutorial()
    {
        dialogueText.text = "";

        if (secretariaImage != null) secretariaImage.sprite = talkingSprite;

        for (int i = 0; i < tutorialText.Length; i++)
        {
            dialogueText.text += tutorialText[i];

            if (!string.IsNullOrEmpty(fraseTriggerImagen1) && dialogueText.text.EndsWith(fraseTriggerImagen1))
            {
                if (imagenNeumococo != null) imagenNeumococo.gameObject.SetActive(true);
                if (imagenBotas != null) imagenBotas.gameObject.SetActive(false);
            }

            if (!string.IsNullOrEmpty(fraseTriggerImagen2) && dialogueText.text.EndsWith(fraseTriggerImagen2))
            {
                if (imagenNeumococo != null) imagenNeumococo.gameObject.SetActive(false);
                if (imagenBotas != null) imagenBotas.gameObject.SetActive(true);
            }

            yield return new WaitForSecondsRealtime(textSpeed);
        }

        if (secretariaImage != null) secretariaImage.sprite = idleSprite;
        if (continueButton != null) continueButton.gameObject.SetActive(true);
    }

    // Se ejecuta al hacer clic en el botón Continuar del texto de la Secretaria
    public void ContinuarJuego()
    {
        // 1. Oculta el tutorial de la secretaria
        if (uiTutorial != null) 
        {
            uiTutorial.SetActive(false);
        }

        // 2. Muestra el panel de controles
        if (uiControles != null)
        {
            uiControles.SetActive(true);
        }
        else
        {
            // Si olvidaste asignar el panel de controles, inicia el nivel directamente para evitar un bloqueo
            BotonEstoyPreparado();
        }
    }

    // Asigna esta función al botón "Estoy Preparado"
    public void BotonEstoyPreparado()
    {
        // 1. Oculta el panel de controles
        if (uiControles != null)
        {
            uiControles.SetActive(false);
        }

        ReanudarMusica(); // La música vuelve al salir de los controles

        // 2. Encender los spawners y despausar el juego
        foreach (ObstacleSpawner spawner in obstacleSpawners)
        {
            if (spawner != null) spawner.enabled = true;
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.StartLevel();
        }

        Time.timeScale = 1f;
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
}