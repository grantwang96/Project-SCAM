using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour { // will handle game events such as item spawning

    public static GameManager Instance; // the singleton
    public List<GameObject> enemiesOnScreen = new List<GameObject> (); // list of enemies

    public Transform[] spellSpawns;
    public LayerMask spellBooksLayer;
    public float spawnFrequency;

    public Text enemyCounter;

    public GameObject messagePrefab;

    [SerializeField] private bool isSpellSpawning;
    [SerializeField] private bool combatTesting;

	// Use this for initialization
	void Start () {
        Instance = this;
        if (isSpellSpawning) { StartCoroutine(spellSpawning()); }
	}
	
	// Update is called once per frame
	void Update () {
        if(combatTesting) { CombatTesting(); }
	}

    void CombatTesting()
    {
        if (enemiesOnScreen.Count > 0)
        {
            for (int i = 0; i < enemiesOnScreen.Count; i++)
            {
                if (enemiesOnScreen[i] == null) { enemiesOnScreen.Remove(enemiesOnScreen[i]); }
            }
        }
        else {
            InitiateWinState();
        }
        enemyCounter.text = "Enemies Remaining: " + enemiesOnScreen.Count;
    }

    IEnumerator spellSpawning()
    {
        if(spellSpawns.Length == 0) {
            Debug.LogError("List of spell spawns is empty!");
            yield break;
        }
        shuffle(spellSpawns);
        int i = 0;
        while (true) {
            Transform spawn = spellSpawns[i];
            Collider[] colls = Physics.OverlapSphere(spawn.position, .5f, spellBooksLayer);
            if(colls.Length == 0) {
                SpellBook newSpell = SpellManager.Instance.GenerateSpell(spawn.position);
            }
            i++;
            if(i >= spellSpawns.Length) { i = 0; }
            yield return new WaitForSeconds(spawnFrequency);
        }
    }

    void shuffle<T>(T[] array)
    {
        for(int i = 0; i < array.Length; i++)
        {
            T temp = array[i];
            int rand = Random.Range(0, array.Length);
            array[i] = array[rand];
            array[rand] = temp;
        }
    }

    void InitiateWinState()
    {
        Debug.Log("You Win!");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Win");
    }

    public void InitiateLoseState()
    {
        Debug.Log("You Lose!");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Lose");
    }
}
