using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalloutOfWorld : MonoBehaviour {

	void OnTriggerExit(Collider coll)
    {
        Damageable collDam = coll.GetComponent<Damageable>();
        if(collDam != null) {
            collDam.Die();
        }
        else {
            Destroy(coll.gameObject);
        }
    }
}
