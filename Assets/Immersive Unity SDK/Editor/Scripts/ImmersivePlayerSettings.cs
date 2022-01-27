using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Immersive.Presets
{
    public static class ImmersivePlayerSettings
    {
        private const FullScreenMode fullScreenMode = FullScreenMode.Windowed;
        private const bool defaultIsNativeResolution = true;
        private const bool runInBackground = true;
        private const bool captureSingleScreen = false;
        private const bool resizableWindow = false;
        private const bool visibleInBackground = true;
        private const bool allowFullscreenSwitch = false;
        private const bool forceSingleInstance = false;

        public static void SetPlayerSettings()
        {
            PlayerSettings.fullScreenMode = fullScreenMode;
            PlayerSettings.defaultIsNativeResolution = defaultIsNativeResolution;
            PlayerSettings.runInBackground = runInBackground;
            PlayerSettings.captureSingleScreen = captureSingleScreen;
            PlayerSettings.resizableWindow = resizableWindow;
            PlayerSettings.visibleInBackground = visibleInBackground;
            PlayerSettings.allowFullscreenSwitch = allowFullscreenSwitch;
            PlayerSettings.forceSingleInstance = forceSingleInstance;
        }

        public static bool CanSetPlayerSettings()
        {
            return PlayerSettings.fullScreenMode != fullScreenMode ||
                PlayerSettings.defaultIsNativeResolution != defaultIsNativeResolution ||
                PlayerSettings.runInBackground != runInBackground ||
                PlayerSettings.captureSingleScreen != captureSingleScreen ||
                PlayerSettings.resizableWindow != resizableWindow ||
                PlayerSettings.visibleInBackground != visibleInBackground ||
                PlayerSettings.allowFullscreenSwitch != allowFullscreenSwitch ||
                PlayerSettings.forceSingleInstance != forceSingleInstance;
        }


        public static bool CanDisableSplashScreen => Application.HasProLicense();
        public static bool IsSplashScreenEnabled => PlayerSettings.SplashScreen.show;

        public static void DisableSplashScreen()
        {
            if (!CanDisableSplashScreen)
                return;
            PlayerSettings.SplashScreen.show = false;
        }
    }

}