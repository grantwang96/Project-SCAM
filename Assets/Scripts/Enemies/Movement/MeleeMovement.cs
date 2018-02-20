using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeMovement : Movement {

    public Vector3 destination;

    public override void setup() {
        agent = GetComponent<NavMeshAgent>(); // set the agent
        base.setup();
    }

    public override void Update()
    {
        destination = agent.destination;
        base.Update();
    }

    public override IEnumerator attack(Vector3 target)
    {
        anim.Play("Attack");
        return base.attack(target);
    }
}
