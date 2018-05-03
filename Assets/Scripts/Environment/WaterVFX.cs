using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.PostProcessing;

[RequireComponent(typeof(PostProcessingBehaviour))]
public class WaterVFX : MonoBehaviour {

	public PostProcessingProfile profile;

	float aperture = 7f;

	PostProcessingBehaviour[] cams;
//	Transform surface;

	void Start() {
//		cam = GetComponent<PostProcessingBehaviour>();
		cams = GetComponentsInChildren<PostProcessingBehaviour>();
		foreach (PostProcessingBehaviour cam in cams) {
			Debug.Log(cam.name);
		}
		SwitchToProfile(null);
//		surface = GetComponentInChildren<Transform>();
	}

	public void Enter() {
		Debug.Log("Hit");
		SwitchToProfile(profile);
		StartCoroutine(BlurIn());
//		surface.rotation.eulerAngles= new Vector3(180,0,0);
		//other.GetComponentInChildren<Transform>().Rotate(180,0,0);

	}

	public void Exit() {
		SwitchToProfile(null);
//		surface.rotation.eulerAngles = Vector3.zero;
//		surface.rotation.Set(Quaternion.Euler(Vector3.zero));
		//other.GetComponentInChildren<Transform>().Rotate(180,0,0);
	}

	void SwitchToProfile(PostProcessingProfile prof) {
		foreach (PostProcessingBehaviour cam in cams) {
			cam.profile = prof;
		}
	}

	IEnumerator BlurIn() {
		float currAperture = 3f;

		DepthOfFieldModel.Settings DOF = profile.depthOfField.settings;

		while (currAperture < aperture) {
			DOF.aperture = currAperture;
			profile.depthOfField.settings = DOF;

			currAperture += Time.deltaTime * 2f;

			yield return null;
		}
	}
}
