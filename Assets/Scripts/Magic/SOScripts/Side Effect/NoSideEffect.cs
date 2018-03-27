using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Secondary Spell Effect/No Side Effect")]
public class NoSideEffect : SpellSecondary {

    public override void MessUp(Transform user, Missile projectile)
    {
        
    }

    public override void OnHit(Transform user, Missile projectile, Collision coll)
    {
        
    }
}
