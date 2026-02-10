using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class ScoreEntry
{
    public string name;
    public int score;
}

public class LeaderboardManager : MonoBehaviour
{
    private List<ScoreEntry> scores = new List<ScoreEntry>();
    private const int MaxEntries = 10;

    public void AddScore(string name, int score)
    {
        LoadScores();
        scores.Add(new ScoreEntry { name = string.IsNullOrEmpty(name) ? "AAA" : name, score = score });
        scores = scores.OrderByDescending(s => s.score).Take(MaxEntries).ToList();
        SaveScores();
        Debug.Log($"[LeaderboardManager] Added score {name}={score}. Total entries={scores.Count}");
    }

    public List<ScoreEntry> GetScores()
    {
        LoadScores();
        return scores;
    }

    private void SaveScores()
    {
        string json = JsonUtility.ToJson(new SerializationWrapper { items = scores });
        PlayerPrefs.SetString("LeaderboardData", json);
        PlayerPrefs.Save();
        Debug.Log("[LeaderboardManager] Saved leaderboard JSON: " + json);
    }

    public void LoadScores()
    {
        if (PlayerPrefs.HasKey("LeaderboardData"))
        {
            string json = PlayerPrefs.GetString("LeaderboardData");
            var wrapper = JsonUtility.FromJson<SerializationWrapper>(json);
            scores = (wrapper != null && wrapper.items != null) ? wrapper.items : new List<ScoreEntry>();
        }
        else
        {
            scores = new List<ScoreEntry>();
        }
    }

    [System.Serializable]
    private class SerializationWrapper { public List<ScoreEntry> items; }
}