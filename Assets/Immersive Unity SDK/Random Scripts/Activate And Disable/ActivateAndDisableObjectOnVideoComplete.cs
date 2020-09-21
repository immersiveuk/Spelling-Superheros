/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Oct 2019
 */

using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
/// <summary>
/// This component will activate or disable one or more provided game object when a video reaches its loop point.
/// </summary>
public class ActivateAndDisableObjectOnVideoComplete : AbstractActivateAndDisable
{
    // Start is called before the first frame update
    void Start()
    {
        var videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += VideoPlayer_loopPointReached;

        DisableActivateObjectsOnStart();
    }

    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        ActivateAndDisable();
    }
}
