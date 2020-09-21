/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using Com.Immersive.Cameras;
using System.IO;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public class HotspotLoader : MonoBehaviour
    {
        public GameObject hotspotPrefab;
        public AbstractImmersiveCamera immersiveCamera;

        private bool start = true;

        // Update is called once per frame
        void Update()
        {
            if (!start) return;
            LoadFromJSON();
            start = false;
        }

        private void LoadFromJSON()
        {

            //Load and Parse JSON
            string json = File.ReadAllText(Application.persistentDataPath + "/hotspots2.json");
            print(json);
            HotspotScene hs = JsonUtility.FromJson<HotspotScene>(json);

            GameObject hotspotHolder = GameObject.Find("Hotspots");
            if (hotspotHolder == null)
            {
                hotspotHolder = new GameObject();
                hotspotHolder.name = "Hotspots";
            }


            foreach (HotspotSaveable hotspot in hs.hotspots)
            {
                //Instantiate Hotspot
                var hotspotObject = Instantiate(hotspotPrefab);
                hotspotObject.transform.parent = hotspotHolder.transform;

                // Works out which camera is associated with the surface hotspot appears on.
                // Also provides the position of the hotspot on that surface in pixels.
                var (cam, pos) = immersiveCamera.FindCameraFromScreenPosition(hotspot.position);
                if (cam == null) return;

                //Make hotspot face camera
                HotspotScript hotScript = hotspotObject.GetComponent<HotspotScript>();

                // Cast ray through screen point and place hotspot 2 units from the camera.
                Ray ray = cam.ScreenPointToRay(new Vector3(pos.x, pos.y));
                hotspotObject.transform.position = ray.GetPoint(2);
            }
        }
    }
}
