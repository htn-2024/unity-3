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
        int offset = 0;

        foreach (var memory in memories.memories)
        {
            GameObject newPedestal = Instantiate(PedestalPrefab, ParentTransform);
            newPedestal.transform.localPosition += new Vector3(offset, 0, 0);
            offset += 2; // Adjust the offset to space out the pedestals

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
                memoryImage.sprite = null; // Optionally, set a default or placeholder image here
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



//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;
//using UnityEngine.Networking;

//public class MemoryLoader : MonoBehaviour
//{
//    public GameObject PedestalPrefab;
//    public Transform ParentTransform; // Assign the parent object in the scene where the pedestals will be placed.

//    private string apiUrl = "http://127.0.0.1:4000/memory";

//    void Start()
//    {
//        StartCoroutine(LoadMemories());
//    }

//    IEnumerator LoadMemories()
//    {
//        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
//        yield return request.SendWebRequest();

//        if (request.result != UnityWebRequest.Result.Success)
//        {
//            Debug.LogError("Error fetching data: " + request.error);
//        }
//        else
//        {
//            string json = request.downloadHandler.text;
//            Debug.Log("Received JSON: " + json); // Log the received JSON for debugging
//            ProcessMemories(json);
//        }
//    }

//    void ProcessMemories(string json)
//    {
//        MemoryCollection memories = JsonUtility.FromJson<MemoryCollection>(json);

//        if (memories == null || memories.memories == null)
//        {
//            Debug.LogError("MemoryCollection or memories array is null. JSON might not be formatted correctly.");
//            return;
//        }

//        Debug.Log("Number of memories: " + memories.memories.Length);

//        foreach (var memory in memories.memories)
//        {
//            Debug.Log("Processing memory: " + memory.title);

//            GameObject newPedestal = Instantiate(PedestalPrefab, ParentTransform);

//            // Assigning the image (mediaUrl)
//            Image memoryImage = newPedestal.GetComponentInChildren<Image>();
//            if (memoryImage != null)
//            {
//                if (!string.IsNullOrEmpty(memory.mediaUrl))
//                {
//                    Debug.Log("Loading Image from URL: " + memory.mediaUrl);
//                    StartCoroutine(LoadImage(memory.mediaUrl, memoryImage));
//                }
//                else
//                {
//                    Debug.LogWarning("No mediaUrl provided for memory with title: " + memory.title);
//                    memoryImage.sprite = null; // Or assign a default sprite
//                }
//            }
//            else
//            {
//                Debug.LogError("Image component not found!");
//            }

//            // Assigning the title
//            Text titleText = newPedestal.transform.Find("TextCard/Panel/TitleText").GetComponent<Text>();
//            if (titleText != null)
//            {
//                titleText.text = memory.title;
//            }
//            else
//            {
//                Debug.LogError("TitleText not found!");
//            }

//            // Assigning the description
//            Text descriptionText = newPedestal.transform.Find("TextCard/Panel/DescriptionText").GetComponent<Text>();
//            if (descriptionText != null)
//            {
//                descriptionText.text = memory.description;
//            }
//            else
//            {
//                Debug.LogError("DescriptionText not found!");
//            }

//            // Assigning the audio (recordingUrl)
//            AudioSource audioSource = newPedestal.GetComponent<AudioSource>();
//            if (audioSource != null)
//            {
//                if (!string.IsNullOrEmpty(memory.recordingUrl))
//                {
//                    Debug.Log("Loading Audio from URL: " + memory.recordingUrl);
//                    StartCoroutine(LoadAudio(memory.recordingUrl, audioSource));
//                }
//                else
//                {
//                    Debug.LogWarning("No recordingUrl provided for memory with title: " + memory.title);
//                    audioSource.clip = null; // Or leave it empty if no audio is provided
//                }
//            }
//            else
//            {
//                Debug.LogError("AudioSource component not found!");
//            }
//        }
//    }

//    IEnumerator LoadImage(string url, Image img)
//    {
//        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
//        yield return request.SendWebRequest();

//        if (request.result != UnityWebRequest.Result.Success)
//        {
//            Debug.LogError("Error loading image: " + request.error);
//        }
//        else
//        {
//            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
//            img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
//        }
//    }

//    IEnumerator LoadAudio(string url, AudioSource audioSource)
//    {
//        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
//        yield return request.SendWebRequest();

//        if (request.result != UnityWebRequest.Result.Success)
//        {
//            Debug.LogError("Error loading audio: " + request.error);
//        }
//        else
//        {
//            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
//            audioSource.clip = clip;
//        }
//    }
//}

//[System.Serializable]
//public class Memory
//{
//    public string title;
//    public string description;
//    public string mediaUrl;
//    public string recordingUrl;
//}

//[System.Serializable]
//public class MemoryCollection
//{
//    public Memory[] memories;
//}


