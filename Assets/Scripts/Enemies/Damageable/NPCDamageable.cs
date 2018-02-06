using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDamageable : Damageable {

    Coroutine seduction;
    Coroutine transmutation;

    public CharacterController charCon; // humanoid type npc

    public MeshRenderer[] renderers; // All renderers on this gameobject

    public override void Seduce(float duration, GameObject target, Transform owner)
    {
        base.Seduce(duration, target, owner);
        if(seduction != null) {
            StopCoroutine(seduction);
            myMovement.hamper--;
        }
        seduction = StartCoroutine(processSeduction(duration, target, owner.GetComponent<SpellCaster>()));
    }

    IEnumerator processSeduction(float duration, GameObject target, SpellCaster owner)
    {
        myMovement.changeState(new NPCSeduced(), duration);

        // add hearts or whatever

        yield return new WaitForSeconds(duration); // wait for duration

        // stop being seduced
        if (myMovement.crush != null) {
            myMovement.crush.removeFromSeductionList(this);
        }
        myMovement.attackTarget = myMovement.blueprint.getOriginTarget();
        myMovement.changeState(new NPCIdle());
        seduction = null;
    }

    public override void InitiateTransmutation(float duration, GameObject replacement)
    {
        if (transmutable) {
            StartCoroutine(processTransmutation(duration, replacement));
        }
    }

    public override IEnumerator processTransmutation(float duration, GameObject replacement)
    {
        myMovement.hamper++;
        // Turn off transmutable
        transmutable = false;

        // Remove Collider
        Collider myColl = GetComponent<Collider>();
        myColl.enabled = false;

        // Freeze Rigidbody
        // rbody.constraints = RigidbodyConstraints.FreezeAll;

        foreach(MeshRenderer rend in renderers) // Turn off all mesh renderers(SECTION NEEDS TO BE MODIFIED ONCE MODELS ARE IN)
        {
            rend.enabled = false;
        }

        GameObject newBody = Instantiate(replacement, transform.position, transform.rotation); // Create a new replacement here
        Damageable newDam = newBody.GetComponent<Damageable>();
        newDam.setTransmutable(false); // Do not allow transmutations on this object either
        newDam.parentHit = this; // redirect damage to enemy
        // Attach some sparkly effect to indicate it is transmuted

        transform.position = Vector3.up * 10000;
            
        yield return new WaitForSeconds(duration); // Wait for this timeframe

        transform.position = newBody.transform.position; // move original body to newbody's position
        Destroy(newBody); // Get rid of old body

        foreach(MeshRenderer rend in renderers) { rend.enabled = true; } // re-enable mesh renderers
        // rbody.constraints = RigidbodyConstraints.FreezeRotation; // re-enable movement
        myColl.enabled = true; // re-enable collisions
        myMovement.hamper--;
        setTransmutable(true);
    } // Transmutation Co-routine

    public override void vortexGrab(Transform center, float force)
    {
        Vector3 dir = (center.position - transform.position).normalized;
        myMovement.knockBack(dir, force);
    }

    void becomeSeduced(EnemyData.CombatType combatType)
    {
        switch (combatType)
        {
            case EnemyData.CombatType.Melee:
                myMovement.changeState(new MeleeEnemySeduced());
                break;
            case EnemyData.CombatType.SpellCaster:
                break;
            case EnemyData.CombatType.Mixed:
                break;
            case EnemyData.CombatType.Ranged:
                break;
            case EnemyData.CombatType.Support:
                break;
        }
    }

    public override void Die()
    {
        // ScoreKeeper.Instance.incrementScore();
        Debug.Log("I died!");
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        if(replacedBody != null) { // if I'm transmuted, apply death effect to the replaced body
            replacedBody.Die();
        }
        else {
            // visualize the death

            myMovement.hamper += 1000;
            myMovement.anim.Play("Death");
            GetComponent<Collider>().enabled = false;
            yield return new WaitForEndOfFrame();
            while (myMovement.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                Debug.Log("I'm dying!");
                yield return new WaitForEndOfFrame();
            }
        }
        base.Die();
    }
}
