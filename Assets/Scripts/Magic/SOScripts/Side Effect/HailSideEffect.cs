using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Secondary Spell Effect/HailFire")]
public class HailSideEffect : SpellSecondary {

    public float delayTime;
    public float intervalTime;
    public float maxRadius;
    public float heightOffset;
    public float lifeTime;

    public override void MessUp(Transform user, Missile projectile)
    {
        Rigidbody rbody = projectile.GetComponent<Rigidbody>(); // Get projectile rigidbody
        rbody.useGravity = false; // do not allow gravity
        rbody.velocity = Vector3.zero; // Stop the missile from moving
        projectile.GetComponent<Collider>().enabled = false;
        projectile.Deactivate();
        projectile.StartCoroutine(hailFire(user, projectile));
    }

    public override void OnHit(Transform user, Missile projectile)
    {
        
    }

    IEnumerator hailFire(Transform user, Missile projectile)
    {
        yield return new WaitForSeconds(delayTime);
        float startTime = Time.time;
        while(Time.time - startTime < lifeTime)
        {
            // Make sure user isn't dead
            if(user == null) { break; }
            if(projectile == null) { break; }

            yield return new WaitForSeconds(intervalTime);

            Vector3 location = user.position + Vector3.up * heightOffset; // Get current height offset
            Missile newProj = Instantiate(projectile, location + Random.insideUnitSphere * maxRadius, Quaternion.Euler(90, 0, 0)); // create new missile
            newProj.originator = projectile.originator;
            newProj.myCaster = projectile.myCaster;

            Rigidbody rbody = newProj.GetComponent<Rigidbody>(); // Get projectile rigidbody
            rbody.useGravity = true; // allow gravity
            newProj.GetComponent<Collider>().enabled = true;
            newProj.Activate();
            // newProj.originator = null;
        }
    }
}
