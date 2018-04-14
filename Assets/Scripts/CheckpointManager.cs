using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour {

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

	public delegate void ResetAction();
	public static event ResetAction OnReset;

	// Use this for initialization
	void Start () {
		if (instance == null) {
			instance = this;
		}
		else {
			throw new UnityException("Error: trying to initialize a CheckpointManager while one exists already");
		}

		SaveCheckpoint();
	}

	public void SaveCheckpoint() {
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
	}

	public void ResetToLastCheckpoint() {
		Debug.Log("Resetting...");
		Debug.Log(savedMagic);
		JsonUtility.FromJsonOverwrite(savedDmg, playerDmg);
		JsonUtility.FromJsonOverwrite(savedMagic, playerMagic);
		playerMagic.ResetSpellsToSerialized(savedSpellInv);
		Debug.Log(playerMagic);
		player.transform.position = savedPos;
		player.transform.rotation = savedRot;
		if (OnReset != null) {
			OnReset();
		}
	}
}
