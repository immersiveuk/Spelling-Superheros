using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class BackgroundAnimator : MonoBehaviour
    {
        float cloudStartPosition, cloudLastPosition;
        public List<SpriteRenderer> clouds = new List<SpriteRenderer>();
        public List<Sprite> cloudSprites = new List<Sprite>();

        int cloudIndex = 0;

        void Start()
        {
            this.transform.localScale = new Vector3(1 / FindObjectOfType<Stage>().transform.localScale.x, 1, 1);

            cloudStartPosition = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[2].transform.localPosition.x + AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[2].aspect;
            cloudLastPosition = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[0].transform.localPosition.x - AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[0].aspect;

            IListExtensions.Shuffle(cloudSprites);

            for (int i = 0; i < clouds.Count; i++)
            {
                clouds[i].sprite = cloudSprites[i];
                AnimateCloud(clouds[i]);
            }

            IListExtensions.Shuffle(cloudSprites);
        }


        void AnimateCloud(SpriteRenderer cloud)
        {
            cloud.transform.localScale = Vector3.one * Random.Range(0.6f, 1.0f);

            float transitionTime = Vector2.Distance(cloud.transform.localPosition, new Vector2(cloudLastPosition, 0));

            iTween.MoveTo(cloud.gameObject, iTween.Hash("x", cloudLastPosition, "y", 0, "z", 0, "islocal", true,
              "time", transitionTime * 5, "easetype", iTween.EaseType.linear, "oncomplete", (System.Action<object>)(newValue =>
                {
                  cloud.sprite = cloudSprites[cloudIndex];
                  cloudIndex++;

                  if (cloudIndex >= clouds.Count)
                  {
                      cloudIndex = 0;
                      IListExtensions.Shuffle(cloudSprites);
                  }

                  cloud.transform.localPosition = new Vector3(cloudStartPosition, 0, 0);
                  AnimateCloud(cloud);
              })));
        }
    }
}