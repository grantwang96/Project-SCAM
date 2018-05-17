using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {

    [SerializeField] Door[] doors;
    public GameObject keyPickUp;

    void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == "Player") {
            foreach(Door door in doors) { door.Unlock(); door.Open(); }
            Destroy(gameObject);
        }
    }

    void OnDestroy() {
        Destroy(Instantiate(keyPickUp, transform.position, transform.rotation), 1.25f);
    }
}
