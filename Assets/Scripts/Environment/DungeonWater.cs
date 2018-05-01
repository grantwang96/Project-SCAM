using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
public class DungeonWater : MonoBehaviour {

//    public int damage;

	BoxCollider col;

	// Use this for initialization
	void Start () {
		col = GetComponent<BoxCollider>();
		col.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay(Collider other) {
		Damageable dmg = other.GetComponent<Damageable>();
		if (dmg != null) {
			AudioSource source = other.GetComponent<AudioSource>();
//			source.
			dmg.Die();
		}
    }
}
