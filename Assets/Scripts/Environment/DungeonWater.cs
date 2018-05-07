using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
public class DungeonWater : MonoBehaviour {

//    public int damage;

	BoxCollider col;
    public bool isKillZone;

	// Use this for initialization
	void Start () {
		col = GetComponent<BoxCollider>();
		col.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay(Collider other) {
        if(isKillZone) {
            Damageable dmg = other.GetComponent<Damageable>();
            if (dmg != null && dmg != PlayerDamageable.Instance) {
                // AudioSource source = other.GetComponent<AudioSource>();
                //		
                dmg.TakeDamage(null, 9999, Vector3.zero, 0f);
            }
        }
    }
}
