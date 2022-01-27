using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Immersive.Cameras
{
    /// <summary>
    /// An object containing information about a touch on an Immersive Surface. From this you can get information about the Surface the touch was on, the Camera rendering that Surface, the Touch position in Screen Space, Viewport Space and World Space and also information about the objects hit when a ray is cast out from the touch.
    /// </summary>
    public class SurfaceTouchedEventArgs
    {
        private ImmersiveRaycastHit Raycast;

        /// <summary>The screen point in Pixels of the touch.</summary>
        public Vector2 ScreenPoint { get; private set; }
        /// <summary>The viewport point of the touch. From 0-1.</summary>
        public Vector2 ViewportPosition => RenderingCamera.ScreenToViewportPoint(ScreenPoint);

        /// <summary>The index of the Camera rendering to the surface which was touched.</summary>
        public int RenderingCameraIndex { get; private set; }

        /// <summary>The Camera which renders to the surface which was touched.</summary>
        public Camera RenderingCamera { get; private set; }

        /// <summary>The Phase of the touch.</summary>
        public TouchPhase Phase { get; private set; }
        
        /// <summary>The Index of the touch. -1 if from Mouse.</summary>
        public int TouchIndex { get; private set; }

        /// <summary> The Immersive Camera which registed the Touch.</summary>
        public AbstractImmersiveCamera Sender { get; private set; }

        /// <summary> The SurfacePosition of the Surface which was touched.</summary>
        public SurfacePosition TouchedSurfacePosition { get; private set; }

        /// <summary>Did the touch hit a 2D Collider.</summary>
        public bool DidHit2DCollider => Raycast.raycastHit2D != null;
        /// <summary>Did the touch hit a 3D Collider.</summary>
        public bool DidHit3DCollider => Raycast.raycastHit3D != null;

        /// <summary>Returns the RaycastHit calculated from shooting a ray out from this touch if the Ray hits a 3D Collider.</summary>
        public RaycastHit? GetRaycastHit3D() => Raycast.raycastHit3D;

        /// <summary>Returns the RaycastHit2D calculated from shooting a ray out from this touch if the Ray hits a 2D Collider.</summary>
        public RaycastHit2D? GetRaycastHit2D() => Raycast.raycastHit2D;

        /// <summary>
        /// Returns the Point in World Space of the touch at the given distance from the Camera.
        /// </summary>
        public Vector3 GetWorldPoint(float distanceFromCamera) => RenderingCamera.ScreenToWorldPoint(new Vector3(ScreenPoint.x, ScreenPoint.y, distanceFromCamera));

        public SurfaceTouchedEventArgs(Vector2 screenPoint, int cameraIndex, Camera renderingCamera, TouchPhase touchPhase, int touchIndex, SurfacePosition touchedSurfacePosition, ImmersiveRaycastHit raycast, AbstractImmersiveCamera sender)
        {
            ScreenPoint = screenPoint;
            RenderingCameraIndex = cameraIndex;
            RenderingCamera = renderingCamera;
            Phase = touchPhase;
            TouchIndex = touchIndex;
            TouchedSurfacePosition = touchedSurfacePosition;
            Raycast = raycast;

            Sender = sender;
        }
    }

    /// <summary>
    /// A class which contains all potential either a 2D or a 3D RaycastHit.
    /// </summary>
    public struct ImmersiveRaycastHit
    {
        public RaycastHit? raycastHit3D { get; private set; }
        public RaycastHit2D? raycastHit2D { get; private set; }

        public bool DidHit2DCollider => raycastHit2D != null;
        public bool DidHit3DCollider => raycastHit3D != null;

        public Transform HitTransform
        {
            get
            {
                if (DidHit2DCollider)
                    return raycastHit2D.Value.transform;
                if (DidHit3DCollider)
                    return raycastHit3D.Value.transform;
                return null;
            }
        }

        public ImmersiveRaycastHit(RaycastHit raycastHit3D)
        {
            this.raycastHit3D = raycastHit3D;
            raycastHit2D = null;
        }

        public ImmersiveRaycastHit(RaycastHit2D raycastHit2D)
        {
            this.raycastHit2D = raycastHit2D;
            raycastHit3D = null;
        }
    }

}