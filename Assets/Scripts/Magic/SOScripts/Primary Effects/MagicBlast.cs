using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Primary Spell Effect/Magic Blast")]
public class MagicBlast : SpellPrimary { // Standard damaging magic attack

    public float knockBackForce;
    public float upwardKnockup;

    public override void ActivateSpell(SpellCaster user, SpellSecondary secondaryEffect, Vector3 fireDir, float chanceFail) {
        base.ActivateSpell(user, secondaryEffect, fireDir, chanceFail);
    }

    public override void OnHit(Missile proj, Collision coll) {
        if(!proj.friendlyOff && coll.transform == proj.originator) {
            Debug.Log("Friendly Hit!");
            return;
        }

        base.OnHit(proj, coll);

        Damageable collDam = coll.collider.GetComponent<Damageable>();
        if (collDam) {
            Vector3 knockBack = (coll.transform.position - proj.transform.position).normalized;
            knockBack.y = upwardKnockup;
            knockBack = knockBack.normalized;
            collDam.TakeDamage(proj.originator, proj.power, knockBack, knockBackForce);
            SpellCaster originator = proj.originator.GetComponent<SpellCaster>();
            if(originator == null) { originator = proj.myCaster; }
            if(originator != null) {
                originator.invokeChangeFollowers(collDam);
            }
            // Instantiate special effect
        } else {
            Rigidbody rbody = coll.collider.attachedRigidbody;
            if (rbody != null) { rbody.AddExplosionForce(knockBackForce, proj.transform.position, 1f); }
        }

        if (proj.bounceCount <= 0) {
            proj.Die();
        }
        else {
            bounce(proj);
            proj.bounceCount--;
        }
    }

    public override void bounce(Missile proj)
    {
        base.bounce(proj);
    }
}
