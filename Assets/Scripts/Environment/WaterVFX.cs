using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.PostProcessing;

[RequireComponent(typeof(PostProcessingBehaviour), typeof(Collider))]
public class WaterVFX : MonoBehaviour {

	public PostProcessingProfile profile;

	PostProcessingBehaviour cam;
//	Transform surface;

	void Start() {
		cam = GetComponent<PostProcessingBehaviour>();
		cam.profile = null;
//		surface = GetComponentInChildren<Transform>();
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log("Hit");
		if (other.CompareTag("Water")) {
			cam.profile = profile;
//			surface.rotation.eulerAngles= new Vector3(180,0,0);
			//other.GetComponentInChildren<Transform>().Rotate(180,0,0);
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.CompareTag("Water")) {
			cam.profile = null;
//			surface.rotation.eulerAngles = Vector3.zero;
//			surface.rotation.Set(Quaternion.Euler(Vector3.zero));
			//other.GetComponentInChildren<Transform>().Rotate(180,0,0);
		}
	}
}
