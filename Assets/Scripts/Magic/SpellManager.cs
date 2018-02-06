using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour {

    public List<SpellPrimary> primarySpellEffects = new List<SpellPrimary>();
    public List<SpellSecondary> secondarySpellEffects = new List<SpellSecondary>();

    public static SpellManager Instance;
    public SpellBook spellBookPrefab;

	// Use this for initialization
	void Awake () {
        Instance = this;
	}

    public SpellBook GenerateSpell(Vector3 position) // Generate spell at location
    {
        SpellBook newSpellBook = Instantiate(spellBookPrefab, position, Quaternion.identity);
        newSpellBook.primaryEffect = primarySpellEffects[Random.Range(0, primarySpellEffects.Count)];
        newSpellBook.secondaryEffect = secondarySpellEffects[Random.Range(0, secondarySpellEffects.Count)];
        return newSpellBook;
    }

    public SpellBook GenerateSpell(SpellPrimary primary) // Generate spell given spell primary
    {
        SpellBook newSpellBook = Instantiate(spellBookPrefab, Vector3.zero, Quaternion.identity);
        newSpellBook.primaryEffect = primary;
        newSpellBook.secondaryEffect = secondarySpellEffects[Random.Range(0, secondarySpellEffects.Count)];
        return newSpellBook;
    }

    public SpellBook GenerateSpell(SpellPrimary primary, Vector3 position) // Generate spell given spell primary and location
    {
        SpellBook newSpellBook = Instantiate(spellBookPrefab, position, Quaternion.identity);
        newSpellBook.primaryEffect = primary;
        newSpellBook.secondaryEffect = secondarySpellEffects[Random.Range(0, secondarySpellEffects.Count)];
        return newSpellBook;
    }

    public SpellBook GenerateSpell(SpellPrimary primary, SpellSecondary secondary, Vector3 position) // Generate spell given primary, secondary, and location
    {
        SpellBook newSpellBook = Instantiate(spellBookPrefab, position, Quaternion.identity);
        newSpellBook.primaryEffect = primary;
        newSpellBook.secondaryEffect = secondary;
        return newSpellBook;
    }

    public void SpawnSpellBook(Vector3 position)
    {
        SpellBook newSpellBook = Instantiate(spellBookPrefab, position, Quaternion.identity);
        newSpellBook.primaryEffect = primarySpellEffects[Random.Range(0, primarySpellEffects.Count)];
        newSpellBook.secondaryEffect = secondarySpellEffects[Random.Range(0, secondarySpellEffects.Count)];
    }

    public void SpawnSpellBook(SpellPrimary primary, Vector3 position)
    {
        SpellBook newSpellBook = Instantiate(spellBookPrefab, position, Quaternion.identity);
        newSpellBook.primaryEffect = primary;
        newSpellBook.secondaryEffect = secondarySpellEffects[Random.Range(0, secondarySpellEffects.Count)];
    }

    public void SpawnSpellBook(SpellSecondary secondary, Vector3 position)
    {
        SpellBook newSpellBook = Instantiate(spellBookPrefab, position, Quaternion.identity);
        newSpellBook.secondaryEffect = secondary;
        newSpellBook.secondaryEffect = secondarySpellEffects[Random.Range(0, secondarySpellEffects.Count)];
    }

    public void SpawnSpellBook(SpellPrimary primary, SpellSecondary secondary, Vector3 position)
    {
        SpellBook newSpellBook = Instantiate(spellBookPrefab, position, Quaternion.identity);
        newSpellBook.primaryEffect = primary;
        newSpellBook.secondaryEffect = secondary;
    }

    public void spawnSpellEffect(Vector3 position) // create visual panache at spawn location
    {

    }
    
    public void SpawnSpell(SpellSpawn spawnPoint)
    {
        spawnPoint.SpawnSpell();
    }
}
