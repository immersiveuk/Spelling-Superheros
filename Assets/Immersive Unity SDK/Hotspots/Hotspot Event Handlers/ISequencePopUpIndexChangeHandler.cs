/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Nov 2019
 */

using UnityEngine;

/// <summary>
/// Subclasses of ImageSequenceIndexChangeHandler can be attached to Hotspots whose action is an Image Sequence Popup and the method IndexChanged(int newIndex) will be called every time the index changes. By implementing this method you can trigger actions associated with the display of a given image in the image sequence.
/// </summary>
namespace Com.Immersive.Hotspots
{
    public interface ISequencePopUpIndexChangeHandler : IPopUpEventHandler
    {
        void IndexChanged(int newIndex);
    }
}