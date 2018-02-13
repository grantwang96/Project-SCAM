using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistScript : MonoBehaviour {

    public int damage;
    public float force;
    public Transform myBody;
    public Movement myMovement;

	// Use this for initialization
	void Start () {
        myMovement = transform.root.GetComponent<Movement>();
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
        Damageable dam = coll.GetComponent<Damageable>();
        if(myBody == null) { return; }
        if (dam != null && dam != myBody.GetComponent<Damageable>()) {
            Vector3 dir = (coll.transform.position - myBody.position).normalized;
            dam.TakeDamage(myBody, damage, dir, force);
            // Debug.Log("Hit");
        }
    }
}
