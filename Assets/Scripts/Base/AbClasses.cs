using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Damageable : MonoBehaviour
{
    // health variables
    public int max_health;
    [SerializeField] int _health;
    public int health {
        get { return _health; }
        set { _health = value; } }

    public bool transmutable = true; // if the object can be transformed

    // hurt/iframes variables
    public bool hurt = false;
    public float hurtTime;
    public bool dead;
    public bool damageable = true;

    // rbody or character controller
    // public Rigidbody rbody;
    // public CharacterController charCon;
    public Collider myCollider;
    public MeshRenderer myRend;

    public Vector3 originSpawn;
    public Movement myMovement;
    public Damageable parentHit;

    public Damageable replacedBody; // for transmutations
    
    public Coroutine seduction;
    public SpriteRenderer blush;
    // public Vector3 blushScale;

    public virtual void Start()
    {
        // rbody = GetComponent<Rigidbody>();
        // charCon = GetComponent<CharacterController>();
        myRend = GetComponent<MeshRenderer>();
        myMovement = GetComponent<Movement>();
        health = max_health;
        originSpawn = transform.position;
    }

    public virtual void Update() {
        if(dead && damageable) { // if we're dead
            StopAllCoroutines();
            Die();
            damageable = false;
        }
    }

    public virtual void TakeDamage(Transform attacker, int hpLost, Vector3 dir, float force)
    {
        // if this is a result of transmutation
        if(parentHit != null && parentHit != this) { // apply damage to base object (i.e. if this were a transmutation). DO NOT MAKE PARENT HIT THIS!!! STACKOVERFLOWS ARE A BITCH
            parentHit.TakeDamage(attacker, hpLost, dir, force);
            return;
        }

        // calculate damage dealt
        health -= hpLost;
        if(health <= 0 && !dead) { dead = true; } // if this damage kills you
        
        knockBack(dir, force);
    }

    public virtual void Heal(int recover)
    {
        if(parentHit != null) { parentHit.Heal(recover); }
        health += recover;
        if(health > max_health) { health = max_health; }
    }

    public virtual void knockBack(Vector3 dir, float force)
    {
        if(myMovement == null) { 
            GetComponent<Rigidbody>().AddForce(dir * force, ForceMode.Impulse); return;
        }
        else { myMovement.knockBack(dir, force); }
    }

    public virtual void Fly(float force, float duration)
    {

    }

    public virtual void InitiateTransmutation(float duration, GameObject replacement)
    {
        StartCoroutine(processTransmutation(duration, replacement));
    }

    public virtual IEnumerator processTransmutation(float duration, GameObject replacement)
    {
        myMovement.hamper++;
        myCollider.enabled = false;
        Renderer[] allRends = GetComponentsInChildren<Renderer>();
        if (allRends.Length > 0) {
            foreach(Renderer rend in allRends) { rend.enabled = false; }
        }
        GameObject myReplace = Instantiate(replacement, transform.position, transform.rotation);
        Rigidbody replaceRigidBody = myReplace.GetComponent<Rigidbody>();
        replaceRigidBody.AddExplosionForce(3f, transform.position, 1f);
        replacedBody = myReplace.GetComponent<Damageable>();
        replacedBody.setTransmutable(false);

        float time = 0f;
        while(time < duration) {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
            transform.position = replacedBody.transform.position;
        }

        transform.position = myReplace.transform.position;
        Destroy(myReplace); // Destroy my replacement
        myCollider.enabled = true;
        if (allRends.Length > 0) {
            foreach (Renderer rend in allRends) { rend.enabled = true; }
        }
        replacedBody = null;
        myMovement.hamper--;
    }

    public virtual void setTransmutable(bool newBool)
    {
        transmutable = newBool;
    }

    public virtual void Seduce(float duration, GameObject target, SpellCaster owner)
    {
        Debug.Log("Seduced!");
        if(myMovement.crushTarget != null && myMovement.crush != null) { // if already seduced
            myMovement.crush.removeFromSeductionList(this); // remove from the seduction list
        }
        if(seduction != null) { StopCoroutine(seduction); }
        myMovement.attackTarget = null;
        Debug.Log(owner);
        myMovement.crushTarget = owner.returnTransform();
        myMovement.crush = myMovement.crushTarget.GetComponent<SpellCaster>();
        if(myMovement.crush != null) { myMovement.crush.addToSeductionList(this); }
        else { Debug.Log("No spellcaster component!"); }

        seduction = StartCoroutine(processSeduction(duration, target, owner));
    }

    public virtual IEnumerator processSeduction(float duration, GameObject target, SpellCaster owner)
    {
        yield return null;
        seduction = null;
    }

    public virtual void setCurrentTarget(Damageable target, SpellCaster owner)
    {
        if(target == this) { return; }
        myMovement.attackTarget = target.transform;
    }

    public virtual void vortexGrab(Transform center, float force)
    {
        // Vector3 dir = (center.position - transform.position).normalized;
        // rbody.AddForce(dir * force);
        // transform.forward = new Vector3(-dir.x, 0, -dir.z);
    }

    public virtual void Die()
    {
        dead = true;
//        Destroy(gameObject);
		//keeping disabled for checkpoint restoration
		CheckpointManager.Instance.AddEnemyToRespawnList(this);
		gameObject.SetActive(false);
    }
}

