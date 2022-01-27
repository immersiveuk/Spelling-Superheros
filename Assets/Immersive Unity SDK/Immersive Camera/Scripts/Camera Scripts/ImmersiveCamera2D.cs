/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Com.Immersive.Cameras
{
    public class ImmersiveCamera2D : AbstractImmersiveCamera
    {

        //Settings
        [Tooltip("The aspect ratio the content is built for. If the room doesn't match this aspect ratio it will be scaled.")]
        public AspectRatio targetAspectRatio = AspectRatio._16x10;


        //CAG Settings
        public bool displayCAG = false;
        private bool _displayingCAG = false;

        [Tooltip("If you don't know what this means, IGNORE IT!")]
        public CAGType cagType;

        //Floor
        public GameObject floorPrefab;
        public Transform floor;

        //DATA STRUCTURES
        public enum AspectRatio { _16x9, _16x10, _4x3 };
        public enum CAGType { Exterior, Interior };

        public float TargetAspectRatioFloat
        {
            get
            {
                switch (targetAspectRatio)
                {
                    case AspectRatio._16x10: return 16f / 10f;
                    case AspectRatio._16x9: return 16f / 9f;
                    case AspectRatio._4x3: return 4f / 3f;
                }
                return 0;
            }

        }

        private Transform cameraHolder;

        private void Start()
        {
            cameraHolder = transform.Find("Cameras");
        }

#if UNITY_EDITOR
        private ScreenSizes _screenSize;
        private SurfacePosition _layout;
        private CAGType _cagType;
        private List<Rect> rects;
        //Generate camera preview gizmo.
        private void OnDrawGizmos()
        {
            //Only draw if 2D mode.
            if (SceneView.currentDrawingSceneView != null && !SceneView.currentDrawingSceneView.in2DMode) return;


            if (EditorPrefs.HasKey("ImmersiveCameraScreenSize")) screenSize = (ScreenSizes)EditorPrefs.GetInt("ImmersiveCameraScreenSize");
            if (EditorPrefs.HasKey("ImmersiveCameraLayout")) layout = (SurfacePosition)EditorPrefs.GetInt("ImmersiveCameraLayout");

            //Check if camera settings have changed.
            if (_screenSize != screenSize || _layout != layout || _cagType != cagType)
            {
                _layout = layout;
                _screenSize = screenSize;
                _cagType = cagType;

                //Calculate new Surface Information
                rects = new List<Rect>();
                var surfacePositions = GetSurfacesFromLayout(layout);
                var surfaceRects = GenerateSurfaceRectsInEditor(surfacePositions);
                GenerateSurfacesInfo(surfacePositions, surfaceRects);

                //Calculate new cameras size and position.
                for (int i = 0; i < walls.Count; i++)
                {
                    var surface = walls[i];
                    Rect rect = new Rect();
                    rect.size = new Vector2(surface.aspectRatio, height * 2);

                    if (cagType == CAGType.Interior) rect.center = GetWallCameraPositionCAGI(surface);
                    else if (cagType == CAGType.Exterior) rect.center = GetWallCameraPositionCAGE(surface);

                    rects.Add(rect);
                }
            }

            //Draw camera frustrums.
            float xPos = transform.position.x;
            float yPos = transform.position.y;
            var zPos = transform.position.z + mainCamera.farClipPlane;
            Gizmos.color = Color.white;
            for (int i = 0; i < rects.Count; i++)
            {
                var rect = rects[i];
                Gizmos.DrawLine(new Vector3(xPos + rect.xMin, yPos + rect.yMax, zPos), new Vector3(xPos + rect.xMax, yPos + rect.yMax, zPos));
                Gizmos.DrawLine(new Vector3(xPos + rect.xMin, yPos + rect.yMin, zPos), new Vector3(xPos + rect.xMax, yPos + rect.yMin, zPos));
                Gizmos.DrawLine(new Vector3(xPos + rect.xMin, yPos + rect.yMin, zPos), new Vector3(xPos + rect.xMin, yPos + rect.yMax, zPos));
                Gizmos.DrawLine(new Vector3(xPos + rect.xMax, yPos + rect.yMin, zPos), new Vector3(xPos + rect.xMax, yPos + rect.yMax, zPos));
            }
        }
#endif
        /// <summary>
        /// The scale factor required to make content fit in desired aspect ratio
        /// </summary>
        private float ScaleFactor
        {
            get
            {
                //This is the actual aspect ratio of the display being targeted
                float displayAspectRatio = widestSideSurface.aspectRatio;
                //If it is  greater than 2 then it is a wide space and is treated differently.
                if (displayAspectRatio > 2) displayAspectRatio /= 2;

                //float scale = SurfaceAspectRatio / displayAspectRatio;
                return displayAspectRatio / TargetAspectRatioFloat;
            }
        }


        protected override void PositionCameras()
        {
            for (var i = 0; i < surfaces.Count; i++)
            {
                Camera cam = cameras[i];
                SurfaceInfo surface = surfaces[i];

                if (surface.position == SurfacePosition.Floor)
                {
                    PositionFloorCamera(cam);
                    continue;
                }
                else
                {
                    PositionWallCamera(cam, surface);
                }
            }

            ScaleStage();

            //For Rotation Camera Buttons
            startPosition = transform.localPosition;
            targetPosition = startPosition;

        }

        private float height = 0.5f;

        /// <summary>
        /// Positions a camera 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="surface"></param>
        private void PositionWallCamera(Camera cam, SurfaceInfo surface)
        {
            if (walls.Count == 1) { return; }

            if (cagType == CAGType.Exterior) cam.transform.localPosition = GetWallCameraPositionCAGE(surface);
            else if (cagType == CAGType.Interior) cam.transform.localPosition = GetWallCameraPositionCAGI(surface);
        }

        private Vector3 GetWallCameraPositionCAGI(SurfaceInfo surface)
        {
            float x = 0;
            switch (surface.position)
            {
                case SurfacePosition.Left:
                    x = -2 * TargetAspectRatioFloat * ScaleFactor;
                    break;
                case SurfacePosition.Right:
                    x = 2 * TargetAspectRatioFloat * ScaleFactor;
                    break;
                case SurfacePosition.Back:
                    x = 4 * TargetAspectRatioFloat * ScaleFactor;
                    break;
            }
            return new Vector3(x, 0, 0);
        }

        private Vector3 GetWallCameraPositionCAGE(SurfaceInfo surface)
        {
            float mainCameraWidth = centerSurface.aspectRatio * height * 2;
            float rightCameraWidth = rightSurface.aspectRatio * height * 2;
            float x = 0;
            switch (surface.position)
            {
                case SurfacePosition.Left:
                    x = -(mainCameraWidth / 2 + surface.aspectRatio * height);
                    break;
                case SurfacePosition.Right:
                    x = (mainCameraWidth / 2 + surface.aspectRatio * height);
                    break;
                case SurfacePosition.Back:
                    x = mainCameraWidth / 2 + rightCameraWidth + surface.aspectRatio * height;
                    break;
            }
            return new Vector3(x, 0, 0);
        }

        private void PositionFloorCamera(Camera cam)
        {
            cam.transform.eulerAngles = new Vector3(90, 0, 0);
        }

        private void ScaleStage()
        {
            if (stage == null) return;

            stage.localScale = new Vector3(ScaleFactor, 1, 1);
            floor.localScale = new Vector3(ScaleFactor, 1, 1);

            //Add scaling to viewport width
            ViewportWidth /= ScaleFactor;
        }


        //==============================================================
        // ROTATE CAMERA
        // Used by rotation buttons.
        //==============================================================

        //Variables associated with moving the camera
        private Vector3 startPosition = Vector3.zero;
        private Vector3 targetPosition = Vector3.zero;
        private float moveTime = 0f;
        private bool movingCam = false;

        private enum CameraRotation { Centre, Left, Right }
        private CameraRotation cameraRotation = CameraRotation.Centre;

        private float StandardTranslation()
        {

            var aspectRatio = mainCamera.aspect;

            //If wide space
            var cagMultiplier = (cagType == CAGType.Interior && aspectRatio < 2) ? 2f : 1f;
            
            //if (aspectRatio > 2)
            //    aspectRatio *= 0.5f;
            return aspectRatio * cagMultiplier;
        }

        private float GetTargetXPos()
        {
            switch (cameraRotation)
            {
                case CameraRotation.Left:
                    return -StandardTranslation();
                case CameraRotation.Right:
                    return StandardTranslation();
                default:
                    return 0;
            }
        }

        private void CalculateNewCameraRotation()
        {
            startPosition = cameraHolder.localPosition;
            targetPosition = new Vector3(GetTargetXPos(), 0, 0);
            moveTime = 0f;
            movingCam = true;
        }

        public override void RotateCameraLeft()
        {
            switch (cameraRotation)
            {
                case CameraRotation.Left:
                    return;
                case CameraRotation.Centre:
                    cameraRotation = CameraRotation.Left;
                    break;
                case CameraRotation.Right:
                    cameraRotation = CameraRotation.Centre;
                    break;
            }
            CalculateNewCameraRotation();
        }

        public override void RotateCameraRight()
        {
            switch (cameraRotation)
            {
                case CameraRotation.Right:
                    return;
                case CameraRotation.Centre:
                    cameraRotation = CameraRotation.Right;
                    break;
                case CameraRotation.Left:
                    cameraRotation = CameraRotation.Centre;
                    break;
            }
            CalculateNewCameraRotation();
        }

        public void Translate(Vector3 translation, CAGType cagType)
        {
            transform.Translate(translation * ScaleFactor);
            this.cagType = cagType;
            cameraHolder.localPosition = new Vector3(GetTargetXPos(), 0, 0);
            PositionFloorCameraWhenRotating();
            PositionCameras();
        }

        //This is a complete hack to stop the floor rotating.
        private void PositionFloorCameraWhenRotating()
        {
            if (floorCamera == null)
                return;
            var floorPosition = cameraHolder.localPosition;
            floorPosition.x *= -1;
            floorCamera.transform.localPosition = floorPosition;
        }

        private new void Update()
        {
            base.Update();

            //Rotate Camera
            if (movingCam == true)
            {
                if (moveTime < 1)
                {
                    Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, moveTime);
                    cameraHolder.localPosition = currentPosition;
                    PositionFloorCameraWhenRotating();

                    moveTime += Time.deltaTime;
                }
                else
                {
                    cameraHolder.localPosition = targetPosition;
                    PositionFloorCameraWhenRotating();
                    moveTime = 0f;
                    movingCam = false;
                }
            }

            //Turn CAG on/off
            if (_displayingCAG != displayCAG)
            {
                _displayingCAG = displayCAG;
                CAGOverlay CAG = transform.Find("CAG Overlay").GetComponent<CAGOverlay>();

                if (displayCAG)
                {
                    if (cagType == CAGType.Exterior) CAG.DisplayCAGE(TargetAspectRatioFloat * ScaleFactor);
                    else if (cagType == CAGType.Interior) CAG.DisplayCAGI(TargetAspectRatioFloat * ScaleFactor);
                }
                else
                {
                    CAG.Hide();
                }
            }
        }

        //----------Add Floor-----------

        public void AddFloor()
        {
#if UNITY_EDITOR
            var instantiatedFloor = PrefabUtility.InstantiatePrefab(floorPrefab) as GameObject;
            floor = instantiatedFloor.transform;
            floor.SetSiblingIndex(transform.GetSiblingIndex());
            floor.name = "Floor (Place All Floor Objects Here)";
#endif
        }

    }

