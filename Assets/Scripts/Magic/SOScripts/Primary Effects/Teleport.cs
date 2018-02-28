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
            Vector3 newpos = proj.transform.position;
            move.Teleport(newpos, coll.contacts[0].normal);
            proj.bounceCount--;
            if (proj.bounceCount <= 0) { proj.Die(); }
        }
    }
}
