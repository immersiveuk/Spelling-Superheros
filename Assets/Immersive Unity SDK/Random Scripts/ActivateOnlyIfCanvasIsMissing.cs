using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ActivateOnlyIfCanvasIsMissing : MonoBehaviour
{
    private enum Side { Left, Right }
    [SerializeField] Side side = Side.Left;

    // Start is called before the first frame update
    void Start()
    {
        var immersiveCam = AbstractImmersiveCamera.CurrentImmersiveCamera;
        var walls = immersiveCam.walls;
        Debug.Log("Text");
        if (side == Side.Left && !HasLeftWall(walls))
            gameObject.SetActive(true);
        else if (side == Side.Right && !HasRightWall(walls))
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
    private bool HasRightWall(List<SurfaceInfo> walls)
    {
        foreach (var wall in walls)
        {
            if (wall.position == SurfacePosition.Right)
                return true;
        }
        return false;
    }

    private bool HasLeftWall(List<SurfaceInfo> walls)
    {
        foreach (var wall in walls)
        {
            if (wall.position == SurfacePosition.Left)
                return true;
        }
        return false;
    }
}
