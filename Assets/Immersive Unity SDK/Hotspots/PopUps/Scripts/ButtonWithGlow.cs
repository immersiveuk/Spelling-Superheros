using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ButtonWithGlow : MonoBehaviour
{
    [SerializeField] Button button = null;
    [SerializeField] Image glow = null;

    public Image ButtonImage => button.GetComponent<Image>();

    public void ToggleGlow(bool enabled) => glow.gameObject.SetActive(enabled);

    public void SetGlowColour(Color color) => glow.color = color;
}
