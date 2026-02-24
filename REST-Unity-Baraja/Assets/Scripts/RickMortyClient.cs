using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TextCore.Text;

public class RickMortyClient : MonoBehaviour
{
    private const string BASE = "https://rickandmortyapi.com/api/character/";

    public IEnumerator GetCharacter(int id, System.Action<RMCharacter> onOk, System.Action<string> onFail)
    {
        string url = BASE + id;
        using var req = UnityWebRequest.Get(url);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            onFail?.Invoke($"RM GET error: {req.error} | URL: {url}");
            yield break;
        }

        try
        {
            var ch = JsonUtility.FromJson<RMCharacter>(req.downloadHandler.text);
            onOk?.Invoke(ch);
        }
        catch (System.Exception ex)
        {
            onFail?.Invoke($"RM JSON parse error: {ex.Message}\nBody:\n{req.downloadHandler.text}");
        }
    }

    public IEnumerator GetImage(string url, System.Action<Texture2D> onOk, System.Action<string> onFail)
    {
        using var req = UnityWebRequestTexture.GetTexture(url);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            onFail?.Invoke($"Image GET error: {req.error} | URL: {url}");
            yield break;
        }

        var tex = DownloadHandlerTexture.GetContent(req);
        onOk?.Invoke(tex);
    }
}