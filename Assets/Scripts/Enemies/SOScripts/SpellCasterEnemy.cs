using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "EnemyData/SpellCasterEnemy")]
public class SpellCasterEnemy : EnemyData {

    public SpellPrimary[] possibleSpellPrimaries;
    public SpellSecondary[] possibleSpellSecondaries;

    public SpellBook spellBookPrefab;
    public int maxBooks;

    public override void setup(Movement owner)
    {
        startingState = new WizardEnemyIdle();
        owner.baseSpeed = baseSpeed;
        owner.maxSpeed = maxSpeed;
        Damageable ownerDam = owner.GetComponent<Damageable>();
        ownerDam.max_health = health;
        owner.damage = damage;
        owner.attackTarget = GameObject.FindGameObjectWithTag(attackTargetTag).transform;

        // Add spellbook to enemy's inventory
        if (SpellManager.Instance != null) { // if spell manager is running
            SpellCaster spellCaster = owner.GetComponent<SpellCaster>();

            for(int i = 0; i < maxBooks; i++) {
                
                SpellPrimary primary = possibleSpellPrimaries[Random.Range(0, possibleSpellPrimaries.Length)];
                SpellSecondary secondary = possibleSpellSecondaries[Random.Range(0, possibleSpellSecondaries.Length)];
                SpellBook newSpellBook = SpellManager.Instance.GenerateSpell(primary, secondary, owner.transform.position); // Create a new spellbook

                newSpellBook.Interact(spellCaster);
                newSpellBook.SetupSpell();
            }
        }

        base.setup(owner);
    }
}
