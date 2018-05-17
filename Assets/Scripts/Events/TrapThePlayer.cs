using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrapThePlayer : MonoBehaviour {

    public Door[] doors;
    public UnityEvent AndDoWhat;

    public AudioClip surpriseSFX;
    AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = surpriseSFX;
        // audioSource.Play();
		foreach(Door door in doors) { door.damageable = false; } // do not let them be broken
	}

    void OnTriggerEnter(Collider coll) {
        if(coll.tag == "Player") {
            LockAllDoors();
            gameObject.SetActive(false);
        }
    }

    private void LockAllDoors() {
        foreach(Door door in doors) { door.Lock(); }
        AndDoWhat.Invoke();
    }
}
