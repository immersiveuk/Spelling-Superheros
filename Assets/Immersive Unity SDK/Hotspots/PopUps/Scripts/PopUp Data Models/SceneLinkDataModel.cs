using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    [System.Serializable]
    public class SceneLinkDataModel
    {
        [System.Serializable]
        public class SceneLinkSetting /*: PopUpSetting*/
        {
            [SceneSelector]
            public string linkName;
            public bool fadeOut;
            public float fadeOutDuration = 3;

            [ColorUsage(false)]
            [JsonConverter(typeof(StringColorConverter))]// parse Json object into color property
            public Color fadeColor = Color.black;
            public bool fadeOutAudio = true;
        }

        public HotspotDataModel hotspotSetting;
        public SceneLinkSetting sceneLinkSettings;
    }
}