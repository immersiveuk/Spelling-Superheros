using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private void FixedUpdate()
    {
        this.transform.Rotate(Vector3.back);
    }
}
