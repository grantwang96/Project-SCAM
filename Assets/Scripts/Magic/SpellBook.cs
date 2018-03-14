using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBook : MonoBehaviour, Interactable {

    public SpellPrimary primaryEffect;
    public List<SideEffect> sideEffects = new List<SideEffect>();
    // public SpellSecondary secondaryEffect;

    [SerializeField] int maxAmmo;
    public int getMaxAmmo() { return maxAmmo; }
    [SerializeField] int ammo;
    public int getAmmo() { return ammo; }
    public void useAmmo() { ammo--; }
    public SpellCaster owner;
    [SerializeField] bool _dead = false;
    public bool dead { get { return _dead; } set { _dead = value; } }
    bool dying = false;

    public string spellTitle;
    public string spellDescription;
    public Color baseColor;

    MeshRenderer[] allMeshes;
    public Transform sparklyEffect;
    Rigidbody rbody;

    public ParticleSystem DieEffect;

    [SerializeField] private Missile projectilePrefab;

	// Use this for initialization
	void Awake () {
        allMeshes = GetComponentsInChildren<MeshRenderer>();
        rbody = GetComponent<Rigidbody>();
        // SetupSpell();
    }

    void Start()
    {
        SetupSpell();
    }

    public void SetupSpell()
    {
        if (primaryEffect) {
            SetUpPrimary();
            SetUpSideEffects();
        }

        maxAmmo = ammo;
    }

    private void SetUpPrimary() {
        ammo += primaryEffect.ammo;
        baseColor = primaryEffect.baseColor;
        baseColor.a = 1f;
        transform.Find("Cover").GetComponent<Renderer>().material.color = baseColor;
        spellTitle = primaryEffect.title;
        spellDescription = "-" + primaryEffect.description;
    }

    private void SetUpSideEffects()
    {
        sideEffects.Clear();

        int sideEffectCount = Random.Range(1, 3);
        List<SpellSecondary> secondaries = new List<SpellSecondary>();
        for(int i = 0; i < primaryEffect.allowedSecondaries.Length; i++) { secondaries.Add(primaryEffect.allowedSecondaries[i]); }

        for(int i = 0; i < sideEffectCount; i++) {
            if(i >= secondaries.Count) { break; }
            SpellSecondary secondary = secondaries[Random.Range(0, secondaries.Count)];
            SideEffect newSideEffect =
                new SideEffect(secondary, Random.Range(secondary.lowerChanceActivate, secondary.upperChanceActivate));
            sideEffects.Add(newSideEffect);
            secondaries.Remove(secondary);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!_dead && owner == null) // If not dead
        {
            // fun rotations and floats!
        }
        if (ammo <= 0 && !_dead) {
            Die();
        }
    }

    public void FireSpell() {
        primaryEffect.ActivateSpell(owner, sideEffects, owner.returnGun().forward);
    }

    public bool Interact(SpellCaster spellCaster)
    {
        if (_dead || owner != null) { return false; }
        owner = spellCaster;
        spellCaster.pickUpSpell(this);
        return true;
    }

    public bool Interact() { Debug.Log("Can't be used by you!"); return false; } // MUST BE USED BY SPELLCASTER

    public IEnumerator Drop(Vector3 newLoc)
    {
        _dead = true;
        Vector3 startPos = transform.position;
        float prog = 0f;
        while(transform.position != newLoc) {
            if (dying) { yield break; }
            transform.position = Vector3.Lerp(startPos, newLoc, prog);
            prog += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _dead = false;
    }

    public void Deactivate()
    {
        foreach(MeshRenderer mr in allMeshes) {
            mr.enabled = false;
        }
        // sparklyEffect.gameObject.SetActive(false);
        GetComponent<SphereCollider>().enabled = false;
    }

    public void Activate()
    {
        foreach (MeshRenderer mr in allMeshes) {
            mr.enabled = true;
        }
        // sparklyEffect.gameObject.SetActive(true);
        GetComponent<SphereCollider>().enabled = true;
    }

    public void Die()
    {
        // Do Die Effect
        if (_dead || dying) { return; } // don't do this again if you're dead
        if (owner != null) { owner.dropSpell(this, owner.returnBody().position); }
        StartCoroutine(processDie());
    }

    IEnumerator processDie()
    {
        _dead = true;
        dying = true;
        owner = null;
        float startTime = Time.time;
        float dieTime = 2f;
        // Deactivate();
        ParticleSystem newDieEffect = Instantiate(DieEffect, transform.position, Quaternion.identity);
        newDieEffect.transform.parent = transform;
        while (Time.time - startTime < dieTime)
        {
            transform.position += Vector3.up * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        newDieEffect.Stop();
        newDieEffect.transform.parent = null;
        Destroy(newDieEffect.gameObject, 1f);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider coll)
    {
        /*
        if(coll.tag == "Player" && !dying) {
            Interact(coll.GetComponent<SpellCaster>());
        }
        */
    }

    [System.Serializable]
    public struct SideEffect
    {
        public SpellSecondary effect;
        public float chanceEffect;
        public SideEffect(SpellSecondary se, float c) {
            effect = se;
            chanceEffect = c;
        }
    }
}
