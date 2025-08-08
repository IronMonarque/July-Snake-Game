using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class PlayerScore
{
    public string username;
    public int score;

    public PlayerScore(string username, int score)
    {
        this.username = username;
        this.score = score;
    }
}

public class LeaderboardManager : MonoBehaviour
{
    public Transform leaderboardPanel;
    public GameObject leaderboardEntryPrefab;

    private DatabaseReference dbRef;

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        //FetchLeaderboard();
    }

    public void FetchLeaderboard()
    {
        dbRef.Child("users").OrderByChild("highscore").LimitToFirst(10).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to fetch leaderboard: " + task.Exception);
                return;
            }

            List<DataSnapshot> users = new List<DataSnapshot>(task.Result.Children);
            users.Reverse(); // Since Firebase returns lowest-to-highest

            foreach (Transform child in leaderboardPanel)
            {
                Destroy(child.gameObject);
            }

            foreach (var userSnapshot in users)
            {
                var user = JsonUtility.FromJson<User>(userSnapshot.GetRawJsonValue());
                GameObject entry = Instantiate(leaderboardEntryPrefab, leaderboardPanel);
                entry.GetComponentInChildren<TMP_Text>().text = user.username + " - " + user.highscore;
            }
        });
    }

    public void UploadScore(string username, int score)
    {
        string key = dbRef.Child("leaderboard").Push().Key;
        PlayerScore entry = new PlayerScore(username, score);
        string json = JsonUtility.ToJson(entry);

        dbRef.Child("leaderboard").Child(key).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Score uploaded.");
            }
            else
            {
                Debug.LogError("Failed to upload score: " + task.Exception);
            }
        });
    }
}
