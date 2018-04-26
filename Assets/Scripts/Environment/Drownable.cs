using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drownable : MonoBehaviour {
	//put this class on the "head" of a drownable enemy

	Damageable dmg;

	public int dmgTickAmt = 10;

	// Use this for initialization
	void Start () {
		dmg = GetComponentInParent<Damageable>();
	}
	
	public void DealDrownDamage() {
		dmg.health -= dmgTickAmt;
	}
}
