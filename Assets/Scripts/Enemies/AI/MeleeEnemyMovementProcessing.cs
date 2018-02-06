using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCIdle : NPCState
{
    /*
    float idleTime;
    float startIdle;
    float sightRange;
    Rigidbody rbody;
    Movement myOwner;

    Vector3 forward;
    Vector3 currRotation;
    Quaternion targetRotation;

    float turnDurationTime;
    float maxAngleChange = 60f;
    float heading;
    */
    
    public override void Enter(Movement owner)
    {
        base.Enter(owner);
        state = stateType.Normal;
        anim.SetInteger("Status", 0);
        myOwner.StartCoroutine(headTurn()); // Possibly temporary solution.
        Debug.Log("Entering Idling...");
    }

    public override void Execute()
    {
        // Do some head turning
        // Some idle animations
        // Check if player is in sight

        if (myOwner.checkView()) {
            // change state to aggro
            becomeAggro(myOwner.myType);
            Debug.Log("I can see you!");
            return;
        }

        // Do some head turning
        base.Execute();

        if (Time.time - stateStartTime >= duration) {
            myOwner.changeState(new NPCWander());
        }
    }

    public override void Exit()
    {
        myOwner.StartCoroutine(returnHeadPos());
    }

    public IEnumerator returnHeadPos()
    {
        Quaternion startRotation = myOwner.Head.transform.localRotation;
        targetRotation = Quaternion.Euler(0, 0, 0);
        float startTime = Time.time;
        float totalJourney = 0.25f;
        float fracJourney = 0;
        while(fracJourney < 1f)
        {
            fracJourney = (Time.time - startTime) / totalJourney;
            myOwner.Head.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, fracJourney);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator headTurn()
    {
        while (true)
        {
            yield return new WaitForSeconds(turnDurationTime);
            getHeadDir();
            turnDurationTime = Random.Range(1f, 2f);
        }
    }

    void getHeadDir()
    {
        heading = myOwner.Head.transform.localEulerAngles.y;
        float floor = Mathf.Clamp(heading - maxAngleChange, -maxAngleChange, maxAngleChange);
        float ceil = Mathf.Clamp(heading + maxAngleChange, -maxAngleChange, maxAngleChange);
        heading = Random.Range(floor, ceil);
        targetRotation = Quaternion.Euler(0, heading, 0);
    }

    void becomeAggro(EnemyData.CombatType combatType)
    {
        switch (combatType)
        {
            case EnemyData.CombatType.Melee:
                myOwner.changeState(new MeleeEnemyChase());
                break;
            case EnemyData.CombatType.SpellCaster:
                myOwner.changeState(new SpellCasterEnemyAggro());
                break;
            case EnemyData.CombatType.Mixed:
                break;
            case EnemyData.CombatType.Ranged:
                break;
            case EnemyData.CombatType.Support:
                break;
        }
    }
}

public class NPCWander : NPCState
{
    float startWander;

    bool emergencyTurning = false;

    Vector3 targRotation;
    float maxHeadingChange = 90f;
    Coroutine headingGetter;

    public override void Enter(Movement owner)
    {
        state = stateType.Normal;
        myOwner = owner;
        myOwner.currSpeed = myOwner.baseSpeed;
        startWander = Time.time;
        duration = Random.Range(4f, 6f);
        myOwner.currSpeed = myOwner.baseSpeed;
        anim = myOwner.anim;
        anim.SetInteger("Status", 1);
        headingGetter = myOwner.StartCoroutine(headingProcessing());
        Debug.Log("Entering Wander...");
    }

    public override void Execute()
    {
        cliffCheck();
        myOwner.transform.eulerAngles = Vector3.Slerp(myOwner.transform.eulerAngles, targRotation, Time.deltaTime * turnDurationTime);
        Vector3 forward = myOwner.transform.TransformDirection(Vector3.forward);
        if (!emergencyTurning) {
            /*myOwner.rbody.MovePosition(myOwner.transform.position + forward * myOwner.currSpeed * Time.deltaTime);*/
            myOwner.Move(forward * myOwner.currSpeed);
        }

        if(Time.time - startWander >= duration) { myOwner.changeState(new NPCIdle()); }
        if (myOwner.checkView())
        {
            // change state to aggro
            becomeAggro(myOwner.myType);
            // Debug.Log("I can see you!");
            return;
        }
    }

    public override void Exit()
    {
        if(headingGetter != null) { myOwner.StopCoroutine(headingGetter); }
    }

    void cliffCheck()
    {
        bool problem = false;
        Vector3 dir = (myOwner.transform.forward - myOwner.transform.up);
        Ray ray = new Ray(myOwner.transform.position, dir);
        RaycastHit[] rayHits = Physics.RaycastAll(ray, 3f);
        Debug.DrawRay(myOwner.transform.position, dir);
        if (rayHits.Length > 0)
        {
            foreach (RaycastHit hit in rayHits)
            {
                if (hit.collider.tag == "Wall")
                {
                    problem = true;
                    emergencyTurn();
                    // Debug.Log("That's a wall");
                }
            }
        }
        else {
            problem = true;
            // Debug.Log("That's a cliff");
        }
        if (problem) { emergencyTurn(); }
        else { emergencyTurning = false; }
    }

    public IEnumerator headingProcessing()
    {
        while (true)
        {
            calculateNewHeading();
            yield return new WaitForSeconds(turnDurationTime);
            turnDurationTime = Random.Range(2f, 3f);
        }
    }

    void calculateNewHeading()
    {
        emergencyTurning = false;
        float heading = myOwner.transform.eulerAngles.y;
        float floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
        float ceil = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
        heading = Random.Range(floor, ceil);
        targRotation = new Vector3(0, heading, 0);
    }

    void emergencyTurn()
    {
        emergencyTurning = true;
        float heading = myOwner.transform.eulerAngles.y;
        int mod = 1;
        if(Random.value < 0.5f) { mod = -1; }
        heading = Mathf.Clamp(heading + 180 * mod, 0, 360f);
        targRotation = new Vector3(0, heading, 0);
    }

    void becomeAggro(EnemyData.CombatType combatType)
    {
        switch (combatType)
        {
            case EnemyData.CombatType.Melee:
                myOwner.changeState(new MeleeEnemyChase());
                break;
            case EnemyData.CombatType.SpellCaster:
                myOwner.changeState(new SpellCasterEnemyAggro());
                break;
            case EnemyData.CombatType.Mixed:
                break;
            case EnemyData.CombatType.Ranged:
                break;
            case EnemyData.CombatType.Support:
                break;
        }
    }
}

public class NPCSeduced : NPCState
{
    Transform originAttackEnemy;

    public override void Enter(Movement owner, float newDuration)
    {
        state = stateType.Seduced;
        duration = newDuration;
        myOwner = owner;
        anim = myOwner.anim;
        originAttackEnemy = owner.attackTarget;
        myOwner.attackTarget = null;
    }

    public override void Enter(Movement owner)
    {
        base.Enter(owner);
        state = stateType.Seduced;
        myOwner = owner;
        anim = myOwner.anim;
        originAttackEnemy = owner.attackTarget;
        myOwner.attackTarget = null;
    }

    public override void Execute()
    {
        // if attack target is no longer null, that means your crush is fighting something!
        if (myOwner.attackTarget != null) { becomeAggro(myOwner.myType); } // go murder for senpai
        else { approachCrush(); } // go follow senpai
    }

    public override void Exit()
    {
        myOwner.attackTarget = myOwner.blueprint.getOriginTarget();
        // myOwner.crush.removeFromSeductionList(myOwner.GetComponent<Damageable>());
        myOwner.crush = null;
        Debug.Log(myOwner == null);
        myOwner.changeState(new MeleeEnemyScan());
    }

    void approachCrush()
    {
        // Make sure we have a crush.
        if(myOwner.crush == null) { Exit(); }

        // Calculate rotation
        Vector3 crushPos = myOwner.crush.returnBody().position; // your crush's location
        crushPos.y = 0; // for calculating the forward
        Vector3 myPos = myOwner.transform.position; // my position
        myPos.y = 0; // for calculating the forward
        Quaternion forward = Quaternion.LookRotation(crushPos - myPos);
        myOwner.transform.rotation = Quaternion.Lerp(myOwner.transform.rotation, forward, 5f * Time.deltaTime);

        // if I am too far from crush, move towards crush
        float dist = Vector3.Distance(myOwner.crush.returnBody().position, myOwner.transform.position);
        if(dist > 4f) {
            myOwner.Move(myOwner.transform.forward * myOwner.currSpeed);
            // set an animation bool to "bouncy walking" or something
        }
        else {
            // set an animation bool to "bounce idling" or something
        }
    }

    void attackEnemy()
    {
        myOwner.changeState(new MeleeEnemyScan());
    }

    void becomeAggro(EnemyData.CombatType combatType)
    {
        switch (combatType)
        {
            case EnemyData.CombatType.Melee:
                myOwner.changeState(new MeleeEnemyChase());
                break;
            case EnemyData.CombatType.SpellCaster:
                myOwner.changeState(new SpellCasterEnemyAggro());
                break;
            case EnemyData.CombatType.Mixed:
                break;
            case EnemyData.CombatType.Ranged:
                break;
            case EnemyData.CombatType.Support:
                break;
        }
    }
}

public class MeleeEnemyChase : NPCState
{
    Transform attackTarget;
    Vector3 lastKnownLocation;

    public override void Enter(Movement owner)
    {
        state = stateType.Aggro;
        myOwner = owner;
        myOwner.currSpeed = myOwner.maxSpeed;
        myOwner.Head.forward = myOwner.transform.forward;
        attackTarget = myOwner.attackTarget;
        lastKnownLocation = attackTarget.position;
        anim = myOwner.anim;
        anim.SetInteger("Status", 2);
        Debug.Log("Entering Chase...");
    }

    public override void Execute()
    {
        if (clearShot()) {
            chaseTarget();
        }
        else {
            goToLastLocation();
        }
    }

    bool clearShot()
    {
        if(attackTarget == null) { return false; }
        RaycastHit rayHit;
        if (Physics.Raycast(myOwner.transform.position, attackTarget.position - myOwner.transform.position, out rayHit, myOwner.sightRange)) {
            if(rayHit.transform == attackTarget) { return true; }
        }
        return false;
    }

    void chaseTarget()
    {
        Vector3 myOwnerPos = new Vector3(myOwner.transform.position.x, 0, myOwner.transform.position.z);
        Vector3 targetPos = new Vector3(myOwner.attackTarget.position.x, 0, myOwner.attackTarget.position.z);
        lastKnownLocation = attackTarget.position;

        Vector3 dir = (targetPos - myOwnerPos).normalized;
        Quaternion lookDir = Quaternion.LookRotation(dir);
        // myOwner.transform.forward = dir;
        myOwner.transform.rotation = Quaternion.Lerp(myOwner.transform.rotation, lookDir, Time.deltaTime * 6f);
        myOwner.Head.forward = myOwner.transform.forward;

        /*myOwner.rbody.MovePosition(myOwner.transform.position + myOwner.transform.forward * myOwner.currSpeed * Time.deltaTime);*/
        myOwner.Move(myOwner.transform.forward * myOwner.currSpeed);

        if(Vector3.Distance(myOwner.transform.position, myOwner.attackTarget.position) < 1.5f) // replace 1f with range variable from myOwner
        {
            myOwner.StartCoroutine(myOwner.attack(attackTarget.position));
        }
        // check if close enough to attack
    }

    void goToLastLocation()
    {
        Vector3 myOwnerPos = new Vector3(myOwner.transform.position.x, 0, myOwner.transform.position.z);
        Vector3 lastKnownLoc = new Vector3(lastKnownLocation.x, 0, lastKnownLocation.z);
        Vector3 dir = (lastKnownLoc - myOwnerPos).normalized;

        Quaternion lookDir = Quaternion.LookRotation(dir);
        myOwner.transform.rotation = Quaternion.Lerp(myOwner.transform.rotation, lookDir, Time.deltaTime * 4f);
        myOwner.Head.forward = myOwner.transform.forward;

        // myOwner.rbody.MovePosition(myOwner.transform.position + myOwner.transform.forward * myOwner.currSpeed * Time.deltaTime);
        myOwner.Move(myOwner.transform.forward * myOwner.currSpeed);
        if (Vector3.Distance(myOwner.transform.position, lastKnownLocation) < 1f) {
            myOwner.changeState(new MeleeEnemyScan());
        }
    }

    public override void Exit()
    {
        myOwner.currSpeed = myOwner.baseSpeed;
    }
}

public class MeleeEnemyScan : NPCState
{
    public override void Enter(Movement owner)
    {
        base.Enter(owner);
        state = stateType.Aggro;
        anim.SetInteger("Status", 1);
        anim.Play("Scan");
        Debug.Log("Entering Scan...");
    }

    public override void Execute()
    {
        if (myOwner.checkView()) {
            myOwner.changeState(new MeleeEnemyChase());
        }
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Scan")) {
            myOwner.changeState(new NPCWander());
        }
    }

    public override void Exit()
    {
        
    }
}

