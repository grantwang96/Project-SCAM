using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrb : MonoBehaviour {

    public int health;
    public float speed;

	// Use this for initialization
	void Start () {
        health = Random.Range(5, 10);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == "Player") {
            // play pickup orb sfx
            Collider[] colls = GetComponents<Collider>();
            foreach(Collider c in colls) { c.enabled = false; }
            Damageable player = coll.GetComponent<Damageable>();
            StartCoroutine(approach(player));
        }
    }

    IEnumerator approach(Damageable player)
    {
        float time = 0f;
        while(time < 1f) {
            transform.position = Vector3.Lerp(transform.position, player.transform.position, time);
            time += Time.deltaTime * speed;
            yield return new WaitForEndOfFrame();
        }
        player.health += health;

        // healing sfx
        // healing fx

        Destroy(this.gameObject);
    }
}
