using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserStartPosition : MonoBehaviour
{
    public GameObject objStart;
    void Start()
    {

    }

    [ContextMenu("CheckSize")]
    void CallFunction()
    {
        objStart.transform.localPosition = CheckSize(GetComponent<SpriteRenderer>().sprite, this.transform);

    }


    Vector2 CheckSize(Sprite sprite, Transform obj)
    {
        float minX, maxX;
        float minY, maxY;

        float xSize = sprite.rect.width;
        float ySize = sprite.rect.height;

        minX = maxX = xSize / 2;
        minY = maxY = ySize / 2;

        Debug.Log(xSize + "   " + ySize);

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                Color col = sprite.texture.GetPixel(x, y);

                if (col.a != 0)
                {
                    if (x < minX)
                        minX = x;

                    if (x > maxX)
                        maxX = x;

                    if (y < minY)
                        minY = y;

                    if (y > maxY)
                        maxY = y;
                }
            }
        }

        float xPos = CalculateWorldPosOfPixelCoordinate((int)maxX, sprite.bounds.size.x, 0.5f, obj.position.x, obj.localScale.x);
        float yPos = CalculateWorldPosOfPixelCoordinate((int)maxY, sprite.bounds.size.y, 0.32f, obj.position.y, obj.localScale.y);

        Debug.Log(xPos + "   " + yPos);

        Vector2 startPosition = new Vector3(xPos, yPos, 0) - (Vector3.one) * 0.03f;

        return startPosition;
    }

    float CalculateWorldPosOfPixelCoordinate(int coord, float boundsSize, float pivot, float position, float scale)
    {
        float PixelInWorldSpace = 1.0f / 1080;
        float startPos = position - (boundsSize * pivot * scale);

        return startPos + (PixelInWorldSpace * coord) * scale;
    }
}
