using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : Damageable
{
    [SerializeField] bool transmuted = false;

    Coroutine seduction;
    Transform attackTarget;
    SpellCaster myOwner;

    public GameObject getGameObject()
    {
        return gameObject;
    }

    public override void TakeDamage(Transform attacker, int damage, Vector3 dir, float force)
    {
        if(parentHit != null) {
            parentHit.TakeDamage(attacker, damage, dir, force);
            if (parentHit.dead) {
                dead = true;
            }
        }
        // TODO: handle elemental damage(fire, for example)
        knockBack(dir, force);
    }

    public override void InitiateTransmutation(float duration, GameObject replacement)
    {
        if (!transmutable) {
            Debug.Log("Cannot transmute!");
            return;
        }
        StartCoroutine(transmute(duration, replacement));
    }

    public IEnumerator transmute(float duration, GameObject replacement)
    {
        transmutable = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<MeshRenderer>().enabled = false;
        GameObject myReplacement = Instantiate(replacement, transform.position, transform.rotation);
        if (myReplacement.GetComponent<Rigidbody>() != null)
        {
            Debug.Log("Boosh!");
            myReplacement.GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
            myReplacement.GetComponent<Rigidbody>().angularVelocity = UnityEngine.Random.insideUnitSphere * 5f;
        }
        myReplacement.GetComponent<Damageable>().setTransmutable(false);
        yield return new WaitForSeconds(duration);
        Destroy(myReplacement);
        transform.position = myReplacement.transform.position;
        transform.rotation = myReplacement.transform.rotation;
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<MeshRenderer>().enabled = true;
        transmutable = true;
        GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
        GetComponent<Rigidbody>().angularVelocity = UnityEngine.Random.insideUnitSphere * 5f;
    }

    public override void setTransmutable(bool newBool)
    {
        transmutable = newBool;
        Debug.Log(transform.name + " transmutable was set to " + transmutable);
    }

    public override void vortexGrab(Transform center, float force)
    {
        Vector3 dir = (center.position - transform.position).normalized;
        GetComponent<Rigidbody>().AddForce(force * dir);
    }

    public void fly(float force, float duration)
    {

    }

    public override void Seduce(float duration, GameObject target, Transform owner)
    {
        myOwner = owner.GetComponent<SpellCaster>();
        if(seduction == null) { seduction = StartCoroutine(processSeduction(duration, target, owner)); }
    }

    public override IEnumerator processSeduction(float duration, GameObject target, Transform owner)
    {
        attackTarget = target.transform;
        float startTime = Time.time;
        Color originColor = GetComponent<MeshRenderer>().material.color;
        GetComponent<MeshRenderer>().material.color = new Color(255, 0, 155);
        while (Time.time - startTime < duration)
        {
            if (target.transform == attackTarget) { processSeducedMovement(target); }
            else { processSeducedAttack(); }
            yield return new WaitForEndOfFrame();
        }
        // myOwner.removeFromSeductionList(this.GetComponent<Damageable>());
        GetComponent<MeshRenderer>().material.color = originColor;
        myOwner = null;
        attackTarget = null;
        Transform dokiFX = transform.Find("Doki");
        if (dokiFX != null) {
            ParticleSystem dFX = dokiFX.GetComponent<ParticleSystem>();
            dFX.Stop();
            dokiFX.parent = null;
            Destroy(dokiFX.gameObject, dFX.startLifetime);
        }
        seduction = null;
    }

    void processSeducedMovement(GameObject target)
    {
        float loverDist = Vector3.Distance(transform.position, target.transform.position);
        Vector3 moveDir = (target.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(moveDir);
        if (loverDist > 5f) {
            GetComponent<Rigidbody>().velocity = moveDir * 10f;
        }
    }

    void processSeducedAttack()
    {
        if (attackTarget == null) { attackTarget = GameObject.Find("Player").transform; }
        float loverDist = Vector3.Distance(transform.position, attackTarget.transform.position);
        Vector3 moveDir = (attackTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(moveDir);
        GetComponent<Rigidbody>().velocity = moveDir * 20f;
    }

    public void setCurrentTarget(List<Damageable> targets, SpellCaster owner)
    {
        myOwner = owner;
        List<Damageable> goodTargets = new List<Damageable>();
        for (int i = 0; i < targets.Count; i++) {
            if (targets[i] != GetComponent<Damageable>()) { goodTargets.Add(targets[i]); }
        }
        if (goodTargets.Count > 0) {
            Damageable target = goodTargets[UnityEngine.Random.Range(0, goodTargets.Count)];
            attackTarget = target.transform;
        }
    }
}
