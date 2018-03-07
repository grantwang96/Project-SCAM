using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarOfDoom : MonoBehaviour {
    
    public int damage; // over time
    public float radius;
    public float initialExplosionRadMod;
    public float force = 10f;
    float startTime;
    float duration;

    ParticleSystem partSys;
    public float shakeForce;

    public Transform myCaster;
    public float partSpeed;

	// Use this for initialization
	void Start () {
        partSys = GetComponent<ParticleSystem>();
        duration = partSys.main.startLifetime.constant;
        // radius = partSys.shape.radius;
        var main = partSys.main;
        partSpeed = main.startSpeed.constant;
        
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, initialExplosionRadMod * radius, Vector3.up, 0f);
        foreach (RaycastHit hit in rayHits) {
            Damageable dam = hit.collider.GetComponent<Damageable>();
            if (dam != null) {
                Vector3 dir = (hit.collider.transform.position - transform.position).normalized;
                dam.TakeDamage(myCaster, damage * 2, dir, force);
            }
            else {
                Rigidbody rbody = hit.collider.attachedRigidbody;
                if (rbody != null) { rbody.AddExplosionForce(force * 10, transform.position, radius); }
            }
        }

        StartCoroutine(burn());
        StartCoroutine(CameraMovement.Instance.shakeCamera(shakeForce));
	}

    IEnumerator burn()
    {
        startTime = Time.time;
        while(Time.time - startTime < duration) {
            float dist = partSpeed * (Time.time - startTime);
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, radius, Vector3.up, dist);
            foreach(RaycastHit hit in rayHits) {
                Damageable dam = hit.collider.GetComponent<Damageable>();
                if(dam != null) {
                    Vector3 dir = (hit.collider.transform.position - transform.position).normalized;
                    dam.TakeDamage(myCaster, damage, dir, force);
                    Debug.Log("Hit " + hit.collider.name);
                }
                else {
                    Rigidbody rbody = hit.collider.attachedRigidbody;
                    if(rbody != null) { rbody.AddExplosionForce(force, transform.position, radius); }
                }
            }
            yield return new WaitForEndOfFrame();
        }
        if (partSys.isPlaying) {
            partSys.Stop();
        }
        Destroy(gameObject, duration);
    }
}
