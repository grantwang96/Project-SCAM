using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : Damageable
{
    [SerializeField] bool transmuted = false;

    [SerializeField] Rigidbody rbody;
    Damageable attackTarget;
    SpellCaster myOwner;

    public override void Start()
    {
        rbody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
//        myRend = GetComponent<MeshRenderer>();
    }

    public GameObject getGameObject()
    {
        return gameObject;
    }

    public override void TakeDamage(Transform attacker, int damage, Vector3 dir, float force)
    {
        if(parentHit != null) {
            Debug.Log(parentHit.name + " got hurt!");
            parentHit.TakeDamage(attacker, damage, dir, force);
            if (parentHit.dead) {
                Die();
            }
        }
        // TODO: handle elemental damage(fire, for example)
        knockBack(dir, force);
    }

    public override void Die()
    {
        // you DON'T die
    }

    public override void InitiateTransmutation(float duration, GameObject replacement)
    {
        if (!transmutable) {
            Debug.Log("Cannot transmute!");
            return;
        }
        if(parentHit != null) { parentHit.InitiateTransmutation(duration, replacement); return; }
        if(transmutationProcess != null) { StopCoroutine(transmutationProcess); }
        if(replacedBody != null) {
            transform.position = replacedBody.transform.position;
            transform.rotation = replacedBody.transform.rotation;
            Destroy(replacedBody.gameObject);
        }
        transmutationProcess = StartCoroutine(transmute(duration, replacement));
    }

    public IEnumerator transmute(float duration, GameObject replacement)
    {
        // transmutable = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<MeshRenderer>().enabled = false;
        GameObject myReplacement = Instantiate(replacement, transform.position, transform.rotation);
        replacedBody = myReplacement.GetComponent<Damageable>();
        replacedBody.parentHit = this;
        if (myReplacement.GetComponent<Rigidbody>() != null) {
            Debug.Log("Boosh!");
            myReplacement.GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
            myReplacement.GetComponent<Rigidbody>().angularVelocity = UnityEngine.Random.insideUnitSphere * 5f;
        }
        // myReplacement.GetComponent<Damageable>().setTransmutable(false);
        yield return new WaitForSeconds(duration);
        transform.position = myReplacement.transform.position;
        transform.rotation = myReplacement.transform.rotation;
        Destroy(replacedBody.gameObject);
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<MeshRenderer>().enabled = true;
        transmutable = true;
        GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
        GetComponent<Rigidbody>().angularVelocity = UnityEngine.Random.insideUnitSphere * 5f;
        transmutationProcess = null;
    }

    public override void setTransmutable(bool newBool)
    {
        transmutable = newBool;
        // Debug.Log(transform.name + " transmutable was set to " + transmutable);
    }

    public override void vortexGrab(Transform center, float force)
    {
        Vector3 dir = (center.position - transform.position).normalized;
        GetComponent<Rigidbody>().AddForce(force * dir);
    }

    public void fly(float force, float duration)
    {

    }

    public override void Seduce(float duration, GameObject target, SpellCaster owner)
    {
        if(seduction != null) { StopCoroutine(seduction); }
        seduction = StartCoroutine(processSeduction(duration, target, owner));
    }

    public override IEnumerator processSeduction(float duration, GameObject target, SpellCaster owner)
    {
        // attackTarget = target.transform;
        float startTime = Time.time;

        owner.addToSeductionList(this);
        /*
        if(blush == null) {
            blush = Instantiate(GameManager.Instance.blushPrefab, transform).GetComponent<SpriteRenderer>();
            blush.transform.localPosition = Vector3.forward * myCollider.bounds.extents.z;
            Debug.Log(myCollider.bounds.extents.z);
        }
        blush.enabled = true;
        blush.transform.localScale = blushScale;*/
        blush.enabled = true;

        /*
        Color originColor = GetComponent<MeshRenderer>().material.color;
        GetComponent<MeshRenderer>().material.color = new Color(255, 0, 155);
        */

        while (Time.time - startTime < duration) {
            if (attackTarget == null) { processSeducedMovement(owner); }
            else { processSeducedAttack(); }
            yield return new WaitForEndOfFrame();
        }

        blush.enabled = false;
        owner.removeFromSeductionList(this);

        // myOwner.removeFromSeductionList(this.GetComponent<Damageable>());
        // GetComponent<MeshRenderer>().material.color = originColor;
        myOwner = null;
        attackTarget = null;
        seduction = null;
    }

    void processSeducedMovement(SpellCaster crush)
    {
        float loverDist = Vector3.Distance(transform.position, crush.returnBody().position);
        Vector3 moveDir = (crush.returnBody().position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(moveDir);
        if (loverDist > 5f) {
            GetComponent<Rigidbody>().velocity = moveDir * 10f;
        }
    }

    void processSeducedAttack()
    {
        if (attackTarget == null) { return; }
        if (attackTarget.dead) { attackTarget = null; return; }
        float loverDist = Vector3.Distance(transform.position, attackTarget.transform.position);
        Vector3 moveDir = (attackTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(moveDir);
        GetComponent<Rigidbody>().velocity = moveDir * 20f;
    }

    public override void setCurrentTarget(Damageable target, SpellCaster owner)
    {
        if (target == this) { return; }
        if(target == owner.returnBody()) { return; }
        attackTarget = target;
    }

	//public override void Die() {
	//	//do nothing wtf walls don't die
        //grant you're so insistent of this that you overwrote this twice
        //i should delete all this but it's 2AM before student showcase and i just wanna have some fun
        //--marcus
        //this block comment is my legacy
	//}
}
