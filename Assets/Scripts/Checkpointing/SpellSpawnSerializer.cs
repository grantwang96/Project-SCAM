using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSpawnSerializer : MonoBehaviour {

	SpellSpawn spellSpawn;
	string savedSpellSpawn;

	void Awake() {
		spellSpawn = GetComponent<SpellSpawn>();

		Serialize();
	}

	void OnEnable() {
		CheckpointManager.OnCheckpoint += Serialize;
		CheckpointManager.OnReset += Deserialize;
	}

	void OnDisable() {
		CheckpointManager.OnCheckpoint -= Serialize;
		CheckpointManager.OnReset -= Deserialize;
	}

	void Serialize() {
		savedSpellSpawn = JsonUtility.ToJson(spellSpawn);
	}

	void Deserialize() {
		JsonUtility.FromJsonOverwrite(savedSpellSpawn, spellSpawn);
		spellSpawn.currBook = null; //forces a respawn
	}
}
