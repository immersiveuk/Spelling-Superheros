using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepAndSinkFloorManager : MonoBehaviour
{

    //Number of Particles
    [Min(1)]
    [SerializeField] int numberOfParticles = 3;

    //Velocity
    [Range(0, 360)]
    [SerializeField] float angle = 0;
    [Range(0.01f, 0.5f)]
    [SerializeField] float speed = 1;
    public Vector2 Velocity {
        get {
            return new Vector2(speed * Mathf.Cos(Mathf.Deg2Rad * (-angle + 90)), speed * Mathf.Sin(Mathf.Deg2Rad * (-angle + 90)));
        }
    }

    //Scale
    [Min(0.01f)] [SerializeField] float minScale = 1;
    [Min(0.01f)] [SerializeField] float maxScale = 1;

    [Min(0.01f)] [SerializeField] float sinkDuration = 1;
    public float SinkDuration { get { return sinkDuration; } set { sinkDuration = value; } }

    //Sprites
    [SerializeField] Sprite[] sprites = null;


    private Rect cameraRect;

    private void OnValidate()
    {
        if (minScale > maxScale)
        {
            Debug.LogError("Min Scale has to be less than or equal to Max Scale.");
            minScale = maxScale;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        var cam = AbstractImmersiveCamera.CurrentImmersiveCamera.floorCamera;
        Vector2 camSize = new Vector2(cam.orthographicSize * cam.aspect * 2 / transform.lossyScale.x, cam.orthographicSize * 2);

        cameraRect = new Rect(Vector2.zero, camSize);
        cameraRect.center = Vector2.zero;

        //Create Initial Particles
        for (int i = 0; i < numberOfParticles; i++)
        {
            CreateParticle();
        }
    }

    /// <summary>
    /// Creates a new StepAndSinkParticle object and initialises it.
    /// </summary>
    private void CreateParticle()
    {
        GameObject go = new GameObject("Step And Sink Paricle");
        var particle = go.AddComponent<StepAndSinkParticle>() as StepAndSinkParticle;
        particle.Init(this);
    }

    private Vector2 GetRandomPointInRect(Rect rect)
    {
        return new Vector2(Random.Range(rect.xMin, rect.xMax), Random.Range(rect.yMin, rect.yMax));
    }

    /// <summary>
    /// Checks whether an object is visible.
    /// </summary>
    public bool CheckIfObjectInBounds(Vector2 objectBounds, Vector3 position)
    {
        Rect screenRect = new Rect(Vector2.zero, cameraRect.size + objectBounds);
        screenRect.center = Vector2.zero;

        return screenRect.Contains((Vector2)position);
    }

    /// <summary>
    /// Returns a new scale value between MinScale and maxScale
    /// </summary>
    public float GetNewScale()
    {
        return Random.Range(minScale, maxScale);
    }

    /// <summary>
    /// Returns a new random sprites from the provided array.
    /// </summary>
    public Sprite GetNewSprite()
    {
        return sprites[Random.Range(0, sprites.Length)];
    }

    /// <summary>
    /// Calculates a new Random point in the cameras view. Used by StepAndSinkParticle to calculate and initial position.
    /// </summary>
    /// <returns>A position in the cameras view point.</returns>
    public Vector3 GetRandomPositionInView()
    {
        return GetRandomPointInRect(cameraRect);
    }

    /// <summary>
    /// Calculates a new position outside just outside the view of the camera. If moving from that position in the direction of Velocity then the particle will move into cameras view. Used by StepAndSinkParticle to get a new position when resetting the particle.
    /// </summary>
    /// <param name="objectBounds">The bounds of the object which position needs reset.</param>
    /// <returns>New position.</returns>
    public Vector3 GetNewStartPosition(Vector2 objectBounds)
    {
        Rect screenRect = new Rect(Vector2.zero, cameraRect.size + objectBounds);
        screenRect.center = Vector2.zero;

        //Get the values of a random line which goes through the cameras view
        Vector2 startPoint = GetRandomPointInRect(cameraRect);
        Vector2 point2 = startPoint + Velocity;
        float m = Velocity.y / Velocity.x;

        //Calculate the Y value at which particle will leave the camera view based on current velocity
        float yLine = 0;
        if (Velocity.y > 0) yLine = screenRect.yMin;
        else if (Velocity.y < 0) yLine = screenRect.yMax;

        //Calculate the X value at which particle will leave the camera view based on current velocity
        float xLine = 0;
        if (Velocity.x > 0) xLine = screenRect.xMin;
        else if (Velocity.x < 0) xLine = screenRect.xMax;

        //Case for when parallel to X or Y
        if (yLine == 0) return new Vector2(xLine, m * (xLine - startPoint.x) + startPoint.y);
        if (xLine == 0) return new Vector2((yLine - startPoint.y) / m + startPoint.x, yLine);

        //Calculate intersections with the x and y edge of camera view.
        Vector2 intersectX = new Vector2((yLine - startPoint.y) / m + startPoint.x, yLine);
        Vector2 intersectY = new Vector2(xLine, m * (xLine - startPoint.x) + startPoint.y);

        //Calculate whether the particle will exit camera view via the side or top/bottom and return the corresponding position.
        if (Vector2.Distance(startPoint, intersectX) < Vector2.Distance(startPoint, intersectY))
            return intersectX;
        else return intersectY;
    }
}
