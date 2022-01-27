using Com.Immersive.Hotspots;
using UnityEngine;

#if UNITY_ATOMS_AVAILABLE
using UnityAtoms.BaseAtoms;
#endif

public class HotspotVisibilityHandler : MonoBehaviour, IHotspotVisibilityHandler
{
    public HotspotController parentController { get; set; }
    
    public void HotspotsVisible()
    {
#if UNITY_ATOMS_AVAILABLE
        onFocused.Raise(GetHashCode());
#endif
    }

    public void HotspotsHidden()
    {
#if UNITY_ATOMS_AVAILABLE
        onUnfocused.Raise(GetHashCode());
#endif
    }
   
#if UNITY_ATOMS_AVAILABLE
    [SerializeField] private IntEvent onFocused;
    [SerializeField] private IntEvent onUnfocused;
    [SerializeField] private bool completelyHide;


    public void OnContextFocused(int hash)
    {
        if (hash == GetHashCode()) return;

        if (completelyHide)
        {
            StartCoroutine(parentController.HideHotspots());
        }
        else
        {
            StartCoroutine(parentController.DisableHotspots());
        }
    }

    public void OnContextUnfocused(int hash)
    {
        if (hash == GetHashCode()) return;

        if (completelyHide)
        {
            StartCoroutine(parentController.ShowHotspots());
        }
        else
        {
            StartCoroutine(parentController.EnableHotspots());
        }
    }
#endif
}
