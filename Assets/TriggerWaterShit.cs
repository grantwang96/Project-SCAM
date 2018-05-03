using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWaterShit : MonoBehaviour {

	public Transform playerHead;

	public WaterVFX vfx;

	int waterObjectsIn = 0; //since waters can overlap

	// Use this for initialization
	void Start () {
		transform.SetPositionAndRotation(playerHead.position, playerHead.rotation);
	}
	
	// Update is called once per frame
	void Update () {
		transform.SetPositionAndRotation(playerHead.position, playerHead.rotation);
	}

	void OnTriggerEnter(Collider other) {
		if (waterObjectsIn == 0 && other.CompareTag("Water")) {
			waterObjectsIn++;
			vfx.Enter();
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.CompareTag("Water")) {
			waterObjectsIn--;
			if (waterObjectsIn == 0){
				vfx.Exit();
			}
		}
	}
}
