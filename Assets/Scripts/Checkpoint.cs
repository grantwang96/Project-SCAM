using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			CheckpointManager.Instance.SaveCheckpoint();
			gameObject.SetActive(false);
		}
	}

}
