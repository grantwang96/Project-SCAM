using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeMovement : Movement {

    public Vector3 destination;

    public override void setup() {
        agent = GetComponent<NavMeshAgent>(); // set the agent
        // agent.updatePosition = false;
        // agent.updateRotation = false;
        // agent.isStopped = true;
        base.setup();
    }

    public override void Update()
    {
        destination = agent.destination;
        base.Update();
    }
}
