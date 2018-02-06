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

        anim = myOwner.anim;
        anim.SetInteger("Status", 0);

        Debug.Log("Entering Idle...");
    }

    public override void Execute() {
        
        if(myOwner.checkView()) { // look for the player
            Debug.Log("I see you!");
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
        myOwner.agent.speed = myOwner.baseSpeed;

        // Set myowner agent's destination(ONLY HAPPENS ONCE)
        Vector3 target = myOwner.getRandomLocation(myOwner.transform.position, myOwner.maxWanderDistance);
        myOwner.agent.SetDestination(target);

        if(target == null) { Debug.Log("No Target!"); }
        Debug.Log("Entering wander...");
        Debug.Log("Target " + myOwner.agent.destination);
  
  }

    public override void Execute()
    {
        if(myOwner.checkView()) { myOwner.changeState(new MeleeEnemyAggro()); }
        if(myOwner.obstruction()) { myOwner.changeState(new MeleeEnemyIdle()); }

        float distToDest = Vector3.Distance(myOwner.transform.position, myOwner.agent.pathEndPosition);
        if(distToDest < 0.2f + myOwner.agent.stoppingDistance) {
            Debug.Log("Reached destination!");
            myOwner.changeState(new MeleeEnemyIdle(), Random.Range(4f, 6f));
        }
        if(myOwner.friction != 1f) { myOwner.rbody.AddForce(myOwner.agent.desiredVelocity * (1f - myOwner.friction)); }
    }

    public override void Exit() {

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
        Debug.Log("Entering Chase...");
    }

    public override void Execute()
    {
        // if you have nothing to chase, stop chasing
        if(myOwner.attackTarget == null) { myOwner.changeState(new MeleeEnemyIdle(), Random.Range(4f, 6f)); }

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

        myOwner.agent.SetDestination(myOwner.attackTarget.position);
        if (myOwner.obstruction()) { myOwner.changeState(new MeleeEnemyIdle()); }

        // Enter attack state if in range to attack
        float dist = Vector3.Distance(myOwner.transform.position, attackTarget.position);
        if(dist <= myOwner.blueprint.attackRange) {
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
        anim.Play("Attack");
        myOwner.attack(myOwner.attackTarget.position);
        Debug.Log(myOwner.transform.name + " attacks!");
    }

    public override void Enter(Movement owner, NPCState prevState)
    {
        base.Enter(owner, prevState);
        anim.Play("Attack");
        myOwner.attack(myOwner.attackTarget.position);
        Debug.Log(myOwner.transform.name + " attacks!");
    }

    public override void Execute()
    {
        // check if attack animation is finished
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
            if(previousState != null) { myOwner.changeState(previousState); }
            else { myOwner.changeState(new MeleeEnemyAggro()); }
        }
    }

    public override void Exit()
    {
        
    }
}

public class MeleeEnemySeduced : NPCState
{
    float time;

    public override void Enter(Movement owner, NPCState prevState, float newDuration)
    {
        base.Enter(owner, prevState, newDuration);
        
        // set animator to seduced animations
        anim = myOwner.anim;

        myOwner.agent.stoppingDistance = 4f;
        myOwner.attackTarget = null;
        time = 0f;
    }

    public override void Execute()
    {
        if(time > duration) { stateChange(); return; }

        // make sure the target and/or crush isn't dead/gone already
        if(myOwner.crush == null || myOwner.crushTarget == null) { stateChange(); return; }

        // if you don't have an attack target, go follow crush around
        if (myOwner.attackTarget == null) {
            myOwner.agent.stoppingDistance = 3f;
            myOwner.agent.SetDestination(myOwner.crushTarget.position);
            if (myOwner.obstruction()) { teleportToLover(); }
        }
        else { // otherwise, go be mean
            myOwner.agent.stoppingDistance = myOwner.blueprint.attackRange;
            myOwner.agent.SetDestination(myOwner.attackTarget.position);
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
        myOwner.crush = null;
        myOwner.crushTarget = null;
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