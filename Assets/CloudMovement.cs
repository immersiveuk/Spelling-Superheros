using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
	public float minX;
	public float maxX;

	public float speed = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		transform.Translate(new Vector3(speed, 0, 0));

		if (transform.position.x < minX)
		{
			transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
		}

		if (transform.position.x > maxX)
		{
			transform.position = new Vector3(minX, transform.position.y, transform.position.z);
		}
	}
}
