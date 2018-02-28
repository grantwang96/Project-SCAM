using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyIdle : NPCState
{
    public override void Enter(Movement owner, float newDuration) {

        myOwner = owner; // save the owner
        duration = newDuration; // set the duration
        stateStartTime = Time.time;

        myOwner.agent.velocity = Vector3.zero;

        anim = myOwner.anim;
        anim.SetInteger("Status", 0);
    }

    public override void Execute() {
        
        if(myOwner.checkView()) { // look for the player
            myOwner.changeState(new MeleeEnemyAggro());
            return;
        }

        if(Time.time - stateStartTime >= duration) { // connect to wander
            myOwner.changeState(new MeleeEnemyWander());
        }
    }

    public override void Exit() {

    }

    public override void becomeAggro(EnemyData.CombatType combatType)
    {
        
    }
}

public class MeleeEnemyWander : NPCState
{
    public override void Enter(Movement owner)
    {
        myOwner = owner;
        stateStartTime = Time.time;
        duration = Random.Range(4f, 6f);
        anim = myOwner.anim;
        anim.SetInteger("Status", 1);
        if(myOwner.agent.enabled) { myOwner.agent.speed = myOwner.baseSpeed; }

        // Set myowner agent's destination(ONLY HAPPENS ONCE)
        Vector3 target = myOwner.getRandomLocation(myOwner.transform.position, myOwner.maxWanderDistance);
        if (myOwner.agent.enabled) { myOwner.agent.SetDestination(target); }
  
  }

    public override void Execute()
    {
        if(myOwner.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) { myOwner.agent.isStopped = true; }
        else { myOwner.agent.isStopped = false; }
        
        if(myOwner.checkView()) { myOwner.changeState(new MeleeEnemyAggro()); }

        // check for obstructions
        Transform obstruction = myOwner.obstruction();
        if (obstruction != null) {
            myOwner.changeState(new MeleeEnemyIdle());
        }

        float distToDest = Vector3.Distance(myOwner.transform.position, myOwner.agent.pathEndPosition);
        if(distToDest < 0.2f + myOwner.agent.stoppingDistance) {
            myOwner.changeState(new MeleeEnemyIdle(), Random.Range(4f, 6f));
        }
        if(myOwner.friction != 1f) { myOwner.rbody.AddForce(myOwner.agent.desiredVelocity * (1f - myOwner.friction)); }
    }

    public override void Exit() {

    }
}

public class MeleeEnemyInjured : NPCState
{
    public override void Enter(Movement owner, NPCState prevState)
    {
        base.Enter(owner, prevState);
    }

