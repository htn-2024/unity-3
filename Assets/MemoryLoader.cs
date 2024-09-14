using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class MemoryLoader : MonoBehaviour
{
    public GameObject PedestalPrefab;
    public Transform ParentTransform; // Assign the parent object in the scene where the pedestals will be placed.

    private string apiUrl = "http://127.0.0.1:4000/memory";

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
            Debug.Log("Received JSON: " + json); // Log the received JSON for debugging
            ProcessMemories(json);
        }
    }

    void ProcessMemories(string json)
    {
        var memories = JsonUtility.FromJson<MemoryCollection>(json);

        foreach (var memory in memories.memories)
        {
            GameObject newPedestal = Instantiate(PedestalPrefab, ParentTransform);

            // Assigning the image (mediaUrl)
            Image memoryImage = newPedestal.GetComponentInChildren<Image>();
            if (!string.IsNullOrEmpty(memory.mediaUrl))
            {
                Debug.Log("Loading Image from URL: " + memory.mediaUrl);
                StartCoroutine(LoadImage(memory.mediaUrl, memoryImage));
            }
            else
            {
                Debug.LogWarning("No mediaUrl provided for memory with title: " + memory.title);
                // Optionally, set a default or placeholder image here
                memoryImage.sprite = null; // Or assign a default sprite
            }

            // Assigning the title
            Text titleText = newPedestal.transform.Find("TextCard/Panel/TitleText").GetComponent<Text>();
            if (titleText != null)
            {
                titleText.text = memory.title;
            }
            else
            {
                Debug.LogError("TitleText not found!");
            }

            // Assigning the description
            Text descriptionText = newPedestal.transform.Find("TextCard/Panel/DescriptionText").GetComponent<Text>();
            if (descriptionText != null)
            {
                descriptionText.text = memory.description;
            }
            else
            {
                Debug.LogError("DescriptionText not found!");
            }

            // Assigning the audio (recordingUrl)
            AudioSource audioSource = newPedestal.GetComponent<AudioSource>();
            if (!string.IsNullOrEmpty(memory.recordingUrl))
            {
                Debug.Log("Loading Audio from URL: " + memory.recordingUrl);
                StartCoroutine(LoadAudio(memory.recordingUrl, audioSource));
            }
            else
            {
                Debug.LogWarning("No recordingUrl provided for memory with title: " + memory.title);
                audioSource.clip = null; // Or leave it empty if no audio is provided
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

    IEnumerator LoadAudio(string url, AudioSource audioSource)
    {
        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
            audioSource.clip = clip;
            // You can choose to auto-play or not
            // audioSource.Play();
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

