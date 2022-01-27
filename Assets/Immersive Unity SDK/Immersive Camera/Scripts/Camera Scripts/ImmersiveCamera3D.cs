/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using UnityEngine;
using UnityEditor;

namespace Com.Immersive.Cameras
{
    public class ImmersiveCamera3D : AbstractImmersiveCamera
    {

        public ScalingMode scalingMode = ScalingMode.FixedHorizontalFOV;
        public FloorCameraMode floorCameraMode = FloorCameraMode.Perspective;

        private Transform cameraHolder;

        private void Start()
        {
            cameraHolder = transform.Find("Cameras");
        }


        protected override void PositionCameras()
        {
            // 1. Calculate the Vertical field of view for main surface
            var vFOVCenter = CalculateCenterVerticalFieldOfView(centerSurface);
            var vFOVSides = CalculateSideVerticalFieldOfView(vFOVCenter);

            for (var i = 0; i < surfaces.Count; i++)
            {
                var surface = surfaces[i];

                var cam = cameras[i];
                // 2. Place the floor camera
                if (surface.position == SurfacePosition.Floor)
                {
                    if (floorCameraMode == FloorCameraMode.Perspective) PositionFloorCamera(cam, surface, vFOVCenter);
                    else if (floorCameraMode == FloorCameraMode.Orthographic) PositionFloorOrthographic(cam, surface);
                    continue;
                }

                // 3. Rotate Camera
                //Rotation in the Y-axis for a camera
                var horizontalRotation = Rotation(surface.position);
                //Rotate camera
                cam.transform.Rotate(new Vector3(0, horizontalRotation, 0));

                // 4. Calculate Field Of View for cameras
                cam.fieldOfView = (surface.position == SurfacePosition.Center) ? vFOVCenter : vFOVSides;

                // 5. Set Obliqueness

                //Set the obliqueness of the camera.
                //Used in situations where the side walls have a diffent aspect ratio to main surface.
                //For explanation https://docs.unity3d.com/Manual/ObliqueFrustum.html
                if (surface.position == SurfacePosition.Left || surface.position == SurfacePosition.Right)
                {

                    //How much to shift cameras
                    var obliqueness = 0f;
                    if (scalingMode == ScalingMode.FixedHorizontalFOV)
                    {
                        obliqueness = (centerSurface.aspectRatio - surface.aspectRatio) / (surface.aspectRatio);
                    }
                    if (scalingMode == ScalingMode.FixedVerticalFOV)
                    {
                        float aspectRatio16x9 = 16f / 9f;
                        obliqueness = (aspectRatio16x9 - surface.aspectRatio) / (surface.aspectRatio);
                    }

                    if (surface.rect.x > centerSurface.rect.x) obliqueness *= -1f;

                    SetCameraObliqueness(cam, obliqueness / 2, 0, surface.aspectRatio);
                }
            }


            //For Rotation Camera Buttons
            //startRotation = transform.eulerAngles;
            //targetRotation = startRotation;
        }

        private void PositionFloorCamera(Camera cam, SurfaceInfo floor, float vFOVWalls)
        {

            //Set Initial Camera Rotation relative to main camera
            cam.transform.localRotation = mainCamera.transform.localRotation;

            // 1. Rotate Camera
            //Rotation in the Y-axis for a camera
            var horizontalRotation = 0;
            //Rotation in the X-axis for a camera
            var verticalRotation = 90;
            //Rotate camera
            cam.transform.Rotate(new Vector3(verticalRotation, horizontalRotation, 0));

            // 2. Calculate Field Of View for cameras
            var vFOVFloor = CalculateVerticalFOV(floor, 180f - vFOVWalls);
            cam.fieldOfView = vFOVFloor;

            // 3. Obliqueness of floor

            ////How much to shift cameras
            //Cannot remember why this works but it does.
            var a = (Mathf.Deg2Rad * (90f - vFOVWalls / 2));
            var b = (Mathf.Deg2Rad * vFOVFloor) / 2f;
            var obliqueness = -(Mathf.Tan(b) - Mathf.Tan(a)) / (Mathf.Tan(b));

            SetCameraObliqueness(cam, 0, obliqueness / 2, floor.aspectRatio);
        }

