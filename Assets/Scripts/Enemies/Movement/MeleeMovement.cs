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
            if (agent.nextPosition != transform.position && agent.Warp(transform.position)) {
                agent.updatePosition = true;
                agent.updateRotation = true;
                agent.isStopped = false;
            }
        }

        base.Update();
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
    
    void OnCollisionEnter(Collision coll)
    {
        if(coll.transform == attackTarget) { changeState(new MeleeEnemyAggro()); }
    }
}
