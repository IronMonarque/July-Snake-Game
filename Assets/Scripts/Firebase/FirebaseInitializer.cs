using UnityEngine;
using Firebase;
using Firebase.Extensions;

public class FirebaseInitializer : MonoBehaviour
{
    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                Debug.Log("Firebase initialized successfully.");
            }
            else
            {
                Debug.LogError("Firebase dependency error: " + status);
            }
        });
    }
}
