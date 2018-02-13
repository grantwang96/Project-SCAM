using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Delegate/Generate Spell")]
public class GenerateSpell : EventFunction {

    [SerializeField] SpellPrimary overridePrimary;
    [SerializeField] SpellSecondary overrideSecondary;

    public override IEnumerator doThing(GameObject target, float delay, string message, int count)
    {
        yield return new WaitForSeconds(delay);
        SpellSpawn spawnPoint = target.GetComponent<SpellSpawn>();
        if(spawnPoint == null) { yield break; }
        if(overridePrimary != null & overrideSecondary != null) { spawnPoint.SpawnSpell(overridePrimary, overrideSecondary); }
        else if(overridePrimary != null) { spawnPoint.SpawnSpell(overridePrimary); }
        else if(overrideSecondary != null) { spawnPoint.SpawnSpell(overrideSecondary); }
        else { spawnPoint.SpawnSpell(); }
    }
}