[System.Serializable]
public abstract class Movement : MonoBehaviour
{
    public float baseSpeed;
    public float maxSpeed;
    public float currSpeed;
    [SerializeField] public float maxWanderDistance;

    public float friction = 1f;
    public Vector3 currVel;

    public float sightRange;
    public float sightAngle;

    public Transform Head;

    public Damageable myDamageable;
    public Rigidbody rbody;
    public Animator anim;
    public NavMeshAgent agent;
    public EnemyData blueprint;
    public EnemyData.CombatType myType;
    public bool sophisticated;

    NPCState currState;
    public NPCState getCurrentState() { return currState; }

    public int damage;
    public int hamper;
    [Range(0, 1)] public float attackDotProd;
    public Coroutine attackRoutine;

    #region For Seeing things
    [SerializeField] int numRaycasts;
    [SerializeField] float raySpread;
    [SerializeField] float obstacleCheckRange;
    public LayerMask groundLayers;
    public LayerMask scanLayer;
    public LayerMask obstacleLayer;
    public LayerMask pathFindingLayers;
    #endregion

    public Transform attackTarget; // who to target

    #region Seduction Stuff
    public Transform crushTarget; // if seduced
    public SpellCaster crush; // if seduced
    #endregion

    public virtual void Awake()
    {
        setup();
    }

	public virtual void Start(){}

	protected virtual void OnEnable() 
	{
		CheckpointManager.OnReset += ToIdle;
	}

	protected virtual void OnDisable()
	{
		CheckpointManager.OnReset -= ToIdle;
	}

	protected abstract void ToIdle();

    public virtual void Update()
    {
        processMovement();
    }

    public virtual void setup()
    {
        hamper = 0;
        rbody = GetComponent<Rigidbody>();
        blueprint.setup(this);
        currSpeed = baseSpeed;
        friction = 1f;
        // setup currState
    }

    public virtual void processMovement()
    {
        if(currState != null && hamper <= 0) {
            currState.Execute();
        }
    }

    public Transform obstruction() {
        RaycastHit[] rayHits = Physics.RaycastAll(
            Head.position, agent.desiredVelocity, agent.radius, obstacleLayer, QueryTriggerInteraction.Ignore);
        Debug.DrawRay(Head.position, agent.desiredVelocity, Color.green);
        foreach (RaycastHit rayhit in rayHits) {
            if(rayhit.collider.tag == "Wall" || rayhit.collider.tag == "Ground") {
                // Debug.Log("Obstruction: " + rayhit.transform);
                return rayhit.transform;
            }
        }
        return null;
    }

    public bool attemptInteract(Interactable inter) {
        StartCoroutine(pauseAgent(2f));
        return inter.Interact();
    }

    IEnumerator pauseAgent(float time) {
        if(agent == null || !agent.enabled) { yield break; }
        agent.isStopped = true;
        yield return new WaitForSeconds(time);
        agent.isStopped = false;
    }

    public Vector3 getRandomLocation(Vector3 origin, float range)
    {

        Vector3 randPos = Random.insideUnitSphere * range;
        randPos += origin;

        NavMeshHit navHit;
        if(NavMesh.SamplePosition(randPos, out navHit, range, NavMesh.AllAreas)) {
            return navHit.position;
        }

        return transform.position;
    }

    public virtual void Move(Vector3 movement)
    {
        rbody.MovePosition(transform.position + movement);
    }

    public virtual void Stop()
    {
        rbody.velocity = Vector3.zero;
    }

    public virtual void Teleport(Vector3 newLocation, Vector3 offset) {
        rbody.MovePosition(newLocation + offset);
    }

