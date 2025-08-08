using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class AuthManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private DatabaseReference dbRef;

    [Header("UI References")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text statusText;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase Auth Initialized.");
            }
            else
            {
                Debug.LogError("Firebase not available: " + task.Result);
            }
        });
    }

    public void CreateUser()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                statusText.text = "Signup Failed: " + task.Exception?.GetBaseException().Message;
                Debug.LogError("Signup Error: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result.User;
            statusText.text = "User created: " + newUser.UserId;
            Debug.Log("User created: " + newUser.UserId);

            // Add user to database
            WriteUserToDatabase(newUser.UserId, email);
        });
    }

    public void LoginUser()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                statusText.text = "Login Failed: " + task.Exception?.GetBaseException().Message;
                Debug.LogError("Login Error: " + task.Exception);
                return;
            }

            FirebaseUser user = task.Result.User;
            statusText.text = "Logged in as: " + user.Email;
            Debug.Log("Login successful: " + user.UserId);
        });
    }

    private void WriteUserToDatabase(string userId, string email)
    {
        User user = new User(email, 0); // default highscore 0
        string json = JsonUtility.ToJson(user);

        dbRef.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("User written to DB.");
            }
            else
            {
                Debug.LogError("Failed to write user to DB: " + task.Exception);
            }
        });
    }
}