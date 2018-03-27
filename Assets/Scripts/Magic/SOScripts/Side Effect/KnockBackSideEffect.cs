using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Secondary Spell Effect/KnockBack")]
public class KnockBackSideEffect : SpellSecondary {

    public float force;
    public float upwardForce;
    public float chargeTime;

    public float valueModifier;

    public override void MessUp(Transform user, Missile projectile)
    {
        Damageable dam = user.GetComponent<Damageable>();
        Vector3 dir = -projectile.transform.forward;
        dir.y = upwardForce;
        dir = dir.normalized;
        projectile.transform.localScale = new Vector3(projectile.transform.localScale.x * valueModifier, projectile.transform.localScale.y, projectile.transform.localScale.z);
        projectile.transform.position += projectile.transform.forward * projectile.GetComponent<SphereCollider>().radius * valueModifier;
        projectile.trail.widthMultiplier *= valueModifier;
        projectile.power *= Mathf.RoundToInt(valueModifier);
        projectile.duration *= valueModifier;
        projectile.friendlyOff = true;

        Debug.DrawRay(user.position, dir, Color.blue, 10f);

        if (dam) { dam.knockBack(dir, force); }
        else { Debug.Log("No damageable!"); }
    }

    IEnumerator chargeAndFire() {
        yield return new WaitForSeconds(chargeTime);
    }

    public override void OnHit(Transform user, Missile projectile, Collision coll)
    {
        base.OnHit(user, projectile, coll);
    }
}
