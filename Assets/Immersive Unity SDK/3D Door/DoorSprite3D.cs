using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[ExecuteAlways]
public class DoorSprite3D : MonoBehaviour
{
    [SerializeField] Sprite sprite = null;
    [Range(-90,90)]
    [SerializeField] float _doorAngle = 0;
    public float DoorAngle
    {
        get { return _doorAngle; }
        set
        {
            _doorAngle = value;
            SetDoorAngle();
        }
    }
    [SerializeField] float yOffset = 0;

    private Camera _cam;
    private Camera Cam
    {
        get {
            if (!_cam) _cam = GetComponentInChildren<Camera>();
            return _cam;
        }
    }

    private SpriteRenderer _door;
    private SpriteRenderer Door
    {
        get
        {
            if (!_door) _door = GetComponentInChildren<SpriteRenderer>();
            return _door;
        }
    }

    private GameObject _viewingQuad;
    private GameObject ViewingQuad
    {
        get
        {
            if (!_viewingQuad) _viewingQuad = transform.Find("Viewing Quad").gameObject;
            return _viewingQuad;
        }
    }

    private RenderTexture rt;

    private void OnDestroy()
    {
        Cam.targetTexture = null;
        Destroy(rt);
    }

    private void Start()
    {
        rt = new RenderTexture(1920, 1920, 16, RenderTextureFormat.ARGB32);
        rt.Create();
        Cam.targetTexture = rt;
        ViewingQuad.GetComponent<Renderer>().sharedMaterial.mainTexture = rt;
        ViewingQuad.GetComponent<Renderer>().enabled = true;
        Cam.Render();
    }

    private void OnValidate()
    {
        SetDoorAngle();
        Door.sprite = sprite;
    }

    private void SetDoorAngle()
    {
        Door.transform.localPosition = new Vector3(0, yOffset);
        Door.transform.eulerAngles = new Vector3(0, _doorAngle);
    }

    /// <summary>
    /// Change the angle to the door to a specified angle over a specified duration.
    /// </summary>
    /// <param name="targetAngle">Angle in degrees.</param>
    /// <param name="duration">Duration in seconds.</param>
    public void ChangeDoorAngle(float targetAngle, float duration)
    {
        StartCoroutine(ChangeAngle(targetAngle, duration));
    }

    /// <summary> Completely opens the door in 1 second. </summary>
    public void OpenDoor() => ChangeDoorAngle(90, 1);
    /// <summary> Closes the door in 1 second. </summary>
    public void CloseDoor() => ChangeDoorAngle(0, 1);

    private IEnumerator ChangeAngle(float targetAngle, float duration)
    {
        var startAngle = DoorAngle;
        var startTime = Time.time;
        var endTime = Time.time + duration;

        while (Time.time < endTime)
        {
            var lerpValue = Mathf.InverseLerp(startTime, endTime, Time.time);
            DoorAngle = Mathf.Lerp(startAngle, targetAngle, lerpValue);

            yield return new WaitForEndOfFrame();
        }
        DoorAngle = targetAngle;
    }

}
