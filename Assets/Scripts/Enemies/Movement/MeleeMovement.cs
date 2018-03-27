using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeMovement : Movement {

    public Vector3 destination;
    public bool isGrounded;

    public override void setup() {
        agent = GetComponent<NavMeshAgent>(); // set the agent
        base.setup();
    }

    public override void Update()
    {
        destination = agent.destination;
        if(isGrounded) {
            agent.updatePosition = true;
            agent.updateRotation = true;
            agent.Warp(transform.position);
            agent.isStopped = false;
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
    
    void OnCollisionStay(Collision coll)
    {
        if (coll.collider.tag == "Ground" || coll.collider.tag == "Wall" || coll.collider.tag == "Roof")
        {
            for (int i = 0; i < coll.contacts.Length; i++) {
                Vector3 point = coll.contacts[i].point;
                if (Vector3.Distance(point, transform.position) < 0.2f) {
                    isGrounded = true;
                }
            }
        }
    }

    void OnCollisionExit(Collision coll)
    {
        if (coll.collider.tag == "Ground") { isGrounded = false; }
    }
}
