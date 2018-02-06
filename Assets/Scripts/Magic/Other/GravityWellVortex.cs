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

	// Use this for initialization
	void Start () {
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
        
        Vector3 before = transform.position + transform.forward * range;
        transform.Rotate(0, speed * Time.deltaTime, 0);
        Vector3 after = transform.position + transform.forward * range;
        pointShift = transform.InverseTransformDirection(after - before);
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

        if(idiots.Count > 0)
        {
            foreach (trappedIdiot idiot in idiots)
            {
                if (idiot == null) { continue; }
                if (idiot.loser == null) { continue; }
                Damageable dam = idiot.loser.GetComponent<Damageable>();
                if (dam) {
                    dam.vortexGrab(transform, force);
                    if(idiot.tracker != null) {
                        Vector3 moveDir = (idiot.tracker.position - idiot.loser.position).normalized;
                        dam.myMovement.Move(idiot.tracker.position - idiot.loser.position);
                    }
                }
                else if (idiot.loser.GetComponent<Rigidbody>() != null) {
                    idiot.loser.GetComponent<Rigidbody>().AddForce((transform.position - idiot.loser.position).normalized * force);
                }
            }
        }
	}

    void FixedUpdate()
    {
        Vector3 before = transform.position + transform.forward * range;
        transform.Rotate(0, speed * Time.deltaTime, 0);
        Vector3 after = transform.position + transform.forward * range;
        pointShift = transform.InverseTransformDirection(after - before);
        
    }

    class trappedIdiot
    {
        public Transform loser;
        public Transform tracker;
    }

    void OnTriggerEnter(Collider coll)
    {
        Vector3 centerForce = (transform.position - coll.transform.position).normalized;
        Vector3 combinedForce = centerForce + coll.transform.TransformDirection(pointShift);
        if (coll.GetComponent<Damageable>() != null || coll.GetComponent<Rigidbody>() != null)
        {
            trappedIdiot newIdiot = new trappedIdiot();
            newIdiot.loser = coll.transform;
            GameObject newTracker = Instantiate(new GameObject(), newIdiot.loser.position, newIdiot.loser.rotation);
            newTracker.transform.parent = transform;
            newIdiot.tracker = newTracker.transform;
            idiots.Add(newIdiot);
            // coll.transform.parent = transform;
            // trapped.Add(coll.transform);
        }
    }

    void OnTriggerExit(Collider coll)
    {
        for(int i = 0; i < idiots.Count; i++)
        {
            if(idiots[i].loser == coll.transform)
            {
                Transform tracker = idiots[i].tracker;
                Destroy(tracker.gameObject);
                idiots.Remove(idiots[i]);
                break;
            }
        }
    }

    void OnTriggerStay(Collider coll)
    {
        Damageable colldam = coll.GetComponent<Damageable>();
        if(colldam != null) {
            
        }
    }

    void Die()
    {
        /*
        foreach(Transform loser in trapped) {
            if(loser != null) {
                loser.parent = null;
            }
        }
        */
        // trapped.Clear();
        if(idiots.Count > 0) {
            foreach (trappedIdiot idiot in idiots) {
                Transform tracker = idiot.tracker;
                Destroy(tracker.gameObject);
            }
            idiots.Clear();
        }
        // Small explosion to send all objects up
        Collider[] colls = Physics.OverlapSphere(transform.position, 3f);
        Transform newExp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        foreach(Collider coll in colls)
        {
            Damageable dam = coll.GetComponent<Damageable>();
            if(dam != null)
            {
                Vector3 dir = (coll.transform.position - transform.position).normalized;
                dam.knockBack(dir, force);
            }
        }
        Destroy(newExp.gameObject, 3f);
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
