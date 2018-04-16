using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour {
	//triggers checkpoint serialization/deserialization and handles those events for player

	//singleton
	static CheckpointManager instance;
	public static CheckpointManager Instance {
		get {
			return instance;
		}
	}


	string savedDmg = null;
	string savedMagic = null;
	Vector3 savedPos;
	Quaternion savedRot;

	public Transform player;
	public PlayerDamageable playerDmg;
	public PlayerMagic playerMagic;

	List<string> savedSpellInv;

	//events
	public delegate void ResetAction();
	public static event ResetAction OnReset;

	public delegate void CheckpointAction();
	public static event CheckpointAction OnCheckpoint;

	//enemies to respawn on checkpoint
	List<GameObject> respawnList;

	void Awake () {
		if (instance == null) {
			instance = this;
		}
		else {
			throw new UnityException("Error: trying to initialize a CheckpointManager while one exists already");
		}

		respawnList = new List<GameObject>();

		SaveCheckpoint();
	}

	public void SaveCheckpoint() {
		respawnList.Clear();

		savedDmg = JsonUtility.ToJson(playerDmg);

		savedMagic = JsonUtility.ToJson(playerMagic);
		List<SpellBook> currSpells = playerMagic.GetSpellsInventory();
		List<string> spellsToSave = new List<string>();
		foreach(SpellBook spell in currSpells) {
			spellsToSave.Add(JsonUtility.ToJson(spell));
		}
		savedSpellInv = spellsToSave;

		savedPos = player.position;
		savedRot = player.rotation;

		if (OnCheckpoint != null) {
			OnCheckpoint();
		}
	}

	public void ResetToLastCheckpoint() {
		//reset enemies
		foreach(GameObject enemy in respawnList) {
			enemy.SetActive(true);
		}
		respawnList.Clear();

		JsonUtility.FromJsonOverwrite(savedDmg, playerDmg);

		JsonUtility.FromJsonOverwrite(savedMagic, playerMagic);
		playerMagic.ResetSpellsToSerialized(savedSpellInv);
		playerMagic.UpdateUI();

		player.transform.position = savedPos;
		player.transform.rotation = savedRot;
		player.gameObject.GetComponent<CharacterController>().Move(Vector3.zero);

		if (OnReset != null) {
			OnReset();
		}
	}

	public void AddEnemyToRespawnList(GameObject enemy) {
		respawnList.Add(enemy);
	}
}
