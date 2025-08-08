using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;

public class FirebaseUserData : MonoBehaviour
{
    [Header("UI Input Fields")]
    public TMP_InputField usernameInput;
    public TMP_InputField highscoreInput;

    private DatabaseReference dbRef;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // Initialize the database reference
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;

                Debug.Log("Firebase Initialized. DB URL: " + FirebaseDatabase.DefaultInstance.App.Options.DatabaseUrl);
            }
            else
            {
                Debug.LogError("Firebase dependencies not resolved: " + dependencyStatus);
            }
        });
    }

    public void OnWriteUserClicked()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("No user is logged in. Cannot write data.");
            return;
        }

        string username = usernameInput.text;
        int highscore;
        if (!int.TryParse(highscoreInput.text, out highscore))
        {
            Debug.LogError("Invalid highscore input.");
            return;
        }

        User user = new User(username, highscore);
        string json = JsonUtility.ToJson(user);

        dbRef.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("User data successfully written.");
            }
            else
            {
                Debug.LogError("Error writing user data: " + task.Exception);
            }
        });
    }

    public void OnReadUserClicked()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        Debug.Log($"Current user id {userId}");

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("No user is logged in. Cannot read data.");
            return;
        }

        dbRef.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error reading user data: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    User user = JsonUtility.FromJson<User>(json);
                    Debug.Log("User loaded: " + user.username + " | Highscore: " + user.highscore);
                }
                else
                {
                    Debug.Log("No user data found for this account.");
                }
            }
        });
    }
}
