using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWellVortex : MonoBehaviour {

    public float force;
    public float lifeTime;
    public float range;
    public float rangeIncreaseFactor;
    public float maxRange;
    private const float gravitationalConstant = 6.672e-11f;

    public float speed;
    public float initialEmissionRate;
    float currEmission;
    public float maxEmissionRate;

    public ParticleSystem effects;
    public SphereCollider rangeFinder;
    public Vector3 pointShift; // vector between edge before and after turn
    public float forceMod;
    public float heightForce;

    float startTime;
    public Transform explosionPrefab;

    public Transform myOwner;
    Rigidbody rbody;

	// Use this for initialization
	void Start () {
        rbody = GetComponent<Rigidbody>();
        rbody.isKinematic = true;
        range = 0.33f;
        rangeFinder = GetComponent<SphereCollider>();
        effects = GetComponent<ParticleSystem>();
        rangeFinder.radius = range;
        ParticleSystem.ShapeModule shapeModule = effects.shape;
        shapeModule.radius = range;
        effects.startSpeed = -range;
        ParticleSystem.EmissionModule emModule = effects.emission;
        maxEmissionRate = emModule.rate.constant;
        emModule.rateOverTime = initialEmissionRate;
        currEmission = initialEmissionRate;
        startTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        
        if (Time.time - startTime >= lifeTime) {
            Die();
        }
        if (range < maxRange) {
            range += Time.deltaTime * rangeIncreaseFactor;
            if(range > maxRange) {
                range = maxRange;
                startTime = Time.time;
            }
            rangeFinder.radius = range;
            ParticleSystem.ShapeModule shapeModule = effects.shape;
            shapeModule.radius = range;
            ParticleSystem.EmissionModule emModule = effects.emission;
            currEmission += Time.deltaTime * (range/maxRange * (maxEmissionRate - initialEmissionRate));
            emModule.rateOverTime = currEmission;
            effects.startSpeed = -range;
        }
	}

    void FixedUpdate()
    {
        Vector3 before = transform.position + transform.forward * range;
        rbody.MoveRotation(Quaternion.Euler(transform.eulerAngles + Vector3.up * speed * Time.deltaTime));
        Vector3 after = transform.position + transform.forward * range;
        pointShift = transform.InverseTransformDirection(after - before);
        /*
        foreach(Transform child in transform) {
            Damageable dam = child.GetComponent<Damageable>();
            Vector3 move = pointShift + (transform.position - child.position);
            if(dam) { dam.knockBack(move.normalized, force); }
            else if(child.GetComponent<Rigidbody>()) { child.GetComponent<Rigidbody>().AddForce(move.normalized * force); }
        }*/
    }

    void LateUpdate()
    {

    }
    /*
    void OnTriggerEnter(Collider coll)
    {
        Damageable dam = coll.GetComponent<Damageable>();
        if(dam) {
            Vector3 dir = (transform.position - coll.transform.position).normalized;
            dir += pointShift;
            dir.y = 2f;
            dam.knockBack(dir, force);
        }
        else if(coll.attachedRigidbody != null && !coll.attachedRigidbody.isKinematic) {
            Vector3 dir = (transform.position - coll.transform.position).normalized;
            dir += pointShift;
            dir.y = 2f;
            coll.attachedRigidbody.AddForce(dir * force, ForceMode.Impulse);
        }
    }*/

    void OnTriggerExit(Collider coll)
    {
        
    }
    
    void OnTriggerStay(Collider coll)
    {
        Damageable dam = coll.GetComponent<Damageable>();
        if(dam) {
            Vector3 dir = (transform.position - coll.transform.position).normalized;
            dir += pointShift * forceMod;
            dir.y = heightForce;
            dam.knockBack(dir, force);
        }
        else if(coll.attachedRigidbody != null && !coll.attachedRigidbody.isKinematic && coll.tag == "Furniture") {
            Vector3 dir = (transform.position - coll.transform.position).normalized;
            dir += pointShift * forceMod;
            dir.y = heightForce;
            coll.attachedRigidbody.AddForce(dir * force, ForceMode.Impulse);
        }
    }

    void Die()
    {
        // Small explosion to send all objects up
        Collider[] colls = Physics.OverlapSphere(transform.position, 3f);
        Transform newExp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        foreach(Collider coll in colls) {
            Damageable dam = coll.GetComponent<Damageable>();
            if(dam != null) {
                Vector3 dir = (coll.transform.position - transform.position).normalized;
                dam.knockBack(dir, force);
            }
        }
        Destroy(newExp.gameObject, 3f);
        /*
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform) {
            Debug.Log(child.name);
            children.Add(child);
        }
        for (int i = 0; i < children.Count; i++) { children[i].SetParent(null); }

        Debug.Log(transform.childCount);*/
        Destroy(gameObject);
    }

    public Vector3 GAcceleration(Vector3 position, float mass, Rigidbody r)
    {
        Vector3 direction = position - r.position;

        float gravityForce = gravitationalConstant * ((mass * r.mass) / direction.sqrMagnitude);
        gravityForce /= r.mass;

        return direction.normalized * gravityForce * Time.fixedDeltaTime;
    }
}
