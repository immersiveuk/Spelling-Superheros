using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAndDisableAfterDelay : AbstractActivateAndDisable
{
    // Start is called before the first frame update
    void Start()
    {
        DisableActivateObjectsOnStart();
        ActivateAndDisable();
    }
}