    public virtual bool checkView()
    {
        if(attackTarget == null || !attackTarget.gameObject.activeInHierarchy) { 
			attackTarget = null;
			return false; 
		}

        float dist = Vector3.Distance(transform.position, attackTarget.position);
        float angle = Vector3.Angle(Head.forward, attackTarget.position - Head.position);

        if(dist <= sightRange && angle <= sightAngle) {
            float angleInterval = 360 / numRaycasts;
            RaycastHit rayHit;
            Debug.DrawRay(Head.position, attackTarget.position - Head.position, Color.yellow);
            if (Physics.Raycast(Head.position, (attackTarget.position - Head.position).normalized, out rayHit, sightRange, scanLayer)) {
                if (rayHit.collider.transform == attackTarget) { return true; }
                Damageable dam = rayHit.collider.GetComponent<Damageable>();
                if(dam != null && dam.parentHit != null) {
                    if(dam.parentHit.transform == attackTarget) { return true; }
                }
            }

            for (int i = 0; i < numRaycasts; i++) {
                float ang = angleInterval * i;

                Vector3 offset = new Vector3();
                offset.x = transform.InverseTransformDirection(attackTarget.position).x + raySpread * Mathf.Sin(ang * Mathf.Deg2Rad);
                offset.y = transform.InverseTransformDirection(attackTarget.position).y + raySpread * Mathf.Cos(ang * Mathf.Deg2Rad);
                offset.z = transform.InverseTransformDirection(attackTarget.position).z;
                offset = transform.TransformDirection(offset);

                if (Physics.Raycast(Head.position, (offset - Head.position).normalized, out rayHit, sightRange, scanLayer)) {
                    if (rayHit.collider.transform == attackTarget) { return true; }
                    Damageable dam = rayHit.collider.GetComponent<Damageable>();
                    if (dam != null && dam.parentHit != null) {
                        if (dam.parentHit.transform == attackTarget) { return true; }
                    }
                }
                Debug.DrawRay(Head.position, offset - Head.position, Color.yellow);
            }
        }
        return false;
    }

    public virtual void changeState(NPCState newState)
    {
        if(currState != null) { currState.Exit(); }
        currState = newState;
        currState.Enter(this);
    }

    public virtual void changeState(NPCState newState, float newDuration)
    {
        if (currState != null) { currState.Exit(); }
        currState = newState;
        currState.Enter(this, newDuration);
    }

    public virtual void changeState(NPCState newState, NPCState prevState)
    {
        // no need to exit previous state(will be returning later, anyways)
        currState = newState;
        currState.Enter(this, prevState);
    }

    public virtual void changeState(NPCState newState, NPCState prevState, float newDuration)
    {
        if (currState != null) { currState.Exit(); }
        currState = newState;
        currState.Enter(this, prevState, newDuration);
    }

    public virtual IEnumerator attack(Vector3 target)
    {
        // hamper++;
        float startTime = Time.time;
        // play attack animation
        anim.Play("Attack");
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
            yield return new WaitForEndOfFrame();
        }
        attackRoutine = null;
        // hamper--;
        // get attack animation length
        // Do attack processing like hitbox, spell spawning, etc.
        // yield return new WaitForSeconds(1f); // set clip length here
    }

    public virtual void knockBack(Vector3 dir, float force)
    {

    }

    public virtual void vortexGrab(Transform center, float force, float duration)
    {

    }
}

public abstract class NPCState
{
    public enum stateType {
        Normal, Aggro, Seduced
    }

    public stateType state;

    public NPCState previousState;
    public float prevStateDuration;

    public float duration;
    public float idleTime;
    public float stateStartTime;
    public float sightRange;
    public Movement myOwner;

    public Vector3 forward;
    public Vector3 currRotation;
    public Quaternion targetRotation;

    public float turnDurationTime;
    public float maxAngleChange = 60f;
    public float heading;

    public Animator anim;

    public virtual void Enter(Movement owner)
    {
        myOwner = owner;
        duration = Random.Range(4f, 6f);
        stateStartTime = Time.time;
        targetRotation = Quaternion.Euler(0, 0, 0);
        turnDurationTime = Random.Range(1f, 2f);
        anim = myOwner.anim;
        maxAngleChange = 60f;
        if(myOwner.agent.enabled) { myOwner.agent.ResetPath(); }
    }

    public virtual void Enter(Movement owner, float newDuration)
    {
        myOwner = owner; // save the owner
        duration = newDuration; // set the duration
        stateStartTime = Time.time;

        anim = myOwner.anim;
        anim.SetInteger("Status", 0);
    }

    public virtual void Enter(Movement owner, NPCState prevState) // if you want to return to a specific state
    {
        myOwner = owner;
        anim = myOwner.anim;
        previousState = prevState;
    }

    public virtual void Enter(Movement owner, NPCState prevState, float newDuration) // if you're "resuming" a previous state
    {
        myOwner = owner;
        anim = myOwner.anim;
        previousState = prevState;
        duration = newDuration;
    }

    public virtual void Execute()
    {
        myOwner.Head.transform.localRotation = Quaternion.Slerp(myOwner.Head.localRotation, targetRotation, Time.deltaTime * turnDurationTime);
        myOwner.transform.eulerAngles = new Vector3(0, myOwner.transform.eulerAngles.y, 0);
    }

    public virtual void Exit()
    {
        // Perform last second actions...
    }

    public virtual void becomeAggro(EnemyData.CombatType combatType)
    {
        switch (combatType)
        {
            case EnemyData.CombatType.Melee:
                // myOwner.changeState(new MeleeEnemyChase());
                break;
            case EnemyData.CombatType.SpellCaster:
                // myOwner.changeState(new SpellCasterEnemyAggro());
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