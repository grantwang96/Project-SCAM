using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarPositioner : MonoBehaviour {

	public Camera cam;
	public Vector3 offset; //from (0,0) to (1,1) (z = distance from cam)

	
	// Update is called once per frame
	void Update () {
		transform.position = cam.ViewportToWorldPoint(offset);
	}
}
