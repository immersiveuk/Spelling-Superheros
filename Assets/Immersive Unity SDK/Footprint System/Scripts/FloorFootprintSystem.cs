/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Dec 2019
 */

using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Immersive.FootprintSystem
{
    /// <summary>
    /// A System which displays footprints where the user stands on the floor.
    /// </summary>
    public class FloorFootprintSystem : MonoBehaviour
    {

        public GameObject footPrefab;
        public float minDistanceBetweenSteps = 200;

        public Sprite leftFootprintSprite;
        public Sprite rightFootprintSprite;
        public float footprintSize = 1;
        public float fadeOutDuration = 5;

        private Dictionary<int, Footprint> previousFootprints;

        // Start is called before the first frame update
        void Start()
        {
            AbstractImmersiveCamera.FloorTouched.AddListener(FloorTouched);
           
            previousFootprints = new Dictionary<int, Footprint>();
        }


        private void FloorTouched(Vector2 position, int cameraIndex, TouchPhase phase, int touchIndex)
        {
            if (previousFootprints.ContainsKey(touchIndex))
            {
                var previousFootprint = previousFootprints[touchIndex];
                var previousPosition = previousFootprint.position;
                var distance = Vector2.Distance(previousPosition, position);

                if (distance > minDistanceBetweenSteps)
                {
                    var foot = previousFootprint.foot == Footprint.Foot.Left ? Footprint.Foot.Right : Footprint.Foot.Left;
                    var footprint = NewFootprint(position, cameraIndex, touchIndex, foot);
                    AngleFootprint(footprint.transform, position, previousPosition);
                    previousFootprints[touchIndex] = footprint;
                }
            }
            else
            {
                var foot = NewFootprint(position, cameraIndex, touchIndex, Footprint.Foot.Left);
                previousFootprints[touchIndex] = foot;
            }
        }

        private Footprint NewFootprint(Vector2 position, int cameraIndex, int touchIndex, Footprint.Foot foot)
        {
            var cam = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[cameraIndex];
            var footprint = Instantiate(footPrefab, transform).GetComponent<Footprint>();
            footprint.name = "Footprint";
            Ray ray = cam.ScreenPointToRay(position);
            footprint.transform.position = ray.GetPoint(1);

            footprint.Setup(position, foot, fadeOutDuration, GetCorrectSprite(foot), footprintSize);
            return footprint;
        }

        private Sprite GetCorrectSprite(Footprint.Foot foot)
        {
            return foot == Footprint.Foot.Left ? leftFootprintSprite : rightFootprintSprite;
        }

        private void AngleFootprint(Transform foot, Vector2 position, Vector2 previousPosition)
        {
            var direction = position - previousPosition;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            foot.transform.eulerAngles = new Vector3(90, 0, angle);
        }
    }
}

