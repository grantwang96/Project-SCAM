﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public Rigidbody rbody;
    public int damage;

    public Transform owner;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider coll) {
        if(rbody.isKinematic || coll.transform == owner) { return; }
        Damageable dam = coll.GetComponent<Damageable>();
        if(dam) {
            dam.TakeDamage(owner, damage, transform.forward, rbody.velocity.magnitude / 2 * rbody.mass);
        }
        Die();
    }

    void Die() {
        Destroy(this.gameObject);
    }
}
