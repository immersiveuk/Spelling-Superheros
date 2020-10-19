using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class BackgroundAnimator : MonoBehaviour
    {
        //float cloudStartPosition, cloudLastPosition;
        public List<SpriteRenderer> clouds = new List<SpriteRenderer>();
        public List<Sprite> cloudSprites = new List<Sprite>();

        int cloudIndex = 0;

        List<float> xRange = new List<float>();

        void Start()
        {
            this.transform.localScale = new Vector3(1 / FindObjectOfType<Stage>().transform.localScale.x, 1, 1);

            //cloudStartPosition = 1;// AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[2].transform.localPosition.x + AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[2].aspect;
            //cloudLastPosition = -1;// AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[0].transform.localPosition.x - AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[0].aspect;

            IListExtensions.Shuffle(cloudSprites);

            for (int i = 0; i < clouds.Count; i++)
            {
                clouds[i].sprite = cloudSprites[i];
                clouds[i].transform.localPosition = new Vector3(clouds[i].transform.localPosition.x, Random.Range(0.7f, 1.2f), 0);
                xRange.Add(clouds[i].transform.localPosition.x);
                AnimateCloud(clouds[i], i);
            }

            IListExtensions.Shuffle(cloudSprites);
        }


        void AnimateCloud(SpriteRenderer cloud, int index)
        {
            cloud.transform.localScale = Vector3.one * Random.Range(0.6f, 1.0f);

            float transitionTime = Vector2.Distance(cloud.transform.localPosition, new Vector2(-1, 0))*2;

            iTween.MoveTo(cloud.gameObject, iTween.Hash("x", cloud.transform.localPosition.x, "y", -1, "z", 0, "islocal", true,
              "time", transitionTime * 5, "easetype", iTween.EaseType.linear, "oncomplete", (System.Action<object>)(newValue =>
                {
                    cloud.sprite = cloudSprites[cloudIndex];
                    cloud.transform.localPosition = new Vector3(Random.Range(xRange[index] - 0.4f, xRange[index] + 0.4f), Random.Range(0.7f, 1.2f), 0);
                    AnimateCloud(cloud, index);

                    cloudIndex++;

                    if (cloudIndex >= clouds.Count)
                    {
                        cloudIndex = 0;
                        IListExtensions.Shuffle(cloudSprites);
                    }
                })));
        }
    }
}