using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepelManager : MonoBehaviour
{
    [Header("Speeds")]
    [Range(0.01f, 2)] [SerializeField] float repelSpeed = 0.7f;
    [Range(0.01f, 2)] [SerializeField] float defaultSpeed = 0.3f;

    [Tooltip("Distance at from the touch at which objects will be repelled.")]
    [Range(0.01f, 2)] [SerializeField] float repelDistance = 1;

    [Range(0.1f, 10)] [SerializeField] float distanceFromCamera = 1;

    [Header("Repel Objects")]
    [SerializeField] RepelObject[] repelObjectPrefabs = null;
    [SerializeField] [Min(1)] int numberOfRepelObjects = 10;

    public enum WallOrFloor
    {
        Wall = 0,
        Floor = 1
    }
    [SerializeField] WallOrFloor wallOrFloor = WallOrFloor.Wall;

    private RepelObject[] repelObjects;

    // Start is called before the first frame update
    void Start()
    {
        if (repelObjectPrefabs.Length == 0)
        {
            Debug.LogError("RepelManager: Must define a RepelObject prefab.");
            return;
        }

        if (wallOrFloor == WallOrFloor.Floor && AbstractImmersiveCamera.CurrentImmersiveCamera.floorCamera == null)
        {
            Debug.LogError("RepelManager: Rendering on floor with no camera.");
            return;
        }

        repelObjects = new RepelObject[numberOfRepelObjects];

        for (int i = 0; i < numberOfRepelObjects; i++)
        {

            repelObjects[i] = InstantiateRepelObject();

        }
    }

    private RepelObject InstantiateRepelObject()
    {
        var prefab = repelObjectPrefabs[Random.Range(0, repelObjectPrefabs.Length)];
        RepelObject repelObj = Instantiate(prefab, transform);

        //Set Position
        Camera initialCamera = null;
        switch (wallOrFloor)
        {
            case WallOrFloor.Wall:
                initialCamera = AbstractImmersiveCamera.CurrentImmersiveCamera.wallCameras[Random.Range(0, AbstractImmersiveCamera.CurrentImmersiveCamera.wallCameras.Count)];
                break;

            case WallOrFloor.Floor:
                initialCamera = AbstractImmersiveCamera.CurrentImmersiveCamera.floorCamera;
                break;
        }

        var position = initialCamera.ViewportToWorldPoint(new Vector3(Random.value, Random.value, distanceFromCamera));
        repelObj.transform.position = position;

        repelObj.Init(repelSpeed, defaultSpeed, repelDistance, distanceFromCamera, wallOrFloor);

        return repelObj;
    }

    private void OnValidate()
    {
        if (Application.isPlaying && repelObjects != null)
        {
            foreach(var repelObj in repelObjects)
            {
                repelObj.SetVariables(repelSpeed, defaultSpeed, repelDistance);
            }
        }
    }
}
