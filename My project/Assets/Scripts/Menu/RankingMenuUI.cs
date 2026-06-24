using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingMenuUI : MonoBehaviour
{
    [Header("Top 10")]
    public Text topTenText;
    public TMP_Text topTenTMPText;
    public string emptyRankingText = "Sin tiempos guardados";

    [Header("Admin")]
    public GameObject adminPanel;
    public Transform adminRowsParent;
    public RankingAdminRow adminRowPrefab;
    public Button clearAllButton;

    [Header("Panel oculto")]
    public bool allowKeyboardToggle = true;
    public KeyCode toggleKey = KeyCode.F10;

    private void Start()
    {
        Time.timeScale = 1f;

        if (adminPanel != null)
        {
            adminPanel.SetActive(false);
        }

        if (clearAllButton != null)
        {
            clearAllButton.onClick.AddListener(ClearRanking);
        }

        Refresh();
    }

    private void OnDestroy()
    {
        if (clearAllButton != null)
        {
            clearAllButton.onClick.RemoveListener(ClearRanking);
        }
    }

    private void Update()
    {
        if (allowKeyboardToggle && Input.GetKeyDown(toggleKey))
        {
            ToggleAdminPanel();
        }
    }

    public void ToggleAdminPanel()
    {
        if (adminPanel == null) return;

        adminPanel.SetActive(!adminPanel.activeSelf);
        Refresh();
    }

    public void ShowAdminPanel()
    {
        if (adminPanel == null) return;

        adminPanel.SetActive(true);
        Refresh();
    }

    public void HideAdminPanel()
    {
        if (adminPanel == null) return;

        adminPanel.SetActive(false);
    }

    public void Refresh()
    {
        List<RankingEntry> entries =
            RankingManager
            .GetOrCreate()
            .GetEntries();

        RefreshTopTen(entries);
        RefreshAdminRows(entries);
    }

    public void ClearRanking()
    {
        RankingManager
            .GetOrCreate()
            .ClearRanking();

        Refresh();
    }

    private void RemoveEntry(int index)
    {
        RankingManager
            .GetOrCreate()
            .RemoveAt(index);

        Refresh();
    }

    private void RefreshTopTen(List<RankingEntry> entries)
    {
        if (entries.Count == 0)
        {
            SetTopTenText(emptyRankingText);
            return;
        }

        StringBuilder builder =
            new StringBuilder();

        int count =
            Mathf.Min(10, entries.Count);

        for (int i = 0; i < count; i++)
        {
            RankingEntry entry =
                entries[i];

            builder
                .Append(i + 1)
                .Append(". ")
                .Append(entry.playerName)
                .Append(" - ")
                .Append(RankingManager.FormatTime(entry.time));

            if (i < count - 1)
            {
                builder.AppendLine();
            }
        }

        SetTopTenText(builder.ToString());
    }

    private void SetTopTenText(string value)
    {
        if (topTenText != null)
        {
            topTenText.text = value;
        }

        if (topTenTMPText != null)
        {
            topTenTMPText.text = value;
        }
    }

    private void RefreshAdminRows(List<RankingEntry> entries)
    {
        if (adminRowsParent == null || adminRowPrefab == null) return;

        for (int i = adminRowsParent.childCount - 1; i >= 0; i--)
        {
            Destroy(adminRowsParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < entries.Count; i++)
        {
            RankingAdminRow row =
                Instantiate(adminRowPrefab, adminRowsParent);

            row.Setup(i, entries[i], RemoveEntry);
        }
    }
}
