using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeKinematicRBody : MonoBehaviour {

    public Rigidbody affectedRbody;

    void OnTriggerEnter(Collider coll) {
        if(coll.tag == "Player") {
            affectedRbody.isKinematic = false;
            Destroy(this.gameObject);
        }
    }
}
