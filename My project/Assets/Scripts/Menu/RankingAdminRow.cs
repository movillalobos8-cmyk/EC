using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingAdminRow : MonoBehaviour
{
    public Text positionText;
    public TMP_Text positionTMPText;
    public Text playerNameText;
    public TMP_Text playerNameTMPText;
    public Text timeText;
    public TMP_Text timeTMPText;
    public Button removeButton;

    private int rankingIndex;
    private Action<int> removeAction;

    public void Setup(
        int index,
        RankingEntry entry,
        Action<int> onRemove)
    {
        rankingIndex = index;
        removeAction = onRemove;

        if (positionText != null)
        {
            positionText.text =
                (index + 1).ToString();
        }

        if (positionTMPText != null)
        {
            positionTMPText.text =
                (index + 1).ToString();
        }

        if (playerNameText != null)
        {
            playerNameText.text =
                entry.playerName;
        }

        if (playerNameTMPText != null)
        {
            playerNameTMPText.text =
                entry.playerName;
        }

        if (timeText != null)
        {
            timeText.text =
                RankingManager.FormatTime(entry.time);
        }

        if (timeTMPText != null)
        {
            timeTMPText.text =
                RankingManager.FormatTime(entry.time);
        }

        if (removeButton != null)
        {
            removeButton.onClick.RemoveListener(Remove);
            removeButton.onClick.AddListener(Remove);
        }
    }

    private void OnDestroy()
    {
        if (removeButton != null)
        {
            removeButton.onClick.RemoveListener(Remove);
        }
    }

    private void Remove()
    {
        removeAction?.Invoke(rankingIndex);
    }
}
