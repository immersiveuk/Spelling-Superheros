using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public abstract class HotspotActionInspectorGUI
    {
        public abstract void OnInspectorGUI();

        public Action UpdateSerializedObjectAction { private get; set; }
        protected void UpdateSerializedObject() => UpdateSerializedObjectAction?.Invoke();
    } 
}
