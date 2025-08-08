using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;
using System.Linq;
using Firebase.Extensions;

public class LeaderboardDisplay : MonoBehaviour
{
    public GameObject scoreEntryPrefab;
    public Transform leaderboardContainer;

    public void LoadLeaderboard()
    {
        FirebaseDatabase.DefaultInstance.GetReference("leaderboard")
            .OrderByChild("score")
            .LimitToLast(10)
            .GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    List<PlayerScore> scores = new List<PlayerScore>();

                    foreach (DataSnapshot child in snapshot.Children)
                    {
                        string username = child.Child("username").Value.ToString();
                        int score = int.Parse(child.Child("score").Value.ToString());
                        scores.Add(new PlayerScore(username, score));
                    }

                    // Sort descending
                    scores = scores.OrderByDescending(s => s.score).ToList();

                    foreach (Transform child in leaderboardContainer)
                        Destroy(child.gameObject);

                    foreach (var score in scores)
                    {
                        GameObject go = Instantiate(scoreEntryPrefab, leaderboardContainer);
                        go.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"{score.username}: {score.score}";
                    }
                }
                else
                {
                    Debug.LogError("? Failed to load leaderboard: " + task.Exception);
                }
            });
    }
}
