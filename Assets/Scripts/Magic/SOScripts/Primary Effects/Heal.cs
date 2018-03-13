using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Primary Spell Effect/Heal")]
public class Heal : SpellPrimary {

    public Transform healEffect;

    public override void ActivateSpell(SpellCaster user, SpellSecondary secondaryEffect, Vector3 fireDir, float chanceFail) {
        if (user.returnGun()) {

            // Create a cast effect
            ParticleSystem gunsmoke = Instantiate(castEffect, user.returnGun().position, Quaternion.LookRotation(fireDir));
            ParticleSystem.MainModule smokeMain = gunsmoke.main;
            smokeMain.startColor = baseColor;
            Destroy(gunsmoke.gameObject, smokeMain.startLifetime.constant);

            // Create the healing missile
            Vector3 firePos = Vector3.Lerp(user.returnGun().position, user.returnBody().position, 0.5f);
            Missile newProjectile = Instantiate(projectilePrefab, firePos, Quaternion.LookRotation(fireDir));
            newProjectile.bounceCount = 0;
            newProjectile.power = power;
            newProjectile.duration = duration;
            newProjectile.primaryEffect = this;
            newProjectile.secondaryEffect = secondaryEffect;
            newProjectile.originator = user.returnBody();

            // Healing missiles do not have velocity!
            Rigidbody projRbody = newProjectile.GetComponent<Rigidbody>();
            projRbody.useGravity = true;
            projRbody.velocity = Vector3.zero;

            // Apply visual effects
            ParticleSystem.MainModule main = newProjectile.sparkles.main;
            main.startColor = baseColor;
            TrailRenderer trail = newProjectile.trail;
            trail.startColor = baseColor;
            trail.endColor = baseColor;

            // Apply secondary effects
            if (secondaryEffect != null){
                secondaryEffect.MessUp(user.returnBody(), newProjectile);
            }

        }
    }

    public override void OnHit(Missile proj, Collision coll) {

        base.OnHit(proj, coll);

        Damageable dam = coll.collider.GetComponent<Damageable>();
        if (dam) // if you hit something damageable
        {
            dam.Heal(proj.power); // heal damage
            Transform newHealEffect = Instantiate(healEffect, dam.transform); // apply heal effect
            newHealEffect.rotation = Quaternion.identity; // make sure it's facing upwards
            Destroy(newHealEffect.gameObject, 3f); // get rid of heal effect
            proj.Die(); // destroy projectile
            return;
        }
        
        if(proj.bounceCount <= 0) { // if the projectile has no more bounces
            proj.Die();
            return;
        }

        proj.bounceCount--; // reduce bounces
    }

    public override void bounce(Missile proj) {
        base.bounce(proj);
    }
}
