using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Primary Spell Effect/Teleport")]
public class Teleport : SpellPrimary {

    public override void OnHit(Missile proj, Collision coll)
    {
        if (proj.originator == null) { return; }
        base.OnHit(proj, coll);
        if(coll.collider.transform != proj.originator) {
            Movement move = proj.originator.GetComponent<Movement>();
            move.Teleport(proj.transform.position + Vector3.up * proj.GetComponent<Collider>().bounds.extents.y);
            proj.bounceCount--;
            if (proj.bounceCount <= 0) { proj.Die(); }
        }
    }
}
