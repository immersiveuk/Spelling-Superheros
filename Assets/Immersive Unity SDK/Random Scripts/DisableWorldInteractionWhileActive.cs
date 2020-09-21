using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component will disable world interaction (Eg. Hotspots and Wipe To Reveals) while it is active.
/// </summary>
public class DisableWorldInteractionWhileActive : MonoBehaviour
{

    
    // Start is called before the first frame update
    void Start()
    {
        AbstractImmersiveCamera.CurrentImmersiveCamera.worldInteractionOn = false;
    }

    private void OnDisable()
    {
        AbstractImmersiveCamera.CurrentImmersiveCamera.worldInteractionOn = true;
    }
}
