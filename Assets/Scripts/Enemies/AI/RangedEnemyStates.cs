using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyIdle : NPCState
{

    public override void Enter(Movement owner, float newDuration)
    {
        base.Enter(owner, newDuration);
    }

    public override void Execute()
    {
        if (myOwner.checkView())
        { // look for the player
            myOwner.changeState(new RangedEnemyAttack());
            return;
        }

        if (Time.time - stateStartTime >= duration) { // connect to wander
            myOwner.changeState(new RangedEnemyWander());
        }
    }
}

public class RangedEnemyWander : NPCState
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
        if (myOwner.agent.enabled) { myOwner.agent.SetDestination(target); }

        if (target == null) { Debug.Log("No Target!"); }
    }

    public override void Execute()
    {
        if (myOwner.checkView()) { myOwner.changeState(new RangedEnemyAttack()); }

        // check for obstructions
        Transform obstruction = myOwner.obstruction();
        if (obstruction != null)
        {
            myOwner.changeState(new RangedEnemyIdle());
        }

        float distToDest = Vector3.Distance(myOwner.transform.position, myOwner.agent.pathEndPosition);
        if (distToDest < 0.2f + myOwner.agent.stoppingDistance)
        {
            myOwner.changeState(new RangedEnemyIdle(), Random.Range(4f, 6f));
        }
        if (myOwner.friction != 1f) { myOwner.rbody.AddForce(myOwner.agent.desiredVelocity * (1f - myOwner.friction)); }
    }

    public override void Exit()
    {

    }
}

public class RangedEnemyAggro : NPCState
{
    bool targetInView;
    bool targetWasInView;
    float lostTargetViewTime = 0f;

    bool hasCoverPosition = false;

    public override void Enter(Movement owner)
    {
        myOwner = owner;

        // always look at the target
        myOwner.agent.updateRotation = false;

        targetInView = false;
        myOwner.agent.speed = myOwner.maxSpeed;
        anim = myOwner.anim;
        anim.SetInteger("Status", 2);
        duration = myOwner.blueprint.attentionSpan;

        FindCover();
    }

    public override void Execute()
    {
        // if you have nothing to chase, stop chasing
        if (myOwner.attackTarget == null) {
            if (previousState != null) { myOwner.changeState(previousState); }
            else { myOwner.changeState(new RangedEnemyIdle(), Random.Range(4f, 6f)); }
        }
        if (myOwner == null || myOwner.transform == null) { return; }
        if (myOwner.attackTarget == null)
        {
            if (previousState != null) { myOwner.changeState(previousState); }
            else { myOwner.changeState(new RangedEnemyIdle()); }
            return;
        }
        Vector3 targetDir = myOwner.attackTarget.position - myOwner.transform.position;
        targetDir.y = 0;
        Quaternion forward = Quaternion.LookRotation(targetDir);
        myOwner.transform.rotation = Quaternion.Lerp(myOwner.transform.rotation, forward, 0.8f);

        // Check to see if the target is still in view
        targetInView = myOwner.checkView();

        // Enter idle if target has been out of view too long
        if (!targetInView)
        {
            hasCoverPosition = false;
            lostTargetViewTime += Time.deltaTime;
            if (myOwner.agent.enabled) { myOwner.agent.SetDestination(myOwner.attackTarget.position); }
            if (lostTargetViewTime >= duration)
            {
                Debug.Log("Where'd you go?");
                myOwner.changeState(new RangedEnemyIdle(), Random.Range(4f, 6f));
            }
        }
        else {
            if (!hasCoverPosition) { FindCover(); }
        }

        // check for obstructions
        Transform obstruction = myOwner.obstruction();
        if (obstruction != null)
        {
            Damageable dam = obstruction.GetComponent<Damageable>();
            if (dam) { myOwner.changeState(new RangedEnemyAttack(), this); }
            else { myOwner.changeState(new RangedEnemyIdle()); }
        }

        if (myOwner.agent.desiredVelocity.magnitude < 0.5f) { myOwner.changeState(new RangedEnemyAttack()); }
        targetWasInView = targetInView;
    }

    public override void Exit()
    {
        myOwner.agent.updateRotation = true;
    }

    void FindCover()
    {
        myOwner.agent.ResetPath();
        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(myOwner.transform.position, out hit, myOwner.agent.areaMask))
        {
            hasCoverPosition = true;
            if (myOwner.agent.enabled) { myOwner.agent.SetDestination(hit.position); }
        }
    }
}

public class RangedEnemyAttack : NPCState
{
    public override void Enter(Movement owner)
    {
        myOwner = owner;
        anim = myOwner.anim;

        myOwner.agent.updateRotation = false;
        myOwner.StartCoroutine(attackProcessing());
    }

    public override void Execute()
    {
        if (myOwner.attackTarget == null)
        {
            if (previousState != null) { myOwner.changeState(previousState); }
            else { myOwner.changeState(new RangedEnemyIdle()); }
        }
        Vector3 targetDir = myOwner.attackTarget.position - myOwner.transform.position;
        targetDir.y = 0;
        Quaternion forward = Quaternion.LookRotation(targetDir);
        myOwner.transform.rotation = Quaternion.Lerp(myOwner.transform.rotation, forward, 5f * Time.deltaTime);
    }

