using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSubMissile : MonoBehaviour {

    public IcySurface icePrefab;
    public LayerMask frostable;

    ParticleSystem partSys;

    public int minIcePrefabs;
    public int maxIcePrefabs;
    public float radius;

    [Range(0, 60)]
    public float minIceDuration;
    [Range(0, 60)]
    public float maxIceDuration;

    void Start() {
        partSys = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update () {
        if(partSys.isStopped) {
            Destroy(gameObject);
        }
	}

    void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        partSys.GetCollisionEvents(other, collisionEvents);
        foreach(ParticleCollisionEvent coll in collisionEvents) {
            Instantiate(icePrefab, coll.intersection, Quaternion.FromToRotation(Vector3.forward, coll.normal));
        }
    }
    
    /*
    void OnCollisionEnter(Collision coll)
    {
        int rand = Random.Range(minIcePrefabs, maxIcePrefabs); // attempt to create this many icy surfaces
        Vector3 center = coll.contacts[0].point - transform.position;
        float interval = 360f / rand;

        
        GameObject fuckboi = new GameObject();
        fuckboi.transform.position = coll.contacts[0].point;
        fuckboi.transform.rotation = Quaternion.FromToRotation(Vector3.forward, coll.contacts[0].normal);
        
        for (int i = 0; i < rand; i++)
        {
            float randrange = Random.Range(1f, 2f);
            float ang = interval * i;
            Vector3 offset;
            offset.x = randrange * radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            offset.y = 0;
            offset.z = randrange * radius * Mathf.Sin(ang * Mathf.Deg2Rad);

            Vector3 worldOff = fuckboi.transform.TransformVector(offset);

            Ray ray = new Ray(transform.position, worldOff - transform.position);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit, radius, frostable, QueryTriggerInteraction.Ignore)) {
                IcySurface newIcySurface = Instantiate(icePrefab, rayHit.point, Quaternion.FromToRotation(Vector3.forward, rayHit.normal));
                newIcySurface.lifeTime = Random.Range(minIceDuration, maxIceDuration);
                float size = 0.5f + Random.value * 5f;
                newIcySurface.targetScale = new Vector3(size, size, 1f);
            }
        }

        // Destroy(fuckboi);
        Destroy(gameObject);
    }
    */
}
