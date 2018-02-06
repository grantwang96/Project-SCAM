using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyData/MeleeEnemy")]
public class MeleeEnemyData : EnemyData {

    public override void setup(Movement owner)
    {
        // startingState = new NPCWander();
        owner.baseSpeed = baseSpeed;
        owner.maxSpeed = maxSpeed;
        Damageable ownerDam = owner.GetComponent<Damageable>();
        ownerDam.max_health = health;
        owner.damage = damage;
        owner.attackTarget = GameObject.FindGameObjectWithTag(attackTargetTag).transform;
        base.setup(owner);
    }
}
