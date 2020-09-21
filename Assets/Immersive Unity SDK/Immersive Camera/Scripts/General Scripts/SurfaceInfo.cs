/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System;
using UnityEngine;

namespace Com.Immersive.Cameras
{
    /// <summary>
    /// Represents a surface in an immersive space.
    /// </summary>
    public struct SurfaceInfo : IEquatable<SurfaceInfo>
    {
        /// <summary>
        /// The Rect which defines the resolution and top left corner position of the surface.
        /// </summary>
        public Rect rect;
        /// <summary>
        /// Aspect ratio of the surface calculated by witdth/height.
        /// </summary>
        public float aspectRatio;
        /// <summary>
        /// The Layout position of the surface in the space.
        /// </summary>
        public SurfacePosition position;

        public SurfaceInfo(Rect rect, SurfacePosition position)
        {
            this.rect = rect;
            aspectRatio = rect.width / rect.height;
            this.position = position;
        }

        public bool Equals(SurfaceInfo other)
        {
            return (rect == other.rect) &&
                (position == other.position);
        }
    }
}
