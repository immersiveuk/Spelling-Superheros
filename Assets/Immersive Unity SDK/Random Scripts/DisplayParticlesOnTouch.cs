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

    private void OnEnable() => AbstractImmersiveCamera.AnySurfaceTouchedEvent.AddListener(OnSurfaceTouched);
    private void OnDisable() => AbstractImmersiveCamera.AnySurfaceTouchedEvent.RemoveListener(OnSurfaceTouched);
       
    private void OnSurfaceTouched(SurfaceTouchedEventArgs args)
    {
        if (!activeSurfaces.HasFlag(args.TouchedSurfacePosition)) return;

        var viewportPosition = args.ViewportPosition;
        if (viewportPosition.y < minHeight ||
            viewportPosition.y > maxHeight)
            return;

        switch (args.Phase)
        {
            case TouchPhase.Began:
                CreateParticleSystem(args.TouchIndex);
                PositionParticleSystem(args);
                break;

            case TouchPhase.Moved:
                PositionParticleSystem(args);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                DisableParticleSystem(args.TouchIndex);
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
        if (!particleSystems.ContainsKey(touchIndex))
        {
            var particleSystem = Instantiate(particlesPrefab, transform);
            particleSystems[touchIndex] = particleSystem;
        }
        else
            particleSystems[touchIndex].Play();
    }

    private void PositionParticleSystem(SurfaceTouchedEventArgs args)
    {
        var position = args.GetWorldPoint(distanceFromCamera);
        var particleSystem = particleSystems[args.TouchIndex];
        particleSystem.transform.position = position;
    }
}
