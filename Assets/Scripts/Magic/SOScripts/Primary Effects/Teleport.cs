using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Primary Spell Effect/Teleport")]
public class Teleport : SpellPrimary {

    public override void OnHit(Missile proj, Collision coll)
    {
        if (proj.originator == null) { return; }
        Movement move = proj.originator.GetComponent<Movement>();
        move.Teleport(proj.transform.position);
        base.OnHit(proj, coll);
    }
}
