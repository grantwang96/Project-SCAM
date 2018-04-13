using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMovement : Movement {
    
    [SerializeField] bool canShoot;
    [SerializeField] Transform gun; // where the projectile launches from

    public bool getCanShoot() { return canShoot; }
    public void setCanShoot(bool can) { canShoot = can; }

    public Rigidbody projectilePrefab;

    public override void setup()
    {
        base.setup();
    }

    public override void Start()
    {
        canShoot = true;
        base.Start();
    }

    public override IEnumerator attack(Vector3 target)
    {
        Vector3 targetingDir = target - transform.position;
        targetingDir.y = 0;
        gun.forward = targetingDir;
        anim.Play("Attack");

        Rigidbody newProjectile = Instantiate(projectilePrefab, gun);
        newProjectile.isKinematic = true;
        newProjectile.useGravity = false;

        bool fired = false;
        yield return new WaitForFixedUpdate();

        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
            if (anim.GetCurrentAnimatorStateInfo(0).length >= 0.5f && !fired) {
                Vector3 vel = gun.forward;
                fired = true;
            }
            yield return new WaitForEndOfFrame();
        }
        attackRoutine = null;
    }
}