    IEnumerator attackProcessing()
    {
        int fireCount = Random.Range(2, 5);
        SpellBook spellbook = myOwner.GetComponent<SpellCaster>().returnSpell();
        float cooldown = spellbook.primaryEffect.coolDown + spellbook.secondaryEffect.coolDown;
        for (int i = 0; i < fireCount; i++)
        {
            if (myOwner.attackTarget == null) { break; }
            myOwner.StartCoroutine(myOwner.attack(myOwner.attackTarget.position));
            yield return new WaitForEndOfFrame();
            while (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(cooldown);
        }
        myOwner.changeState(new RangedEnemyAggro());
    }

    public override void Exit()
    {
        myOwner.agent.updateRotation = true;
    }
}

public class RangedEnemySeduced : NPCState
{
    bool targetInView;
    bool targetWasInView;

    bool hasCoverPosition;

    public override void Enter(Movement owner, NPCState prevState, float newDuration)
    {
        base.Enter(owner, prevState, newDuration);

        // always look at the target
        myOwner.agent.updateRotation = false;

        targetInView = false;
        myOwner.agent.speed = myOwner.maxSpeed;
        anim = myOwner.anim;
        anim.SetInteger("Status", 2);

        hasCoverPosition = false;
        // FindCover();
    }

    public override void Execute()
    {
        if (myOwner.attackTarget != null)
        {
            // Check to see if the target is still in view
            targetInView = myOwner.checkView();

            // Enter idle if target has been out of view too long
            if (!targetInView)
            {
                hasCoverPosition = false;
                if (myOwner.agent.enabled) { myOwner.agent.SetDestination(myOwner.attackTarget.position); }
            }
            else {
                if (!hasCoverPosition) { FindCover(); }
                float distance = Vector3.Distance(myOwner.transform.position, myOwner.agent.destination);
                if (distance < myOwner.blueprint.attackRange) { myOwner.changeState(new RangedEnemyAttack(), this); }
            }

            if (myOwner.agent.desiredVelocity.magnitude < 0.5f) { myOwner.changeState(new RangedEnemyAttack()); }
        }
        else {
            Vector3 targetDir = myOwner.crushTarget.position - myOwner.transform.position;
            targetDir.y = 0;
            Quaternion forward = Quaternion.LookRotation(targetDir);
            myOwner.transform.rotation = Quaternion.Lerp(myOwner.transform.rotation, forward, 0.8f);

            myOwner.agent.stoppingDistance = 5f;
            if (myOwner.agent.enabled) { myOwner.agent.SetDestination(myOwner.crushTarget.position); }
        }

        // check for obstructions
        Transform obstruction = myOwner.obstruction();
        if (obstruction != null) { teleportToLover(); }
    }

    public override void Exit()
    {
        myOwner.agent.updateRotation = true;
    }

    void FindCover()
    {
        myOwner.agent.ResetPath();
        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(myOwner.transform.position, out hit, myOwner.agent.areaMask))
        {
            hasCoverPosition = true;
            if (myOwner.agent.enabled) { myOwner.agent.SetDestination(hit.position); }
        }
    }

    private void teleportToLover()
    {
        if (myOwner.crushTarget == null) { myOwner.changeState(new RangedEnemyIdle()); return; }
        Vector3 randomLocation = Random.insideUnitSphere * 4f + myOwner.crushTarget.position;
        randomLocation.y = myOwner.crushTarget.position.y;
        myOwner.agent.Warp(randomLocation);
    }

    NavMeshHit[] SplitNMerge(NavMeshHit[] array)
    {
        if (array.Length < 2) { return array; } // if the resulting array is length 1
        int middle = array.Length / 2; // split the array down the middle

        // recursively handle each array

        NavMeshHit[] leftArray = new NavMeshHit[middle];
        CopyArray(array, leftArray, 0, middle);
        leftArray = SplitNMerge(leftArray);

        NavMeshHit[] rightArray = new NavMeshHit[array.Length - middle];
        CopyArray(array, rightArray, middle, array.Length);
        rightArray = SplitNMerge(rightArray);

        // merge back the left and right array
        NavMeshHit[] newArray = MergeBack(leftArray, rightArray);
        return newArray;
    }

    NavMeshHit[] MergeBack(NavMeshHit[] a1, NavMeshHit[] a2)
    {
        int a = 0, b = 0;
        int length = a1.Length + a2.Length;
        NavMeshHit[] finishedArray = new NavMeshHit[length];
        for (int i = 0; i < length; i++)
        {
            float distA = -1;
            float distB = -1;

            // only compare a1 and a2 if a and be are respectively in range
            if (a < a1.Length) { distA = Vector3.Distance(a1[a].position, myOwner.attackTarget.position); }
            if (b < a2.Length) { distB = Vector3.Distance(a2[b].position, myOwner.attackTarget.position); }

            // if one of them is out of range
            if (distB == -1) { finishedArray[i] = a1[a]; a++; }
            else if (distA == -1) { finishedArray[i] = a2[b]; b++; }

            // otherwise compare
            else {
                if (distA <= distB) { finishedArray[i] = a1[a]; a++; }
                else { finishedArray[i] = a2[b]; b++; }
            }
        }
        return finishedArray;
    }

    void CopyArray(NavMeshHit[] A, NavMeshHit[] B, int start, int end)
    {
        int idx = start;
        for (int i = 0; i < B.Length; i++)
        {
            B[i] = A[idx];
            idx++;
        }
    }
}

public class RangedEnemyInjured : NPCState
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
