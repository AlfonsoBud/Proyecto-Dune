using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("Base URL de la API, p. ej. https://miapi.example.com")]
    public string ApiBaseUrl = "https://localhost:5000";

    // --- Public coroutine API ---

    /// <summary>
    /// Crea una partida. Llama a POST /api/games/create
    /// onSuccess recibe el JSON devuelto por la API.
    /// onError recibe el mensaje de error.
    /// </summary>
    public IEnumerator CreateGame(string playerAlias, Action<string> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(ApiBaseUrl))
        {
            onError?.Invoke("ApiBaseUrl no está configurada.");
            yield break;
        }

        var url = CombineUrl(ApiBaseUrl, "api/games/create");

        var body = JsonUtility.ToJson(new CreateGameRequest { playerAlias = playerAlias });
        using (var req = new UnityWebRequest(url, "POST"))
        {
            var bodyRaw = Encoding.UTF8.GetBytes(body);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
#else
            if (req.isNetworkError || req.isHttpError)
#endif
            {
                var message = SafeResponseText(req);
                onError?.Invoke($"Error {req.responseCode}: {message}");
            }
            else
            {
                onSuccess?.Invoke(req.downloadHandler.text);
            }
        }
    }

    /// <summary>
    /// Obtiene una partida por id. Llama a GET /api/games/{id}
    /// onSuccess recibe el JSON devuelto por la API.
    /// onError recibe el mensaje de error.
    /// </summary>
    public IEnumerator GetGame(Guid id, Action<string> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(ApiBaseUrl))
        {
            onError?.Invoke("ApiBaseUrl no está configurada.");
            yield break;
        }

        var url = CombineUrl(ApiBaseUrl, $"api/games/{id}");

        using (var req = UnityWebRequest.Get(url))
        {
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Accept", "application/json");

            yield return req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
#else
            if (req.isNetworkError || req.isHttpError)
#endif
            {
                var message = SafeResponseText(req);
                onError?.Invoke($"Error {req.responseCode}: {message}");
            }
            else
            {
                onSuccess?.Invoke(req.downloadHandler.text);
            }
        }
    }

    /// <summary>
    /// Ejecuta una ronda. Llama a POST /api/games/{id}/round
    /// onSuccess recibe el JSON devuelto por la API.
    /// onError recibe el mensaje de error.
    /// </summary>
    public IEnumerator ExecuteRound(Guid id, Action<string> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(ApiBaseUrl))
        {
            onError?.Invoke("ApiBaseUrl no está configurada.");
            yield break;
        }

        var url = CombineUrl(ApiBaseUrl, $"api/games/{id}/round");

        // POST sin cuerpo (si tu API requiere un body, sustitúyelo por JSON similar a CreateGame)
        using (var req = new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(new byte[0]);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
#else
            if (req.isNetworkError || req.isHttpError)
#endif
            {
                var message = SafeResponseText(req);
                onError?.Invoke($"Error {req.responseCode}: {message}");
            }
            else
            {
                onSuccess?.Invoke(req.downloadHandler.text);
            }
        }
    }

    // --- Helpers ---

    private static string CombineUrl(string baseUrl, string relative)
    {
        if (string.IsNullOrEmpty(baseUrl)) return relative;
        return $"{baseUrl.TrimEnd('/')}/{relative.TrimStart('/')}";
    }

    private static string SafeResponseText(UnityWebRequest req)
    {
        try
        {
            return req.downloadHandler != null ? req.downloadHandler.text : req.error;
        }
        catch
        {
            return req.error ?? "Unknown error";
        }
    }

    [Serializable]
    private class CreateGameRequest
    {
        public string playerAlias;
    }
}