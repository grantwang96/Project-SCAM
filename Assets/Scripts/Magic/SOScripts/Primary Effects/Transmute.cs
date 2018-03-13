using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Primary Spell Effect/Transmute")]
public class Transmute : SpellPrimary {
    
    public GameObject[] possibleReplacements;

    public override void ActivateSpell(SpellCaster user, List<SpellBook.SideEffect> sideEffects, Vector3 fireDir)
    {
        base.ActivateSpell(user, sideEffects, fireDir);
    }

    public override void OnHit(Missile proj, Collision coll)
    {
        base.OnHit(proj, coll);
        Damageable collDam = coll.collider.GetComponent<Damageable>();
        if (collDam) {
            collDam.InitiateTransmutation(proj.duration, possibleReplacements[Random.Range(0, possibleReplacements.Length)]);
            SpellCaster originator = proj.originator.GetComponent<SpellCaster>();
            if(originator == null) { originator = proj.myCaster; }
            originator.invokeChangeFollowers(collDam);
        }
        if(proj.bounceCount <= 0)
        {
            proj.Die();
            return;
        }
        bounce(proj);
        proj.bounceCount--;
    }

    public override void bounce(Missile proj)
    {
        // create new bounce particles
        ParticleSystem newBounce = Instantiate(proj.bounceEffect, proj.transform.position, Quaternion.identity);
        ParticleSystem.MainModule main = newBounce.main;
        ParticleSystem.MinMaxGradient startCol = main.startColor;
        startCol.color = baseColor;
        Destroy(newBounce.gameObject, 1f);
    }
}
