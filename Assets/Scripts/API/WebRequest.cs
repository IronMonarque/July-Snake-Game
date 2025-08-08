using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequest : MonoBehaviour
{

    public string url;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SnakeGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SnakeGame()
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log(json);
        }
        else
        {
            Debug.Log("Error " + request.error);
        }
    }
}
