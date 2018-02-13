using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardDamageable : Damageable {

    public Rigidbody rbody;
    Coroutine knockBackRoutine;

    public override void knockBack(Vector3 dir, float force)
    {
        // rbody.AddForce(dir * force, ForceMode.Impulse);
        if(knockBackRoutine != null) {
            StopCoroutine(knockBackRoutine);
        }
        knockBackRoutine = StartCoroutine(knockingBack(dir, force));
    }

    IEnumerator knockingBack(Vector3 dir, float force)
    {
        myMovement.agent.isStopped = true;
        myMovement.agent.updatePosition = false;
        myMovement.agent.updateRotation = false;

        float groundTime = 0f;
        rbody.AddForce(Vector3.up * force, ForceMode.Impulse);
        Debug.Log("Force is: " + force);

        while (groundTime < .3f) {
            if (rbody.velocity.y == 0) { groundTime += Time.deltaTime; }
            yield return new WaitForEndOfFrame();
        }
        myMovement.agent.Warp(transform.position);
        myMovement.agent.isStopped = false;
        myMovement.agent.updatePosition = true;
        myMovement.agent.updateRotation = true;
        knockBackRoutine = null;
    }

    public override IEnumerator processTransmutation(float duration, GameObject replacement)
    {
        myMovement.hamper++;
        GetComponent<SpellCaster>().setCanShoot(false);

        // shut off the renderers
        Collider myColl = GetComponent<Collider>();
        myColl.enabled = false;
        Renderer[] allRends = GetComponentsInChildren<Renderer>();
        if (allRends.Length > 0) { foreach (Renderer rend in allRends) { rend.enabled = false; } }

        // stop the navmesh agent
        myMovement.agent.isStopped = true;

        // Create the replacement object
        GameObject myReplace = Instantiate(replacement, transform.position, transform.rotation);
        Rigidbody replaceRigidBody = myReplace.GetComponent<Rigidbody>();
        replaceRigidBody.AddExplosionForce(3f, transform.position, 1f);
        replacedBody = myReplace.GetComponent<Damageable>();
        replacedBody.setTransmutable(false);

        // wait for the spell duration
        yield return new WaitForSeconds(duration);

        // move to transmuted object(in case object was moved)
        myMovement.agent.nextPosition = myReplace.transform.position;
        transform.position = myMovement.agent.nextPosition;
        
        Destroy(myReplace); // Destroy my replacement

        // reaactivate colliders and renderers
        myColl.enabled = true;
        if (allRends.Length > 0) { foreach (Renderer rend in allRends) { rend.enabled = true; } }
        replacedBody = null;
        myMovement.hamper--;
        GetComponent<SpellCaster>().setCanShoot(true);
    }

    public override IEnumerator processSeduction(float duration, GameObject target, Transform owner)
    {
        myMovement.changeState(new WizardEnemySeduced(), duration);
        yield return new WaitForSeconds(duration);
        myMovement.changeState(new WizardEnemyIdle());
        seduction = null;
    }
}
