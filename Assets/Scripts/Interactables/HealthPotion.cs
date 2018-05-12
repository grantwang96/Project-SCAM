using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour {

    [SerializeField] private int strength;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float bobSpeed;
    [SerializeField] private float heightOffset;
    [SerializeField] private GameObject deathFX;

    float originY;
    private bool dead = false;

	// Use this for initialization
	void Start () {
        originY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.position;
        pos.y = originY + Mathf.Sin(Time.time * bobSpeed) * heightOffset;
        transform.position = pos;
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
	}

    void OnTriggerEnter(Collider coll)
    {
        if(dead) { return; }
        if(coll.tag == "Player") {
            Damageable dam = coll.GetComponent<Damageable>();
            if(dam.health == dam.max_health) { return; }
            dam.Heal(strength);
            Die();
        }
    }

    void Die()
    {
        dead = true;
        GameObject newDeathFX = Instantiate(deathFX, transform.position, transform.rotation);
        Destroy(newDeathFX, 1f);
        Destroy(gameObject);
    }
}
