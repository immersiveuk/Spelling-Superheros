using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This will display the current number of Hotspot with the script ActivateAndDisableOnAllHotspotActivated which remain to be opened.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class DisplayHotspotCountTextMeshPro : MonoBehaviour
{
    private enum CountingDirection { Increment, Decrement };
    [SerializeField] CountingDirection countingDirection = CountingDirection.Increment;

    private TextMeshProUGUI textMesh;
    private ActivateAndDisableOnAllHotspotsActivatedManager manager;

    private int _remainingHotspots;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        manager = ActivateAndDisableOnAllHotspotsActivatedManager.Instance;
        if (manager) _remainingHotspots = manager.RemainingHotspots;
    }

    // Update is called once per frame
    void Update()
    {
        if (manager && _remainingHotspots != manager.RemainingHotspots)
        {
            _remainingHotspots = manager.RemainingHotspots;
            string remainingHotspotString = countingDirection == CountingDirection.Increment ? (manager.TotalHotspots - _remainingHotspots).ToString() : _remainingHotspots.ToString();
            textMesh.text = remainingHotspotString + "/" + manager.TotalHotspots;
        }
    }
}
