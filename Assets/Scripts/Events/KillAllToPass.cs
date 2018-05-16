using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAllToPass : MonoBehaviour {

    public List<Damageable> enemiesToKill = new List<Damageable>();
    public GameObject key;

	// Update is called once per frame
	void Update () {
        bool alldead = true;
        for(int i = 0; i < enemiesToKill.Count; i++) {
            if(!enemiesToKill[i].dead) { alldead = false; return; }
        }
        if(alldead && key != null) {
            Debug.Log("Dropping key...");
            key.SetActive(true);
            key.GetComponent<FloatyRotaty>().SetPosition();
            Destroy(this);
        }
	}
}
