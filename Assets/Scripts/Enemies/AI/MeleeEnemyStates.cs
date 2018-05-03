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

        if (myOwner.agent.enabled) { myOwner.agent.velocity = Vector3.zero; }

        anim = myOwner.anim;
        anim.SetInteger("Status", 0);
        myOwner.attackTarget = myOwner.blueprint.getOriginTarget();
    }

    public override void Execute() {

        if (myOwner.checkView()) { // look for the player
            myOwner.anim.Play("Notice");
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
    Vector3 target;
    float time = 0f;

    public override void Enter(Movement owner)
    {
        myOwner = owner;
        stateStartTime = Time.time;
        duration = Random.Range(4f, 6f);
        anim = myOwner.anim;
        anim.SetInteger("Status", 1);
        if(myOwner.agent.enabled) { myOwner.agent.speed = myOwner.baseSpeed; }

        // Set myowner agent's destination(ONLY HAPPENS ONCE)
        target = myOwner.getRandomLocation(myOwner.agent.nextPosition, myOwner.maxWanderDistance);
        if (myOwner.agent.enabled && !myOwner.agent.isStopped) { 
//			Debug.Log("Target set!"); 
			myOwner.agent.SetDestination(target); 
		}
//        Debug.Log(myOwner.name + "'s new target is: " + target);
//        Debug.Log(myOwner.name + "'s current position is: " + myOwner.transform.position);
        // Debug.Log("Begin Wander...");
        // Debug.Log("Status=" + anim.GetInteger("Status"));
  }

    public override void Execute()
    {
        if(myOwner.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) { myOwner.agent.SetDestination(myOwner.transform.position); }
        else { myOwner.agent.SetDestination(target); }

        if(time > duration) { myOwner.changeState(new MeleeEnemyIdle()); }

        if (myOwner.checkView()) {
            myOwner.anim.Play("Notice");
            myOwner.changeState(new MeleeEnemyAggro());
        }

        // check for obstructions
        Transform obstruction = myOwner.obstruction();
        if (obstruction != null) {
//            Debug.Log("Switching to idling...");
            myOwner.changeState(new MeleeEnemyIdle());
        }

        float distToDest = Vector3.Distance(myOwner.transform.position, target);
        if(distToDest < 0.2f + myOwner.agent.stoppingDistance) {
//            Debug.Log("Stopped moving from " + distToDest + " away");
//            Debug.Log("Switching to idling...");
            myOwner.changeState(new MeleeEnemyIdle(), Random.Range(4f, 6f));
        }
        if(myOwner.friction != 1f) { myOwner.rbody.AddForce(myOwner.agent.desiredVelocity * (1f - myOwner.friction)); }
        time += Time.deltaTime;
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
    }

    public override void Execute()
    {
        // if you have nothing to chase, stop chasing
        if (myOwner.attackTarget == null || attackTarget == null) { myOwner.changeState(new MeleeEnemyIdle(), Random.Range(4f, 6f)); return; }
        if(myOwner == null || myOwner.transform == null || myOwner.attackTarget == null || attackTarget == null) {
            // Debug.Log("Nope");
            return;
        }

        float dist = Vector3.Distance(myOwner.transform.position, attackTarget.position);

        if (myOwner.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
            myOwner.anim.GetCurrentAnimatorStateInfo(0).IsName("Notice")) {
            Vector3 attackDir = myOwner.attackTarget.position - myOwner.transform.position;
            attackDir.y = 0;
            myOwner.transform.rotation = Quaternion.Lerp(myOwner.transform.rotation, Quaternion.LookRotation(attackDir), 0.5f);
            myOwner.agent.velocity = Vector3.zero;
        }
        else if(dist < myOwner.blueprint.attackRange) {
            Vector3 attackDir = myOwner.attackTarget.position - myOwner.transform.position;
            attackDir.y = 0;
            myOwner.transform.rotation = Quaternion.Lerp(myOwner.transform.rotation, Quaternion.LookRotation(attackDir), 0.5f);
        }
        
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

		if (myOwner.agent.enabled && !myOwner.agent.isStopped && attackTarget != null) { 
			myOwner.agent.SetDestination(attackTarget.position); 
		}

        // check for obstructions
        Transform obstruction = myOwner.obstruction();
        if (obstruction != null) {
            Damageable dam = obstruction.GetComponent<Damageable>();
            if(dam) { myOwner.changeState(new MeleeEnemyAttack(), this); }
            else { myOwner.changeState(new MeleeEnemyIdle()); }
        }

        // Enter attack state if in range to attack
        Vector3 forward = myOwner.transform.forward;
        Vector3 dir = attackTarget.position - myOwner.transform.position;
        forward.y = 0;
        dir.y = 0;
        // float ang = Vector3.Angle(myOwner.transform.forward, attackTarget.position - myOwner.transform.position);
        float dotprod = Vector3.Dot(myOwner.transform.forward, (attackTarget.position - myOwner.transform.position).normalized);
        
        if(dist <= myOwner.blueprint.attackRange && dotprod >= myOwner.attackDotProd) {
            myOwner.changeState(new MeleeEnemyAttack());
        }
        // myOwner.rbody.AddForce(myOwner.agent.desiredVelocity / myOwner.friction);
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
        if(myOwner.agent.enabled) { myOwner.agent.SetDestination(myOwner.transform.position); }
        // myOwner.agent.isStopped = true;
    }

    public override void Enter(Movement owner, NPCState prevState)
    {
        base.Enter(owner, prevState);
        // anim.Play("Attack");
        myOwner.attackRoutine = myOwner.StartCoroutine(myOwner.attack(myOwner.attackTarget.position));
        if (myOwner.agent.enabled) { myOwner.agent.SetDestination(myOwner.transform.position); }
        // Debug.Log(myOwner.transform.name + " attacks!");
        // if (myOwner.agent.enabled) { myOwner.agent.velocity = Vector3.zero; }
    }

    public override void Execute()
    {
        // check if attack animation is finished
        if(myOwner.agent.enabled) { myOwner.agent.velocity = Vector3.zero; }
        if (myOwner.attackRoutine == null) {
            if(previousState != null) { myOwner.changeState(previousState); }
            else { myOwner.changeState(new MeleeEnemyAggro()); }
        }
    }

    public override void Exit()
    {
		if(myOwner.agent.enabled && myOwner.attackTarget != null) { 
			myOwner.agent.SetDestination(myOwner.attackTarget.position); 
		}
    }
}

public class MeleeEnemySeduced : NPCState
{
    float time;
    float originStopDistance;

    public override void Enter(Movement owner, NPCState prevState, float newDuration)
    {
        base.Enter(owner, prevState, newDuration);
//        Debug.Log("Seduced for: " + duration);
        
        // set animator to seduced animations
        anim = myOwner.anim;
        originStopDistance = myOwner.agent.stoppingDistance;
        myOwner.agent.stoppingDistance = 4f;
        myOwner.agent.speed = myOwner.maxSpeed;
        myOwner.attackTarget = null;
        time = 0f;
    }

    public override void Enter(Movement owner, float newDuration)
    {
        base.Enter(owner, newDuration); // Debug.Log("Seduced for: " + duration);
        // set animator to seduced animations
        anim = myOwner.anim;

        originStopDistance = myOwner.agent.stoppingDistance;
        myOwner.agent.stoppingDistance = 4f;
        myOwner.agent.speed = myOwner.maxSpeed;
        myOwner.attackTarget = null;
        time = 0f;
    }

    public override void Execute()
    {
        // if(time > duration) { stateChange(); return; }
        // make sure the target and/or crush isn't dead/gone already
        if(myOwner.crush == null || myOwner.crushTarget == null) { Debug.Log("No Crush"); stateChange(); return; }

        if (myOwner.anim.GetCurrentAnimatorStateInfo(0).IsTag("Hurt")) {
            // myOwner.agent.isStopped = true;
            // myOwner.agent.updatePosition = true;
            myOwner.agent.Warp(myOwner.transform.position);
        }
        else {
            // myOwner.agent.isStopped = false;
        }

        if (myOwner.agent.velocity.magnitude > myOwner.agent.speed / 2) {
            myOwner.anim.SetInteger("Status", 2);
        }
        else {
            myOwner.anim.SetInteger("Status", 0);
        }

        // if you don't have an attack target, go follow crush around
        if (myOwner.attackTarget == null) {
            myOwner.agent.stoppingDistance = 5f;
            if (myOwner.agent.enabled) { myOwner.agent.SetDestination(myOwner.crushTarget.position); }

            // check for obstructions
            Transform obstruction = myOwner.obstruction();
            if (obstruction != null) { teleportToLover(); }
        }
        else { // otherwise, go be mean
            myOwner.agent.stoppingDistance = originStopDistance;
            if (myOwner.agent.enabled) { myOwner.agent.SetDestination(myOwner.attackTarget.position); }
            float distance = Vector3.Distance(myOwner.transform.position, myOwner.attackTarget.position);
            if(distance < myOwner.blueprint.attackRange) {
                myOwner.changeState(new MeleeEnemyAttack(), this);
            }
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
        myOwner.agent.stoppingDistance = originStopDistance;
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