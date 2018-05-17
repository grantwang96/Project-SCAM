using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drownable : MonoBehaviour {
	//put this class on the "head" of a drownable enemy

	public Damageable dmg;

	public int dmgTickAmt = 10;

	// Use this for initialization
	void Start () {
//		dmg = GetComponent<Damageable>();
		if (dmg == null) {
			throw new UnityException("no damageable attached!");
		}
		else {
			// Debug.Log("we have a damageable");
		}
	}
	
	public void DealDrownDamage() {
        // dmg.health -= dmgTickAmt;
        dmg.TakeDamage(null, dmgTickAmt, Vector3.zero, 0f);
	}
}
