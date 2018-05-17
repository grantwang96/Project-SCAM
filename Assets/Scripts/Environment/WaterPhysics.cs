using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Collider))]
public class WaterPhysics : MonoBehaviour {

	public float rho = 1;//density of fluid

	float origSlownessSeverity;
	float currSlownessSeverity = 0f;
	PlayerMovementV2 move;

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			move = other.GetComponent<PlayerMovementV2>();
			origSlownessSeverity=move.slownessSeverity;
			currSlownessSeverity = 0f;
		}
	}

//	float easeWater(float curr) {
//		
//	}

	//Buoyant force = rho * g * V(displaced)
	void OnTriggerStay(Collider other) {
//		Debug.Log(other.name + " in water!");
		if (other.Equals(move)) {
//			move.slownessSeverity = 

//			Debug.Log("Pushing");
//
//			other.attachedRigidbody.AddForce(rho * Physics.gravity);
		}
	}
}
