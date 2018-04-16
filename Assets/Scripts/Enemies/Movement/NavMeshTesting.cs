using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTesting : Movement {
    
    public Transform target;
    public Vector3 targetPoint;

    // [SerializeField] float minWanderDistance;
    // [SerializeField] float maxWanderDistance;

    // Use this for initialization
    public override void Start()
    {
        // base.Start();
        agent = GetComponent<NavMeshAgent>();
        targetPoint = getRandomLocation(transform.position, maxWanderDistance);
        agent.SetDestination(targetPoint);
    }

    // Update is called once per frame
    public override void Update() {
        
    }

	protected override void ToIdle() {
		//left empty because idk what this script is for
	}

    /*
    public Vector3 getRandomLocation(Vector3 origin, float range) {
        Vector3 randPos = Random.insideUnitSphere * maxWanderDistance;
        randPos += transform.position;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randPos, out navHit, range, pathFindingLayers);

        return navHit.position;
    }*/

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
}
