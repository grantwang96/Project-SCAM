using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatyRotaty : MonoBehaviour {

    public float rotationSpeed;
    public float floatySpeed;

    public Vector3 originPos;
    public float magnitude;

	// Use this for initialization
	void Start () {
        originPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = originPos + Vector3.up * magnitude * Mathf.Sin(Time.time * floatySpeed);

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
	}
}
