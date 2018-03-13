using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellPrimary : ScriptableObject {

    public float force; // for travel
    public int power; // For damage or healing
    public float duration;

    public float powerLevelModifier;
    [Range(1, 10)] public int powerLevel;

    public Color baseColor;
    public int ammo;
    public float coolDown;

    public string title;
    public string description;

    public Missile projectilePrefab; // projectile used
    public ParticleSystem castEffect; // particle effect when firing spell
    public SpellSecondary[] allowedSecondaries;

    public virtual void ActivateSpell(SpellCaster user, List<SpellBook.SideEffect> sideEffects, Vector3 fireDir) // When the spell is fired
    {
        Transform firingPoint = user.returnGun();
        if (firingPoint)
        {
            // Make sure we dont fire inside the user
            Vector3 firePoint = firingPoint.position;

            // Create spawn shots
            ParticleSystem gunsmoke = Instantiate(castEffect, firePoint, Quaternion.LookRotation(fireDir));
            ParticleSystem.MainModule smokeMain = gunsmoke.main;
            smokeMain.startColor = baseColor;
            Destroy(gunsmoke.gameObject, smokeMain.startLifetime.constant);

            // Create a new missile object
            Missile newProjectile = Instantiate(projectilePrefab, firePoint, Quaternion.LookRotation(fireDir));
            newProjectile.bounceCount = 0;
            newProjectile.power = power;
            newProjectile.duration = duration;
            newProjectile.primaryEffect = this;
            newProjectile.sideEffects = sideEffects;
            newProjectile.originator = user.returnBody();
            newProjectile.myCaster = user;

            // Modify rigidbody settings for takeoff
            Rigidbody projRbody = newProjectile.GetComponent<Rigidbody>();
            projRbody.useGravity = false;
            projRbody.AddForce(newProjectile.transform.forward * force, ForceMode.Impulse);

            // Apply visual effects
            ParticleSystem.MainModule main = newProjectile.sparkles.main;
            main.startColor = baseColor;
            TrailRenderer trail = newProjectile.trail;
            trail.startColor = baseColor;
            trail.endColor = baseColor;
        }
    }

    public virtual void OnHit(Missile proj, Collision coll) // When the spell hits something
    {
        
    }

    public virtual void bounce(Missile proj)
    {
        ParticleSystem newBounce = Instantiate(proj.bounceEffect, proj.transform.position, Quaternion.identity);
        ParticleSystem.MainModule main = newBounce.main;
        ParticleSystem.MinMaxGradient startCol = main.startColor;
        startCol.color = baseColor;
        Destroy(newBounce.gameObject, 1f);
    }
}
