using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotate : MonoBehaviour
{
    public float range = 10;

	void Update ()
    {
        transform.Rotate(Vector3.up, Random.Range(-range, range));
        transform.Rotate(Vector3.right, Random.Range(-range, range));
		
	}
}
