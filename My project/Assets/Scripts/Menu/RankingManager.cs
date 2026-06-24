using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class RankingManager : MonoBehaviour
{
    public static RankingManager Instance;

    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePath();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public static RankingManager GetOrCreate()
    {
        if (Instance != null)
        {
            Instance.InitializePath();
            return Instance;
        }

        GameObject rankingObject =
            new GameObject("RankingManager");

        Instance =
            rankingObject.AddComponent<RankingManager>();

        DontDestroyOnLoad(rankingObject);

        Instance.InitializePath();

        return Instance;
    }

    private void InitializePath()
    {
        if (!string.IsNullOrEmpty(savePath)) return;

        savePath =
            Path.Combine(Application.persistentDataPath, "ranking.json");
    }

    public void SaveScore(string playerName, float time)
    {
        InitializePath();

        RankingData data = LoadData();

        RankingEntry entry =
            new RankingEntry();

        entry.playerName =
            string.IsNullOrWhiteSpace(playerName)
                ? "Jugador"
                : playerName.Trim();

        entry.time = time;

        data.entries.Add(entry);

        SortData(data);
        SaveData(data);
    }

    public RankingData LoadData()
    {
        InitializePath();

        if (!File.Exists(savePath))
        {
            return new RankingData();
        }

        string json =
            File.ReadAllText(savePath);

        RankingData data =
            JsonUtility.FromJson<RankingData>(json);

        if (data == null)
        {
            data = new RankingData();
        }

        if (data.entries == null)
        {
            data.entries =
                new List<RankingEntry>();
        }

        SortData(data);

        return data;
    }

    private void SaveData(RankingData data)
    {
        InitializePath();
        SortData(data);

        string json =
            JsonUtility.ToJson(data, true);

        File.WriteAllText(savePath, json);
    }

    private void SortData(RankingData data)
    {
        if (data.entries == null)
        {
            data.entries =
                new List<RankingEntry>();
        }

        data.entries =
            data.entries
            .OrderBy(x => x.time)
            .ToList();
    }

    public List<RankingEntry> GetEntries()
    {
        return LoadData().entries;
    }

    public void ClearRanking()
    {
        SaveData(new RankingData());
    }

    public void RemoveAt(int index)
    {
        RankingData data = LoadData();

        if (index >= 0 &&
            index < data.entries.Count)
        {
            data.entries.RemoveAt(index);

            SaveData(data);
        }
    }

    public static string FormatTime(float time)
    {
        int minutes =
            Mathf.FloorToInt(time / 60f);

        int seconds =
            Mathf.FloorToInt(time % 60f);

        int centiseconds =
            Mathf.FloorToInt((time * 100f) % 100f);

        return
            minutes.ToString("00") +
            ":" +
            seconds.ToString("00") +
            "." +
            centiseconds.ToString("00");
    }
}
