/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* Written by Luke Bissell <luke@immersive.co.uk>, Sept 2019
*/

using Com.Immersive.Cameras;
using Com.Immersive.Cameras.PostProcessing;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An example of how to control the Blur and Desaturate filter.
/// Focus point is wherer the user touches to wall.
/// Intensity is increased and decreased using the W and S keys.
/// Focal Radius is increased and decreased using the D and A keys.
/// </summary>
public class BlurAroundTouch : MonoBehaviour
{

    private float intensity = 0;
    private float radius = 0;
    private float increment = 0.003f;

    private void OnEnable() => AbstractImmersiveCamera.AnySurfaceTouchedEvent.AddListener(OnSurfaceTouched);
    private void OnDisable() => AbstractImmersiveCamera.AnySurfaceTouchedEvent.RemoveListener(OnSurfaceTouched);

    private void OnSurfaceTouched(SurfaceTouchedEventArgs args)
    {
        BlurAndDesaturate.CurrentBlurAndDesaturate.SetFocalPoint(args.ScreenPoint, args.RenderingCameraIndex);
    }
     
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.W))
        {
            intensity += increment;
            if (intensity > 3) intensity = 3;
            BlurAndDesaturate.CurrentBlurAndDesaturate.SetIntensity(intensity);
        }
        if (Input.GetKey(KeyCode.S))
        {
            intensity -= increment;
            if (intensity < 0) intensity = 0;
            BlurAndDesaturate.CurrentBlurAndDesaturate.SetIntensity(intensity);
        }

        if (Input.GetKey(KeyCode.D))
        {
            radius += increment;
            if (radius > 1) radius = 1;
            BlurAndDesaturate.CurrentBlurAndDesaturate.SetFocalRadius(radius);
        }

        if (Input.GetKey(KeyCode.A))
        {
            radius -= increment;
            if (radius < 0) radius = 0;
            BlurAndDesaturate.CurrentBlurAndDesaturate.SetFocalRadius(radius);
        }

    }
}
