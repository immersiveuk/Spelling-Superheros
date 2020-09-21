using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Enables a screen shake effect.
/// </summary>
public class ScreenShake : MonoBehaviour
{
    public Transform camerasHolder;

    private float shakeDuration = 1f;
    private float shakeMagnitude = 0.01f;

    private float timeRemaining = 0;

    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Update()
    {
        if (timeRemaining > 0)
        {
            camerasHolder.localPosition = initialPosition + Random.insideUnitSphere * (shakeMagnitude * (timeRemaining/shakeDuration));
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            timeRemaining = 0;
            camerasHolder.localPosition = initialPosition;
        }
    }

    /// <summary>
    /// Shakes the screen with default properties
    /// </summary>
    public void Shake()
    {
        Shake(1f, 0.01f);
    }

    /// <summary>
    /// Shakes the screen with the provided properties.
    /// </summary>
    public void Shake(float duration, float magnitude)
    {
        initialPosition = camerasHolder.localPosition;

        shakeDuration = duration;
        shakeMagnitude = magnitude;

        timeRemaining = duration;
    }
}


//#if UNITY_EDITOR

//[CustomEditor(typeof(ScreenShake))]
//public class ScreenShakeEditor: Editor
//{
//    private ScreenShake screenShake;

//    private void OnEnable()
//    {
//        screenShake = (ScreenShake)target; 
//    }

//    public override void OnInspectorGUI()
//    {
//        if (GUILayout.Button("Shake"))
//        {
//            screenShake.Shake();
//        }
//    }
//}

//#endif
