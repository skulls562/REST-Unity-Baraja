using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class FakeApiClient : MonoBehaviour
{
    [Header("My JSON Server base URL")]
    [Tooltip("Ej: https://my-json-server.typicode.com/TU_USER/TU_REPO")]
    public string baseUrl = "https://my-json-server.typicode.com/TU_USER/TU_REPO";

    public IEnumerator GetUser(int userId, System.Action<UserData> onOk, System.Action<string> onFail)
    {
        string url = $"{baseUrl}/users/{userId}";
        using var req = UnityWebRequest.Get(url);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            onFail?.Invoke($"FakeApi GET error: {req.error} | URL: {url}");
            yield break;
        }

        try
        {
            var user = JsonUtility.FromJson<UserData>(req.downloadHandler.text);
            onOk?.Invoke(user);
        }
        catch (System.Exception ex)
        {
            onFail?.Invoke($"FakeApi JSON parse error: {ex.Message}\nBody:\n{req.downloadHandler.text}");
        }
    }
}