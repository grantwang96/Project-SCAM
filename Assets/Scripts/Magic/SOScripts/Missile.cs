using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    public Transform originator;
    public SpellCaster myCaster;
    public bool friendlyOff = true;
    public bool mainShot = true;
    [SerializeField] float delayFriendlyOn;

    [SerializeField] int _bouncesLeft;
    public int bounceCount { get { return _bouncesLeft; } set { _bouncesLeft = value; } }
    bool _dead;
    public bool dead { get { return _dead; } set { _dead = value; } }
    public List<GameObject> toBeDeleted = new List<GameObject>(); // list of gameobjects that must also be destroyed when missile dies
    public float lifeSpan;
    float startTime;

    public int power = 0;
    public float duration = 0f;

    public SpellPrimary primaryEffect;
    public SpellSecondary secondaryEffect;

    public ParticleSystem sparkles;
    public TrailRenderer trail;

    public ParticleSystem bounceEffect;
    [SerializeField] ParticleSystem deathEffect;

    public float messUpChance;
    public bool derped;
    public Coroutine messUpEffect; // movement coroutine that overrides/modifies normal movement

	// Use this for initialization
	void Start () {
        startTime = Time.time;
        // StartCoroutine(delayEffectiveness());
	}

    void Update() {
        if(Time.time - startTime > lifeSpan) { Die(); }
    }

    void OnCollisionEnter(Collision coll) {
        if (primaryEffect) { // if primaryEffect is not null
            primaryEffect.OnHit(this, coll);
        }
        else {
            // Uh...whelp
            Die();
        }
    }

    public void Deactivate()
    {
        if (sparkles) { sparkles.gameObject.SetActive(false); }
        if (trail) { trail.gameObject.SetActive(false); }
    }

    public void Activate()
    {
        sparkles.gameObject.SetActive(true);
        trail.gameObject.SetActive(true);
    }

    public void Die()
    {
        if (_dead) { return; }
        _dead = true;
        transform.parent = null;
        if(messUpEffect != null) { StopCoroutine(messUpEffect); }
        ParticleSystem newDeath = Instantiate(deathEffect, transform.position, Quaternion.identity);
        var main = newDeath.main;
        main.startColor = primaryEffect.baseColor;
        Destroy(newDeath.gameObject, 1f);

        if(toBeDeleted.Count != 0) {
            while(toBeDeleted.Count != 0) {
                GameObject wrecked = toBeDeleted[0];
                toBeDeleted.RemoveAt(0);
                Destroy(wrecked.gameObject);
            }
        }

        if(sparkles != null) {
            sparkles.Stop();
            sparkles.transform.parent = null;
            Destroy(sparkles.gameObject, 1f);
        }

        if(trail != null) {
            trail.transform.parent = null;
            Destroy(trail.gameObject, 1f);
        }
        Destroy(gameObject);
    }

    IEnumerator delayEffectiveness()
    {
        friendlyOff = true;
        yield return new WaitForSeconds(delayFriendlyOn);
        friendlyOff = false;
    }
}
