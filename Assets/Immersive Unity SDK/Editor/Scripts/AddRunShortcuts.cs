using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Com.Immersive
{
    /// <summary>
    /// Automatically generates Run Shortcuts at build time. These allow you to quickly open your Immersive Experience with different configurations.
    /// </summary>
    public class AddRunShortcuts: IPreprocessBuildWithReport
    {
        private const string directoryName = "Run Shortcuts";

        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.result != BuildResult.Failed || report.summary.result != BuildResult.Cancelled)
            {
                var path = report.summary.outputPath;
                var dir = Directory.GetParent(path);
                string directoryPath = dir.FullName + "\\"+directoryName;
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                //Standard
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard, SurfacePosition.AllWalls);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard, SurfacePosition.WallsAndFloor);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard, SurfacePosition.Center);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard, SurfacePosition.LeftCenter);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard, SurfacePosition.Walls);

                //Wide Front
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.WideFront, SurfacePosition.AllWalls);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.WideFront, SurfacePosition.WallsAndFloor);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.WideFront, SurfacePosition.Center);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.WideFront, SurfacePosition.LeftCenter);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.WideFront, SurfacePosition.Walls);

                //Wide
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Wide, SurfacePosition.AllWalls);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Wide, SurfacePosition.WallsAndFloor);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Wide, SurfacePosition.Center);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Wide, SurfacePosition.LeftCenter);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Wide, SurfacePosition.Walls);

                //Standard 16x10
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard16x10, SurfacePosition.AllWalls);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard16x10, SurfacePosition.WallsAndFloor);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard16x10, SurfacePosition.Center);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard16x10, SurfacePosition.LeftCenter);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard16x10, SurfacePosition.Walls);

                //Standard 4x3
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard4x3, SurfacePosition.AllWalls);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard4x3, SurfacePosition.WallsAndFloor);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard4x3, SurfacePosition.Center);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard4x3, SurfacePosition.LeftCenter);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard4x3, SurfacePosition.Walls);

                //Wide Front 16x10
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.WideFront16x10, SurfacePosition.AllWalls);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.WideFront16x10, SurfacePosition.WallsAndFloor);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.WideFront16x10, SurfacePosition.Center);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.WideFront16x10, SurfacePosition.LeftCenter);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.WideFront16x10, SurfacePosition.Walls);

                //Wide 16x10
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Wide16x10, SurfacePosition.AllWalls);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Wide16x10, SurfacePosition.WallsAndFloor);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Wide16x10, SurfacePosition.Center);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Wide16x10, SurfacePosition.LeftCenter);
                CreateRunShortcutsForAllSizes(directoryPath, AbstractImmersiveCamera.ScreenSizes.Wide16x10, SurfacePosition.Walls);


                //Virtual Room
                CreateRunShortcutVirtualRoom(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard, SurfacePosition.AllWallsAndFloor);
                CreateRunShortcutVirtualRoom(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard16x10, SurfacePosition.AllWallsAndFloor);
                CreateRunShortcutVirtualRoom(directoryPath, AbstractImmersiveCamera.ScreenSizes.Standard4x3, SurfacePosition.AllWallsAndFloor);
                CreateRunShortcutVirtualRoom(directoryPath, AbstractImmersiveCamera.ScreenSizes.Wide, SurfacePosition.AllWallsAndFloor);
                CreateRunShortcutVirtualRoom(directoryPath, AbstractImmersiveCamera.ScreenSizes.Wide16x10, SurfacePosition.AllWallsAndFloor);
                CreateRunShortcutVirtualRoom(directoryPath, AbstractImmersiveCamera.ScreenSizes.WideFront, SurfacePosition.AllWallsAndFloor);
                CreateRunShortcutVirtualRoom(directoryPath, AbstractImmersiveCamera.ScreenSizes.WideFront16x10, SurfacePosition.AllWallsAndFloor);
            }
        }

        private enum Size { Small, Medium, Standard }
        private int GetStandardWidth(Size size)
        {
            switch (size)
            {
                case Size.Small:
                    return 480;
                case Size.Medium:
                    return 960;
                case Size.Standard:
                    return 1920;
                default:
                    return 0;
            }
        }

        private void CreateRunShortcutsForAllSizes(string directoryPath, AbstractImmersiveCamera.ScreenSizes screenSizes, SurfacePosition layout)
        {
            CreateRunShortcut(directoryPath, screenSizes, layout, Size.Small);
            CreateRunShortcut(directoryPath, screenSizes, layout, Size.Medium);
            CreateRunShortcut(directoryPath, screenSizes, layout, Size.Standard);
        }

        private void CreateRunShortcut(string directoryPath, AbstractImmersiveCamera.ScreenSizes screenSizes, SurfacePosition layout, Size size)
        {
            string fileName = $"Run [Type={screenSizes}, {layout}, Size={size}]";
            CreateRunShortcut(fileName, directoryPath, screenSizes, layout, GetStandardWidth(size));
        }

        private void CreateRunShortcutVirtualRoom(string directoryPath, AbstractImmersiveCamera.ScreenSizes screenSizes, SurfacePosition layout)
        {
            string fileName = $"Run Virtual Room [Type={screenSizes}, {layout}]";
            CreateRunShortcut(fileName, directoryPath, screenSizes, layout, 1920, true);
        }

        private static void CreateRunShortcut(string fileName, string directoryPath, AbstractImmersiveCamera.ScreenSizes screenSizes, SurfacePosition layout, int standardSurfaceWidth, bool virtualRoom = false)
        {
            string path = Path.Combine(directoryPath, fileName+".bat");
            string content = GenateRunShortcut(screenSizes, layout, standardSurfaceWidth);
            if (virtualRoom)
                content += "-virtualRoom=true";
            File.WriteAllText(path, content);
        }


        private static string GenateRunShortcut(AbstractImmersiveCamera.ScreenSizes screenSizes, SurfacePosition layout, int standardSurfaceWidth) 
        {
            string applicationPath = $"\"..\\{PlayerSettings.productName}.exe\"";
            string layoutArgs = GenerateLayoutArgs(screenSizes, layout, standardSurfaceWidth);
            return $"{applicationPath} {layoutArgs}";
        }


        private static string GenerateLayoutArgs(AbstractImmersiveCamera.ScreenSizes screenSizes, SurfacePosition layout, int standardSurfaceWidth)
        {
            List<Rect> surfaceRects = GenerateSurfaceResolutionsInEditor.GenerateSurfaceRectsFromLayout(screenSizes, layout, standardSurfaceWidth);
            return $"-layout=\"{(int)layout}\" -surfaces=\"{GenerateSurfacesString(surfaceRects)}\"";
        }

        private static string GenerateSurfacesString(List<Rect> surfaceRects)
        {
            string surfaces = "";
            foreach (Rect surfaceRect in surfaceRects)
                surfaces += $"[{surfaceRect.x},{surfaceRect.y},{surfaceRect.width},{surfaceRect.height}]";
            return surfaces;
        }
    }   
}