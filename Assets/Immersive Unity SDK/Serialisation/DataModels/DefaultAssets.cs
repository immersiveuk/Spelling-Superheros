using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public enum ImageEnum
{
    None,
    CloseButton,
    NextButton,
    PreviousButton,
    PopupIcon,
    InformationIcon,
    PauseIcon,
    PlayIcon,
    ReplayIcon,
    QuestionIcon,
    TickIcon,
}

public class DefaultAssets : MonoBehaviour
{
    public static DefaultAssets _instance;

    public List<ImageFor> listDefaultImage = new List<ImageFor>();
    public List<FontDictionary> fontList = new List<FontDictionary>();

    private void Awake()
    {
        _instance = this;
    }
  
    public Sprite GetImage(ImageEnum imageEnum)
    {
        ImageFor imageFor = listDefaultImage.Find(obj => obj.imageEnum == imageEnum);

        if (imageFor != null)
            return imageFor.sprite;
        else
            return null;
    }

    /// <summary>
    /// returns the TextMesh font based on font name
    /// if fontName not matched it will returns default font
    /// </summary>
    /// <param name="fontName"></param>
    /// <returns></returns>
    public TMP_FontAsset GetFont(string fontName)
    {
        FontDictionary font = new FontDictionary();
        font = fontList.Find(obj => obj.fontName.Equals(fontName));

        if (font == null)
            return fontList[0].font;
        else
            return font.font;
    }

    [System.Serializable]
    public class ImageFor
    {
        public ImageEnum imageEnum;
        public Sprite sprite;
    }

    [System.Serializable]
    public class FontDictionary
    {
        public string fontName;
        public TMP_FontAsset font;
    }
}