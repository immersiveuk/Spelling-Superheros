using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Com.Immersive.Hotspots.MatchingPairPopUpSetting.MatchingPair;
using static Com.Immersive.Hotspots.MatchingPairPopUpSetting;
using UnityEngine.UI;

public class MatchingPairOption : MonoBehaviour
{
    public TextMeshProUGUI textOption;
    public Button button;
    public RectTransform from, target;
    public RectTransform rectLine;

    public Pair pair;

    public void SetOption(Pair pair, OptionType optionType, System.Action<MatchingPairOption> action)
    {
        this.pair = pair;

        if (optionType == OptionType.Left)
            textOption.text = pair.leftPart;
        else
            textOption.text = pair.rightPart;

        button.onClick.AddListener(delegate
        {
            action(this);
        });
    }

    public void DrawLine(RectTransform contentRect, Transform to)
    {
        rectLine.gameObject.SetActive(true);

        Vector2 toWorld = contentRect.TransformPoint(to.position);
        Vector2 fromWorld = contentRect.TransformPoint(from.position);

        Vector2 rot = toWorld - fromWorld;

        rectLine.localEulerAngles = new Vector3(0, 0, FindDegree(rot.y, rot.x));

        float distance = Vector2.Distance(RectTransformUtility.CalculateRelativeRectTransformBounds(contentRect, from).center, RectTransformUtility.CalculateRelativeRectTransformBounds(contentRect, to).center);

        rectLine.sizeDelta = new Vector2(Mathf.Abs(distance), 5);
    }

    public float FindDegree(float x, float y)
    {
        float value = (float)((Mathf.Atan2(x, y) / Mathf.PI) * 180f);

        if (value < 0) value += 360f;

        return value;
    }
}
