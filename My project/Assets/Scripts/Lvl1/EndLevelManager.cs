using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class SecretariaEndManager : MonoBehaviour
{
    public static SecretariaEndManager Instance;

    [Header("UI")]
    public GameObject panel;
    public TMP_Text dialogueText;
    public Button continueButton;

    [Header("Multiple Choice")]
    public GameObject choicePanel;
    public Button option1Button;
    public Button option2Button;
    public Button option3Button;
    public Button option4Button;
    public int correctOption = 3;

    [Header("Character")]
    public Image secretariaImage;
    public Sprite idleSprite;
    public Sprite talkingSprite;

    [Header("Text")]
    [TextArea(2, 5)]
    public string finalText;

    [TextArea(2, 5)]
    public string correctAnswerText;

    [TextArea(2, 5)]
    public string wrongAnswerText;

    public float textSpeed = 0.03f;

    private bool waitingToShowChoices = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (panel == null || dialogueText == null || continueButton == null || secretariaImage == null)
        {
            Debug.LogError("❌ Faltan referencias en EndManager");
            return;
        }

        panel.SetActive(false);
        continueButton.gameObject.SetActive(false);

        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (waitingToShowChoices && Input.GetKeyDown(KeyCode.Space))
        {
            waitingToShowChoices = false;
            ShowChoices();
        }
    }

    public void StartEndDialogue()
    {
        Debug.Log("✔ END DIALOGUE START");

        panel.SetActive(true);
        continueButton.gameObject.SetActive(false);
        waitingToShowChoices = false;

        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        Time.timeScale = 0f;

        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        dialogueText.text = "";
        secretariaImage.sprite = talkingSprite;

        foreach (char c in finalText)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(textSpeed);
        }

        secretariaImage.sprite = idleSprite;
        waitingToShowChoices = true;
    }

    void ShowChoices()
    {
        if (choicePanel == null || option1Button == null || option2Button == null || option3Button == null || option4Button == null)
        {
            continueButton.gameObject.SetActive(true);
            return;
        }

        choicePanel.SetActive(true);
        dialogueText.text = "";

        option1Button.onClick.RemoveListener(ChooseOption1);
        option2Button.onClick.RemoveListener(ChooseOption2);
        option3Button.onClick.RemoveListener(ChooseOption3);
        option4Button.onClick.RemoveListener(ChooseOption4);

        option1Button.onClick.AddListener(ChooseOption1);
        option2Button.onClick.AddListener(ChooseOption2);
        option3Button.onClick.AddListener(ChooseOption3);
        option4Button.onClick.AddListener(ChooseOption4);
    }

    void ChooseOption1()
    {
        AnswerChoice(1);
    }

    void ChooseOption2()
    {
        AnswerChoice(2);
    }

    void ChooseOption3()
    {
        AnswerChoice(3);
    }

    void ChooseOption4()
    {
        AnswerChoice(4);
    }

    void AnswerChoice(int selectedOption)
    {
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        continueButton.gameObject.SetActive(false);

        if (selectedOption == correctOption)
        {
            StartCoroutine(TypeResultText(correctAnswerText));
        }
        else
        {
            StartCoroutine(TypeResultText(wrongAnswerText));
        }
    }

    IEnumerator TypeResultText(string resultText)
    {
        dialogueText.text = "";
        secretariaImage.sprite = talkingSprite;

        foreach (char c in resultText)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(textSpeed);
        }

        secretariaImage.sprite = idleSprite;
        continueButton.gameObject.SetActive(true);
    }

    public void ContinueToNextScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(3);
    }
}
