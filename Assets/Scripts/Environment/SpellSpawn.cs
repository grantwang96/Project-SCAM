using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSpawn : MonoBehaviour {

    public List<SpellPrimary> OverridePrimaryEffects = new List<SpellPrimary>();
    public SpellBook bookPrefab;
    public Transform spawnPoint;
    public SpellBook currBook;

    public float checkRadius;
    public LayerMask bookLayer;

    void Update()
    {
        if(currBook == null) {
            SpawnSpell();
        }
    }

    public void SpawnSpell()
    {
        Collider[] colls = Physics.OverlapSphere(spawnPoint.position, checkRadius, bookLayer, QueryTriggerInteraction.Collide);
        if(colls.Length > 0) { // if there is a book here
            return;
        }

        SpellBook newSpellBook = Instantiate(bookPrefab, spawnPoint.position, spawnPoint.rotation);
        SpellPrimary newPrimary;
        if (OverridePrimaryEffects.Count != 0) { newPrimary = OverridePrimaryEffects[Random.Range(0, OverridePrimaryEffects.Count)]; } // use this list if not empty
        else { newPrimary = SpellManager.Instance.primarySpellEffects[Random.Range(0, SpellManager.Instance.primarySpellEffects.Count)]; }
        newSpellBook.primaryEffect = newPrimary;

        currBook = newSpellBook;
    }

    public void SpawnSpellRando()
    {
        Collider[] colls = Physics.OverlapSphere(spawnPoint.position, checkRadius, bookLayer, QueryTriggerInteraction.Collide);
        if (colls.Length > 0)
        { // if there is a book here
            return;
        }
        SpellBook newSpellBook = SpellManager.Instance.GenerateSpell(spawnPoint.position);
        currBook = newSpellBook;
    }

    public void SpawnSpell(SpellPrimary primary)
    {
        Collider[] colls = Physics.OverlapSphere(spawnPoint.position, checkRadius, bookLayer, QueryTriggerInteraction.Collide);
        if (colls.Length > 0) { // if there is a book here
            return;
        }

        SpellBook newSpellBook = Instantiate(bookPrefab, spawnPoint.position, spawnPoint.rotation);
        newSpellBook.primaryEffect = primary;
        
        currBook = newSpellBook;
    }
}
