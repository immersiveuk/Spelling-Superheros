/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* Written by Luke Bissell <luke@immersive.co.uk>, July 2019
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Com.Immersive.Cameras.AbstractImmersiveCamera;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Com.Immersive.Cameras
{
    public abstract class AbstractImmersiveCamera : MonoBehaviour
    {

        //==============================================================
        // STATIC VARIABLES
        //==============================================================

        public static AbstractImmersiveCamera CurrentImmersiveCamera;

        //==============================================================
        // PUBLIC VARIABLES
        //==============================================================

        public bool background16x10 = false;

        //The desired screen size setup. Only used in editor
        [NonSerialized]
        public ScreenSizes screenSize = ScreenSizes.Standard;
        //Desired Layout. Can be overridden by arguments.
        [NonSerialized]
        public SurfacePosition layout = SurfacePosition.WallsAndFloor;

        //Whether to play in the virtual room mode
        public EditorDisplayMode editorDisplayMode = EditorDisplayMode.VirtualRoom;
        public enum EditorDisplayMode { VirtualRoom, Panning };

        [Tooltip("What display mode should be used in Build. Default will just run as an immersive application.")]
        public BuildDisplayMode buildDisplayMode = BuildDisplayMode.Default;
        public enum BuildDisplayMode { Default, VirtualRoom, Panning };

        //INTERACTION SETTINGS
        [Tooltip("Whether the Interaction system should be used in this scene.")]
        public bool interactionOn = true;
        [Tooltip("Whether point touches should be registered.")]
        public bool pointTouchesOn = true;
        [Tooltip("Whether an icon should be displayed for point touches.")]
        public bool displayTouchPoints = true;
        [Tooltip("Whether area touches should be registered.")]
        public bool areaTouchesOn = true;
        [Tooltip("Whether a representation of area touches should be displayed.")]
        public bool displayAreaTouches = true;
        //Floor
        public bool customFloorInteractionSettings = false;
        [Tooltip("Whether the Interaction system should be used in this scene.")]
        public bool floorInteractionOn = true;
        [Tooltip("Whether point touches should be registered.")]
        public bool floorPointTouchesOn = true;
        [Tooltip("Whether an icon should be displayed for point touches.")]
        public bool floorDisplayTouchPoints = true;
        [Tooltip("Whether area touches should be registered.")]
        public bool floorAreaTouchesOn = true;
        [Tooltip("Whether a representation of area touches should be displayed.")]
        public bool floorDisplayAreaTouches = true;


        public Camera mainCamera;
        public Camera floorCamera;
        public Camera leftUICamera;
        public Camera centerUICamera;
        public Camera rightUICamera;
        public Camera backUICamera;
        public Camera floorUICamera;
        public List<Camera> uiCameras;

        public ProcessTouches interactionPrefab;
        public GameObject canvasTouchPrefab;

        private AudioSource audioSource;

        public GameObject virtualRoomPrefab;
        public GameObject panningViewPrefab;
        private RenderTexture leftTexture;
        private RenderTexture centerTexture;
        private RenderTexture rightTexture;
        private RenderTexture backTexture;
        private RenderTexture floorTexture;
        private List<RenderTexture> renderTextures;

        //Empty Object which contains all game objects
        public Transform stage;
        public GameObject stagePrefab;


        //==============================================================
        // PRIVATE AND PROTECTED VARIABLES
        //==============================================================

        [NonSerialized]
        public DisplayMode displayMode = DisplayMode.SpanningDisplay;

        //List of all the camera
        [NonSerialized]
        public List<Camera> cameras;
        [NonSerialized]
        public List<Camera> wallCameras;

        //Information about all the surfaces in the immersive space.
        protected SurfaceInfo leftSurface;
        [NonSerialized]
        public SurfaceInfo centerSurface;
        protected SurfaceInfo rightSurface;
        protected SurfaceInfo widestSideSurface;
        [NonSerialized]
        public List<SurfaceInfo> surfaces = new List<SurfaceInfo>();
        [NonSerialized]
        public List<SurfaceInfo> walls = new List<SurfaceInfo>();
        [NonSerialized]
        public Rect resolution;
        [NonSerialized]
        public Rect resolutionWithoutFloor;
        public int WallCount => walls.Count;
        public bool HasLeftWall => (surfaces[0].position == SurfacePosition.Left);
        /// <summary>
        /// The width in Unity Units of the entire wall viewport.
        /// Note. This takes into account that Stage scaling on 2D scenes.
        /// </summary>
        public float ViewportWidth { get; protected set; }

        /// <summary>If true touches will be passed to interactive components on the canvasses.</summary>
        [NonSerialized]
        public bool canvasInteractionOn = true;
        /// <summary>If true touches will be passed to interactive objects in the scene.</summary>
        [NonSerialized]
        public bool worldInteractionOn = true;

        //Instantiated Virtual Room
        private VirtualRoomController virtualRoom;
        private PanViewController panningView;

        //==============================================================
        //SETUP
        //==============================================================

        private static bool IsFirstScene = true;

        // Start is called before the first frame update
        protected void Awake()
        {
            CurrentImmersiveCamera = this;

            Cursor.visible = false;

            List<Rect> surfaceRects = null;
            // 1. Read Parameters
            if (!Application.isEditor)
            {
                layout = ReadParameters.Settings.Layout;
                surfaceRects = ReadParameters.Settings.Surfaces;

#if UNITY_WEBGL && !UNITY_EDITOR
                layout = SurfacePosition.Walls;
                surfaceRects = new List<Rect>();
                surfaceRects.Add(new Rect(0, 0, 1920, 1080));
                surfaceRects.Add(new Rect(1920, 0, 1920, 1080));
                surfaceRects.Add(new Rect(3840, 0, 1920, 1080));
#endif

                if (ReadParameters.Settings.Debug)
                {
                    displayAreaTouches = true;
                    displayTouchPoints = true;
                    floorDisplayAreaTouches = true;
                    floorDisplayTouchPoints = true;
                }
            }
            else
            {
#if UNITY_EDITOR
                if (EditorPrefs.HasKey("EditorDisplayMode"))
                {
                    editorDisplayMode = (EditorDisplayMode)EditorPrefs.GetInt("EditorDisplayMode");
                }

                if (EditorPrefs.HasKey("ImmersiveCameraScreenSize"))
                {
                    screenSize = (ScreenSizes)EditorPrefs.GetInt("ImmersiveCameraScreenSize");
                }

                if (EditorPrefs.HasKey("ImmersiveCameraLayout"))
                {
                    layout = (SurfacePosition)EditorPrefs.GetInt("ImmersiveCameraLayout");
                }
#endif
            }
            if (layout == SurfacePosition.None)
            {
                Debug.LogError("Error: No Surfaces selected.");
                return;
            }


            // 2. Create a list of all the inidividual surfaces from provided layout
            var surfacesPositions = GetSurfacesFromLayout(layout);
            var surfacesPositionsWithoutFloor = GetSurfacesFromLayout(layout);
            surfacesPositionsWithoutFloor.Remove(SurfacePosition.Floor);

            // 3. When screen resolutions are not provided generate them from editor settings
            if (surfaceRects == null)
            {
                surfaceRects = GenerateSurfaceRectsInEditor(surfacesPositions);
            }

            // 4. Calculate total resolutions and resolution excluding the floor
            CalculateResolutions(surfaceRects);

            // 5. Create a unified list of surfaces, containing their resolution and layout position
            GenerateSurfacesInfo(surfacesPositions, surfaceRects);

            // 6. Activates the required displays for Target Display Mode or Spanning Display Mode
            ActicateDisplays();

            // 7. Setup Cameras to render correct part of the scene
            CreateCameras();

            // 9. Remove unnecessary canvases.
            SetupCanvases(surfacesPositions);

            // 8. Position Cameras
            // Abstract Completed by Implementation
            PositionCameras();

            // 10. Creates TCP listeners for touches
            SetupInteractionsListeners();

            // 11. Find Audio Player
            audioSource = transform.Find("Audio")?.GetComponent<AudioSource>();
        }

        //-------------------
        // 2. Create a list of all the inidividual surfaces from provided layout
        //-------------------

        //Provides a sorted list of surfaces from a given layout
        protected List<SurfacePosition> GetSurfacesFromLayout(SurfacePosition layout)
        {
            var surfaces = new List<SurfacePosition>();
            foreach (Enum value in Enum.GetValues(layout.GetType()))
            {
                var pos = (SurfacePosition)value;
                if ((layout & pos) == pos)
                {
                    if (pos != SurfacePosition.CentreRight && pos != SurfacePosition.LeftCenter && pos != SurfacePosition.Walls
                        && pos != SurfacePosition.None && pos != SurfacePosition.WallsAndFloor && pos != SurfacePosition.AllWalls
                        && pos != SurfacePosition.AllWallsAndFloor)
                    {
                        surfaces.Add(pos);
                    }
                }
            }

            //Sort surfaces from left to right, then floor
            surfaces.Sort();

            return surfaces;
        }

        protected void CalculateResolutions(List<Rect> surfaceRects)
        {
            resolution = new Rect();
            foreach (var rect in surfaceRects)
            {
                //Check if its the rightmost camera
                if (rect.x + rect.width > resolution.width)
                {
                    resolution.width = rect.x + rect.width;
                }

                if (rect.y + rect.width > resolution.height)
                {
                    resolution.height = rect.y + rect.height;
                }
            }
            resolutionWithoutFloor = resolution;
            ViewportWidth = resolutionWithoutFloor.width / resolutionWithoutFloor.height;
        }


        //-------------------
        // 3. When screen resolutions are not provided generate them from editor settings
        //-------------------

        protected List<Rect> GenerateSurfaceRectsInEditor(List<SurfacePosition> surfacePositions)
        {

            return GenerateSurfaceResolutionsInEditor.GenerateSurfaceRects(screenSize, surfacePositions, 1920);
        }

        //-------------------
        // 5. Create a unified list of surfaces,
        //    containing their resolution and layout position and store reference to main surface
        //-------------------

        protected void GenerateSurfacesInfo(List<SurfacePosition> surfacePositions, List<Rect> surfaceRects)
        {
            surfaces = new List<SurfaceInfo>();
            walls = new List<SurfaceInfo>();
            leftSurface = new SurfaceInfo();
            centerSurface = new SurfaceInfo();
            rightSurface = new SurfaceInfo();
            //Make sure arguments match
            if (surfacePositions.Count != surfaceRects.Count)
            {
                Debug.LogError("Layout provided doesn't match surface dimensions provided.");
                return;
            }

            for (int i = 0; i < surfacePositions.Count; i++)
            {

                var position = surfacePositions[i];
                var rect = surfaceRects[i];

                var surfaceInfo = new SurfaceInfo(rect, position);

                //Add to all surfaces list
                surfaces.Add(surfaceInfo);

                //Not floor
                if (position != SurfacePosition.Floor)
                {
                    walls.Add(surfaceInfo);
                }
                else
                {
                    resolutionWithoutFloor = new Rect(0, 0, resolution.width - rect.width, resolution.height);
                }

                switch (position)
                {
                    case SurfacePosition.Left: leftSurface = surfaceInfo; break;
                    case SurfacePosition.Center: centerSurface = surfaceInfo; break;
                    case SurfacePosition.Right: rightSurface = surfaceInfo; break;
                }

                //Check if widest center
                widestSideSurface = centerSurface;
                if (position == SurfacePosition.Left || position == SurfacePosition.Right)
                {
                    if (widestSideSurface.aspectRatio < surfaceInfo.aspectRatio) widestSideSurface = surfaceInfo;
                }
            }
        }

        //-------------------
        // 6. Activates the required displays for Target Display Mode or Spanning Display Mode
        //-------------------

        private Rect windowRect;
        /// <summary>
        /// Calculates a rect for the entire window.
        /// </summary>
        private void CalculateWindowRect()
        {
            windowRect = new Rect();
            foreach (var surface in surfaces)
            {
                if (surface.rect.xMin < windowRect.xMin)
                    windowRect.xMin = surface.rect.xMin;
                if (surface.rect.yMin < windowRect.yMin)
                    windowRect.yMin = surface.rect.yMin;
                if (surface.rect.xMax > windowRect.xMax)
                    windowRect.xMax = surface.rect.xMax;
                if (surface.rect.yMax > windowRect.yMax)
                    windowRect.yMax = surface.rect.yMax;
            }
        }

        private Rect CalculateViewportRect(SurfaceInfo surfaceInfo)
        {
            Rect viewportRect = new Rect
            {
                xMin = Mathf.InverseLerp(windowRect.xMin, windowRect.xMax, surfaceInfo.rect.xMin),
                xMax = Mathf.InverseLerp(windowRect.xMin, windowRect.xMax, surfaceInfo.rect.xMax),
                yMin = Mathf.InverseLerp(windowRect.yMin, windowRect.yMax, surfaceInfo.rect.yMin),
                yMax = Mathf.InverseLerp(windowRect.yMin, windowRect.yMax, surfaceInfo.rect.yMax)
            };

            //Note: This is to fix an ensure that the floor aligns to the top of the window.
            //      This was an issue when the floor was lower resolution than the other surfaces.
            if (viewportRect.height < 1)
                viewportRect.y += 1 - viewportRect.height;

            return viewportRect;
        }

        private void CalculateDisplayModeBuild()
        {
            if (buildDisplayMode == BuildDisplayMode.VirtualRoom || ReadParameters.Settings.VirtualRoom) displayMode = DisplayMode.VirtualRoom;
            else if (buildDisplayMode == BuildDisplayMode.Panning || ReadParameters.Settings.PanningPreview) displayMode = DisplayMode.PanningView;
            else
            {
#if UNITY_2019_1_OR_NEWER
                displayMode = DisplayMode.BorderlessWindow;
#else
                displayMode = (Display.displays.Length >= walls.Count) ? DisplayMode.TargetDisplay : DisplayMode.SpanningDisplay;
#endif
            }
        }

        private void CalculateDisplayModeEditor()
        {
            if (editorDisplayMode == EditorDisplayMode.VirtualRoom) displayMode = DisplayMode.VirtualRoom;
            else displayMode = DisplayMode.PanningView;
        }

        private void ActicateDisplays()
        {
#if UNITY_EDITOR
            CalculateDisplayModeEditor();
#else
            CalculateDisplayModeBuild();
#endif

            Application.targetFrameRate = 60;

            // 2. Activate Displays

            //Borderless Window
            if (displayMode == DisplayMode.BorderlessWindow)
            {
                CalculateWindowRect();
                if (IsFirstScene)
                {
                    Screen.fullScreen = false;
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    BorderlessWindow.SetWindowRect(windowRect);
                }
            }

            //Target Display
            if (IsFirstScene && displayMode == DisplayMode.TargetDisplay)
            {
                for (var i = 1; i < surfaces.Count; i++)
                {
                    if (i < Display.displays.Length)
                    {
                        Debug.LogError("Activate Display : " + i);
                        Display.displays[i].Activate();
                    }
                }
            }
            //Spanning Display
            if (IsFirstScene && displayMode == DisplayMode.SpanningDisplay)
            {
                Screen.SetResolution(Display.displays[0].systemWidth, Display.displays[0].systemWidth, true);
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                //If there is a floor activate second display
                if (surfaces.Count != walls.Count && Display.displays.Length > 1)
                {
                    Display.displays[1].Activate();
                }
            }
            //Virtual Room
            if (displayMode == DisplayMode.VirtualRoom)
            {

#if UNITY_STANDALONE
                Screen.SetResolution(Display.displays[0].systemWidth, Display.displays[0].systemWidth, true);
                Screen.fullScreen = true;
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
#endif
                virtualRoom = Instantiate(virtualRoomPrefab, new Vector3(0, -500, -500), Quaternion.identity).GetComponent<VirtualRoomController>();
                renderTextures = new List<RenderTexture>();
                for (var i = 0; i < surfaces.Count; i++)
                {
                    var size = surfaces[i].rect.size;
                    var renderTexture = new RenderTexture((int)size.x, (int)size.y, 16);
                    renderTextures.Add(renderTexture);

                    switch (surfaces[i].position)
                    {
                        case SurfacePosition.Left:
                            leftTexture = renderTexture;
                            break;
                        case SurfacePosition.Center:
                            centerTexture = renderTexture;
                            break;
                        case SurfacePosition.Right:
                            rightTexture = renderTexture;
                            break;
                        case SurfacePosition.Back:
                            backTexture = renderTexture;
                            break;
                        case SurfacePosition.Floor:
                            floorTexture = renderTexture;
                            break;
                    }
                }
                virtualRoom.LayoutSurfaces(surfaces, renderTextures);
            }
            if (displayMode == DisplayMode.PanningView)
            {
                panningView = Instantiate(panningViewPrefab, new Vector3(0, -500, -500), Quaternion.identity).GetComponent<PanViewController>();

                renderTextures = new List<RenderTexture>();
                for (var i = 0; i < surfaces.Count; i++)
                {
                    var size = surfaces[i].rect.size;
                    var renderTexture = new RenderTexture((int)size.x, (int)size.y, 24);
                    renderTextures.Add(renderTexture);

                    switch (surfaces[i].position)
                    {
                        case SurfacePosition.Left:
                            leftTexture = renderTexture;
                            break;
                        case SurfacePosition.Center:
                            centerTexture = renderTexture;
                            break;
                        case SurfacePosition.Right:
                            rightTexture = renderTexture;
                            break;
                        case SurfacePosition.Back:
                            backTexture = renderTexture;
                            break;
                        case SurfacePosition.Floor:
                            floorTexture = renderTexture;
                            break;
                    }
                }
                panningView.LayoutSurfaces(surfaces, renderTextures);
            }

            //WebGL
            if (IsFirstScene && displayMode == DisplayMode.WebGL)
            {
                Debug.LogError("WEB GL MODE");
                //NOTHIGN????
            }
            IsFirstScene = false;
        }

        public void SwitchToVirtualRoom()
        {
            displayMode = DisplayMode.VirtualRoom;
            Destroy(panningView.gameObject);

            virtualRoom = Instantiate(virtualRoomPrefab, new Vector3(0, -500, -500), Quaternion.identity).GetComponent<VirtualRoomController>();
            virtualRoom.LayoutSurfaces(surfaces, renderTextures);
        }

        public void SwitchToPanningView()
        {
            displayMode = DisplayMode.PanningView;

            Destroy(virtualRoom.gameObject);

            panningView = Instantiate(panningViewPrefab, new Vector3(0, -500, -500), Quaternion.identity).GetComponent<PanViewController>();
            panningView.LayoutSurfaces(surfaces, renderTextures);
        }

        private void CreateCameras()
        {

            cameras = new List<Camera>();
            wallCameras = new List<Camera>();

            // 1. Setup features of main camera
            mainCamera.stereoTargetEye = StereoTargetEyeMask.None;


            for (var i = 0; i < surfaces.Count; i++)
            {
                var surface = surfaces[i];

                if (surface.position == SurfacePosition.Floor)
                {
                    CreateFloorCamera(surface);
                    continue;
                }

                // 2. Get camera for surface. If Surface == Centre, use existing mainCamera
                var cam = (surface.Equals(centerSurface) || walls.Count == 1) ? mainCamera : Instantiate(mainCamera);
                //Remove Audio Listener from all cameras apart from center camera
                if (cam != mainCamera) cam.GetComponent<AudioListener>().enabled = false;

                // 3. Set parent of camera
                cam.transform.parent = mainCamera.transform.parent.transform;
                cam.transform.position = mainCamera.transform.position;
                cam.name = String.Format("{0} - Camera", surface.position.ToString());
                cameras.Add(cam);
                wallCameras.Add(cam);

                cam.transform.localPosition = mainCamera.transform.localPosition;
                cam.transform.localRotation = mainCamera.transform.localRotation;

                // 4. Setup displays

                //Borderless Window
                if (displayMode == DisplayMode.BorderlessWindow)
                {
                    cam.rect = CalculateViewportRect(surface);
                }

                // Target Displays
                if (displayMode == DisplayMode.TargetDisplay)
                {
                    cam.targetDisplay = i;
                    float actualAspectRatio = Display.displays[i].renderingWidth / (float)Display.displays[i].renderingHeight;
                    float height = actualAspectRatio / surface.aspectRatio;
                    cam.rect = new Rect(0, (1f - height) / 2, 1, height);
                }

                // Spanning Display
                if (displayMode == DisplayMode.SpanningDisplay)
                {
                    //Used to "Letterbox" the display when it doesn't match provided resolution/aspect ratio.
                    //If setup correctly will only happen when running in Unity editor
                    //var normaliserX = (resolutionWithoutFloor.width / resolutionWithoutFloor.height) / ((float)Screen.width / (float)Screen.height);
                    var normaliserX = (resolutionWithoutFloor.width / resolutionWithoutFloor.height) / ((float)Display.displays[0].systemWidth / (float)Screen.height);
                    var normaliserY = 1f;
                    if (normaliserX > 1)
                    {
                        normaliserY /= normaliserX;
                        normaliserX = 1;
                    }

                    var x = surface.rect.x / resolutionWithoutFloor.width * normaliserX + (1f - normaliserX) / 2f;
                    var y = (1f - normaliserY) / 2f;
                    var width = surface.rect.width / resolutionWithoutFloor.width * normaliserX;
                    var height = surface.rect.height / resolutionWithoutFloor.height * normaliserY;
                    cam.rect = new Rect(x, y, width, height);
                }

                //Virtual Room
                if (displayMode == DisplayMode.VirtualRoom || displayMode == DisplayMode.PanningView)
                {
                    cam.targetTexture = renderTextures[i];
                }
                if (displayMode == DisplayMode.WebGL)
                {
                    //Used to "Letterbox" the display when it doesn't match provided resolution/aspect ratio.
                    //If setup correctly will only happen when running in Unity editor
                    var normaliserX = (resolutionWithoutFloor.width / resolutionWithoutFloor.height) / ((float)Screen.width / (float)Screen.height);
                    var normaliserY = 1f;
                    if (normaliserX > 1)
                    {
                        normaliserY /= normaliserX;
                        normaliserX = 1;
                    }

                    var x = surface.rect.x / resolutionWithoutFloor.width * normaliserX + (1f - normaliserX) / 2f;
                    var y = (1f - normaliserY) / 2f;
                    var width = surface.rect.width / resolutionWithoutFloor.width * normaliserX;
                    var height = surface.rect.height / resolutionWithoutFloor.height * normaliserY;
                    cam.rect = new Rect(x, y, width, height);

                }
            }
        }

        /// <summary>
        /// Creates a special camera for the floor.
        /// </summary>
        private void CreateFloorCamera(SurfaceInfo floor)
        {
            // 1. Create camera for surface.
            var cam = Instantiate(mainCamera);
            //Remove Audio Listener from all cameras
            cam.GetComponent<AudioListener>().enabled = false;

            // 2. Set parent of camera
            cam.transform.parent = mainCamera.transform.parent.transform;
            cam.transform.position = mainCamera.transform.position;
            cam.name = String.Format("{0} - Camera", floor.position.ToString());
            cameras.Add(cam);


            // 3. Setup Display Mode

            //Borderless Window
            if (displayMode == DisplayMode.BorderlessWindow)
            {
                cam.rect = CalculateViewportRect(floor);
            }
            else if (displayMode == DisplayMode.TargetDisplay)
            {
                cam.targetDisplay = walls.Count;
                Debug.LogError("Floor Target Display: " + cam.targetDisplay);
            }
            else if (displayMode == DisplayMode.SpanningDisplay)
            {
                //For testing in editor.
                if (Application.isEditor)
                {
                    var currentAspectRatio = Screen.width / Screen.height;
                    cam.rect = new Rect(0, 0, floor.aspectRatio / currentAspectRatio, 1);
                }
                else
                {
                    //Fill entire dispay
                    cam.rect = new Rect(0, 0, 1, 1);
                }
                cam.targetDisplay = 1;
            }
            else
            {
                cam.targetTexture = floorTexture;
            }

            floorCamera = cam;
        }

        //-------------------
        // 8. Position Cameras
        //-------------------

        /// <summary>
        /// Places the position of cameras.
        /// </summary>
        protected abstract void PositionCameras();


        //-------------------
        // 9. Set the canvases to render to the correct area and destroy unneeded canvases
        //-------------------
        private void SetupCanvases(List<SurfacePosition> surfacesPositions)
        {

            // Powerwall. Only one surface. Uses center canvas.
            // It creates a copy of the center canvas and replaces the existing one.
            if (walls.Count == 1)
            {

                switch (surfaces[0].position)
                {
                    case SurfacePosition.Left:
                        //Copys the main canvas
                        GameObject newCanvas = Instantiate(centerUICamera.GetComponent<UICamera>().canvas.gameObject);
                        newCanvas.name = "Left Canvas";
                        var oldLeftCanvas = leftUICamera.GetComponent<UICamera>().canvas;
                        newCanvas.transform.SetParent(oldLeftCanvas.transform.parent);
                        leftUICamera.GetComponent<UICamera>().canvas = newCanvas.GetComponent<Canvas>();
                        newCanvas.GetComponent<Canvas>().worldCamera = leftUICamera;

                        //Destroy Old Canvas
                        Destroy(oldLeftCanvas.gameObject);
                        break;

                    case SurfacePosition.Right:
                        //Copys the main canvas
                        newCanvas = Instantiate(centerUICamera.GetComponent<UICamera>().canvas.gameObject);
                        newCanvas.name = "Right Canvas";
                        var oldRightCanvas = rightUICamera.GetComponent<UICamera>().canvas;
                        newCanvas.transform.SetParent(oldRightCanvas.transform.parent);
                        rightUICamera.GetComponent<UICamera>().canvas = newCanvas.GetComponent<Canvas>();
                        newCanvas.GetComponent<Canvas>().worldCamera = rightUICamera;

                        //Destroy Old Canvas
                        Destroy(oldRightCanvas.gameObject);
                        break;
                }
            }

            //Setup canvas display settings
            for (var i = 0; i < surfaces.Count; i++)
            {
                var surface = surfaces[i];

                // Target Displays
                if (displayMode == DisplayMode.TargetDisplay)
                {
                    switch (surface.position)
                    {
                        case SurfacePosition.Left: leftUICamera.targetDisplay = i; break;
                        case SurfacePosition.Center: centerUICamera.targetDisplay = i; break;
                        case SurfacePosition.Right: rightUICamera.targetDisplay = i; break;
                        case SurfacePosition.Back: backUICamera.targetDisplay = i; break;
                        case SurfacePosition.Floor: floorUICamera.targetDisplay = i; break;
                    }
                }

                // Spanning Display
                else if (displayMode == DisplayMode.SpanningDisplay || displayMode == DisplayMode.BorderlessWindow)
                {
                    switch (surface.position)
                    {
                        case SurfacePosition.Left: leftUICamera.rect = cameras[i].rect; break;
                        case SurfacePosition.Center: centerUICamera.rect = cameras[i].rect; break;
                        case SurfacePosition.Right: rightUICamera.rect = cameras[i].rect; break;
                        case SurfacePosition.Back: backUICamera.rect = cameras[i].rect; break;
                        case SurfacePosition.Floor:
                            floorUICamera.rect = cameras[i].rect;
                            floorUICamera.targetDisplay = cameras[i].targetDisplay;
                            break;
                    }
                }

                //Virtual Room
                else if (displayMode == DisplayMode.VirtualRoom || displayMode == DisplayMode.PanningView)
                {
                    switch (surface.position)
                    {
                        case SurfacePosition.Left: leftUICamera.targetTexture = renderTextures[i]; break;
                        case SurfacePosition.Center: centerUICamera.targetTexture = renderTextures[i]; break;
                        case SurfacePosition.Right: rightUICamera.targetTexture = renderTextures[i]; break;
                        case SurfacePosition.Back: backUICamera.targetTexture = renderTextures[i]; break;
                        case SurfacePosition.Floor: floorUICamera.targetTexture = renderTextures[i]; break;
                    }
                }

                //WebGL
                else if (displayMode == DisplayMode.WebGL)
                {
                    switch (surface.position)
                    {
                        case SurfacePosition.Left: leftUICamera.rect = cameras[i].rect; break;
                        case SurfacePosition.Center: centerUICamera.rect = cameras[i].rect; break;
                        case SurfacePosition.Right: rightUICamera.rect = cameras[i].rect; break;
                        case SurfacePosition.Back: backUICamera.rect = cameras[i].rect; break;
                        case SurfacePosition.Floor: floorUICamera.rect = cameras[i].rect; break;
                    }
                }
            }

            //Destroy Unncessary Canvases
            if (!surfacesPositions.Contains(SurfacePosition.Left))
            {
                Destroy(leftUICamera.GetComponent<UICamera>().canvas.gameObject);
                Destroy(leftUICamera.gameObject);
            }
            else uiCameras.Add(leftUICamera);
            if (!surfacesPositions.Contains(SurfacePosition.Center))
            {
                Destroy(centerUICamera.GetComponent<UICamera>().canvas.gameObject);
                Destroy(centerUICamera.gameObject);
            }
            else uiCameras.Add(centerUICamera);
            if (!surfacesPositions.Contains(SurfacePosition.Right))
            {
                Destroy(rightUICamera.GetComponent<UICamera>().canvas.gameObject);
                Destroy(rightUICamera.gameObject);
            }
            else uiCameras.Add(rightUICamera);
            if (!surfacesPositions.Contains(SurfacePosition.Back))
            {
                Destroy(backUICamera.GetComponent<UICamera>().canvas.gameObject);
                Destroy(backUICamera.gameObject);
            }
            else uiCameras.Add(backUICamera);
            if (!surfacesPositions.Contains(SurfacePosition.Floor))
            {
                Destroy(floorUICamera.GetComponent<UICamera>().canvas.gameObject);
                Destroy(floorUICamera.gameObject);
            }
            else uiCameras.Add(floorUICamera);

            //Disable Graphics Raycasters if in virtual room mode.
            //This is necessary to stop touches incorrect input.
            if (displayMode == DisplayMode.VirtualRoom)
            {
                foreach (var uiCam in uiCameras)
                {
                    var canvas = uiCam.GetComponent<UICamera>().canvas;
                    var raycaster = canvas.GetComponent<GraphicRaycaster>();
                    raycaster.enabled = false;
                }
            }
        }

        //-------------------
        // 10. Creates and sets up TCP listeners for interaction sensors.
        //     Creates mesh to handle interactions with objects.
        //-------------------

        private void SetupInteractionsListeners()
        {
            GameObject interactionObjects = new GameObject();
            interactionObjects.name = "Interactions";

            for (int i = 0; i < surfaces.Count; i++)
            {
                CreateInteractionListener(surfaces[i], cameras[i], interactionObjects.transform, i);
            }
        }

        private void CreateInteractionListener(SurfaceInfo surface, Camera cam, Transform parent, int surfaceIndex)
        {
            int port = 0;
            switch (surface.position)
            {
                case SurfacePosition.Left:
                    port = 3030;
                    break;
                case SurfacePosition.Center:
                    port = 3040;
                    break;
                case SurfacePosition.Right:
                    port = 3050;
                    break;
                case SurfacePosition.Back:
                    port = 3060;
                    break;
                case SurfacePosition.Floor:
                    port = 3070;
                    break;
            }

            ProcessTouches pt = Instantiate(interactionPrefab) as ProcessTouches;
            pt.cam = cam;
            pt.port = port;
            pt.immersiveCamera = this;
            pt.surfaceIndex = surfaceIndex;
            pt.isFloor = (surface.position == SurfacePosition.Floor);

            pt.name = String.Format("{0} - Interaction", surface.position.ToString());
            pt.transform.parent = parent;
        }



        //==============================================================
        // UPDATE
        //==============================================================

        // Update is called once per frame
        protected void Update()
        {

            //----------Quit Application----------
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            //----------Input----------
            if (interactionOn && pointTouchesOn)
            {


                if (displayMode == DisplayMode.VirtualRoom || displayMode == DisplayMode.PanningView)
                {
                    //Mouse Released
                    if (Input.GetMouseButtonUp(0))
                    {

                        HandleClickVirtualRoom(Input.mousePosition, TouchPhase.Ended);
                    }
                    //Mouse Pressed
                    else if (Input.GetMouseButtonDown(0))
                    {
                        HandleClickVirtualRoom(Input.mousePosition, TouchPhase.Began);
                    }
                    //Mouse Held
                    else if (Input.GetMouseButton(0))
                    {
                        HandleClickVirtualRoom(Input.mousePosition, TouchPhase.Moved);
                    }

                }
                else
                {
                    //Touch Input
                    for (var i = 0; i < Input.touchCount; i++)
                    {
                        Touch touch = Input.GetTouch(i);
                        var position = Display.RelativeMouseAt(touch.position);
                        HandleTouchOrClick(position, touch.phase, i);
                    }

                    //Mouse released
                    if (Input.GetMouseButtonUp(0))
                    {
                        var position = GetCurrentMousePosition();
                        HandleTouchOrClick(position, TouchPhase.Ended, -1);
                    }
                    //Mouse Pressed
                    else if (Input.GetMouseButtonDown(0))
                    {
                        var position = GetCurrentMousePosition();
                        HandleTouchOrClick(position, TouchPhase.Began, -1);
                    }
                    //Mouse Held
                    else if (Input.GetMouseButton(0))
                    {
                        var position = GetCurrentMousePosition();
                        HandleTouchOrClick(position, TouchPhase.Moved, -1);
                    }
                }
            }
        }

        private Vector3 GetCurrentMousePosition()
        {
            var position = Input.mousePosition;
#if !UNITY_EDITOR && !UNITY_WEBGL
            if (displayMode != DisplayMode.BorderlessWindow)
                position = Display.RelativeMouseAt(Input.mousePosition);
#endif
            return position;
        }

        private void LateUpdate()
        {
            //Find objects which the user the user is no longer touching.
            foreach (var io in objectsTouchedLastFrame)
            {
                io.OnTouchExit();
            }

            objectsTouchedLastFrame = objectsTouchedCurrentFrame;
            objectsTouchedCurrentFrame = new List<IInteractableObject>();


            //Find UI Object which the user the user is no longer touching.
            PointerEventData ped = new PointerEventData(EventSystem.current);
            foreach (var exitHandler in pointerExitHandlersTouchedLastFrame)
            {
                if (exitHandler != null && !exitHandler.Equals(null))
                    exitHandler.OnPointerExit(ped);
            }
            pointerExitHandlersTouchedLastFrame = pointerExitHandlersTouchedCurrentFrame;
            pointerExitHandlersTouchedCurrentFrame = new List<IPointerExitHandler>();

            pointerEnterHandlersTouchedLastFrame = pointerEnterHandlersTouchedCurrentFrame;
            pointerEnterHandlersTouchedCurrentFrame = new List<IPointerEnterHandler>();
        }

        /// <summary>
        /// arg1: Vector2     - Touch position.
        /// arg2: CameraIndex - Camera index associated with the touch.
        /// arg3: TouchPhase  - The phase of touch.
        /// arg4: int         - The index of the touch. -1 if its a click.
        /// private void OnWallTouched(Vector2 touchPosition, int cameraIndex, TouchPhase touchPhase, int touchIndex)
        /// </summary>
        public class WallTouchEvent : UnityEvent<Vector2, int, TouchPhase, int>
        {
            internal void AddListener()
            {
                throw new NotImplementedException();
            }

            internal void Invoke(SurfaceTouchedEventArgs surfaceTouchedEventArgs)
            {
                Invoke(surfaceTouchedEventArgs.ScreenPoint, surfaceTouchedEventArgs.RenderingCameraIndex, surfaceTouchedEventArgs.Phase, surfaceTouchedEventArgs.TouchIndex);
            }
        }

        [Obsolete("Please use AnySurfaceTouchedEvent instead")]
        public static WallTouchEvent AnyWallTouched = new WallTouchEvent();
        [Obsolete("Please use LeftSurfaceTouchedEvent instead")]
        public static WallTouchEvent LeftScreenTouched = new WallTouchEvent();
        [Obsolete("Please use CentreSurfaceTouchedEvent instead")]
        public static WallTouchEvent CenterScreenTouched = new WallTouchEvent();
        [Obsolete("Please use RightSurfaceTouchedEvent instead")]
        public static WallTouchEvent RightScreenTouched = new WallTouchEvent();
        [Obsolete("Please use BackSurfaceTouchedEvent instead")]
        public static WallTouchEvent BackScreenTouched = new WallTouchEvent();
        [Obsolete("Please use FloorSurfaceTouchedEvent instead")]
        public static WallTouchEvent FloorTouched = new WallTouchEvent();

        public static SurfaceTouchedEvent AnySurfaceTouchedEvent = new SurfaceTouchedEvent();
        public static SurfaceTouchedEvent LeftSurfaceTouchedEvent = new SurfaceTouchedEvent();
        public static SurfaceTouchedEvent CentreSurfaceTouchedEvent = new SurfaceTouchedEvent();
        public static SurfaceTouchedEvent RightSurfaceTouchedEvent = new SurfaceTouchedEvent();
        public static SurfaceTouchedEvent BackSurfaceTouchedEvent = new SurfaceTouchedEvent();
        public static SurfaceTouchedEvent FloorSurfaceTouchedEvent = new SurfaceTouchedEvent();

        private void SendSurfaceTouchedEvents(SurfaceTouchedEventArgs surfaceTouchedEventArgs)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            AnyWallTouched.Invoke(surfaceTouchedEventArgs);
            switch (surfaceTouchedEventArgs.TouchedSurfacePosition)
            {
                case SurfacePosition.Left: LeftScreenTouched.Invoke(surfaceTouchedEventArgs); break;
                case SurfacePosition.Center: CenterScreenTouched.Invoke(surfaceTouchedEventArgs); break;
                case SurfacePosition.Right: RightScreenTouched.Invoke(surfaceTouchedEventArgs); break;
                case SurfacePosition.Back: BackScreenTouched.Invoke(surfaceTouchedEventArgs); break;
                case SurfacePosition.Floor: FloorTouched.Invoke(surfaceTouchedEventArgs); break;
            }
#pragma warning restore CS0618 // Type or member is obsolete

            AnySurfaceTouchedEvent.Invoke(surfaceTouchedEventArgs);
            switch (surfaceTouchedEventArgs.TouchedSurfacePosition)
            {
                case SurfacePosition.Left: LeftSurfaceTouchedEvent.Invoke(surfaceTouchedEventArgs); break;
                case SurfacePosition.Center: CentreSurfaceTouchedEvent.Invoke(surfaceTouchedEventArgs); break;
                case SurfacePosition.Right: RightSurfaceTouchedEvent.Invoke(surfaceTouchedEventArgs); break;
                case SurfacePosition.Back: BackSurfaceTouchedEvent.Invoke(surfaceTouchedEventArgs); break;
                case SurfacePosition.Floor: FloorSurfaceTouchedEvent.Invoke(surfaceTouchedEventArgs); break;
            }
        }

        public static WallTouchEvent PlaceHotspotsWallTouchEvent = new WallTouchEvent();

        public bool IsPlaceHotspotMode
        {
            get
            {
#if UNITY_EDITOR
                if (EditorPrefs.HasKey("PlaceHotspotMode"))
                {
                    return EditorPrefs.GetBool("PlaceHotspotMode");
                }
#endif
                return false;
            }
        }

        /// <summary>
        /// Handles a user's clicks when in virtual room mode.
        /// </summary>
        /// <param name="position">A Vector3 representing the position of the click.</param>
        /// <param name="phase">The phase of the touched. Eg. Began, Ended or Moved.</param>
        private void HandleClickVirtualRoom(Vector3 position, TouchPhase phase)
        {
            // 1. Cast Ray from virtual room camera to screen
            Ray ray;
            if (displayMode == DisplayMode.VirtualRoom)
            {
                ray = virtualRoom.cam.ScreenPointToRay(position);
            }
            else
            {
                ray = panningView.cam.ScreenPointToRay(position);
            }
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Transform objectHit = hit.transform;
                var virtualScreen = objectHit.GetComponent<VirtualRoomScreen>();

                // 2. If ray hits a screen, find the associated camera and the position on the screen.
                if (virtualScreen != null)
                {
                    // 3. Get the render texture of screen.
                    RenderTexture tex = null;
                    switch (virtualScreen.surface)
                    {
                        case SurfacePosition.Left: tex = leftTexture; break;
                        case SurfacePosition.Center: tex = centerTexture; break;
                        case SurfacePosition.Right: tex = rightTexture; break;
                        case SurfacePosition.Back: tex = backTexture; break;
                        case SurfacePosition.Floor: tex = floorTexture; break;
                    }

                    // 4. Get position of touch in local space of the screen.
                    //Values between -0.5 and 0.5 in each dimension.
                    var localPoint = objectHit.InverseTransformPoint(hit.point);
                    position = new Vector2((localPoint.x + 0.5f) * tex.width, (localPoint.y + 0.5f) * tex.height);

                    // 5. Get Camera associated with screen
                    int cameraIndex = GetIndexOfSurface(virtualScreen.surface);

                    //Remove try catch when bug caught.
                    Camera cam = null;
                    try
                    {
                        cam = cameras[cameraIndex];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Debug.LogError("======= VIRTUAL ROOM ERROR - Camera Index = " + cameraIndex + ", cameras length = " + cameras.Count + ", virtualScreen Surface: " + virtualScreen.surface);
                    }

                    // 6. Invoke Wall Touched Events
                    if (IsPlaceHotspotMode)
                    {
                        PlaceHotspotsWallTouchEvent.Invoke(position, cameraIndex, phase, -1);
                        return;
                    }

                    // 7. Pass clicks onto UI
                    var hitUIObject = RaycastOnClickEventsToUI(cam, position, phase);

                    if (worldInteractionOn)
                    {
                        // 8. Raycast from associated camera and trigger interactive object
                        ImmersiveRaycastHit raycastHit;
                        if (!hitUIObject)
                            raycastHit = RaycastToInteractiveObject(cam, new Vector3(position.x, position.y), phase);
                        else
                            raycastHit = new ImmersiveRaycastHit();

                        SurfaceTouchedEventArgs args = new SurfaceTouchedEventArgs(position, cameraIndex, cam, phase, -1, virtualScreen.surface, raycastHit, this);
                        SendSurfaceTouchedEvents(args);
                    }

                    // 9. Show touch icon
                    if (displayTouchPoints && (phase == TouchPhase.Began || phase == TouchPhase.Moved || phase == TouchPhase.Stationary))
                    {
                        DisplayTouchIcon(position, cam);
                    }
                }
            }
        }

        /// <summary>
        /// Handles a users touch or click.
        /// </summary>
        /// <param name="position">A Vector3 representing the position of the click.</param>
        /// <param name="phase"></param>
        public void HandleTouchOrClick(Vector3 position, TouchPhase phase, int touchIndex, bool didHitUIObject = false)
        {
            // 1. Calculates the position of the touch relative the display which was touched
            //if (!Application.isEditor)
            //{
            //    position = Display.RelativeMouseAt(position);
            //}


            // 2. Calculate the index of the camera corresponding to touch area
            var cameraIndex = (int)position.z;

            //The position in a surface space as defined in arguments.
            var positionInSurfacePixelSpace = position;

            //In spanning mode which section of screen touched.
            if (displayMode == DisplayMode.SpanningDisplay && cameraIndex == 0)
            {
                var xNormalised = position.x / (float)Display.displays[0].renderingWidth;
                for (var i = 0; i < walls.Count; i++)
                {
                    var surface = walls[i];
                    //If touch is in screen zone corresponding to surface
                    if (xNormalised <= (surface.rect.x + surface.rect.width) / resolutionWithoutFloor.width)
                    {
                        cameraIndex = i;
                        break;
                    }
                    else positionInSurfacePixelSpace.x -= cameras[i].pixelWidth;
                }

                //Convert position so that it is relative to camera
                var yOffset = (Display.displays[0].renderingHeight - cameras[cameraIndex].pixelHeight) / 2;
                positionInSurfacePixelSpace.y -= yOffset;
            }

            if (displayMode == DisplayMode.BorderlessWindow)
            {
                var xNormalised = position.x / (float)Display.displays[0].renderingWidth;
                for (var i = 0; i < surfaces.Count; i++)
                {
                    var surface = surfaces[i];
                    //If touch is in screen zone corresponding to surface
                    if (xNormalised <= (surface.rect.x + surface.rect.width) / resolution.width)
                    {
                        cameraIndex = i;
                        break;
                    }
                    else positionInSurfacePixelSpace.x -= cameras[i].pixelWidth;
                }
            }

            var cam = cameras[cameraIndex];

            if (worldInteractionOn)
            {
                ImmersiveRaycastHit raycastHit = CreateImmersiveRaycastHit(cam, position, phase);

                if (!didHitUIObject)
                    PassEventsToInteractableObjects(raycastHit, phase);

                // 4. Invoke Surface touched event
                SurfaceTouchedEventArgs surfaceTouchedEventArgs = new SurfaceTouchedEventArgs(position, cameraIndex, cam, phase, touchIndex, surfaces[cameraIndex].position, raycastHit, this);
                SendSurfaceTouchedEvents(surfaceTouchedEventArgs);
            }


            // 5. Show touch icon
            if (displayTouchPoints && (phase == TouchPhase.Began || phase == TouchPhase.Moved || phase == TouchPhase.Stationary))
            {
                var scale = 1080f / cam.pixelHeight;
                DisplayTouchIcon(positionInSurfacePixelSpace * scale, cam);
            }
        }

        //Draws an icon on the screen where the user touches
        private void DisplayTouchIcon(Vector2 position, Camera cam)
        {
            //Get Canvas
            Camera uiCam = null;
            switch (cam.name)
            {
                case "Left - Camera": uiCam = leftUICamera; break;
                case "Center - Camera": uiCam = centerUICamera; break;
                case "Right - Camera": uiCam = rightUICamera; break;
                case "Back - Camera": uiCam = backUICamera; break;
                case "Floor - Camera": uiCam = floorUICamera; break;
            }
            if (uiCam == null) return;
            Canvas canvas = uiCam.GetComponent<UICamera>().canvas;

            //Display touch point
            var touch = Instantiate(canvasTouchPrefab, canvas.transform);
            touch.GetComponent<RectTransform>().anchoredPosition = position;

        }


        private List<IInteractableObject> objectsTouchedLastFrame = new List<IInteractableObject>();
        private List<IInteractableObject> objectsTouchedCurrentFrame = new List<IInteractableObject>();

        private ImmersiveRaycastHit CreateImmersiveRaycastHit(Camera cam, Vector3 position, TouchPhase phase)
        {
            //Sprites only work with 2D raycasting therefore must work with both 2D and 3D
            RaycastHit2D hit2D = Physics2D.GetRayIntersection(cam.ScreenPointToRay(new Vector2(position.x, position.y)));
            Ray ray = cam.ScreenPointToRay(position);

            bool hit3DSuccess = Physics.Raycast(ray, out RaycastHit hit3D);

            ImmersiveRaycastHit immersiveRaycast;

            if (hit3DSuccess && hit2D)
            {
                if (hit3D.distance < hit2D.distance)
                    immersiveRaycast = new ImmersiveRaycastHit(hit3D);
                else
                    immersiveRaycast = new ImmersiveRaycastHit(hit2D);
            }
            else if (hit3DSuccess)
                immersiveRaycast = new ImmersiveRaycastHit(hit3D);
            else if (hit2D)
                immersiveRaycast = new ImmersiveRaycastHit(hit2D);
            else
                immersiveRaycast = new ImmersiveRaycastHit();
            return immersiveRaycast;
        }

        private void PassEventsToInteractableObjects(ImmersiveRaycastHit immersiveRaycast, TouchPhase phase)
        {
            if (immersiveRaycast.HitTransform != null)
            {
                IInteractableObject[] ios = immersiveRaycast.HitTransform.GetComponents<IInteractableObject>();
                foreach (var io in ios)
                {
                    if (io != null && ((MonoBehaviour)io).isActiveAndEnabled)
                    {
                        switch (phase)
                        {
                            case TouchPhase.Began:
                                io.OnPress(); break;
                            case TouchPhase.Ended:
                                io.OnRelease();
                                break;
                            case TouchPhase.Moved:
                            case TouchPhase.Stationary:
                                io.OnTouchEnter(); break;
                        }

                        //Used to find objects that are no longer being touched.
                        objectsTouchedCurrentFrame.Add(io);
                        objectsTouchedLastFrame.Remove(io);
                    }
                }
            }
        }

        private ImmersiveRaycastHit RaycastToInteractiveObject(Camera cam, Vector3 position, TouchPhase phase)
        {
            ImmersiveRaycastHit immersiveRaycast = CreateImmersiveRaycastHit(cam, position, phase);
            PassEventsToInteractableObjects(immersiveRaycast, phase);
            return immersiveRaycast;
        }

        private List<IPointerEnterHandler> pointerEnterHandlersTouchedLastFrame = new List<IPointerEnterHandler>();
        private List<IPointerEnterHandler> pointerEnterHandlersTouchedCurrentFrame = new List<IPointerEnterHandler>();

        private List<IPointerExitHandler> pointerExitHandlersTouchedLastFrame = new List<IPointerExitHandler>();
        private List<IPointerExitHandler> pointerExitHandlersTouchedCurrentFrame = new List<IPointerExitHandler>();


        // This will pass raycasts to UI through to canvases
        public bool RaycastOnClickEventsToUI(Camera cam, Vector3 position, TouchPhase phase)
        {
            if (!canvasInteractionOn) return false;

            // 1. Get correct UI camera
            Camera uiCam = null;
            switch (cam.name)
            {
                case "Left - Camera": uiCam = leftUICamera; break;
                case "Center - Camera": uiCam = centerUICamera; break;
                case "Right - Camera": uiCam = rightUICamera; break;
                case "Back - Camera": uiCam = backUICamera; break;
                case "Floor - Camera": uiCam = floorUICamera; break;
            }

            if (uiCam == null) return false;
            // When in virtual screen mode the position only has to be the pixel position for that screen.
            // When not in virtual screen mode you have to add the width of the cameras to the left.
            // Eg. There are a left and center surface each 1920x1080. The center of left surface is 960x1080 in all modes.
            //     However the center of the center surface is 960x1080 in virtual room mode and 2880x1080 in other modes.
            var offset = 0;
            foreach (var c in cameras)
            {
                if (c == cam) break;
                else offset += c.pixelWidth;
            }

            // 2. Get the canvas assossiated with that camera.
            Canvas canvas = uiCam.GetComponent<UICamera>().canvas;

            // 3. Raycast
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            PointerEventData ped = new PointerEventData(EventSystem.current);

            if (displayMode == DisplayMode.VirtualRoom ||
                displayMode == DisplayMode.PanningView ||
                displayMode == DisplayMode.BorderlessWindow)
                ped.position = position;
            else
                ped.position = new Vector2(position.x + offset, position.y);


            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(ped, results);

            bool didHitObject = false;
            foreach (RaycastResult result in results)
            {
                switch (phase)
                {
                    case TouchPhase.Began:
                        //Pointer Down
                        IPointerDownHandler pointerDownHandler = result.gameObject.GetComponent<IPointerDownHandler>();
                        if (pointerDownHandler != null)
                            pointerDownHandler.OnPointerDown(ped);

                        //Begin Drag
                        IBeginDragHandler bdh = result.gameObject.GetComponent<IBeginDragHandler>();
                        if (bdh != null)
                            bdh.OnBeginDrag(ped);

                        break;

                    case TouchPhase.Moved:
                        //Drag
                        IDragHandler dh = result.gameObject.GetComponent<IDragHandler>();
                        if (dh != null)
                            dh.OnDrag(ped);
                        break;

                    case TouchPhase.Ended:
                        //Pointer Up
                        IPointerUpHandler pointerUpHandler = result.gameObject.GetComponent<IPointerUpHandler>();
                        if (pointerUpHandler != null)
                            pointerUpHandler.OnPointerUp(ped);

                        //Pointer Click
                        IPointerClickHandler pointerClickHandler = result.gameObject.GetComponent<IPointerClickHandler>();
                        if (pointerClickHandler != null)
                            pointerClickHandler.OnPointerClick(ped);

                        //Drag Ended
                        IEndDragHandler edh = result.gameObject.GetComponent<IEndDragHandler>();
                        if (edh != null)
                            edh.OnEndDrag(ped);
                        break;
                }


                //Pointer Entered
                IPointerEnterHandler pointerEnterHandler = result.gameObject.GetComponent<IPointerEnterHandler>();
                if (pointerEnterHandler != null)
                {
                    if (!pointerEnterHandlersTouchedLastFrame.Contains(pointerEnterHandler))
                        pointerEnterHandler.OnPointerEnter(ped);

                    pointerEnterHandlersTouchedCurrentFrame.Add(pointerEnterHandler);
                }

                //Pointer Exit
                IPointerExitHandler pointerExitHandler = result.gameObject.GetComponent<IPointerExitHandler>();
                if (pointerEnterHandler != null)
                {
                    pointerExitHandlersTouchedCurrentFrame.Add(pointerExitHandler);
                    pointerExitHandlersTouchedLastFrame.Remove(pointerExitHandler);
                }

                didHitObject = true;
            }
            return didHitObject;
        }


        //==============================================================
        // WEATHER EFFECTS
        //==============================================================

        //Game object which holds all the weather effects.
        private GameObject weatherEffectHolder;

        public void TurnOnWeatherEffect()
        {
            weatherEffectHolder.SetActive(true);
        }

        public void TurnOffWeatherEffect()
        {
            weatherEffectHolder.SetActive(false);
        }

        /// <summary>
        /// Instantiates provided weather effect and adds it to each camera.
        /// </summary>
        /// <param name="effectPrefab">A weather effect prefab.</param>
        public void SetupWeatherEffects(GameObject effectPrefab)
        {
            weatherEffectHolder = new GameObject("Rain Holder");
            weatherEffectHolder.transform.parent = transform;

            for (int i = 0; i < walls.Count; i++)
            {
                var cam = cameras[i];
                var effect = Instantiate(effectPrefab);
                effect.transform.parent = weatherEffectHolder.transform;
                var effectCam = effect.GetComponent<Camera>();

                if (displayMode == DisplayMode.VirtualRoom || displayMode == DisplayMode.PanningView)
                {
                    effectCam.targetTexture = cam.targetTexture;
                }
                else
                {
                    effectCam.targetDisplay = cam.targetDisplay;
                    effectCam.rect = cam.rect;
                }
            }
            weatherEffectHolder.SetActive(false);
        }

        //==============================================================
        // ROTATE BUTTONS
        //==============================================================

        /// <summary>
        /// Should rotate the camera one center surface width to the left.
        /// </summary>
        public abstract void RotateCameraLeft();
        /// <summary>
        /// Should rotate the camera one center surface width to the left.
        /// </summary>
        public abstract void RotateCameraRight();

        //==============================================================
        // HELPER METHODS
        //==============================================================

        /// <summary>
        /// Determines whether a renderer is visible on the walls.
        /// </summary>
        public bool IsVisibleOnWalls(Renderer renderer)
        {
            foreach (Camera cam in wallCameras)
            {
                if (IsRendererVisibleFromCamera(renderer, cam))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the provided renderer is visible on the floor.
        /// </summary>
        public bool IsVisibleOnFloor(Renderer renderer)
        {
            return floorCamera != null && IsRendererVisibleFromCamera(renderer, floorCamera);
        }

        private static bool IsRendererVisibleFromCamera(Renderer renderer, Camera camera)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }


        /// <summary>
        /// Finds the camera within whose frustrum an a given object is located.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns>The located camera if one is found. Null if not.</returns>
        public Camera FindCameraLookingAtPosition(Vector3 position)
        {
            foreach (Camera cam in cameras)
            {
                Vector3 screenPoint = cam.WorldToViewportPoint(position);
                bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
                if (onScreen) return cam;
            }

            return null;
        }

        /// <summary>
        /// Find the camera a position in the total spanning image (in pixels) is located in.
        /// Also provides the position (in pixels) of the point on the calculated surface.
        /// </summary>
        /// <param name="spanPos">A position (in pixels) relative to the leftmost surface.</param>
        /// <returns>The camera that renders the position (or null) and the position in that cameras space.</returns>
        public (Camera, Vector2) FindCameraFromScreenPosition(Vector2 spanPos)
        {
            var xOffset = 0;
            foreach (Camera cam in cameras)
            {
                var camX = xOffset + cam.pixelWidth;
                if (spanPos.x > xOffset && spanPos.x < camX && spanPos.y > 0 && spanPos.y < cam.pixelHeight)
                {
                    return (cam, new Vector2(spanPos.x - xOffset, spanPos.y));
                }
                xOffset = camX;
            }
            return (null, Vector2.zero);
        }

        public (Camera, Canvas) FindRenderingCameraAndCanvas(GameObject target)
        {
            foreach (Camera cam in cameras)
            {
                Vector3 screenPoint = cam.WorldToViewportPoint(target.transform.position);
                bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
                if (onScreen)
                {
                    switch (cam.name)
                    {
                        case "Left - Camera": return (cam, leftUICamera.GetComponent<UICamera>().canvas);
                        case "Center - Camera": return (cam, centerUICamera.GetComponent<UICamera>().canvas);
                        case "Right - Camera": return (cam, rightUICamera.GetComponent<UICamera>().canvas);
                        case "Back - Camera": return (cam, backUICamera.GetComponent<UICamera>().canvas);
                        case "Floor - Camera": return (cam, floorUICamera.GetComponent<UICamera>().canvas);
                    }


                    return (cam, null);
                }
            }
            return (null, null);
        }

        /// <summary>
        /// Get the index of a in the list "surfaces" from a given position
        /// </summary>
        /// <param name="position">The SurfacePositon of the surface.</param>
        /// <returns>Index of that surface</returns>
        public int GetIndexOfSurface(SurfacePosition position)
        {
            for (int i = 0; i < surfaces.Count; i++)
            {
                if (surfaces[i].position == position) return i;
            }
            Debug.LogError("Cannot Find Index of Surface: " + position + ", " + surfaces.Count);

            return -1;
        }

        /// <summary>
        /// Get the surface position from a camera index.
        /// </summary>
        public SurfacePosition GetSurfacePositionFromIndex(int index)
        {
            if (index > surfaces.Count || index < 0) return SurfacePosition.None;
            return surfaces[index].position;
        }

        /// <summary>
        /// Gets the index of a camera.
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public int GetIndexOfCamera(Camera camera)
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                if (camera == cameras[i])
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Play an audio clip at the position of the camera.
        /// </summary>
        /// <param name="clip"></param>
        public static void PlayAudio(AudioClip clip, float volume = 1)
        {
            var audioSource = CurrentImmersiveCamera.audioSource;
            if (audioSource != null)
            {
                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.Play();
            }
        }

        /// <summary>
        /// Stops any audio playing from the camera system.
        /// </summary>
        public static void StopAudio()
        {
            var audioSource = CurrentImmersiveCamera.audioSource;
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }

        private Canvas GetCanvasOfUICamera(Camera uiCamera)
        {
            if (uiCamera == null)
                return null;
            return uiCamera.GetComponent<UICamera>().canvas;
        }
        public Canvas LeftCanvas => GetCanvasOfUICamera(leftUICamera);
        public Canvas CentreCanvas => GetCanvasOfUICamera(centerUICamera);
        public Canvas RightCanvas => GetCanvasOfUICamera(rightUICamera);
        public Canvas BackCanvas => GetCanvasOfUICamera(backUICamera);
        public Canvas FloorCanvas => GetCanvasOfUICamera(floorUICamera);

        public List<Canvas> GetAllCanvases()
        {
            List<Canvas> canvases = new List<Canvas>();
            if (LeftCanvas != null)
                canvases.Add(LeftCanvas);
            if (CentreCanvas != null)
                canvases.Add(CentreCanvas);
            if (RightCanvas != null)
                canvases.Add(RightCanvas);
            if (BackCanvas != null)
                canvases.Add(BackCanvas);
            if (FloorCanvas != null)
                canvases.Add(FloorCanvas);
            return canvases;
        }

        //----------Add Stage-----------
#if UNITY_EDITOR
        public void AddStage()
        {
            var instantiatedStage = PrefabUtility.InstantiatePrefab(stagePrefab) as GameObject;
            print(instantiatedStage);

            //stage = Instantiate(stagePrefab).transform;
            stage = instantiatedStage.transform;
            stage.SetSiblingIndex(transform.GetSiblingIndex());
            stage.name = "Stage (Place All GameObjects Here)";
        }
#endif

        //==============================================================
        // DATA STRUCTURES
        //==============================================================

        /// <summary>
        /// Target Display is used when there is a "Virtual Display" available for each actual display.
        /// Spanning Display is used when the side walls are all combined into a single virtual display.
        /// Floor will always be a seperate display
        /// </summary>
        public enum DisplayMode
        {
            TargetDisplay,
            SpanningDisplay,
            VirtualRoom,
            PanningView,
            WebGL,
            BorderlessWindow
        }


        public enum ScreenSizes
        {
            Standard,           //All Screens are 16x9
            WideFront,          //Front screen is double width eg. 32x9
            Wide,               //All screens are double width eg. 32x9
            Standard4x3,        //All screens are 4x3
            Standard16x10,      //All screens are 16x10
            WideFront16x10,
            Wide16x10           //All screens are 16x10
        }
    }

