using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpellCasterMovement : Movement, SpellCaster
{
    #region Public Variables
    SpellBook heldSpell;

    public float pickupSpeed;

    [SerializeField] Transform gun;
    [SerializeField] Transform body;

    public CharacterController charCon;

    [SerializeField] float yMove = Physics.gravity.y;
    bool falling = true;

    public delegate void seductionHit(Damageable target, SpellCaster owner);
    public event seductionHit changeFollowerTarget;

    #endregion

    #region Movement Implementations

    public override void Start()
    {
        hamper = 0;
        charCon = GetComponent<CharacterController>();
        yMove = Physics.gravity.y;
        blueprint.setup(this);
        currSpeed = baseSpeed;
    }

    public override void Update() {
        base.Update();
    }

    void FixedUpdate()
    {
        if (falling) {
            yMove += Time.deltaTime * Physics.gravity.y;
        }
        Move(Vector3.up * yMove * Time.deltaTime);
    }

    public override void Move(Vector3 movement)
    {
        if (!charCon.enabled) { return; }
        charCon.Move(movement);
    }

    public override void knockBack(Vector3 dir, float force)
    {
        StartCoroutine(processKnockBack(dir * force));
    }

    IEnumerator processKnockBack(Vector3 dir)
    {
        hamper++;
        yMove = dir.y;
        charCon.Move(dir * Time.deltaTime);
        falling = true;
        Vector3 flatForce = dir;
        flatForce.y = 0;
        while (!charCon.isGrounded)
        {
            charCon.Move(flatForce * Time.deltaTime);
            // charCon.Move(Vector3.up * yMove * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        Vector3 start = flatForce;
        float prog = 0f;
        while (flatForce != Vector3.zero)
        {
            charCon.Move(flatForce * Time.deltaTime);
            prog += Time.deltaTime;
            flatForce = Vector3.Lerp(start, Vector3.zero, prog);
            yield return new WaitForEndOfFrame();
        }
        hamper--;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(Vector3.Distance(hit.point, transform.position + Vector3.up * charCon.height / 2) < 0.1f) {
            yMove = 0f;
            return;
        }
        if (charCon.isGrounded) {
            falling = false;
            yMove = Physics.gravity.y;
            return;
        }
    }

    public override IEnumerator attack(Vector3 target)
    {
        hamper++;
        Vector3 targetingDir = target - transform.position;
        targetingDir.y = 0;
        gun.forward = targetingDir;
        anim.Play("Attack");
        bool fired = false;
        if (heldSpell != null) { heldSpell.FireSpell(); }
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if(anim.GetCurrentAnimatorStateInfo(0).length >= 0.5f && !fired) {
                fired = true;
                // if (heldSpell != null) { heldSpell.primaryEffect.ActivateSpell(this, heldSpell.secondaryEffect); }
            }
            yield return new WaitForEndOfFrame();
        }
        hamper--;
    }

    #endregion

    #region SpellCaster Implementations
    public void pickUpSpell(SpellBook newSpell)
    {
        if(heldSpell != null) { dropSpell(heldSpell, newSpell.transform.position); }
        heldSpell = newSpell;
        heldSpell.Deactivate();
        StartCoroutine(pullSpell(newSpell.transform));
    }

    IEnumerator pullSpell(Transform spellTrans)
    {
        spellTrans.parent = transform;
        while(Vector3.Distance(spellTrans.position, transform.position) > 0.2f) {
            spellTrans.position = Vector3.Lerp(spellTrans.position, transform.position, Time.deltaTime * pickupSpeed);
            yield return new WaitForEndOfFrame();
        }
        spellTrans.localPosition = Vector3.zero;
    }

    public void addToSeductionList(Damageable loser)
    {
        
    }

    public void dropSpell(SpellBook dropSpell, Vector3 originPos)
    {
        if(heldSpell == dropSpell)
        {
            heldSpell.transform.parent = null;
            heldSpell = null;
        }
    }

    public void invokeChangeFollowers(Damageable target)
    {

    }

    public Transform returnGun() { return gun; }

    public Transform returnBody() { return body; }

    public Transform returnHead() { return transform; }

    public Transform returnTransform() { return transform; }

    public void getHitList(List<Damageable> hitList, SpellCaster owner)
    {
        
    }

    public void initiateSeduction(float duration)
    {
        
    }

    public void removeFromSeductionList(Damageable loser)
    {
        
    }

    public SpellBook returnSpell()
    {
        if(heldSpell == null) { return null; }
        return heldSpell;
    }

    public void fireSpell()
    {
        
    }

    public bool canShoot()
    {
        throw new NotImplementedException();
    }

    public void setCanShoot(bool can)
    {
        throw new NotImplementedException();
    }

    #endregion
}
