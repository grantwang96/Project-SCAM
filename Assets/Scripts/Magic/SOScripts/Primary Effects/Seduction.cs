﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Primary Spell Effect/Seduction")]
public class Seduction : SpellPrimary {

    public Transform blushPrefab;

    public override void ActivateSpell(SpellCaster user, SpellSecondary secondaryEffect, Vector3 fireDir)
    {
        base.ActivateSpell(user, secondaryEffect, fireDir);
    }

    public override void OnHit(Missile proj, Collision coll)
    {
        Damageable dam = coll.collider.GetComponent<Damageable>();
        if (!proj.friendlyOff && dam.transform == proj.originator) { // if friendly fire is on and the collider is the owner
            return;
        }

        base.OnHit(proj, coll);

        // the part where you seduce that PIMPLE-POPPING, NOSE-PICKING, NIPPLE-TWISTING, DICK OF AN ASS.

        if (dam) {
            Debug.Log("Duration: " + duration);
            Debug.Log("Collided Object: " + coll.gameObject);
            Debug.Log("SpellCaster: " + proj.originator.name);
            dam.Seduce(duration, coll.gameObject, proj.originator); // BECOME SEDUCED!
            proj.Die(); // don't need this anymore
            return;
        }

        if(proj.bounceCount <= 0) {
            proj.Die();
            return;
        } // if the projectile is out of bounces, die.

        proj.bounceCount--;
        // apologies, I thought it was kinda funny. Carry on.
    }

    public override void bounce(Missile proj)
    {
        base.bounce(proj);
    }
}
