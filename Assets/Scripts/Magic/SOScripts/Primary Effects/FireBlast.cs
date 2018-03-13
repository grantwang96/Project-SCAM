using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Primary Spell Effect/FireBlast")]
public class FireBlast : SpellPrimary {

    public ParticleSystem tinyFlamePrefab;
    public Transform firePillarPrefab;

    public int shrapnelCountLowerBound;
    public int shrapnelCountUpperBound;
    public float subShotMinimumForce;
    public float subShotMaximumForce;
    public int subBlastPowerMod;
    public float mass;
    public float radius;
    public float subRadius;

    public float knockBackForce;
    public float upwardKnockup;

    public override void ActivateSpell(SpellCaster user, SpellSecondary secondaryEffect, Vector3 fireDir, float chanceFail)
    {
        base.ActivateSpell(user, secondaryEffect, fireDir, chanceFail);
    }

    public override void OnHit(Missile proj, Collision coll)
    {
        if (proj.mainShot && coll.collider.transform == proj.originator) {
            Debug.Log("Friendly Hit!");
            return;
        }

        if(proj.secondaryEffect != null) {
            base.OnHit(proj, coll);
        }

        if (proj.bounceCount <= 0) {
            if (proj.mainShot) {
                proj.StartCoroutine(firePillar(coll.collider.transform, proj));
            }
            else {
                subBlastHit(proj);
            }
            // proj.Die();
        }
        else {
            bounce(proj);
            proj.bounceCount--;
        }
    }

    IEnumerator firePillar(Transform coll, Missile proj)
    {
        proj.Deactivate();
        Rigidbody rbody = proj.GetComponent<Rigidbody>();
        rbody.useGravity = false;
        rbody.isKinematic = true;
        proj.GetComponent<Collider>().enabled = false;
        ParticleSystem flame = Instantiate(tinyFlamePrefab, proj.transform.position, Quaternion.identity);
        ParticleSystem.MainModule flameMain = flame.main;
        while(flame.startLifetime > 0f) {
            flame.startLifetime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        // Transform newPillar = Instantiate(firePillarPrefab, flame.transform.position, Quaternion.identity);
        mainBlast(proj, flame.transform);
        Destroy(flame.gameObject);
    }

    void mainBlast(Missile projFired, Transform flame)
    {
        Transform newPillarOfDoom = Instantiate(firePillarPrefab, flame.position, Quaternion.identity);
        newPillarOfDoom.GetComponent<PillarOfDoom>().damage = projFired.power;
        newPillarOfDoom.GetComponent<PillarOfDoom>().myCaster = projFired.originator;
        int shrapCount = UnityEngine.Random.Range(shrapnelCountLowerBound, shrapnelCountUpperBound);
        float angInterval = 360 / shrapCount;
        for (int i = 0; i < shrapCount; i++)
        {
            float ang = angInterval * i;
            Vector3 offset;
            offset.x = projFired.transform.position.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            offset.y = projFired.transform.position.y + 1f;
            offset.z = projFired.transform.position.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            subBlast(offset, newPillarOfDoom.position, projFired.originator, projFired.myCaster);
        }
        Destroy(projFired.gameObject);
    }

    void subBlast(Vector3 position, Vector3 startingPos, Transform caster, SpellCaster myCaster)
    {
        Missile newSubBlast = Instantiate(projectilePrefab, position, Quaternion.identity);
        newSubBlast.transform.forward = position - startingPos;
        Missile newproj = newSubBlast.GetComponent<Missile>();
        newproj.originator = caster;
        newproj.myCaster = myCaster;
        newproj.primaryEffect = this;
        newproj.power = power;
        newproj.mainShot = false;
        newproj.trail.startColor = baseColor;
        Rigidbody rbody = newSubBlast.GetComponent<Rigidbody>();
        rbody.useGravity = true;
        rbody.mass = mass;
        rbody.AddForce(newSubBlast.transform.forward * Random.Range(subShotMinimumForce, subShotMaximumForce), ForceMode.Impulse);
        ParticleSystem.MainModule main = newproj.sparkles.main;
        main.startColor = baseColor;
    }

    void subBlastHit(Missile proj) {
        Collider[] colls = Physics.OverlapSphere(proj.transform.position, subRadius);
        if(colls.Length != 0) {
            foreach(Collider coll in colls) {
                Damageable dam = coll.GetComponent<Damageable>();
                if (dam) {
                    Vector3 knockBack = (coll.transform.position - proj.transform.position).normalized;
                    knockBack.y = upwardKnockup;
                    knockBack = knockBack.normalized;
                    dam.TakeDamage(proj.originator, proj.power, knockBack, knockBackForce);
                    if(proj.originator == null) { return; }
                    proj.myCaster.invokeChangeFollowers(dam);
                }
            }
        }
        proj.Die();
    }

}
