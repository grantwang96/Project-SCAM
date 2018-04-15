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

		Serialize();
	}

	void Serialize() {
		savedPosition = transform.position;
		savedRotation = transform.rotation;

		savedDamageable = JsonUtility.ToJson(damageable);
	}

	void Deserialize() {
		transform.SetPositionAndRotation(savedPosition, savedRotation);
		JsonUtility.FromJsonOverwrite(savedDamageable, damageable);
	}

}
