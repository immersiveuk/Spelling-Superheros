using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPositioner : MonoBehaviour
{
    public WallType wallType;

    void Start()
    {
        transform.localPosition = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[(int)wallType].transform.localPosition;

        if (wallType == WallType.Center)
            transform.parent.localScale = new Vector3(1 / FindObjectOfType<Stage>().transform.localScale.x, 1, 1);
    }
}
