using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cinematic : MonoBehaviour {

	public Camera cam;
	public CameraMovement look;
	//public Collider trigger;

	public Movement featured; //enemy

	public float zoomTime = 1.5f;
	public float focusTime = 3f;
	private float timer = 0;

	private bool fired = false;

	public float focusDistance;
	private Vector3 focusOffset;
	private Vector3 focusOrientation;

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			StartCoroutine(StartFocus());	
		}
	}

	IEnumerator StartFocus() {
		timer = 0;
		fired = true;
		look.enabled=false;

		//get focus camera position offset + orientation
		focusOrientation = new Vector3(0f, -featured.transform.forward.y + 180f, 0f);
		focusOffset = new Vector3(
			featured.transform.position.x * focusDistance,
			featured.transform.position.y,
			featured.transform.position.z * focusDistance);

		//move to pos + orientation
		while (timer < zoomTime) {
			timer += Time.deltaTime;
			cam.transform.position = Vector3.Slerp(
				cam.transform.position, 
				featured.transform.position+focusOffset, 
				Time.deltaTime/zoomTime);
			cam.transform.eulerAngles = Vector3.Slerp(
				cam.transform.eulerAngles,
				focusOrientation,
				Time.deltaTime/zoomTime);
			yield return null;
		}
		StartCoroutine(Focus());
	}

	IEnumerator Focus() {
		yield return new WaitForSeconds(focusTime);
	}

	IEnumerator Return() {
		timer=0;

		yield return new WaitForSeconds(zoomTime); //todo
	}

}
