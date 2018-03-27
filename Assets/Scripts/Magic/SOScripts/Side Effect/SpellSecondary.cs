using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSecondary : ScriptableObject {

    public int ammo;
    public float coolDown;

    public string title;
    public string description;

    public Transform decoration;
    public Transform sideEffect;

    public virtual void setup(SpellBook spellBook)
    {

    }

    public virtual void MessUp(Transform user, Missile projectile) // manipulate how the projectile is created
    {
        if(sideEffect == null) { return; }
        Transform sideFX = Instantiate(sideEffect, projectile.transform);
        sideFX.localPosition = Vector3.zero;
        sideFX.localRotation = Quaternion.identity;
    }

    public virtual void OnHit(Transform user, Missile projectile, Collision coll) // when the projectile hits something
    {

    }
}
