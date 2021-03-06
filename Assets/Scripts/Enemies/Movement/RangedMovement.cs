﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedMovement : Movement {
    
    [SerializeField] bool canShoot;
    [SerializeField] Transform gun; // where the projectile launches from

    public bool getCanShoot() { return canShoot; }
    public void setCanShoot(bool can) { canShoot = can; }

    public Projectile projectilePrefab;
    public float throwForce;

    public override void setup()
    {
        base.setup();
    }

    public override void Start()
    {
        canShoot = true;
        base.Start();
        changeState(new RangedEnemyIdle());
    }

    public override void Update()
    {
        RaycastHit rayHit;
        if (hamper <= 0 && Physics.Raycast(new Ray(transform.position + Vector3.up * 0.1f, Vector3.down),
            out rayHit, 0.2f, groundLayers, QueryTriggerInteraction.Ignore))
        {
            NavMeshHit hit;
            agent.nextPosition = transform.position;
            if (agent.nextPosition != transform.position && NavMesh.SamplePosition(transform.position, out hit, agent.radius, NavMesh.AllAreas))
            { // check if agent is synced and on/near the navmesh
                agent.Warp(hit.position);
                ReactivateNavMesh();
            }
        }
        else
        {
            rbody.isKinematic = false;
        }
        base.Update();
    }

    private void ReactivateNavMesh()
    {
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = false;
        rbody.isKinematic = true;
    }

    protected override void ToIdle() {
		changeState(new RangedEnemyIdle());
	}

    public override IEnumerator attack(Vector3 target)
    {
        Vector3 targetingDir = target - transform.position;
        targetingDir.y = 0;
        gun.forward = targetingDir;
        anim.Play("Attack");

        Projectile newProjectile = Instantiate(projectilePrefab, gun);
        newProjectile.transform.position = gun.position;
        newProjectile.rbody.isKinematic = true;
        newProjectile.rbody.useGravity = false;
        newProjectile.owner = transform;
        newProjectile.damage = damage;

        bool fired = false;
        yield return new WaitForEndOfFrame();

        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
            if(hamper > 0) { // we can't attack
                Destroy(newProjectile.gameObject);
                changeState(new RangedEnemyAggro());
                yield break;
            }

            if(attackTarget == null && newProjectile != null) { Destroy(newProjectile.gameObject); yield break; }

            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f && !fired) {

                float targetDistance = Vector3.Distance(transform.position, attackTarget.position);

                Vector3 vel = attackTarget.position - gun.position;
                vel.y += (4f * targetDistance / throwForce);
                vel = vel.normalized;

                newProjectile.transform.SetParent(null);
                newProjectile.rbody.isKinematic = false;
                newProjectile.rbody.useGravity = true;
                newProjectile.rbody.AddForce(vel * throwForce * newProjectile.rbody.mass, ForceMode.Impulse);

                fired = true;
            }
            yield return new WaitForEndOfFrame();
        }
        attackRoutine = null;
    }
    
    void OnCollisionEnter(Collision coll)
    {
        if(coll.transform == attackTarget) { changeState(new RangedEnemyAggro()); }
    }
}