        private void SetCameraObliqueness(Camera cam, float xShift, float yShift, float surfaceAspectRatio)
        {
            float fov = cam.fieldOfView;
            cam.usePhysicalProperties = true;
            cam.sensorSize = new Vector2(surfaceAspectRatio, 1);
            cam.lensShift = new Vector2(xShift, yShift);
            cam.fieldOfView = fov;
        }

        private void PositionFloorOrthographic(Camera cam, SurfaceInfo floor)
        {
            //Set Initial Camera Rotation relative to main camera
            cam.transform.localRotation = mainCamera.transform.localRotation;

            // 1. Rotate Camera
            //Rotation in the Y-axis for a camera
            var horizontalRotation = 0;
            //Rotation in the X-axis for a camera
            var verticalRotation = 90;
            //Rotate camera
            cam.transform.Rotate(new Vector3(verticalRotation, horizontalRotation, 0));

            cam.orthographic = true;
            cam.orthographicSize = 1;
            cam.nearClipPlane = 1;
        }

        //Horizontal Rotation for a particular surface position
        private int Rotation(SurfacePosition s)
        {
            switch (s)
            {
                case SurfacePosition.Left:
                    return -90;
                case SurfacePosition.Right:
                    return 90;
                case SurfacePosition.Back:
                    return 180;
                default:
                    return 0;
            }
        }


        private float CalculateSideVerticalFieldOfView(float vFOVCenter)
        {
            //If fixed horizontal field of view, side have same vFOV as center
            if (scalingMode == ScalingMode.FixedHorizontalFOV) return vFOVCenter;

            //Fixed Vertical Field Of View

            var hFOVCenter = CalculateHorizontalFOV(centerSurface, vFOVCenter);
            var hFOVSide = 180 - hFOVCenter;

            //Returns the vertical field of view of a side surface with 16x9 aspect ratio.
            return Mathf.Rad2Deg * 2.0f * Mathf.Atan(Mathf.Tan(hFOVSide * Mathf.Deg2Rad / 2) * (9f / 16));
        }

        private float CalculateCenterVerticalFieldOfView(SurfaceInfo surface)
        {
            //If fixed horizontal field of view, side have same vFOV as center
            if (scalingMode == ScalingMode.FixedHorizontalFOV) return CalculateVerticalFOV(surface, 90f);

            //Fixed Vertical Field Of View

            //This is the field of view of a 16x9 surface with 90 degress horizontal field of view
            return 58.71551f;
        }

        /// <summary>
        /// Calculates the Vertical Field Of View of a given surface
        /// to achieve a desired Horizontal Field Of View
        /// </summary>
        private float CalculateVerticalFOV(SurfaceInfo surface, float hFOV)
        {
            //Desired Horizontal Field of View in Radians
            var hFOVRad = hFOV * Mathf.Deg2Rad;
            //Vertical Field of View required to get desired horizontal field of view at current aspect ratio
            var vFOVRad = 2.0f * Mathf.Atan(Mathf.Tan(hFOVRad / 2) * (surface.rect.height / surface.rect.width));
            //Vertical Field of View in Degrees
            return (float)(Mathf.Rad2Deg * vFOVRad);
        }

        /// <summary>
        /// Calculates the Vertical Field Of View of a given surface
        /// to achieve a desired Vertical Field Of View
        /// </summary>
        private float CalculateHorizontalFOV(SurfaceInfo surface, float vFOV)
        {

            var vFOVRad = vFOV * Mathf.Deg2Rad;
            var hFOVRad = 2f * Mathf.Atan(Mathf.Tan(vFOVRad / 2f) * (surface.rect.width / surface.rect.height));
            return (float)(Mathf.Rad2Deg * hFOVRad);
        }