#if UNITY_EDITOR

    //==============================================================
    // CUSTOM EDITOR
    //==============================================================

    [CustomEditor(typeof(ImmersiveCamera2D))]
    public class ImmersiveCamera2DEditor : AbstractImmersiveCameraEditor
    {
        protected override void OnInspectorGUICameraSpecificSettings()
        {
            base.OnInspectorGUICameraSpecificSettings();

            SerializedProperty displayCAG = serializedObject.FindProperty("displayCAG");
            SerializedProperty floorPrefab = serializedObject.FindProperty("floorPrefab");
            SerializedProperty targetAspectRatio = serializedObject.FindProperty("targetAspectRatio");
            SerializedProperty cagType = serializedObject.FindProperty("cagType");


            EditorGUILayout.LabelField("2D Camera Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(targetAspectRatio);
            EditorGUILayout.PropertyField(displayCAG);
            EditorGUILayout.PropertyField(cagType);


            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(floorPrefab);

            AddFloor();

            serializedObject.ApplyModifiedProperties();
        }


        private void AddFloor()
        {
            ImmersiveCamera2D immersiveCamera = (ImmersiveCamera2D)target;
            if (immersiveCamera.floor == null && immersiveCamera.gameObject.scene.IsValid())
            {
                immersiveCamera.AddFloor();
            }
        }
    }

#endif

}