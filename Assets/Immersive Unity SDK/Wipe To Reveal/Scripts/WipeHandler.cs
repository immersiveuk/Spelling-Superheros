using System;
using System.Collections;
using System.Collections.Generic;
using Com.Immersive.Cameras;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Com.Immersive.WipeToReveal
{

    public class WipeAssets
    {
        public Transform objBrush;
        public Transform objParticle;

        public WipeAssets(Transform brush, Transform particle)
        {
            this.objBrush = brush;
            this.objParticle = particle;
        }
    }

    /// <summary>
    /// The script handles all the functionality of Wipe to reveal.
    /// </summary>
    public class WipeHandler : MonoBehaviour
    {

        //Dictionaries which store brush sprites and particle systems.
        private Dictionary<int, WipeAssets> dictionaryWipeAssets = new Dictionary<int, WipeAssets>();

        private SpriteRenderer spriteRenderer;//, brushSprite;
        private Sprite spriteToRemove, wipeBrushSprite;
        private Vector2 spriteBoundsSize, spriteSize;

        WipeSettings wipeSettings;
        private bool paintFinished = false;

        private IOnWipeEventHandler[] onWipeEventHandlers;

        Color[] brushColorDefault;

        public ComputeShader computeShader;
        int kernelHandle;
        int totalPixels;
        uint[] outputAlpha = new uint[1];
        ComputeBuffer computeBuffer;

        private void Awake()
        {
            AbstractImmersiveCamera.AnyWallTouched.AddListener(WallTouched);
        }

        void Start()
        {
            spriteRenderer = transform.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteBoundsSize = spriteRenderer.bounds.size;
                spriteToRemove = spriteRenderer.sprite;
                spriteSize = new Vector2(spriteToRemove.texture.width, spriteToRemove.texture.height);

                //Remove one pixle from each border of Foreground image
                RemoveBorder(spriteToRemove.texture);

                //Get total pixle of Foreground image
                totalPixels = spriteToRemove.texture.GetPixels().Length;

            }
               
            computeBuffer = new ComputeBuffer(1, sizeof(uint));
        }

        private void OnDestroy()
        {
            computeBuffer.Dispose();
        }

        /// <summary>
        /// initialize wipe setting and create brush sprite and particle if available
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="wipeTransform"></param>
        public void Init(WipeSettings settings, Transform wipeTransform)
        {
            paintFinished = false;
            wipeSettings = settings;

            //Get compute shader from WipeManager
            computeShader = settings.computeShader;
            //Get ID of function from Compute Shader to call
            kernelHandle = computeShader.FindKernel("CSMain");

            onWipeEventHandlers = wipeTransform.GetComponentsInChildren<IOnWipeEventHandler>(); //use wipemanager

            //Get WipeBrush texture from editor
            Texture2D newTexture = wipeSettings.wipeBrush.texture;

            //Set size of WipeBrush based on "brushSize" from Editor
            Vector2 newSize = new Vector2(wipeSettings.brushSize * ((float)newTexture.width / newTexture.height), wipeSettings.brushSize);

            //Get WipeBrush texture with "brushSize"
            newTexture = ScaleTexture(newTexture, (int)newSize.x, (int)newSize.y);

            //Create sprite from WipeBrush texture
            wipeBrushSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f), 2000);
            wipeBrushSprite.texture.Apply();

            //Get all pixels from WipeBrush image
            brushColorDefault = wipeBrushSprite.texture.GetPixels();
        }

        /// <summary>
        /// Check is Wipe assets need to create for the touchID
        /// </summary>
        /// <param name="touchID"></param>
        void CheckWipeAssets(int touchID)
        {
            if (dictionaryWipeAssets.ContainsKey(touchID))
                return;

            SpriteRenderer brushSprite = null;
            ParticleSystem touchParticleSystem = null;

            //Create Brush if it is set in editor
            if (wipeSettings.brushSprite != null)
            {
                brushSprite = new GameObject().AddComponent<SpriteRenderer>();
                brushSprite.GetComponent<SpriteRenderer>().sortingOrder = 4090;
                brushSprite.sprite = wipeSettings.brushSprite.sprite;
                brushSprite.color = wipeSettings.brushSprite.color;
                brushSprite.enabled = false;
                brushSprite.transform.localScale = wipeSettings.scale;
            }

            //Create particle if it is set in editor
            if (wipeSettings.touchParticleSystem != null)
            {
                touchParticleSystem = Instantiate(wipeSettings.touchParticleSystem);
            }

            //Add Brush and Particle in dictionary against touchID
            if (wipeSettings.brushSprite != null || wipeSettings.touchParticleSystem != null)
                dictionaryWipeAssets.Add(touchID, new WipeAssets(brushSprite ? brushSprite.transform : null, touchParticleSystem ? touchParticleSystem.transform : null));
        }

        /// <summary>
        /// clear dictionary of Wipe assets
        /// </summary>
        void ClearDictionary()
        {
            foreach (var obj in dictionaryWipeAssets)
            {
                if (obj.Value.objBrush)
                    Destroy(obj.Value.objBrush.gameObject);

                if (obj.Value.objParticle)
                    Destroy(obj.Value.objParticle.gameObject);
            }

            dictionaryWipeAssets.Clear();
        }

        /// <summary>
        /// Enable/Disable Brush or Particle of index value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        void EnableDisableBrushParticle(bool value, int index)
        {
            if (dictionaryWipeAssets.ContainsKey(index))
            {
                WipeAssets wipeAssets = dictionaryWipeAssets[index];

                if (wipeAssets.objBrush)
                    wipeAssets.objBrush.GetComponent<SpriteRenderer>().enabled = value;

                if (wipeAssets.objParticle)
                {
                    if (value == true) wipeAssets.objParticle?.GetComponent<ParticleSystem>().Play();
                    else wipeAssets.objParticle?.GetComponent<ParticleSystem>().Stop();
                }
            }
        }

        /// <summary>
        /// event listener of wall touched
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="cameraIndex"></param>
        /// <param name="phase"></param>
        /// <param name="index"></param>
        private void WallTouched(Vector2 pos, int cameraIndex, TouchPhase phase, int index)
        {
            if (paintFinished)
                return;

            Camera cam = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[cameraIndex];

            switch (phase)
            {
                case TouchPhase.Began:
                    CheckWipeAssets(index);
                    //EnableDisableBrushParticle(true, index);
                    OnWipe(pos, phase, index, cam);
                    break;

                case TouchPhase.Moved:
                    OnWipe(pos, phase, index, cam);
                    break;

                case TouchPhase.Ended:
                    EnableDisableBrushParticle(false, index);
                    break;
            }
        }

        /// <summary>
        /// Handles a user touching the Wipe To Reveal System.
        /// Will wipe away part of the foreground and display brush and particle system as necessary.
        /// </summary>
        /// <param name="position">The position of the touch in screen space.</param>
        /// <param name="phase">The TouchPhase.</param>
        /// <param name="touchID">The ID of the Touch.</param>
        /// <param name="cam">The camera associated with the surface which was touched.</param>
        void OnWipe(Vector3 position, TouchPhase phase, int touchID, Camera cam)
        {
            //1. Check if touch hit this Wipe Object.
            var ray = cam.ScreenPointToRay(position);
            RaycastHit rayInfo;
            bool didHit = true;
            //Ray didn't hit anything    
            if (!Physics.Raycast(ray, out rayInfo)) didHit = false;//return;
            //Ray hit object but not current GameObject
            if (didHit && rayInfo.collider.gameObject != gameObject) didHit = false;//return;

            if (didHit)
            {
                EnableDisableBrushParticle(true, touchID);
            }
            else
            {
                EnableDisableBrushParticle(false, touchID);
                return;
            }

            //2. Convert position of touch to Local Space of this object
            var localTouchPos = (Vector2)transform.InverseTransformPoint(rayInfo.point);

            //3. Convert the position of touch to a pixel coordinate in the original sprite original sprite.
            Vector2 pixelPosition = localTouchPos * spriteToRemove.pixelsPerUnit;
            pixelPosition.x += spriteToRemove.rect.width / 2;
            pixelPosition.y += spriteToRemove.rect.height / 2;

            //4. Remove a circle of pixels around the current touch point.            
            RemoveCircle(spriteToRemove.texture, (int)pixelPosition.x - wipeBrushSprite.texture.width / 2, (int)pixelPosition.y - wipeBrushSprite.texture.height / 2); //get circle           

            //5. Calculate current wipe percentage                    
            float currentPercentage = 100 - CalculateAverageColorFromTexture(); //check for transparency percentage

            //6. Handle Brush sprite and Particles
            OnWipeBrushAndParticles(rayInfo.point, touchID, currentPercentage, cam);

            //7. Pass wipe info onto WipeEventHandlers.
            // The position between 0 and 1 in each dimension.
            Vector2 touchPosInLocalScale = new Vector2(pixelPosition.x / spriteToRemove.rect.width, pixelPosition.y / spriteToRemove.rect.height);
            WipeOccuring(phase, touchPosInLocalScale, currentPercentage);



        }

        /// <summary>
        /// This handles the positioning of the Brush Sprite and Particle System when a user touches the Wipe to Reveal System.
        /// </summary>
        /// <param name="position">The position in which to place brush sprite and particle system.</param>
        private void OnWipeBrushAndParticles(Vector3 position, int touchID, float currentPercentage, Camera cam)
        {
            if (!dictionaryWipeAssets.ContainsKey(touchID))
                return;

            WipeAssets wipeAssets = dictionaryWipeAssets[touchID];

            //BRUSH
            if (wipeAssets.objBrush)
            {
                wipeAssets.objBrush.position = position + (Vector3)wipeSettings.offset;
                wipeAssets.objBrush.eulerAngles = cam.transform.eulerAngles;   //Make Brush face camera.
            }

            //Particles
            if (wipeAssets.objParticle)
            {
                wipeAssets.objParticle.position = position;
                SetParticleEmisions(currentPercentage, touchID);
            }
        }

        //Varies the rate of emission by how much of wipe is being revealed.
        private float prevWipePercentage = 100;
        private void SetParticleEmisions(float wipePercentage, int touchID)
        {
            if (dictionaryWipeAssets.ContainsKey(touchID) && dictionaryWipeAssets[touchID].objParticle != null)
            {
                var wipeDelta = wipePercentage - prevWipePercentage;
                prevWipePercentage = wipePercentage;
                var emission = dictionaryWipeAssets[touchID].objParticle.GetComponent<ParticleSystem>().emission;
                emission.rateOverTimeMultiplier = wipeDelta * wipeSettings.emisionRateMultiplier;
            }
        }

        /// <summary>
        /// wiped out pixles in form of Brush image
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="radius"></param>
        public void RemoveCircle(Texture2D tex, int cx, int cy)
        {
            Color[] brushColor;
            int brushWidth = wipeBrushSprite.texture.width;
            int brushHeight = wipeBrushSprite.texture.height;

            int brushX = 0, brushY = 0;
            bool isEdge = false;


            //If any part of WipeBrush image is outside of Foreground image it required to exclude from color[] array

            if (cx + brushWidth > spriteSize.x) //1 Check if WipeBrush is at left edge, change the width of WipeBrush to apply for wipe            
            {
                isEdge = true;
                brushWidth = (int)spriteSize.x - cx;
            }
            else if (cx < 0) //2 Check if WipeBrush is at right edge, change the width of WipeBrush to apply for wipe
            {
                isEdge = true;
                brushWidth = brushWidth + cx;
                brushX = -cx;
                cx = 0;
            }

            if (cy + brushHeight > spriteSize.y) //3 Check if WipeBrush is at top edge, change the height of WipeBrush to apply for wipe            
            {
                isEdge = true;
                brushHeight = (int)spriteSize.y - cy;
            }
            else if (cy < 0) //4 Check if WipeBrush is at bottom edge, change the height of WipeBrush to apply for wipe            
            {
                isEdge = true;
                brushHeight = brushHeight + cy;
                brushY = -cy;
                cy = 0;
            }

            if (isEdge) //5 if WipeBrush is at any edge of foreground image, recalculate the color of WipeBrush for given position and height,width
            {
                brushColor = wipeBrushSprite.texture.GetPixels(brushX, brushY, brushWidth, brushHeight);  //Get the color of WipeBrush required for edge
            }
            else //6 if WipeBrush is not at any edge of foreground image, get the color of origional brush
            {
                brushColor = brushColorDefault; //get all colors of brush if not edge
            }

            //7 Get color of foreground image from given position with given height and width
            Color[] colorToApply = tex.GetPixels(cx, cy, brushWidth, brushHeight);

            for (int i = 0; i < colorToApply.Length; i++)
            {
                //8 Change transparency of pixel based on WipeBrush pixels.
                colorToApply[i].a *= (1 - brushColor[i].a);
            }

            //9 Set the pexels back to the foreground image after changing transparency
            tex.SetPixels(cx, cy, brushWidth, brushHeight, colorToApply);
            tex.Apply();
        }

        /// <summary>
        /// calculate transparency percentage
        /// </summary>
        /// <returns></returns>
        float CalculateAverageColorFromTexture()
        {
            ///*uint[] */outputAlpha = new uint[1];
            ////1. Create buffre to store the value of "AlphaBuffer" variable
            ///*ComputeBuffer*/ computeBuffer = new ComputeBuffer(1, sizeof(uint));

            //2. Set the value for "AlphaBuffer" variable in Compute Shader
            computeShader.SetBuffer(kernelHandle, "AlphaBuffer", computeBuffer);

            //3. Assign texture for "InputTexture" variable in Compute Shader
            computeShader.SetTexture(kernelHandle, "InputTexture", spriteToRemove.texture);

            //4. Call the function by passing function id and image size devide by threads group
            computeShader.Dispatch(kernelHandle, (int)spriteSize.x / 8, (int)spriteSize.y / 8, 1);

            //5. Get the value of average alpha
            computeBuffer.GetData(outputAlpha);
            computeBuffer.SetData(new uint[1]);

            //6. Divide by total number of pixles of Foreground Image
            float currentPercentage = (float)outputAlpha[0] / totalPixels * 100.0f;

            //8. Check if wipe percentage is greater than given percentage
            if ((100 - currentPercentage) >= wipeSettings.wipePercentage && !paintFinished)
            {
                paintFinished = true;
                OnComplete(); //enable when wipe functionality implemented as Hotspot
                Destroy(GetComponent<BoxCollider>());
                StartCoroutine(FadeOut());
            }

            return currentPercentage;
        }

        /// <summary>
        /// On Wipe complete
        /// </summary>
        void OnComplete()
        {
            foreach (var handler in onWipeEventHandlers)
            {
                handler.WipeComplete();
            }
        }

        /// <summary>
        /// On Wipe Occuring
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="position"></param>
        /// <param name="currentPercentage"></param>
        void WipeOccuring(TouchPhase phase, Vector2 position, float currentPercentage)
        {
            foreach (var handler in onWipeEventHandlers)
            {
                handler.WipeOccuring(phase, position, currentPercentage);
            }
        }

        /// <summary>
        /// Fade Out Foregroung image when Wipe complete
        /// </summary>
        /// <returns></returns>
        IEnumerator FadeOut()
        {
            ClearDictionary();

            float time = 0;

            while (spriteRenderer.color.a >= 0)
            {
                time += Time.deltaTime;

                var lerpValue = time / (wipeSettings.fadeOutDuration);

                Color col = spriteRenderer.color;
                col.a = Mathf.Lerp(100, 0, lerpValue) / 100;
                spriteRenderer.color = col;

                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Resize the brush sprite
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <returns></returns>
        private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
            Color[] rpixels = result.GetPixels(0);
            float incX = ((float)1 / source.width) * ((float)source.width / targetWidth);
            float incY = ((float)1 / source.height) * ((float)source.height / targetHeight);
            for (int px = 0; px < rpixels.Length; px++)
            {
                rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth),
                                  incY * ((float)Mathf.Floor(px / targetWidth)));
            }
            result.SetPixels(rpixels, 0);
            result.Apply();
            return result;
        }

        /// <summary>
        /// Remove one pixle of each border to solve edge issue
        /// It seems like Unity issue due to which it required to remove one pixle from each border
        /// </summary>
        /// <param name="texture"></param>
        void RemoveBorder(Texture2D texture)
        {
            //1 Remove one pixle row at bottm
            for (int x = 0; x < texture.width; x++)
            {
                Color col = texture.GetPixel(x, 0);
                col.a = 0;
                texture.SetPixel(x, 0, col);
            }

            //2 Remove one pixle row at top
            for (int x = 0; x < texture.width; x++)
            {
                Color col = texture.GetPixel(x, texture.height - 1);
                col.a = 0;
                texture.SetPixel(x, texture.height - 1, col);
            }

            //3 Remove one pixle row at left
            for (int y = 0; y < texture.width; y++)
            {
                Color col = texture.GetPixel(0, y);
                col.a = 0;
                texture.SetPixel(0, y, col);
            }

            //4 Remove one pixle row at right
            for (int y = 0; y < texture.width; y++)
            {
                Color col = texture.GetPixel(texture.width - 1, y);
                col.a = 0;
                texture.SetPixel(texture.width - 1, y, col);
            }

            texture.Apply();
        }
    }
}