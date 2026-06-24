using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossRankingTimer : MonoBehaviour
{
    [Header("Boss")]
    public HeadHealth[] heads;

    [Header("Victory UI")]
    public GameObject victoryPanel;
    public GameObject nameInputPanel;
    public InputField playerNameInput;
    public TMP_InputField playerNameTMPInput;
    public Text timeText;
    public TMP_Text timeTMPText;
    public Button submitButton;

    private float startTime;
    private float finalTime;
    private bool bossDefeated;
    private bool scoreSaved;

    private void Start()
    {
        Time.timeScale = 1f;

        if (heads == null || heads.Length == 0)
        {
            heads = FindObjectsOfType<HeadHealth>();
        }

        startTime = Time.unscaledTime;

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        if (nameInputPanel != null)
        {
            nameInputPanel.SetActive(false);
        }

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(SaveRankingEntry);
        }

        UpdateTimeText(0f);
    }

    private void OnDestroy()
    {
        if (submitButton != null)
        {
            submitButton.onClick.RemoveListener(SaveRankingEntry);
        }
    }

    private void Update()
    {
        if (bossDefeated) return;

        float elapsed =
            Time.unscaledTime - startTime;

        UpdateTimeText(elapsed);

        if (AreAllHeadsDead())
        {
            ShowVictory(elapsed);
        }
    }

    private bool AreAllHeadsDead()
    {
        if (heads == null || heads.Length == 0) return false;

        for (int i = 0; i < heads.Length; i++)
        {
            if (heads[i] == null || !heads[i].IsDead())
            {
                return false;
            }
        }

        return true;
    }

    private void ShowVictory(float elapsed)
    {
        bossDefeated = true;
        finalTime = elapsed;

        UpdateTimeText(finalTime);

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        if (nameInputPanel != null)
        {
            nameInputPanel.SetActive(true);
        }

        if (playerNameInput != null)
        {
            playerNameInput.text = string.Empty;
            playerNameInput.ActivateInputField();
        }

        if (playerNameTMPInput != null)
        {
            playerNameTMPInput.text = string.Empty;
            playerNameTMPInput.ActivateInputField();
        }

        Time.timeScale = 0f;
    }

    public void SaveRankingEntry()
    {
        if (!bossDefeated || scoreSaved) return;

        string playerName =
            playerNameTMPInput != null
                ? playerNameTMPInput.text
                : playerNameInput != null
                ? playerNameInput.text
                : string.Empty;

        RankingManager
            .GetOrCreate()
            .SaveScore(playerName, finalTime);

        scoreSaved = true;

        if (submitButton != null)
        {
            submitButton.interactable = false;
        }

        if (nameInputPanel != null)
        {
            nameInputPanel.SetActive(false);
        }
    }

    private void UpdateTimeText(float time)
    {
        string formattedTime =
            RankingManager.FormatTime(time);

        if (timeText != null)
        {
            timeText.text = formattedTime;
        }

        if (timeTMPText != null)
        {
            timeTMPText.text = formattedTime;
        }
    }
}
