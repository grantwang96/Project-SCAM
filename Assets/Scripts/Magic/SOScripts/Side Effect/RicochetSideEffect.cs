using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Secondary Spell Effect/Ricochet Side Effect")]
public class RicochetSideEffect : SpellSecondary {

    public int additionalBounces;
    public PhysicMaterial physMat;

    public float forceModifier;

    public override void MessUp(Transform user, Missile projectile)
    {
        projectile.bounceCount = additionalBounces;
        projectile.friendlyOff = true;
        Collider coll = projectile.GetComponent<Collider>();
        coll.material = physMat;
    }

    public override void OnHit(Transform user, Missile projectile, Collision coll)
    {
        Rigidbody rbody = projectile.GetComponent<Rigidbody>();
        rbody.velocity *= forceModifier;
    }
}
