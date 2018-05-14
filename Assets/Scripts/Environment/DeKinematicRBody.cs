using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeKinematicRBody : MonoBehaviour {

    public Rigidbody affectedRbody;

    void OnTriggerEnter(Collider coll) {
        Debug.Log("Who touched me?");
        if(coll.tag == "Player") {
            Debug.Log("Dropping the thing!");
            affectedRbody.isKinematic = false;
            Destroy(this.gameObject);
        }
    }
}
