/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Immersive.Cameras
{
    [Flags]
    public enum SurfacePosition
    {
        None = 0,
        Left = 1,
        Center = 2,
        Right = 4,
        Back = 8,
        Floor = 16,
        WallsAndFloor = Left | Center | Right | Floor,
        Walls = Left | Center | Right,
        LeftCenter = Left | Center,
        CentreRight = Center | Right,
        AllWalls = Left | Center | Right | Back, 
        AllWallsAndFloor = AllWalls | Floor
    }
}
