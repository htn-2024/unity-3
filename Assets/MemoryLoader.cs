using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using TMPro;

public class MemoryLoader : MonoBehaviour
{
    public GameObject PedestalPrefab;
    public Transform ParentTransform;

    private string apiUrl = "https://0128-2620-101-f000-7c2-00-9b4c.ngrok-free.app/memory";

    void Start()
    {
        if (PedestalPrefab == null)
        {
            Debug.LogError("PedestalPrefab is not assigned!");
            return;
        }

        if (ParentTransform == null)
        {
            Debug.LogError("ParentTransform is not assigned!");
            return;
        }

        StartCoroutine(LoadMemories());
    }

    IEnumerator LoadMemories()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch memories: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("Received JSON: " + json);
            ProcessMemories(json);
        }
    }

    void ProcessMemories(string json)
    {
        var memories = JsonUtility.FromJson<MemoryCollection>(json);
        int offset = 0;

        foreach (var memory in memories.memories)
        {
            Debug.Log("Instantiating pedestal for memory: " + memory.title);

            GameObject newPedestal = Instantiate(PedestalPrefab, ParentTransform);
            newPedestal.transform.localPosition += new Vector3(offset, 0, 0);
            offset += 4;

            // Image (mediaUrl)
            Image memoryImage = newPedestal.GetComponentInChildren<Image>();
            if (!string.IsNullOrEmpty(memory.mediaUrl))
            {
                Debug.Log("Loading Image from URL: " + memory.mediaUrl);
                StartCoroutine(LoadImage(memory.mediaUrl, memoryImage));
            }
            else
            {
                Debug.LogWarning("No mediaUrl provided for memory with title: " + memory.title);
                memoryImage.sprite = null; // Or assign a default sprite
            }

            // Title
            TextMeshProUGUI titleText = newPedestal.transform.Find("TextCard/Panel/TitleText").GetComponent<TextMeshProUGUI>();
            if (titleText != null)
            {
                titleText.text = memory.title;
                Debug.Log("Assigned title: " + memory.title);
            }
            else
            {
                Debug.LogError("TitleText not found!");
            }

            // Description
            TextMeshProUGUI descriptionText = newPedestal.transform.Find("TextCard/Panel/DescriptionText").GetComponent<TextMeshProUGUI>();
            if (descriptionText != null)
            {
                descriptionText.text = memory.description;
                Debug.Log("Assigned description: " + memory.description);
            }
            else
            {
                Debug.LogError("DescriptionText not found!");
            }
        }
    }

    IEnumerator LoadImage(string url, Image img)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load image: " + request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            Debug.Log("Image loaded and assigned successfully.");
        }
    }
}

[System.Serializable]
public class Memory
{
    public string title;
    public string description;
    public string mediaUrl;
    public string recordingUrl;
}

[System.Serializable]
public class MemoryCollection
{
    public Memory[] memories;
}
