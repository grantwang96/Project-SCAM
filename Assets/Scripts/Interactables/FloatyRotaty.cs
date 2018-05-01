using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatyRotaty : MonoBehaviour {

    public float rotationSpeed;
    public float floatySpeed;

    public Vector3 originPos;
    public float magnitude;

    public bool active;

    void Start()
    {
        SetPosition();
    }

    public void SetPosition() {
        originPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        if(!active) { return; }
        transform.position = originPos + Vector3.up * magnitude * Mathf.Sin(Time.time * floatySpeed);
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
	}
}
