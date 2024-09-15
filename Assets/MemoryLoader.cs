using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class MemoryLoader : MonoBehaviour
{
    public GameObject PedestalPrefab;
    public Transform ParentTransform;

    private string apiUrl = "https://0128-2620-101-f000-7c2-00-9b4c.ngrok-free.app/memory";
    //private string apiUrl = "http://127.0.0.1:4000/memory";

    void Start()
    {
        StartCoroutine(LoadMemories());
    }

    IEnumerator LoadMemories()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("Received JSON: " + json); // Log received JSON
            ProcessMemories(json);
        }
    }

    void ProcessMemories(string json)
    {
        var memories = JsonUtility.FromJson<MemoryCollection>(json);
        int offset = 0;

        foreach (var memory in memories.memories)
        {
            GameObject newPedestal = Instantiate(PedestalPrefab, ParentTransform);
            newPedestal.transform.localPosition += new Vector3(offset, 0, 0);
            offset += 2;

            // Assigning the image (mediaUrl)
            Image memoryImage = newPedestal.GetComponentInChildren<Image>();
            if (!string.IsNullOrEmpty(memory.mediaUrl))
            {
                Debug.Log("Loading Image from URL: " + memory.mediaUrl);
                StartCoroutine(LoadImage(memory.mediaUrl, memoryImage));
            }
            else
            {
                Debug.LogWarning("No mediaUrl provided for a memory.");
                memoryImage.sprite = null; // Default null placeholder image
            }
        }
    }

    IEnumerator LoadImage(string url, Image img)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}

[System.Serializable]
public class Memory
{
    public string mediaUrl;
}

[System.Serializable]
public class MemoryCollection
{
    public Memory[] memories;
}
