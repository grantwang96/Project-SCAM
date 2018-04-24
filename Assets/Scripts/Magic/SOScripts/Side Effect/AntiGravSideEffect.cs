using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Secondary Spell Effect/Anti-Gravity Shot")]
public class AntiGravSideEffect : SpellSecondary {

    public float upwardForce;

    public override void MessUp(Transform user, Missile projectile)
    {
        projectile.messUpEffect = projectile.StartCoroutine(antiGravity(projectile));
    }

    IEnumerator antiGravity(Missile projectile)
    {
        Rigidbody projRbody = projectile.GetComponent<Rigidbody>();
        projRbody.useGravity = false;
        while (true) {
            if(projRbody == null) { break; }
            projRbody.AddForce(Vector3.down * Physics.gravity.y * upwardForce);
            yield return new WaitForEndOfFrame();
        }
    }
}
