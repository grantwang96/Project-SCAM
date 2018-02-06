using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : Movement {

    public Collider FistColl;
    bool attacking;
    bool falling = true;

    float yMove = Physics.gravity.y;
    Coroutine movementTakeOver;

    public CharacterController charCon;

    public override void Start()
    {
        setup();
        hamper = 0;
        charCon = GetComponent<CharacterController>();
        yMove = Physics.gravity.y;
        blueprint.setup(this);
        currSpeed = baseSpeed;
    }

    public override void Update()
    {
        processMovement();
    }

    void FixedUpdate()
    {
        if (falling) {
            yMove += Time.deltaTime * Physics.gravity.y;
        }
        Move(Vector3.up * yMove * Time.deltaTime);
    }

    public override void setup()
    {
        base.setup();
        FistColl.GetComponent<FistScript>().damage = damage;
    }

    public override void processMovement()
    {
        base.processMovement();
    }

    public override void Move(Vector3 movement) {
        if (charCon == null || !charCon.enabled) { return; }
        if (charCon.isGrounded) {
            currVel = Vector3.Lerp(currVel, movement, friction);
            charCon.Move(currVel * Time.deltaTime);
        }
        else {
            currVel = Vector3.Lerp(currVel, movement, friction * 0.1f);
            charCon.Move(currVel * Time.deltaTime);
        }
    }

    public override void Teleport(Vector3 newLocation)
    {
        Vector3 dir = newLocation - transform.position;
        if(charCon.enabled) { charCon.Move(dir); }
    }

    public override void knockBack(Vector3 dir, float force)
    {
        if (movementTakeOver != null) { StopCoroutine(movementTakeOver); hamper--; }
        movementTakeOver = StartCoroutine(processKnockBack(dir, force));
    }

    IEnumerator processKnockBack(Vector3 dir, float force)
    {
        hamper++;
        yMove = dir.y;
        Move(dir * Time.deltaTime);
        falling = true;
        Vector3 flatForce = dir;
        flatForce.y = 0;
        while (!charCon.isGrounded)
        {
            Move(flatForce * Time.deltaTime);
            // charCon.Move(Vector3.up * yMove * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        Vector3 start = flatForce;
        float prog = 0f;
        while (flatForce != Vector3.zero)
        {
            Move(flatForce * Time.deltaTime);
            prog += Time.deltaTime;
            flatForce = Vector3.Lerp(start, Vector3.zero, prog);
            yield return new WaitForEndOfFrame();
        }
        hamper--;
    }

    public override void changeState(NPCState newState)
    {
        base.changeState(newState);
    }

    public override void changeState(NPCState newState, float newDuration)
    {
        base.changeState(newState, newDuration);
    }

    public override IEnumerator attack(Vector3 target)
    {
        if (attacking) { yield return null; }
        attacking = true;
        hamper++;
        FistColl.enabled = true;
        // play attack animation
        anim.Play("Attack");
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            yield return new WaitForEndOfFrame();
        }
        hamper--;
        FistColl.enabled = false;
        attacking = false;
        // get attack animation length
        // Do attack processing like hitbox, spell spawning, etc.
        // yield return new WaitForSeconds(1f); // set clip length here
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Vector3.Distance(hit.point, transform.position + Vector3.up * charCon.height / 2) < 0.1f)
        {
            yMove = 0f;
            return;
        }
        if (charCon.isGrounded)
        {
            falling = false;
            yMove = Physics.gravity.y;
            return;
        }
    }
}
