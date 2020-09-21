using Com.Immersive.Cameras;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component which will position a provided particle system where the user touches.
/// Note: The ParticleSystem should have World Space Simulation.
/// </summary>
public class DisplayParticlesOnTouch : MonoBehaviour
{
    [Min(0.01f)][SerializeField] float distanceFromCamera = 1;
    [SerializeField] ParticleSystem particlesPrefab = null;
    [SerializeField] SurfacePosition activeSurfaces = SurfacePosition.AllWallsAndFloor;

    [Header("Active Region")]
    [Range(0, 1)][SerializeField] float minHeight = 0;
    [Range(0, 1)] [SerializeField] float maxHeight = 1;

    private void OnValidate()
    {
        if (minHeight > maxHeight)
        {
            minHeight = maxHeight;
        }
    }

    private Dictionary<int, ParticleSystem> particleSystems = new Dictionary<int, ParticleSystem>();

    // Start is called before the first frame update
    void Start()
    {
        AbstractImmersiveCamera.AnyWallTouched.AddListener(OnWallTouched);
    }

    private void OnWallTouched(Vector2 screenPosition, int cameraIndex, TouchPhase phase, int touchIndex)
    {
        
        if (!activeSurfaces.HasFlag(AbstractImmersiveCamera.CurrentImmersiveCamera.GetSurfacePositionFromIndex(cameraIndex))) return;

        var cam = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[cameraIndex];
        var viewportPosition = cam.ScreenToViewportPoint(screenPosition);
        if (viewportPosition.y < minHeight ||
            viewportPosition.y > maxHeight)
            return;

        switch (phase)
        {
            case TouchPhase.Began:
                CreateParticleSystem(touchIndex);
                PositionParticleSystem(screenPosition, cameraIndex, touchIndex);
                break;

            case TouchPhase.Moved:
                PositionParticleSystem(screenPosition, cameraIndex, touchIndex);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                DisableParticleSystem(touchIndex);
                break;
        }
    }

    private void DisableParticleSystem(int touchIndex)
    {
        var particleSystem = particleSystems[touchIndex];
        particleSystem.Stop();
    }

    private void CreateParticleSystem(int touchIndex)
    {
        if (particleSystems.ContainsKey(touchIndex))
        {
            particleSystems[touchIndex].Play();
        }
        else
        {
            var particleSystem = Instantiate(particlesPrefab, transform);
            particleSystems[touchIndex] = particleSystem;
        }
    }

    private void PositionParticleSystem(Vector2 screenPosition, int cameraIndex, int touchIndex)
    {
        var cam = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[cameraIndex];
        var position = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, distanceFromCamera));

        var particleSystem = particleSystems[touchIndex];
        particleSystem.transform.position = position;
    }
}
