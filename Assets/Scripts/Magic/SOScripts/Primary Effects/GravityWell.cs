﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Primary Spell Effect/Gravity Well")]
public class GravityWell : SpellPrimary {

    public GravityWellVortex gravWellPrefab;

    public override void ActivateSpell(SpellCaster user, SpellSecondary secondaryEffect, Vector3 fireDir)
    {
        base.ActivateSpell(user, secondaryEffect, fireDir);
    }

    public override void OnHit(Missile proj, Collision coll)
    {
        if (proj.friendlyOff && coll.transform == proj.originator) { return; }
        base.OnHit(proj, coll);
        if (proj.bounceCount <= 0)
        {
            GravityWellVortex newGravWell = Instantiate(gravWellPrefab, proj.transform.position, Quaternion.identity); // create gravity well
            // Instantiate special effect
            proj.Die();
            return;
        }
        bounce(proj);
        proj.bounceCount--;
    }

    public override void bounce(Missile proj)
    {
        ParticleSystem newBounce = Instantiate(proj.bounceEffect, proj.transform.position, Quaternion.identity);
        ParticleSystem.MainModule main = newBounce.main;
        ParticleSystem.MinMaxGradient startCol = main.startColor;
        startCol.color = baseColor;
        Destroy(newBounce.gameObject, 1f);
    }
}