#if UNITY_EDITOR

    //==============================================================
    // CUSTOM EDITOR
    //==============================================================

    [CustomEditor(typeof(AbstractImmersiveCamera))]
    public class AbstractImmersiveCameraEditor : Editor
    {
        private AbstractImmersiveCamera immersiveCamera;

        private int currentTab = 0;

        private EditorDisplayMode editorDisplayMode = EditorDisplayMode.VirtualRoom;
        private SerializedProperty buildDisplayMode;

        private AbstractImmersiveCamera.ScreenSizes screenSize = AbstractImmersiveCamera.ScreenSizes.Standard;

        private bool leftWallActive = true;
        private bool centerWallActive = true;
        private bool rightWallActive = true;
        private bool backWallActive = false;
        private bool floorActive = false;

        //INTERACTION SETTINGS
        private SerializedProperty interactionOn;
        private SerializedProperty pointTouchesOn;
        private SerializedProperty displayTouchPoints;
        private SerializedProperty areaTouchesOn;
        private SerializedProperty displayAreaTouches;

        private SerializedProperty customFloorInteractionSettings;
        private SerializedProperty floorInteractionOn;
        private SerializedProperty floorPointTouchesOn;
        private SerializedProperty floorDisplayTouchPoints;
        private SerializedProperty floorAreaTouchesOn;
        private SerializedProperty floorDisplayAreaTouches;

        // REFERENCE OBJECTS

        //Cameras
        private SerializedProperty mainCamera;
        private SerializedProperty leftUICamera;
        private SerializedProperty centerUICamera;
        private SerializedProperty rightUICamera;
        private SerializedProperty backUICamera;
        private SerializedProperty floorUICamera;

        //Interaction Prefabs
        private SerializedProperty interactionPrefab;
        private SerializedProperty canvasTouchPrefab;

        //Virtual Room
        private SerializedProperty virtualRoomPrefab;
        private SerializedProperty panViewPrefab;

        //Create Prefabs
        private SerializedProperty stagePrefab;

        private void OnEnable()
        {
            immersiveCamera = (AbstractImmersiveCamera)target;

            AbstractImmersiveCamera.CurrentImmersiveCamera = immersiveCamera;

            if (EditorPrefs.HasKey("ImmersiveCameraScreenSize")) screenSize = (AbstractImmersiveCamera.ScreenSizes)EditorPrefs.GetInt("ImmersiveCameraScreenSize");

            //VIRTUAL ROOM MODE
            if (EditorPrefs.HasKey("EditorDisplayMode")) editorDisplayMode = (EditorDisplayMode)EditorPrefs.GetInt("EditorDisplayMode");
            buildDisplayMode = serializedObject.FindProperty(nameof(immersiveCamera.buildDisplayMode));

            //LAYOUT
            var layoutInt = (int)SurfacePosition.WallsAndFloor;
            if (EditorPrefs.HasKey("ImmersiveCameraLayout")) layoutInt = EditorPrefs.GetInt("ImmersiveCameraLayout");
            var layout = (SurfacePosition)layoutInt;
            leftWallActive = ((layout & SurfacePosition.Left) == SurfacePosition.Left);
            centerWallActive = ((layout & SurfacePosition.Center) == SurfacePosition.Center);
            rightWallActive = ((layout & SurfacePosition.Right) == SurfacePosition.Right);
            backWallActive = ((layout & SurfacePosition.Back) == SurfacePosition.Back);
            floorActive = ((layout & SurfacePosition.Floor) == SurfacePosition.Floor);

            //INTERACTION SETTINGS
            interactionOn = serializedObject.FindProperty("interactionOn");
            pointTouchesOn = serializedObject.FindProperty("pointTouchesOn");
            displayTouchPoints = serializedObject.FindProperty("displayTouchPoints");
            areaTouchesOn = serializedObject.FindProperty("areaTouchesOn");
            displayAreaTouches = serializedObject.FindProperty("displayAreaTouches");

            customFloorInteractionSettings = serializedObject.FindProperty("customFloorInteractionSettings");
            floorInteractionOn = serializedObject.FindProperty("floorInteractionOn");
            floorPointTouchesOn = serializedObject.FindProperty("floorPointTouchesOn");
            floorDisplayTouchPoints = serializedObject.FindProperty("floorDisplayTouchPoints");
            floorAreaTouchesOn = serializedObject.FindProperty("floorAreaTouchesOn");
            floorDisplayAreaTouches = serializedObject.FindProperty("floorDisplayAreaTouches");

            //Cameras
            mainCamera = serializedObject.FindProperty("mainCamera");
            leftUICamera = serializedObject.FindProperty("leftUICamera");
            centerUICamera = serializedObject.FindProperty("centerUICamera");
            rightUICamera = serializedObject.FindProperty("rightUICamera");
            backUICamera = serializedObject.FindProperty("backUICamera");
            floorUICamera = serializedObject.FindProperty("floorUICamera");

            //Interaction Prefabs
            interactionPrefab = serializedObject.FindProperty("interactionPrefab");
            canvasTouchPrefab = serializedObject.FindProperty("canvasTouchPrefab");

            //Virtual Room
            virtualRoomPrefab = serializedObject.FindProperty("virtualRoomPrefab");
            panViewPrefab = serializedObject.FindProperty(nameof(immersiveCamera.panningViewPrefab));

            //Create Prefabs
            stagePrefab = serializedObject.FindProperty("stagePrefab");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            currentTab = GUILayout.Toolbar(currentTab, new string[] { "Settings", "Referenced Objects" });

            switch (currentTab)
            {
                case 0:
                    OnInspectorGUIEditorOnlySettings();
                    break;
                case 1:
                    OnInspectorGUIReferencedObjects();
                    break;
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("---------------------------------", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            OnInspectorGUICameraSpecificSettings();

            DrawJumpToCanvasButtons();

            //Add Stage if its not present
            if (immersiveCamera.stage == null && immersiveCamera.gameObject.scene.IsValid())
            {
                if (!EditorSceneManager.IsPreviewSceneObject(target))
                    immersiveCamera.AddStage();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawJumpToCanvasButtons()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Show Canvas", new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter });
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Left"))
                Selection.activeObject = immersiveCamera.LeftCanvas;
            if (GUILayout.Button("Centre"))
                Selection.activeObject = immersiveCamera.CentreCanvas;
            if (GUILayout.Button("Right"))
                Selection.activeObject = immersiveCamera.RightCanvas;
            if (GUILayout.Button("Back"))
                Selection.activeObject = immersiveCamera.BackCanvas;
            if (GUILayout.Button("Floor"))
                Selection.activeObject = immersiveCamera.FloorCanvas;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Override this method to provide a Custom UI for settings specific to the custom camera type.
        /// </summary>
        protected virtual void OnInspectorGUICameraSpecificSettings() { }


        protected virtual void OnInspectorGUIReferencedObjects()
        {
            //CAMERAS
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Cameras", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(mainCamera);
            EditorGUILayout.PropertyField(leftUICamera);
            EditorGUILayout.PropertyField(centerUICamera);
            EditorGUILayout.PropertyField(rightUICamera);
            EditorGUILayout.PropertyField(backUICamera);
            EditorGUILayout.PropertyField(floorUICamera);

            //INTERACTION
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Interaction Prefabs", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(interactionPrefab);
            EditorGUILayout.PropertyField(canvasTouchPrefab);

            //VIRTUAL ROOM
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Virtual Room", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(virtualRoomPrefab);
            EditorGUILayout.PropertyField(panViewPrefab);

            //STAGE
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Stage", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(stagePrefab);
        }

        protected void OnInspectorGUIEditorOnlySettings()
        {
            serializedObject.Update();


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Only Settings", EditorStyles.boldLabel);
            if (EditorApplication.isPlaying)
            {
                //Switch to Panning Mode
                if (immersiveCamera.displayMode == DisplayMode.VirtualRoom)
                {
                    bool switchMode;
                    switchMode = GUILayout.Button("Switch to Panning View");
                    if (switchMode) immersiveCamera.SwitchToPanningView();
                }
                //Switch to Virtual Room
                if (immersiveCamera.displayMode == DisplayMode.PanningView)
                {
                    bool switchMode;
                    switchMode = GUILayout.Button("Switch to Virtual Room");
                    if (switchMode) immersiveCamera.SwitchToVirtualRoom();
                }
            }
            else
            {
                editorDisplayMode = (EditorDisplayMode)EditorGUILayout.EnumPopup("Editor Display Mode", editorDisplayMode);
                EditorPrefs.SetInt("EditorDisplayMode", (int)editorDisplayMode);
                EditorGUILayout.PropertyField(buildDisplayMode);
            }

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            EditorGUILayout.Space();

            //Screen Size
            screenSize = (AbstractImmersiveCamera.ScreenSizes)EditorGUILayout.EnumPopup("Screen Size", screenSize);
            EditorPrefs.SetInt("ImmersiveCameraScreenSize", (int)screenSize);

            EditorGUILayout.Space();

            //Active Walls
            EditorGUILayout.LabelField("Active Walls", EditorStyles.boldLabel);

            leftWallActive = EditorGUILayout.Toggle("Left", leftWallActive);
            centerWallActive = EditorGUILayout.Toggle("Center", centerWallActive);
            rightWallActive = EditorGUILayout.Toggle("Right", rightWallActive);
            backWallActive = EditorGUILayout.Toggle("Back", backWallActive);
            floorActive = EditorGUILayout.Toggle("Floor", floorActive);

            SurfacePosition surfacePosition = new SurfacePosition();
            if (leftWallActive) surfacePosition += 1;
            if (centerWallActive) surfacePosition += 2;
            if (rightWallActive) surfacePosition += 4;
            if (backWallActive) surfacePosition += 8;
            if (floorActive) surfacePosition += 16;

            EditorPrefs.SetInt("ImmersiveCameraLayout", (int)surfacePosition);

            EditorGUI.EndDisabledGroup();

            //Interaction Settings
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("---------------------------------", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Interaction Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(interactionOn);
            EditorGUI.indentLevel++;

            EditorGUI.BeginDisabledGroup(!immersiveCamera.interactionOn);
            EditorGUILayout.PropertyField(pointTouchesOn);
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(!immersiveCamera.pointTouchesOn);
            EditorGUILayout.PropertyField(displayTouchPoints);
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(areaTouchesOn);
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(!immersiveCamera.areaTouchesOn);
            EditorGUILayout.PropertyField(displayAreaTouches);
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;

            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(customFloorInteractionSettings);

            if (immersiveCamera.customFloorInteractionSettings)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(floorInteractionOn);
                EditorGUI.indentLevel++;

                EditorGUI.BeginDisabledGroup(!immersiveCamera.floorInteractionOn);
                EditorGUILayout.PropertyField(floorPointTouchesOn);
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(!immersiveCamera.floorPointTouchesOn);
                EditorGUILayout.PropertyField(floorDisplayTouchPoints);
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;

                EditorGUILayout.PropertyField(floorAreaTouchesOn);
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(!immersiveCamera.floorAreaTouchesOn);
                EditorGUILayout.PropertyField(floorDisplayAreaTouches);
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;

                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }

            EditorUtility.SetDirty(immersiveCamera);
        }
    }

#endif

}
