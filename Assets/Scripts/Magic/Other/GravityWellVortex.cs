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

    float startTime;
    List<Transform> trapped = new List<Transform>();
    List<trappedIdiot> idiots = new List<trappedIdiot>();
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
        
        foreach(Transform child in transform) {
            Damageable dam = child.GetComponent<Damageable>();
            Vector3 move = pointShift + (transform.position - child.position);
            if(dam) { dam.knockBack(move.normalized, force); }
            else if(child.GetComponent<Rigidbody>()) { child.GetComponent<Rigidbody>().AddForce(move.normalized * force); }
        }
    }

    void LateUpdate()
    {

    }

    class trappedIdiot
    {
        public Transform loser;
        public Transform tracker;
    }

    void OnTriggerEnter(Collider coll)
    {
        /*
        Vector3 centerForce = (transform.position - coll.transform.position).normalized;
        Vector3 combinedForce = centerForce + coll.transform.TransformDirection(pointShift);*/
        if (coll.transform.parent != transform) {
            if(coll.attachedRigidbody != null && !coll.attachedRigidbody.isKinematic) { coll.transform.parent = transform; }
            else if(coll.GetComponent<Damageable>() != null) {
                if(coll.attachedRigidbody != null) {
                    if(!coll.attachedRigidbody.isKinematic) { coll.transform.parent = transform; }
                }
                else { coll.transform.parent = transform; }
            }
            // trappedIdiot newIdiot = new trappedIdiot();
            // newIdiot.loser = coll.transform;
            // GameObject newTracker = Instantiate(new GameObject(), newIdiot.loser.position, newIdiot.loser.rotation);
            // newTracker.transform.parent = transform;
            // newIdiot.tracker = newTracker.transform;
            // idiots.Add(newIdiot);
            // coll.transform.parent = transform;
            // trapped.Add(coll.transform);
        }

    }

    void OnTriggerExit(Collider coll)
    {
        /*
        for(int i = 0; i < idiots.Count; i++)
        {
            if(idiots[i].loser == coll.transform)
            {
                Transform tracker = idiots[i].tracker;
                Destroy(tracker.gameObject);
                idiots.Remove(idiots[i]);
                break;
            }
        }*/
        coll.transform.SetParent(null);
    }
    /*
    void OnTriggerStay(Collider coll)
    {
        Damageable colldam = coll.GetComponent<Damageable>();
        Vector3 dir = (transform.position - coll.transform.position).normalized;
        dir += pointShift.normalized * force;
        if (colldam != null) {
            colldam.knockBack(dir, force);
        }
        else if(coll.attachedRigidbody != null) {
            coll.attachedRigidbody.AddForce(dir * force);
        }
    }
    */
    void Die()
    {
        /*
        foreach(Transform loser in trapped) {
            if(loser != null) {
                loser.parent = null;
            }
        }
        
        // trapped.Clear();
        if(idiots.Count > 0) {
            foreach (trappedIdiot idiot in idiots) {
                Transform tracker = idiot.tracker;
                Destroy(tracker.gameObject);
            }
            idiots.Clear();
        }*/


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

        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform) {
            Debug.Log(child.name);
            children.Add(child);
        }
        for (int i = 0; i < children.Count; i++) { children[i].SetParent(null); }

        Debug.Log(transform.childCount);
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
