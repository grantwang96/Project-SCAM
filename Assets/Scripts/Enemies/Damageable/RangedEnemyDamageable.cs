using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyDamageable : Damageable {
    
    public Rigidbody rbody;
    Coroutine knockBackRoutine;
    Coroutine targetSwitchRoutine;

    public GameObject deathFX;
    public GameObject specialDrop;

    public List<GameObject> drops = new List<GameObject>();

    public override void Die() {
        base.Die();

        // play some death animations
        // play some death sfxs

        myMovement.blueprint.DropLoot(transform.position + Vector3.up);
        // activate special drop if you have one
        if (specialDrop != null) {
            FloatyRotaty fr = specialDrop.GetComponent<FloatyRotaty>();
            fr.active = true;
            fr.SetPosition();
            specialDrop.SetActive(true);
        }
        Destroy(Instantiate(deathFX, transform.position, transform.rotation), 5f);
    }

    public override void TakeDamage(Transform attacker, int hpLost, Vector3 dir, float force)
    {
        float dirDotProd = Vector3.Dot(transform.forward, dir); // it's y trajectory (positive means from behind)

        base.TakeDamage(attacker, hpLost, dir, force);

        Debug.Log("Ow");

        if (dead) {
            // handle death animations here
            return;
        }

        PlayHurtAnimation(dirDotProd, dir);

        if (attacker != null && attacker != myMovement.attackTarget &&
           myMovement.getCurrentState().GetType() != typeof(MeleeEnemySeduced))
        {
            if (targetSwitchRoutine != null) { StopCoroutine(targetSwitchRoutine); }
            targetSwitchRoutine = StartCoroutine(SwitchTargets(attacker));
            myMovement.changeState(new MeleeEnemyAggro());
        }
        myMovement.changeState(new MeleeEnemyInjured(), myMovement.getCurrentState());
    }

    IEnumerator SwitchTargets(Transform attacker)
    {
        myMovement.attackTarget = attacker;
        if (myMovement.attackTarget == null) { Debug.Log("No attack target!"); yield break; }
        while (attacker != null)
        {
            if (myMovement.attackTarget != attacker) { yield break; }
            yield return new WaitForFixedUpdate();
        }
        myMovement.attackTarget = myMovement.blueprint.getOriginTarget();
        targetSwitchRoutine = null;
    }

    public override void knockBack(Vector3 dir, float force)
    {
        myMovement.agent.updatePosition = false;
        myMovement.agent.updateRotation = false;
        myMovement.agent.isStopped = true;
        myMovement.agent.velocity = Vector3.zero;
        rbody.velocity = Vector3.zero;
        rbody.AddForce(dir * force, ForceMode.Impulse);
    }

    IEnumerator knockingBack(Vector3 dir, float force)
    {
        myMovement.agent.isStopped = true;
        Vector3 prevDestination = myMovement.agent.destination;
        myMovement.agent.ResetPath();
        myMovement.agent.updatePosition = false;
        myMovement.agent.updateRotation = false;

        myMovement.agent.velocity = Vector3.zero;

        // myMovement.agent.enabled = false;
        rbody.velocity = Vector3.zero;

        float groundTime = 0f;
        rbody.AddForce(dir * force, ForceMode.Impulse);

        while (groundTime < .3f) {
            RaycastHit rayHit;
            if (Physics.Raycast(transform.position, Vector3.down,
                out rayHit, myCollider.bounds.extents.y + 0.1f, ~0, QueryTriggerInteraction.Ignore)) {
                if (rayHit.collider.tag == "Ground" || rayHit.collider.tag == "Wall") { groundTime += Time.deltaTime; Debug.Log("Grounded"); }
            }
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("Back to work!");
        myMovement.agent.enabled = true;
        myMovement.agent.isStopped = false;
        if (myMovement.agent.Warp(transform.position))
        {
            Debug.Log("Success!");
        }
        else {
            Debug.Log("Fail");
        }
        Debug.Log(myMovement.agent.nextPosition);
        myMovement.agent.updatePosition = true;
        myMovement.agent.updateRotation = true;
        myMovement.agent.SetDestination(prevDestination);
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
        if (allRends.Length > 0) { foreach (Renderer rend in allRends)
            { if(rend != blush) { rend.enabled = false; } } }

        // stop the navmesh agent
        myMovement.agent.isStopped = true;
        myMovement.agent.updatePosition = false;
        myMovement.agent.updateRotation = false;

        // Create the replacement object
        GameObject myReplace = Instantiate(replacement, transform.position, transform.rotation);
        Rigidbody replaceRigidBody = myReplace.GetComponent<Rigidbody>();
        replaceRigidBody.AddExplosionForce(3f, transform.position, 1f);
        replacedBody = myReplace.GetComponent<Damageable>();
        replacedBody.parentHit = this;
        replacedBody.setTransmutable(false);

        // wait for the spell duration
        float time = 0f;
        while (time < duration) {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
            transform.position = replacedBody.transform.position;
        }
        
        // reaactivate colliders and renderers
        myColl.enabled = true;
        if (allRends.Length > 0)
        {
            foreach (Renderer rend in allRends)
                if (rend != null && rend != blush) { rend.enabled = true; }
        }
        if (dead) {
            yield break;
        }

        // move to transmuted object(in case object was moved)
        transform.position = myReplace.transform.position;
        myMovement.agent.nextPosition = myReplace.transform.position;
        // myMovement.agent.Warp(myReplace.transform.position);
        // myMovement.agent.isStopped = false;
        // transform.position = myMovement.agent.nextPosition;

        Destroy(myReplace); // Destroy my replacement
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
        myMovement.changeState(new RangedEnemySeduced(), duration);
        blush.enabled = true;
        yield return new WaitForSeconds(duration);
        myMovement.changeState(new RangedEnemyIdle());
        myMovement.attackTarget = myMovement.blueprint.getOriginTarget();
        seduction = null;
        blush.enabled = false;
    }

    public void PlayHurtAnimation(float dirDotProd, Vector3 dir)
    {
        if (dirDotProd < -0.5f) { myMovement.anim.Play("FrontHurt"); } // it came from the front
        else if (dirDotProd > 0.5f) { myMovement.anim.Play("BackHurt"); } // it came from behind
        else {
            if (-dir.x > 0) { myMovement.anim.Play("RightHurt"); } // it came from the right
            else { myMovement.anim.Play("LeftHurt"); } // it came from the left
        }
    }
}
