using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Secondary Spell Effect/Teleport")]
public class TeleportSideEffect : SpellSecondary {

    public override void OnHit(Transform user, Missile projectile) {
        if(user == null) { return; }
        Movement move = user.GetComponent<Movement>();
        move.Teleport(projectile.transform.position, Vector3.zero);
    }
}