/*
public class MeleeEnemySeduced : NPCState
{
    Transform master;

    public override void Enter(Movement owner) // assumes new attackTarget has been established.
    {
        myOwner = owner;
        // myOwner.rbody = owner.rbody;
        myOwner.currSpeed = myOwner.maxSpeed;
        master = myOwner.attackTarget;
    }

    public override void Execute()
    {
        if(myOwner.attackTarget == master) {
            fawn();
        }
        else if(myOwner.attackTarget == null) {
            myOwner.changeState(new NPCIdle());
        }
        else {
            attackTarget();
        }
    }

    void fawn()
    {
        float dist = Vector3.Distance(myOwner.transform.position, myOwner.attackTarget.position);
        Vector3 dirModded = myOwner.attackTarget.position - myOwner.transform.position;
        dirModded.y = 0;
        myOwner.transform.rotation = Quaternion.Lerp(myOwner.transform.rotation, Quaternion.LookRotation(dirModded), 4f * Time.deltaTime);
        if (dist > 5f)
        {
            Vector3 dir = dirModded.normalized;
            // myOwner.rbody.MovePosition(dir * myOwner.currSpeed * Time.deltaTime);
            myOwner.Move(dir * myOwner.currSpeed);
        }
    }

    void attackTarget()
    {
        float dist = Vector3.Distance(myOwner.transform.position, myOwner.attackTarget.position);
        Vector3 dirModded = myOwner.attackTarget.position - myOwner.transform.position;
        dirModded.y = 0;
        myOwner.transform.rotation = Quaternion.Lerp(myOwner.transform.rotation, Quaternion.LookRotation(dirModded), 4f * Time.deltaTime);
        if(dist <= 1f) {
            myOwner.StartCoroutine(myOwner.attack(myOwner.attackTarget.position));
        }
        else {
            Vector3 dir = dirModded.normalized;
            // myOwner.rbody.MovePosition(dir * myOwner.currSpeed * Time.deltaTime);
            myOwner.Move(dir * myOwner.currSpeed);
        }
    }

    public override void Exit()
    {
        
    }
}
*/
