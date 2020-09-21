/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using Com.Immersive.Cameras;
using System;
using System.IO;
using UnityEngine;

namespace Com.Immersive.Hotspots
{

    //NOTE: DOESN'T WORK WITH MULTIPLE HOTSPOT CONTROLLERS.
    public class HotspotSaver : MonoBehaviour
    {
        private GameObject Controller;
        public AbstractImmersiveCamera immersiveCamera;

        // Start is called before the first frame update
        void Start()
        {
            Controller = HotspotController.CurrentControllers[0].gameObject;
            SaveJson();
        }

        // Convert all hotspots in the Hotspots game object into a JSON representation.
        public void SaveJson()
        {

            var hotspotScene = new HotspotScene();
            hotspotScene.hotspots = new HotspotSaveable[Controller.transform.childCount];

            //Save position of each hotspot.
            //The position is calculated in pixels from the leftmost surface.
            for (int i = 0; i < hotspotScene.hotspots.Length; i++)
            {
                var child = Controller.transform.GetChild(i);
                Vector3 screenPos = immersiveCamera.cameras[0].WorldToScreenPoint(child.position);

                HotspotSaveable hotspot = new HotspotSaveable()
                {
                    position = new Vector2(screenPos.x, screenPos.y),
                    name = child.name
                };
                hotspotScene.hotspots[i] = hotspot;
            }

            string json = JsonUtility.ToJson(hotspotScene);
            print(json);

            File.WriteAllText(Application.persistentDataPath + "/hotspots2.json", json);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif

        }
    }

    [Serializable]
    public class HotspotScene
    {
        public HotspotSaveable[] hotspots;
    }

    [Serializable]
    public class HotspotSaveable
    {
        public Vector2 position;
        public string name;
    }
}