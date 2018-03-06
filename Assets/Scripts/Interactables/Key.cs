using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {

    [SerializeField] Door[] doors;

    void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == "Player") {
            foreach(Door door in doors) { door.Unlock(); }
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        
    }
}
