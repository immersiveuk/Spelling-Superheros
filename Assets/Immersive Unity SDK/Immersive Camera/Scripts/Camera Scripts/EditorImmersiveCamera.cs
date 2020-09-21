using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Immersive.Cameras
{
    [ExecuteInEditMode]
    public class EditorImmersiveCamera : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            AbstractImmersiveCamera.CurrentImmersiveCamera = GetComponent<AbstractImmersiveCamera>();
        }
    }
}