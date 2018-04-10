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

    public Transform measuringStick;

    Vector3 before;
    Vector3 after;
    public Vector3 pointShift;

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
        range = 0.01f;
        rangeFinder = GetComponent<SphereCollider>();
        effects = GetComponent<ParticleSystem>();
        rangeFinder.radius = range;
        ParticleSystem.ShapeModule shapeModule = effects.shape;
        shapeModule.radius = range;
        effects.startSpeed = range;
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
            measuringStick.localPosition = Vector3.forward * range;
        }
	}

    void FixedUpdate()
    {
        before = measuringStick.transform.position;
        rbody.MoveRotation(Quaternion.Euler(transform.eulerAngles + Vector3.up * speed * Time.deltaTime));
    }

    void LateUpdate()
    {
        after = measuringStick.transform.position;
        pointShift = after - before;
    }
    
    void OnTriggerEnter(Collider coll)
    {

    }

    void OnTriggerExit(Collider coll)
    {

    }
    
    void OnTriggerStay(Collider coll)
    {
        float heightDiff = Mathf.Abs(coll.transform.position.y - transform.position.y);
        if (heightDiff > range / 2f) { return; }

        Damageable dam = coll.GetComponent<Damageable>();
        if(dam) {

            float dist = Vector3.Distance(coll.transform.position, transform.position);
            float angle = Vector3.SignedAngle(transform.forward, coll.transform.position - transform.position, Vector3.up);
            float angleInRadians = angle * Mathf.Deg2Rad;
            Vector3 beforePos = new Vector3(Mathf.Cos(angleInRadians), 0f, Mathf.Sin(angleInRadians)) * range;
            beforePos += transform.position;
            angle += speed * Time.deltaTime;
            Vector3 afterPos = new Vector3(Mathf.Cos(angleInRadians), 0f, Mathf.Sin(angleInRadians)) * range;
            afterPos += transform.position;
            Vector3 dir = (afterPos - beforePos).normalized;

            dir += (transform.position - coll.transform.position).normalized * forceMod * range / dist;
            dir.y = heightForce;

            dam.knockBack(dir, force);
        }
        else if(coll.attachedRigidbody != null && !coll.attachedRigidbody.isKinematic) {
            Vector3 dir = (transform.position - coll.transform.position).normalized;
            dir += pointShift * forceMod;
            dir.y = heightForce;
            coll.attachedRigidbody.AddForce(dir * force);
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
