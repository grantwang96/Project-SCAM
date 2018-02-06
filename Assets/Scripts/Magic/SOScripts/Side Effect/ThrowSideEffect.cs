using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Secondary Spell Effect/Throw Side Effect")]
public class ThrowSideEffect : SpellSecondary {

    public PhysicMaterial bounceMat;
    public int bounceCount;
    public float modMass;
    public float modForce;

    public override void MessUp(Transform user, Missile projectile)
    {
        // change projectile bounce count
        projectile.bounceCount = bounceCount;

        Missile pRoJeCtIlE = Instantiate(projectile, projectile.transform.position, projectile.transform.rotation);
        Destroy(projectile.gameObject);

        Rigidbody rbody = pRoJeCtIlE.GetComponent<Rigidbody>();
        rbody.mass = modMass;
        rbody.useGravity = true;
        rbody.AddForce(projectile.transform.forward * modForce, ForceMode.Impulse);

        Collider coll = pRoJeCtIlE.GetComponent<Collider>();
        coll.material = bounceMat;
    }

    public override void OnHit(Transform user, Missile projectile)
    {
        
    }

    IEnumerator processMessUp(Missile projectile)
    {
        yield return new WaitForEndOfFrame();
        // Make projectile use gravity
        Rigidbody rbody = projectile.GetComponent<Rigidbody>();
        rbody.useGravity = true;

        // Replace force
        Vector3 forceDir = rbody.velocity.normalized;
        Debug.Log("Velocity before: " + rbody.velocity);
        rbody.velocity = Vector3.zero;
        rbody.angularVelocity = Vector3.zero;
        rbody.mass = modMass;
        // rbody.AddForce(forceDir * modForce, ForceMode.Impulse);

        // change projectile's bounciness
        Collider coll = projectile.GetComponent<Collider>();
        coll.material = bounceMat;
    }
}
