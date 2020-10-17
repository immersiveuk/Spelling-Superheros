using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static float AngleInRad(Vector3 vec1, Vector3 vec2)
    {
        return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);
    }

    //This returns the angle in degrees
    public static float AngleInDeg(Vector3 vec1, Vector3 vec2)
    {
        return AngleInRad(vec1, vec2) * 180 / Mathf.PI;
    }

    public static Vector2 GetNewPoint(Vector2 p1, Vector2 p2)
    {
        float newPointDistance = 5;

        float distance = Vector3.Distance(p1, p2);

        float dx = (p2.x - p1.x) / distance;
        float dy = (p2.y - p1.y) / distance;

        Vector2 p3 = new Vector2(p1.x + newPointDistance * dx, p1.y + newPointDistance * dy);

        return p3;
    }
}
