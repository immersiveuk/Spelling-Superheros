using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class RuntimeLoading : Singleton<RuntimeLoading>
{

    Dictionary<string, Texture2D> dictionaryImages = new Dictionary<string, Texture2D>();

    string BaseAssetsPath;
    public void SetBasePath(string assetsPath)
    {
        BaseAssetsPath = assetsPath;
    }

    public void LoadImage(string path, Action<Texture2D, Sprite> action)
    {

        if (string.IsNullOrEmpty(path))
        {
            action.Invoke(null, null);
            return;
        }

        if (dictionaryImages.ContainsKey(Path.GetFileName(path)))
        {
            Texture2D texture = dictionaryImages[Path.GetFileName(path)];
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1080);
            action.Invoke(texture, sprite);

            return;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        path = BaseAssetsPath + "/" + path; 
#else
        if (!path.Contains(BaseAssetsPath))
        {
            //Debug.LogError(path);
            path = "file:///" + BaseAssetsPath + "/" + path;
        }

        if (!path.Contains("file:///"))
        {
            path = "file:///" + path;
        }


#endif

        StartCoroutine(DownloadImage(path, action));
    }

    IEnumerator DownloadImage(string path, Action<Texture2D, Sprite> action)
    {
        Texture2D texture = new Texture2D(1, 1);

#if UNITY_WEBGL && !UNITY_EDITOR
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(path))
            {                
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    texture = DownloadHandlerTexture.GetContent(www);                                        
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1080);
                    action.Invoke(texture, sprite);
                }
            }
#else
        string fileName = string.Format(Application.persistentDataPath + "/Assets/Images/" + Path.GetFileName(path));
        if (!File.Exists(fileName))
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(path))
            {
                www.downloadHandler = new DownloadHandlerFile(fileName);

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    //var texture = DownloadHandlerTexture.GetContent(www);                    
                    texture.LoadImage(File.ReadAllBytes(fileName));
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1080);
                    action.Invoke(texture, sprite);
                }
            }
        }
        else
        {
            texture.LoadImage(File.ReadAllBytes(fileName));
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1080);
            action.Invoke(texture, sprite);
        }
#endif

        if (!dictionaryImages.ContainsKey(Path.GetFileName(path)))
            dictionaryImages.Add(Path.GetFileName(path), texture);
    }




    /// <summary>
    /// Load AudioFile from URL
    /// </summary>
    /// <param name="path"></param>
    /// <param name="action"></param>
    public void LoadAudio(string path, Action<AudioClip> action)
    {
        if (string.IsNullOrEmpty(path))
        {
            action.Invoke(null);
            return;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        path = BaseAssetsPath + "/" + path; 
#else
        path = "file:///" + BaseAssetsPath + "/" + path;
#endif

        StartCoroutine(LoadAudioFile(path, action));
    }

    IEnumerator LoadAudioFile(string path, Action<AudioClip> action)
    {

        UnityWebRequest AudioFiles = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.UNKNOWN);

        yield return AudioFiles.SendWebRequest();

        if (AudioFiles.isNetworkError)
        {
            Debug.Log(AudioFiles.error);
        }
        else
        {
            try
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(AudioFiles);
                action.Invoke(clip);
            }
            catch
            {
                action.Invoke(null);
            }
        }
    }

    public void LoadJson(string path, Action<string, bool> action)
    {
        StartCoroutine(DownloadJson(path, action));
    }

    IEnumerator DownloadJson(string path, Action<string, bool> action)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                action.Invoke("", false);
            }
            else
            {
                //Debug.Log(www.downloadHandler.text);
                action.Invoke(www.downloadHandler.text, true);
            }
        }
    }

    public static Sprite ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();

        Sprite sprite = Sprite.Create(result, new Rect(0, 0, result.width, result.height), new Vector2(0.5f, 0.5f), 1080);
        return sprite;
    }
}