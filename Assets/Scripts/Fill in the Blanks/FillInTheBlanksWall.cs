using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.FillInTheBlank
{
    public class FillInTheBlanksWall : MonoBehaviour
    {
        [Header("Popup Complete")]
        public GameObject completePopup;

        private void Start()
        {
            completePopup.SetActive(false);
        }

        public void OnComplete()
        {
            completePopup.SetActive(true);
        }
    }
}