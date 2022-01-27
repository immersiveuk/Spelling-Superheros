/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* Written by Luke Bissell <luke@immersive.co.uk>, June 2020
*/

using System;
using System.Runtime.InteropServices;
using UnityEngine;

internal class BorderlessWindow
{
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern IntPtr FindWindow(System.String className, System.String windowName);
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern System.IntPtr GetActiveWindow();
    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);

    const int GWL_STYLE = -16;
    const int WS_BORDER = 0x00800000;
    const int SWP_SHOWWINDOW = 0x0040;
#endif


    /// <summary>
    /// Sets the window to be a borderless window with the position and resolution defined by the windowRect.
    /// </summary>
    /// <param name="windowRect"></param>
    public static void SetWindowRect(Rect windowRect)
    {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        SetWindowLong(GetActiveWindow(), GWL_STYLE, WS_BORDER);
        bool success = SetWindowPos(GetActiveWindow(), 0, Convert.ToInt32(windowRect.xMin-1),
            Convert.ToInt32(windowRect.yMin-1),
            Convert.ToInt32(windowRect.width+2),
            Convert.ToInt32(windowRect.height+2), SWP_SHOWWINDOW);
#else
        Debug.LogError("BorderlessWindow.SetWindowRect() can only be called in a Standalone Windows application");
#endif

    }
}
