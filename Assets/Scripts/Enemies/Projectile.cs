using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public Rigidbody rbody;
    public int damage;

    public Transform owner;
    public GameObject explosionPrefab;

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
            Vector3 vel = rbody.velocity;
            vel.y = 1f;
            dam.TakeDamage(owner, damage, vel, rbody.mass);
        }
        Die();
    }

    void Die() {
        Destroy(Instantiate(explosionPrefab, transform.position, transform.rotation), 1f);
        Destroy(this.gameObject);
    }
}
