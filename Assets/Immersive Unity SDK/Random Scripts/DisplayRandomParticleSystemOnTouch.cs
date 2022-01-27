using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayRandomParticleSystemOnTouch : MonoBehaviour
{
    [Min(0.01f)] [SerializeField] float distanceFromCamera = 1;
    [SerializeField] ParticleSystem[] particlesPrefabs = null;
    [SerializeField] SurfacePosition activeSurfaces = SurfacePosition.AllWallsAndFloor;

    [Header("Active Region")]
    [Range(0, 1)] [SerializeField] float minHeight = 0;
    [Range(0, 1)] [SerializeField] float maxHeight = 1;

    private Dictionary<int, ParticleSystemGroup> particleSystems = new Dictionary<int, ParticleSystemGroup>();

    private void OnEnable() => AbstractImmersiveCamera.AnySurfaceTouchedEvent.AddListener(OnSurfaceTouched);
    private void OnDisable() => AbstractImmersiveCamera.AnySurfaceTouchedEvent.RemoveListener(OnSurfaceTouched);

    private void OnValidate()
    {
        if (minHeight > maxHeight)
        {
            minHeight = maxHeight;
        }
    }
    
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
        particleSystems[touchIndex].StopParticles();
    }

    private void CreateParticleSystem(int touchIndex)
    {
        if (!particleSystems.ContainsKey(touchIndex))
        {
            var particleSystemGroup = new ParticleSystemGroup(particlesPrefabs, transform);
            particleSystems[touchIndex] = particleSystemGroup;
        }
        particleSystems[touchIndex].PlayRandomParticleSystem();
    }
    
    private void PositionParticleSystem(SurfaceTouchedEventArgs args)
    {
        particleSystems[args.TouchIndex].Position(args, distanceFromCamera);
    }

    private class ParticleSystemGroup
    {
        //References
        private ParticleSystem[] particleSystems;
        private Transform transform;

        private ParticleSystem currentlyActiveParticleSystem = null;

        public ParticleSystemGroup(ParticleSystem[] particlesPrefabs, Transform parent)
        {
            particleSystems = new ParticleSystem[particlesPrefabs.Length];
            
            GameObject go = new GameObject("Particles Group");
            transform = go.transform;
            transform.SetParent(parent);

            for (int i = 0; i < particlesPrefabs.Length; i++)
            {
                particleSystems[i] = Instantiate(particlesPrefabs[i], transform);
                particleSystems[i].Stop();
            }
        }
        
        public void Position(SurfaceTouchedEventArgs args, float distanceFromCamera)
        {
            transform.position = args.GetWorldPoint(distanceFromCamera);
        }

        public void PlayRandomParticleSystem()
        {
            int random = Random.Range(0, particleSystems.Length);
            currentlyActiveParticleSystem = particleSystems[random];
            currentlyActiveParticleSystem.Play();
        }

        public void StopParticles()
        {
            currentlyActiveParticleSystem.Stop();
            currentlyActiveParticleSystem = null;
        }
    }
}
