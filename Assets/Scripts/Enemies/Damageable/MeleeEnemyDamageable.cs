﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyDamageable : Damageable {

    public Rigidbody rbody;
    Coroutine knockBackRoutine;
    Coroutine targetSwitchRoutine;

    public GameObject deathFX;

    public override void Die()
    {
        base.Die();
        Destroy(Instantiate(deathFX, transform.position, transform.rotation), 5f);
    }

    public override void TakeDamage(Transform attacker, int hpLost, Vector3 dir, float force)
    {
        float dirDotProd = Vector3.Dot(transform.forward, dir); // it's y trajectory (positive means from behind)
        
        base.TakeDamage(attacker, hpLost, dir, force);

        if (dead) {
            // handle death animations here
            return;
        }

        if (dirDotProd < -0.5f) { myMovement.anim.Play("FrontHurt"); } // it came from the front
        else if (dirDotProd > 0.5f) { myMovement.anim.Play("BackHurt"); } // it came from behind
        else {
            if (-dir.x > 0) { myMovement.anim.Play("RightHurt"); } // it came from the right
            else { myMovement.anim.Play("LeftHurt"); } // it came from the left
        }

        if(attacker != myMovement.attackTarget &&
           myMovement.getCurrentState().GetType() != typeof(MeleeEnemySeduced)) {
            if(targetSwitchRoutine != null) { StopCoroutine(targetSwitchRoutine); }
            targetSwitchRoutine = StartCoroutine(SwitchTargets(attacker));
            myMovement.changeState(new MeleeEnemyAggro());
        }
        myMovement.changeState(new MeleeEnemyInjured(), myMovement.getCurrentState());
    }

    IEnumerator SwitchTargets(Transform attacker) {
        myMovement.attackTarget = attacker;
        if(myMovement.attackTarget == null) { Debug.Log("No attack target!"); yield break; }
        while(attacker != null) {
            yield return new WaitForFixedUpdate();
        }
        myMovement.attackTarget = myMovement.blueprint.getOriginTarget();
        targetSwitchRoutine = null;
    }

    public override void knockBack(Vector3 dir, float force)
    {
        if (knockBackRoutine != null) {
            StopCoroutine(knockBackRoutine);
        }
        knockBackRoutine = StartCoroutine(knockingBack(dir, force));
    }

    IEnumerator knockingBack(Vector3 dir, float force)
    {
        myMovement.agent.isStopped = true;
        myMovement.agent.updatePosition = false;
        myMovement.agent.updateRotation = false;

        myMovement.agent.velocity = Vector3.zero;
        rbody.velocity = Vector3.zero;

        float groundTime = 0f;
        rbody.AddForce(dir * force, ForceMode.Impulse);

        while (groundTime < .3f) {
            myMovement.agent.isStopped = true;
            if (rbody.velocity.y == 0) { groundTime += Time.deltaTime; }
            yield return new WaitForEndOfFrame();
        }
        myMovement.agent.Warp(transform.position);
        myMovement.agent.isStopped = false;
        myMovement.agent.updatePosition = true;
        myMovement.agent.updateRotation = true;
        knockBackRoutine = null;
    }

    public override void InitiateTransmutation(float duration, GameObject replacement)
    {
        base.InitiateTransmutation(duration, replacement);
    }

    public override IEnumerator processTransmutation(float duration, GameObject replacement)
    {
        myMovement.hamper++;

        // shut off the renderers
        Collider myColl = GetComponent<Collider>();
        myColl.enabled = false;
        Renderer[] allRends = GetComponentsInChildren<Renderer>();
        if (allRends.Length > 0) { foreach (Renderer rend in allRends) { rend.enabled = false; } }

        // stop the navmesh agent
        myMovement.agent.isStopped = true;

        // Create the replacement object
        GameObject myReplace = Instantiate(replacement, transform.position, transform.rotation);
        Rigidbody replaceRigidBody = myReplace.GetComponent<Rigidbody>();
        replaceRigidBody.AddExplosionForce(3f, transform.position, 1f);
        replacedBody = myReplace.GetComponent<Damageable>();
        replacedBody.parentHit = this;
        replacedBody.setTransmutable(false);

        // wait for the spell duration
        yield return new WaitForSeconds(duration);

        // move to transmuted object(in case object was moved)
        myMovement.agent.nextPosition = myReplace.transform.position;
        myMovement.agent.isStopped = false;
        transform.position = myMovement.agent.nextPosition;

        Destroy(myReplace); // Destroy my replacement

        // reaactivate colliders and renderers
        myColl.enabled = true;
        if (allRends.Length > 0) { foreach (Renderer rend in allRends)
                if(rend != null) { rend.enabled = true; } }
        replacedBody = null;
        myMovement.hamper--;
    }

    public override void Seduce(float duration, GameObject target, SpellCaster owner)
    {
        base.Seduce(duration, target, owner);
        // if(myMovement.crushTarget != null) { return; }
        // myMovement.changeState(new MeleeEnemySeduced(), myMovement.getCurrentState(), duration);
    }

    public override IEnumerator processSeduction(float duration, GameObject target, SpellCaster owner)
    {
        myMovement.anim.Play("FrontHurt");
        myMovement.changeState(new MeleeEnemySeduced(), duration);
        yield return new WaitForSeconds(duration);
        myMovement.changeState(new MeleeEnemyIdle());
        seduction = null;
    }
}
