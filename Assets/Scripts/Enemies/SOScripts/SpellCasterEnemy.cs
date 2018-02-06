using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "EnemyData/SpellCasterEnemy")]
public class SpellCasterEnemy : EnemyData {

    public SpellPrimary[] possibleSpellPrimaries;
    public SpellSecondary[] possibleSpellSecondaries;

    public SpellBook spellBookPrefab;

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

            if (!spellCaster.returnSpell()) {
                
                SpellPrimary primary = possibleSpellPrimaries[Random.Range(0, possibleSpellPrimaries.Length)];
                SpellSecondary secondary = possibleSpellSecondaries[Random.Range(0, possibleSpellSecondaries.Length)];
                SpellBook newSpellBook = SpellManager.Instance.GenerateSpell(primary, secondary, owner.transform.position); // Create a new spellbook

                newSpellBook.Interact(spellCaster);
                newSpellBook.SetupSpell();
            }
        }

        /*
        SpellBook newSpellBook = Instantiate(spellBookPrefab, owner.transform.position, owner.transform.rotation);
        newSpellBook.primaryEffect = possibleSpellPrimaries[Random.Range(0, possibleSpellPrimaries.Length)];
        newSpellBook.secondaryEffect = possibleSpellSecondaries[Random.Range(0, possibleSpellSecondaries.Length)];
        
        // Have enemy pick up spellbook
        SpellCaster spellCaster = owner.GetComponent<SpellCaster>();
        newSpellBook.Interact(spellCaster);
        */

        base.setup(owner);
    }
}