        //==============================================================
        // ROTATE CAMERA
        // Used by rotation buttons.
        //==============================================================

        private const float rotationDuration = 1f;
        private CameraRotation currentRotation = null;

        public override void RotateCameraLeft()
        {
            if (currentRotation == null)
                currentRotation = new CameraRotation(cameraHolder.localRotation, -90, rotationDuration);
            else
                currentRotation = new CameraRotation(currentRotation, -90, rotationDuration);
        }

        public override void RotateCameraRight()
        {
            if (currentRotation == null)
                currentRotation = new CameraRotation(cameraHolder.localRotation, 90, rotationDuration);
            else
                currentRotation = new CameraRotation(currentRotation, 90, rotationDuration);
        }

        private class CameraRotation {

            private Quaternion startRotation, targetRotation;
            private float startTime, endTime;

            public CameraRotation(Quaternion startRotation, float rotationDegrees, float duration)
            {
                startTime = Time.time;
                endTime = startTime + duration;

                this.startRotation = startRotation;
                var euler = startRotation.eulerAngles;
                euler.y += rotationDegrees;
                targetRotation = Quaternion.Euler(euler);
            }

            //Create new rotations from an existing rotation.
            public CameraRotation(CameraRotation currentRotation, float rotationDegrees, float duration)
            {
                startTime = Time.time;
                endTime = startTime + duration;

                this.startRotation = currentRotation.GetCurrentRotation();
                var euler = currentRotation.targetRotation.eulerAngles;
                euler.y += rotationDegrees;
                targetRotation = Quaternion.Euler(euler);
            }

            public bool IsComplete => Time.time >= endTime;

            public Quaternion GetCurrentRotation()
            {
                var t = Mathf.InverseLerp(startTime, endTime, Time.time);
                var rotation = Quaternion.Lerp(startRotation, targetRotation, t);
                return rotation;
            }
        }

        protected new void Update()
        {
            base.Update();

            if (currentRotation != null)
            {
                cameraHolder.localRotation = currentRotation.GetCurrentRotation();
                if (floorCamera != null)
                {
                    floorCamera.transform.rotation = transform.rotation;
                    floorCamera.transform.Rotate(new Vector3(90, 0, 0));
                }

                if (currentRotation.IsComplete)
                    currentRotation = null;
            }
        }

        //==============================================================
        // DATA STRUCTURES
        //==============================================================

        /// <summary>
        /// When in FixedVerticalFOV mode the vertical field of view of the center 
        /// screen is fixed to 58.71551. This is the equivalent vertical field of view of a
        /// 16x9 surface with 90 degrees horizontal field of view. In this mode whatever the screen
        /// sizes and aspect ratios the same amount of vertical information is shown.
        /// This mode can result in more warping artifacts.
        /// 
        /// When in FixedHorizontalFOV mode the horizontal field of view of the center
        /// screen is fixed to 90 degrees. 
        /// This can lead to vertical cropping of the image in non-standard spaces.
        /// </summary>
        public enum ScalingMode
        {
            FixedVerticalFOV,
            FixedHorizontalFOV
        }

        public enum FloorCameraMode { Perspective, Orthographic };
    }

    //==============================================================
    // CUSTOM EDITOR
    //==============================================================

#if UNITY_EDITOR

    [CustomEditor(typeof(ImmersiveCamera3D))]
    public class ImmersiveCamera3DEditor : AbstractImmersiveCameraEditor
    {
        protected override void OnInspectorGUICameraSpecificSettings()
        {
            base.OnInspectorGUICameraSpecificSettings();
            SerializedProperty scalingMode = serializedObject.FindProperty(nameof(scalingMode));
            SerializedProperty floorCameraMode = serializedObject.FindProperty(nameof(floorCameraMode));

            EditorGUILayout.LabelField("3D Camera Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(scalingMode);
            EditorGUILayout.PropertyField(floorCameraMode);
        }

    }

#endif

}
