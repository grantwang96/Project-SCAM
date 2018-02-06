using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Secondary Spell Effect/Surround EFfect")]
public class SurroundSideEffect : SpellSecondary {

    public float forwardTime;
    public float modifiedLifeSpan;
    public float speed;

    public Rigidbody axisPrefab;

    public override void MessUp(Transform user, Missile projectile)
    {
        base.MessUp(user, projectile);
        // projectile.StartCoroutine(waitAndSurround(user, projectile));
        projectile.messUpEffect = projectile.StartCoroutine(processSurround(user, projectile));
    }

    public override void OnHit(Transform user, Missile projectile)
    {
        base.OnHit(user, projectile);
    }

    IEnumerator waitAndSurround(Transform user, Missile projectile)
    {
        projectile.bounceCount = 0;
        yield return new WaitForSeconds(forwardTime);
        Rigidbody projRbody = projectile.GetComponent<Rigidbody>();
        projRbody.velocity = Vector3.zero;
        Rigidbody newAxis = Instantiate(axisPrefab);
        newAxis.rotation = user.rotation;
        projectile.transform.parent = newAxis.transform;
        projectile.toBeDeleted.Add(newAxis.gameObject);
        newAxis.angularVelocity = new Vector3(0, speed, 0);
        while (!projectile.dead) {
            newAxis.MovePosition(user.position);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator processSurround(Transform user, Missile projectile)
    {
        projectile.bounceCount = 0;
        projectile.lifeSpan = modifiedLifeSpan;
        yield return new WaitForSeconds(forwardTime);
        Rigidbody projRbody = projectile.GetComponent<Rigidbody>();
        projRbody.velocity = Vector3.zero;
        while (true)
        {
            Vector3 userBefore = user.position;
            if(projectile == null) { break; }
            Debug.Log("woo!");
            // fly around the user
            Vector3 before = projectile.transform.position;
            projectile.transform.RotateAround(user.position, user.up, speed * Time.deltaTime);
            Vector3 after = projectile.transform.position;
            RaycastHit hit;
            if (projRbody.SweepTest(before - after, out hit, Vector3.Distance(before, after))) {
                if (hit.collider.GetComponent<Damageable>()) {
                    projRbody.MovePosition(hit.point);
                    // projectile.primaryEffect.OnHit(projectile, hit.collider);
                }
            }
            yield return new WaitForEndOfFrame();
            Vector3 userAfter = user.position;
            projRbody.MovePosition(projRbody.position + (userAfter - userBefore)); // update position to maintain distance
        }
    }
}
