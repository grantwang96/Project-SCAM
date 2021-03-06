﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistScript : MonoBehaviour {

    public int damage;
    public float force;
    public float upwardForce;
    public Transform myBody;
    public Movement myMovement;

	// Use this for initialization
	void Start () {
        if(myMovement != null) {
            EnemyData blueprint = myMovement.blueprint;
            damage = blueprint.damage;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider coll)
    {
        if(myBody == null) { return; }
        if (myMovement != null && myMovement.attackRoutine != null) {
            Damageable dam = coll.GetComponent<Damageable>();
            if(dam == null) { return; }
            Vector3 dir = (coll.transform.position - myBody.position).normalized;
            dir.y = upwardForce;
            dam.TakeDamage(myBody, myMovement.damage, dir, force);
        }
    }
}
