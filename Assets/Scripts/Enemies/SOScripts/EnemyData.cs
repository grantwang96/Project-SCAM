using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : ScriptableObject {

    [Range(5, 700)]
    public int health;
    public float baseSpeed;
    public float maxSpeed;

    [Range(0, 100)] public float sightRange;
    [Range(0, 90)] public float sightAngle;

    [Range(0, 90)] public float attackRange;
    [Range(1, 30)] public float attentionSpan;

    public List<EnemyDrop> possibleDrops = new List<EnemyDrop>();

    public string[] firstName;
    public string[] lastNameP1;
    public string[] lastNameP2;
    public string[] verbings;
    public string[] nouns;

    [SerializeField] public string preferredName;

    public string attackTargetTag;

    public int damage;

    public CombatType myType;

    public NPCState startingState;

    public enum CombatType
    {
        Melee,
        Ranged,
        Mixed,
        SpellCaster,
        Support,
    }

    public virtual void setup(Movement owner)
    {
        if(startingState != null) {
            owner.changeState(startingState);
        }
        
        // owner.GetComponent<Damageable>().max_health = health;
        owner.sightRange = sightRange;
        owner.sightAngle = sightAngle;
        owner.myType = myType;
        owner.gameObject.name = generateName();
    }

    public virtual Transform getOriginTarget() {
        return GameObject.FindGameObjectWithTag(attackTargetTag).transform;
    }

    public virtual string generateName()
    {
        if(preferredName != "" && preferredName != null) { return preferredName; }
        string newname = firstName[Random.Range(0, firstName.Length)];
        int mode = Random.Range(0, 3);
        switch(mode) {
            case 0:
                newname += " " + generateLastName();
                break;
            case 1:
                List<string> possibleFirstNames = new List<string>();
                foreach(string name in firstName) { if(name != newname) { possibleFirstNames.Add(name); } }
                if(Random.value > 0.5f) { newname += ", Son of "; }
                else { newname += ", Daughter of "; }
                newname += possibleFirstNames[Random.Range(0, possibleFirstNames.Count)];
                break;
            default:
                newname += " of the " + generatePlace();
                break;
        }
        return newname;
    }

    public string generateLastName()
    {
        string newLastName = "";
        newLastName += lastNameP1[Random.Range(0, lastNameP1.Length)];
        newLastName += lastNameP2[Random.Range(0, lastNameP2.Length)].ToLower();
        return newLastName;
    }

    public string generatePlace()
    {
        string newPlace = "";
        newPlace += verbings[Random.Range(0, verbings.Length)];
        newPlace += " " + nouns[Random.Range(0, nouns.Length)];
        return newPlace;
    }

    public void DropLoot(Vector3 deathLocation) {
        float chance = Random.value;
        foreach(EnemyDrop drop in possibleDrops) {
            if(chance < drop.chanceDrop) {
                int count = Random.Range(drop.dropCountLower, drop.dropCountUpper);
                for(int i = 0; i < count; i++) {
                    Instantiate(drop.dropPrefab, deathLocation, Quaternion.Euler(Vector3.up * Random.Range(0, 360f)));
                }
            }
        }
    }
}

public class EnemyDrop
{
    public GameObject dropPrefab;
    public int dropCountLower;
    public int dropCountUpper;
    [Range(0, 1)] public float chanceDrop;
}
