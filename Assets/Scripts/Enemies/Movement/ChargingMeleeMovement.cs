﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargingMeleeMovement : Movement {

    public Vector3 destination;
    public Vector3 chargingForce;

    public override void setup()
    {
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
        Debug.Log("Attack");
        agent.isStopped = true;
        agent.updatePosition = false;
        agent.updateRotation = false;

        agent.velocity = Vector3.zero;
        rbody.velocity = Vector3.zero;

        float groundTime = 0f;
        rbody.AddForce(transform.forward + transform.TransformVector(chargingForce), ForceMode.Impulse);

        while (groundTime < .3f) {
            if (rbody.velocity.y == 0) { groundTime += Time.deltaTime; }
            yield return new WaitForEndOfFrame();
        }

        hamper++;
        float startTime = Time.time;
        // play attack animation
        anim.Play("Attack");
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
            yield return new WaitForEndOfFrame();
        }
        hamper--;

        agent.Warp(transform.position);
        agent.isStopped = false;
        agent.updatePosition = true;
        agent.updateRotation = true;
    }
}
