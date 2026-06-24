using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SecretariaTutorialManager : MonoBehaviour
{
    [Header("Tutorial Inicial")]
    public GameObject panel;
    public TMP_Text dialogueText;
    public Button continueButton;

    [Header("Secretaria")]
    public Image secretariaImage;
    public Sprite idleSprite;
    public Sprite talkingSprite;

    [Header("Texto Inicial")]
    [TextArea(2, 5)]
    public string tutorialText;

    [Header("Segundo Panel")]
    public GameObject segundoPanel;
    public TMP_Text segundoTexto;

    [TextArea(2, 5)]
    public string textoSegundoPanel;

    [Header("Tutorial")]
    public GameObject globuloTutorialPrefab;

    [Header("Spawners")]
    public ObstacleSpawner[] obstacleSpawners;

    [Header("Configuración")]
    public float textSpeed = 0.03f;

    private GameObject tutorialObstacle;

    void Start()
    {
        panel.SetActive(true);

        if (segundoPanel != null)
        {
            segundoPanel.SetActive(false);
        }

        continueButton.gameObject.SetActive(false);

        foreach (ObstacleSpawner spawner in obstacleSpawners)
        {
            if (spawner != null)
            {
                spawner.enabled = false;
            }
        }

        Time.timeScale = 0f;

        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        dialogueText.text = "";

        if (secretariaImage != null)
        {
            secretariaImage.sprite = talkingSprite;
        }

        foreach (char c in tutorialText)
        {
            dialogueText.text += c;

            yield return new WaitForSecondsRealtime(textSpeed);
        }

        if (secretariaImage != null)
        {
            secretariaImage.sprite = idleSprite;
        }

        continueButton.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        panel.SetActive(false);

        Time.timeScale = 1f;

        SpawnTutorialObstacle();
    }

    void SpawnTutorialObstacle()
    {
        Vector3 spawnPos = new Vector3(
            Camera.main.transform.position.x + 12f,
            0f,
            0f
        );

        tutorialObstacle = Instantiate(
            globuloTutorialPrefab,
            spawnPos,
            Quaternion.identity
        );

        ObstacleMove move =
            tutorialObstacle.GetComponent<ObstacleMove>();

        if (move != null)
        {
            move.tutorialObstacle = true;
        }

        StartCoroutine(CheckTutorialObstacle());
    }

    IEnumerator CheckTutorialObstacle()
    {
        while (tutorialObstacle != null)
        {
            if (tutorialObstacle.transform.position.x <
                Camera.main.transform.position.x - 10f)
            {
                Destroy(tutorialObstacle);

                MostrarSegundoPanel();

                yield break;
            }

            yield return null;
        }

        MostrarSegundoPanel();
    }

    void MostrarSegundoPanel()
    {
        Time.timeScale = 0f;

        if (segundoPanel != null)
        {
            segundoPanel.SetActive(true);
        }

        if (segundoTexto != null)
        {
            segundoTexto.text = textoSegundoPanel;
        }
    }

    public void ContinuarNivel()
    {
        if (segundoPanel != null)
        {
            segundoPanel.SetActive(false);
        }

        foreach (ObstacleSpawner spawner in obstacleSpawners)
        {
            if (spawner != null)
            {
                spawner.enabled = true;
            }
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.StartLevel();
        }

        Time.timeScale = 1f;
    }
}