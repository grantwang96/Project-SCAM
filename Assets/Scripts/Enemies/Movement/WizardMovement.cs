using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class WizardMovement : Movement, SpellCaster {

    [SerializeField] List<SpellBook> heldSpells = new List<SpellBook>(); // the current spell held by this character
    private SpellBook currSpell;
    [SerializeField] bool canShootSpells;
    [SerializeField] Transform gun; // the gun that will shoot spells
    
    public bool canShoot() { return canShootSpells; }
    public void setCanShoot(bool can) { canShootSpells = can; }

    public override void setup()
    {
        base.setup();
        currSpell = heldSpells[0];
    }

    public override void Start()
    {
        canShootSpells = true;
        base.Start();
    }

    public override IEnumerator attack(Vector3 target)
    {
        Vector3 targetingDir = target - transform.position;
        targetingDir.y = 0;
        gun.forward = targetingDir;
        anim.Play("Attack");
        bool fired = false;
        yield return new WaitForFixedUpdate();
        currSpell = heldSpells[Random.Range(0, heldSpells.Count)];
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
            if (anim.GetCurrentAnimatorStateInfo(0).length >= 0.5f && !fired) {
                fired = true;
                fireSpell();
            }
            yield return new WaitForEndOfFrame();
        }
        attackRoutine = null;
    }

    public void pickUpSpell(SpellBook newSpell)
    {
        /*
        if(heldSpells.Count != 0) {
            SpellBook loseSpell = heldSpells[Random.Range(0, heldSpells.Count)];
            if (loseSpell != null) { dropSpell(loseSpell, newSpell.transform.position); }
        }*/
        heldSpells.Add(newSpell);
        newSpell.Deactivate();
        newSpell.transform.parent = transform;
        newSpell.transform.localPosition = Vector3.zero;
        newSpell.transform.localRotation = Quaternion.identity;
    }

    public void dropSpell(SpellBook dropSpell, Vector3 originPos)
    {
        if (dropSpell.owner == this.GetComponent<SpellCaster>()) { dropSpell.owner = null; }
        dropSpell.Activate();
        dropSpell.transform.parent = null;
        dropSpell.transform.position = transform.position;
    }

    public void fireSpell()
    {
        if(currSpell != null && canShootSpells) { currSpell.primaryEffect.ActivateSpell(this, currSpell.secondaryEffect, gun.forward); }
        else { Debug.Log("I don't has spell"); }
    }

    public void invokeChangeFollowers(Damageable target)
    {
        
    }
    
    public void addToSeductionList(Damageable loser)
    {

    }

    public void removeFromSeductionList(Damageable loser)
    {

    }

    public Transform returnBody()
    {
        return transform;
    }

    public Transform returnTransform() { return transform; }

    public Transform returnGun()
    {
        return gun;
    }

    public Transform returnHead()
    {
        return Head;
    }

    public SpellBook returnSpell()
    {
        if(currSpell == null) { return null; }
        return currSpell;
    }
}
