using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RestClient : MonoBehaviour
{
    public static RestClient _instance;

    private RestClient() { }

    public void Awake()
    {
        if (!_instance)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>Sends the server info to the remote server.</summary>
    /// <param name="url">Url of the remote server.</param>
    /// <param name="server">Server information.</param>
    public IEnumerator Post(string url, ServerModel server)
    {
        string jsonData = JsonUtility.ToJson(server);
        using (UnityWebRequest www = UnityWebRequest.Post(url, jsonData))
        {
            www.SetRequestHeader("content-type", "application/json");
            www.uploadHandler.contentType = "application/json";
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));

            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.LogError("Server couldn't connect with master-server: \n["+ www.error+"]");
            }
            else
            {
                if (www.isDone)
                {
                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    Debug.Log("Server added to master-server correctly");
                }
            }
        }
    }
}
