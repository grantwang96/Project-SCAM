using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Primary Spell Effect/Ice")]
public class IcyBlast : SpellPrimary {

    public LayerMask frostable;

    public IcySurface icySurfacePrefab;

    public Rigidbody submissile;

    public GameObject iceExplosion;
    public GameObject frostEffect;

    [Range(1, 20)]
    public int minIcySurfaces;
    [Range(1, 20)]
    public int maxIcySurfaces;

    public int minDamage;
    public int maxDamage;

    public float radius;
    public float maxIcePrefabRadius;

    public float knockBackForce;
    public float upwardKnockup;

    public override void ActivateSpell(SpellCaster user, SpellSecondary secondaryEffect, Vector3 fireDir)
    {
        base.ActivateSpell(user, secondaryEffect, fireDir);
    }

    public override void OnHit(Missile proj, Collision coll)
    {
        base.OnHit(proj, coll);
        proj.bounceCount--;
        explode(proj, coll);
        if(proj.bounceCount <= 0) { proj.Die(); }
    }

    public override void bounce(Missile proj)
    {
        base.bounce(proj);
    }

    void explode(Missile missile, Collision target)
    {
        GameObject newIceExplosion = Instantiate(iceExplosion, missile.transform.position, Quaternion.identity);
        Destroy(newIceExplosion, 1f);

        // damage the damageables
        
        Collider[] colls = Physics.OverlapSphere(missile.transform.position, radius, frostable, QueryTriggerInteraction.Ignore);
        if(colls.Length != 0) {
            foreach(Collider coll in colls) {
                Damageable dam = coll.GetComponent<Damageable>();
                if(dam) {
                    Vector3 knockBack = (missile.transform.position - coll.transform.position).normalized;
                    knockBack.y = upwardKnockup;
                    knockBack = knockBack.normalized;
                    dam.TakeDamage(missile.originator, missile.power, knockBack, knockBackForce);
                }
            }
        }
        
        Rigidbody sub = Instantiate(submissile, missile.transform.position, Quaternion.identity);
        // sub.GetComponent<IceSubMissile>().radius = radius;
    }
}
