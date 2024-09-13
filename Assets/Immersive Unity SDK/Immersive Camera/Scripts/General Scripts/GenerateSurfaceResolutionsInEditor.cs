using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Immersive.Cameras
{
    public static class GenerateSurfaceResolutionsInEditor
    {
        public static List<SurfacePosition> GetSurfacesFromLayout(SurfacePosition layout)
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

        public static List<Rect> GenerateSurfaceRectsFromLayout(AbstractImmersiveCamera.ScreenSizes screenSizes, SurfacePosition layout, int standardWidth)
        {
            List<SurfacePosition> surfaces = GetSurfacesFromLayout(layout);
            return GenerateSurfaceRects(screenSizes, surfaces, standardWidth);
        }

        public static List<Rect> GenerateSurfaceRects(AbstractImmersiveCamera.ScreenSizes screenSizes, List<SurfacePosition> surfacePositions, int standardWidth)
        {
            var surfaceResolutions = GetSurfaceResolutionForScreenSizes(screenSizes, standardWidth);
            int xOffset = 0;
            List<Rect> surfaceRects = new List<Rect>();
            foreach(SurfacePosition surfacePosition in surfacePositions)
            {
                var pos = new Vector2Int(xOffset, 0);
                var resolution = surfaceResolutions[surfacePosition];
                surfaceRects.Add(new Rect(pos, resolution));
                xOffset += resolution.x;
            }
            return surfaceRects;
        }

        private static Dictionary<SurfacePosition, Vector2Int> GetSurfaceResolutionForScreenSizes(AbstractImmersiveCamera.ScreenSizes screenSizes, int standardWidth)
        {
            int standardHeight = 0;

            switch (screenSizes)
            {
                case AbstractImmersiveCamera.ScreenSizes.Standard:
                case AbstractImmersiveCamera.ScreenSizes.Wide:
                case AbstractImmersiveCamera.ScreenSizes.WideFront:
                    standardHeight = Mathf.RoundToInt(standardWidth / (16f / 9f));
                    break;
                case AbstractImmersiveCamera.ScreenSizes.Standard16x10:
                case AbstractImmersiveCamera.ScreenSizes.Wide16x10:
                case AbstractImmersiveCamera.ScreenSizes.WideFront16x10:
                    standardHeight = Mathf.RoundToInt(standardWidth / (16f /10f));
                    break;
                case AbstractImmersiveCamera.ScreenSizes.Standard4x3:
                    standardHeight = Mathf.RoundToInt(standardWidth / (4f/3f));
                    break;
            }


            switch (screenSizes)
            {
                case AbstractImmersiveCamera.ScreenSizes.Standard:
                case AbstractImmersiveCamera.ScreenSizes.Standard16x10:
                case AbstractImmersiveCamera.ScreenSizes.Standard4x3:
                    return CreateSurfaceResolutions(standardWidth, standardHeight, 1, 1, 1, 1, 1);
                case AbstractImmersiveCamera.ScreenSizes.WideFront:
                case AbstractImmersiveCamera.ScreenSizes.WideFront16x10:
                    return CreateSurfaceResolutions(standardWidth, standardHeight, 1, 2, 1, 2, 1);
                case AbstractImmersiveCamera.ScreenSizes.Wide:
                case AbstractImmersiveCamera.ScreenSizes.Wide16x10:
                    return CreateSurfaceResolutions(standardWidth, standardHeight, 2, 2, 2, 2, 2);
                default:
                    Debug.LogError($"Generation Of Surface Rects for {screenSizes} not yet implemented.");
                    return new Dictionary<SurfacePosition, Vector2Int>();
            }
        }

        private static Dictionary<SurfacePosition, Vector2Int> CreateSurfaceResolutions(int standardWidth, int standardHeight, int leftMultiplier, int centreMultiplier, int rightMultiplier, int backMultiplier, int floorMultiplier)
        {
            var surfaceResolutions = new Dictionary<SurfacePosition, Vector2Int>();
            surfaceResolutions.Add(SurfacePosition.Left, new Vector2Int(standardWidth * leftMultiplier, standardHeight));
            surfaceResolutions.Add(SurfacePosition.Center, new Vector2Int(standardWidth * centreMultiplier, standardHeight));
            surfaceResolutions.Add(SurfacePosition.Right, new Vector2Int(standardWidth * rightMultiplier, standardHeight));
            surfaceResolutions.Add(SurfacePosition.Back, new Vector2Int(standardWidth * backMultiplier, standardHeight));
            surfaceResolutions.Add(SurfacePosition.Floor, new Vector2Int(standardWidth * floorMultiplier, standardHeight));
            return surfaceResolutions;
        }


    }

}