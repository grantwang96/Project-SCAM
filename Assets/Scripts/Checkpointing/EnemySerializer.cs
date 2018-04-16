using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySerializer : MonoBehaviour {
	//stores:
	//position
	//rotation
	//Damageable

	Vector3 savedPosition;
	Quaternion savedRotation;

	Damageable damageable;
	string savedDamageable;

	Movement movement;
	string savedMovement;

	void OnEnable() {
		CheckpointManager.OnCheckpoint += Serialize;
		CheckpointManager.OnReset += Deserialize;
	}

	void OnDisable() {
		CheckpointManager.OnCheckpoint -= Serialize;
		CheckpointManager.OnReset -= Deserialize;
	}

	void Awake () {
		damageable = GetComponent<Damageable>();
		movement = GetComponent<Movement>();

		Serialize();
	}

	void Serialize() {
		savedPosition = transform.position;
		savedRotation = transform.rotation;

		savedDamageable = JsonUtility.ToJson(damageable);
		savedMovement = JsonUtility.ToJson(movement);
	}

	void Deserialize() {
		transform.SetPositionAndRotation(savedPosition, savedRotation);
		JsonUtility.FromJsonOverwrite(savedDamageable, damageable);
		JsonUtility.FromJsonOverwrite(savedMovement, movement);
	}

}
