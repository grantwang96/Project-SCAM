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
    public int playerHealth;
	public PlayerDamageable playerDmg;
	public PlayerMagic playerMagic;
    public SpellBook spellBookPrefab;

	List<SpellBookData> savedSpellInv = new List<SpellBookData>();

	//events
	public delegate void ResetAction();
	public static event ResetAction OnReset;

	public delegate void CheckpointAction();
	public static event CheckpointAction OnCheckpoint;

	//enemies to respawn on checkpoint
	List<EnemyRestart> respawnList = new List<EnemyRestart>();

	void Awake () {
		if (instance == null) {
			instance = this;
		}
		else {
			throw new UnityException("Error: trying to initialize a CheckpointManager while one exists already");
		}

		respawnList = new List<EnemyRestart>();

		SaveCheckpoint();
	}

	public void SaveCheckpoint() {
		respawnList.Clear();

        playerHealth = playerDmg.health;
        savedSpellInv.Clear();
        for (int i = 0; i < playerMagic.GetSpellsInventory().Count; i++) {
            SpellBookData bookData = new SpellBookData();
            bookData.primary = playerMagic.GetSpellsInventory()[i].primaryEffect;
            bookData.secondary = playerMagic.GetSpellsInventory()[i].secondaryEffect;
            bookData.ammo = playerMagic.GetSpellsInventory()[i].getAmmo();
            savedSpellInv.Add(bookData);
        }

        savedPos = playerDmg.transform.position;
        savedRot = playerDmg.transform.rotation;

        if (OnCheckpoint != null) {
			OnCheckpoint();
		}
	}

	public void ResetToLastCheckpoint() {
        Debug.Log("Resetting to last checkpoint...");
		//reset enemies
		foreach(EnemyRestart enemy in respawnList) {
            enemy.dam.transform.position = enemy.dam.originSpawn;
            enemy.dam.transform.rotation = enemy.originalRot;
            enemy.dam.gameObject.SetActive(true);
            enemy.dam.health = enemy.dam.max_health;
            enemy.dam.dead = false;
            enemy.dam.damageable = true;
            if(enemy.dam.myMovement != null) {
                enemy.dam.myMovement.agent.Warp(enemy.dam.originSpawn);
            }
		}
		respawnList.Clear();

        // JsonUtility.FromJsonOverwrite(savedDmg, playerDmg);

        // JsonUtility.FromJsonOverwrite(savedMagic, playerMagic);
        // playerMagic.ResetSpellsToSerialized(savedSpellInv);
        
        for(int i = 0; i < savedSpellInv.Count; i++) {
            if(i >= playerMagic.GetSpellsInventory().Count) {
                SpellBook newBook = Instantiate(spellBookPrefab);
                playerMagic.pickUpSpell(newBook);
            }
            SpellBook currBook = playerMagic.GetSpellsInventory()[i];
            currBook.primaryEffect = savedSpellInv[i].primary;
            currBook.secondaryEffect = savedSpellInv[i].secondary;
            currBook.active = false; // all these books will be held by player
            currBook.SetupSpell();
            currBook.setAmmo(savedSpellInv[i].ammo);
        }
        for(int i = 0; i < playerMagic.GetSpellsInventory().Count; i++) {
            playerMagic.GetSpellsInventory()[i].Deactivate();
        }

		playerMagic.UpdateUI();

		player.transform.position = savedPos;
		player.transform.rotation = savedRot;
        playerDmg.health = playerHealth;
        player.Find("PlayerHead").localEulerAngles = Vector3.zero;
        player.gameObject.GetComponent<CharacterController>().Move(Vector3.zero);

		if (OnReset != null) {
			OnReset();
		}
	}

	public void AddEnemyToRespawnList(Damageable enemy) {
        EnemyRestart newER = new EnemyRestart();
        newER.dam = enemy;
        newER.health = enemy.health;
        newER.originalPos = enemy.transform.position;
        newER.originalRot = enemy.transform.rotation;
		respawnList.Add(newER);
	}

    public class SpellBookData
    {
        public SpellPrimary primary;
        public SpellSecondary secondary;
        public int ammo;
    }

    public class EnemyRestart
    {
        public Damageable dam;
        public int health;
        public Vector3 originalPos;
        public Quaternion originalRot;
    }
}
