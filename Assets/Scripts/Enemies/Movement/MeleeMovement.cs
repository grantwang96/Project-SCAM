using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class MeleeMovement : Movement {

    public Vector3 destination;
    public bool isGrounded;

    public override void setup() {
        agent = GetComponent<NavMeshAgent>(); // set the agent
        base.setup();
        changeState(new MeleeEnemyIdle());
    }

	protected override void ToIdle() {
		changeState(new MeleeEnemyIdle());
	}

    public override void Update()
    {
        destination = agent.destination;

        RaycastHit rayHit;
        if (hamper <= 0 && Physics.Raycast(new Ray(transform.position + Vector3.up * 0.1f, Vector3.down),
            out rayHit, 0.2f, groundLayers, QueryTriggerInteraction.Ignore)) {
            NavMeshHit hit;
            agent.nextPosition = transform.position;
            if(agent.nextPosition != transform.position && NavMesh.SamplePosition(transform.position, out hit, agent.radius, NavMesh.AllAreas)) { // check if agent is synced and on/near the navmesh
                agent.Warp(hit.position);
                ReactivateNavMesh();
            }
        }
        else {
            rbody.isKinematic = false;
        }

        base.Update();
    }

    private void ReactivateNavMesh()
    {
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = false;
        rbody.isKinematic = true;
    }

    public override IEnumerator attack(Vector3 target)
    {
        anim.Play("Attack");
        return base.attack(target);
    }

    public override void knockBack(Vector3 dir, float force)
    {
        base.knockBack(dir, force);
    }

    public override void Teleport(Vector3 newLocation, Vector3 offset)
    {
        
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.transform == attackTarget) { changeState(new MeleeEnemyAggro()); }
    }
}