    public override void Execute()
    {
        int loops = Mathf.FloorToInt(myOwner.anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        Debug.Log(myOwner.anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        if (myOwner.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) { // wait for animation to end
            myOwner.changeState(previousState);
        }
    }
}

public class MeleeEnemyAggro : NPCState
{
    Transform attackTarget;
    bool targetInView;
    float lostTargetViewTime = 0f;

    public override void Enter(Movement owner)
    {
        // Initialize information from movement object(owner)
        myOwner = owner;
        attackTarget = myOwner.attackTarget;
        targetInView = false;
        myOwner.agent.speed = myOwner.maxSpeed;

        // Grab and set animation state
        anim = myOwner.anim;
        anim.SetInteger("Status", 2);
        duration = myOwner.blueprint.attentionSpan;

        // myOwner.agent.isStopped = false;
    }

    public override void Execute()
    {

        if (myOwner.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) { myOwner.agent.isStopped = true; }
        else { myOwner.agent.isStopped = false; }
        
        // if you have nothing to chase, stop chasing
        if (myOwner.attackTarget == null) { myOwner.changeState(new MeleeEnemyIdle(), Random.Range(4f, 6f)); }

        // Check to see if the target is still in view
        targetInView = myOwner.checkView();

        // Enter idle if target has been out of view too long
        if(!targetInView) {
            lostTargetViewTime += Time.deltaTime;
            if(lostTargetViewTime >= duration) { myOwner.changeState(new MeleeEnemyIdle(), Random.Range(4f, 6f)); }
        }
        else {
            lostTargetViewTime = 0f;
        }

        if (myOwner.agent.enabled) { myOwner.agent.SetDestination(myOwner.attackTarget.position); }

        // check for obstructions
        Transform obstruction = myOwner.obstruction();
        if (obstruction != null) {
            Damageable dam = obstruction.GetComponent<Damageable>();
            if(dam) { myOwner.changeState(new MeleeEnemyAttack(), this); }
            else { myOwner.changeState(new MeleeEnemyIdle()); }
        }

        // Enter attack state if in range to attack
        float dist = Vector3.Distance(myOwner.transform.position, attackTarget.position);
        Vector3 forward = myOwner.transform.forward;
        Vector3 dir = attackTarget.position - myOwner.transform.position;
        forward.y = 0;
        dir.y = 0;
        // float ang = Vector3.Angle(myOwner.transform.forward, attackTarget.position - myOwner.transform.position);
        float dotprod = Vector3.Dot(myOwner.transform.forward, (attackTarget.position - myOwner.transform.position).normalized);
        
        if(dist <= myOwner.blueprint.attackRange && dotprod >= myOwner.attackDotProd) {
            myOwner.changeState(new MeleeEnemyAttack());
        }
        myOwner.rbody.AddForce(myOwner.agent.desiredVelocity / myOwner.friction);
    }

    public override void Exit()
    {
        
    }
}

public class MeleeEnemyAttack : NPCState
{
    public override void Enter(Movement owner)
    {
        base.Enter(owner);
        // anim.Play("Attack");
        myOwner.attackRoutine = myOwner.StartCoroutine(myOwner.attack(myOwner.attackTarget.position));
        myOwner.agent.isStopped = true;
        myOwner.agent.velocity = Vector3.zero;
    }

    public override void Enter(Movement owner, NPCState prevState)
    {
        base.Enter(owner, prevState);
        // anim.Play("Attack");
        myOwner.attackRoutine = myOwner.StartCoroutine(myOwner.attack(myOwner.attackTarget.position));
        Debug.Log(myOwner.transform.name + " attacks!");
        myOwner.agent.isStopped = true;
        myOwner.agent.velocity = Vector3.zero;
    }

    public override void Execute()
    {
        // check if attack animation is finished
        if(myOwner.attackRoutine == null) {
            if(previousState != null) { myOwner.changeState(previousState); }
            else { myOwner.changeState(new MeleeEnemyAggro()); }
        }
    }

    public override void Exit()
    {
        myOwner.agent.isStopped = false;
        Debug.Log("Exiting Attack...");
    }
}

public class MeleeEnemySeduced : NPCState
{
    float time;

    public override void Enter(Movement owner, NPCState prevState, float newDuration)
    {
        base.Enter(owner, prevState, newDuration);
        Debug.Log("Seduced for: " + duration);
        
        // set animator to seduced animations
        anim = myOwner.anim;

        myOwner.agent.stoppingDistance = 4f;
        myOwner.attackTarget = null;
        time = 0f;
    }

    public override void Enter(Movement owner, float newDuration)
    {
        base.Enter(owner, newDuration); Debug.Log("Seduced for: " + duration);
        // set animator to seduced animations
        anim = myOwner.anim;

        myOwner.agent.stoppingDistance = 4f;
        myOwner.attackTarget = null;
        time = 0f;
    }

    public override void Execute()
    {
        // if(time > duration) { stateChange(); return; }
        
        // make sure the target and/or crush isn't dead/gone already
        if(myOwner.crush == null || myOwner.crushTarget == null) { Debug.Log("No Crush"); stateChange(); return; }

        // if you don't have an attack target, go follow crush around
        if (myOwner.attackTarget == null) {
            myOwner.agent.stoppingDistance = 3f;
            if (myOwner.agent.enabled) { myOwner.agent.SetDestination(myOwner.crushTarget.position); }

            // check for obstructions
            Transform obstruction = myOwner.obstruction();
            if (obstruction != null) { teleportToLover(); }
        }
        else { // otherwise, go be mean
            myOwner.agent.stoppingDistance = myOwner.blueprint.attackRange;
            if (myOwner.agent.enabled) { myOwner.agent.SetDestination(myOwner.attackTarget.position); }
            float distance = Vector3.Distance(myOwner.transform.position, myOwner.attackTarget.position);
            if(distance < myOwner.blueprint.attackRange) { myOwner.changeState(new MeleeEnemyAttack(), this); }
        }

        time += Time.deltaTime;
    }

    public override void Exit()
    {
        if(myOwner.crush != null) {
            myOwner.crush.removeFromSeductionList(myOwner.GetComponent<Damageable>());
        }
        // myOwner.crush = null;
        // myOwner.crushTarget = null;
        myOwner.attackTarget = myOwner.blueprint.getOriginTarget();
        myOwner.agent.stoppingDistance = myOwner.blueprint.attackRange;
    }

    private void stateChange()
    {
        if (previousState != null) { myOwner.changeState(previousState); }
        else { myOwner.changeState(new MeleeEnemyIdle()); }
    }

    private void teleportToLover()
    {
        if(myOwner.crushTarget == null) { myOwner.changeState(new MeleeEnemyIdle()); return; }
        Vector3 randomLocation = Random.insideUnitSphere * 4f + myOwner.crushTarget.position;
        randomLocation.y = myOwner.crushTarget.position.y;
        myOwner.agent.Warp(randomLocation);
    }
}