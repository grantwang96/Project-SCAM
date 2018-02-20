using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : Damageable, Interactable {

    [SerializeField] Vector3 openRotation;
    [SerializeField] Vector3 closedRotation;
    [SerializeField] Transform myHinge;
    [SerializeField] float speed;
    [SerializeField] private bool _locked;
    public bool locked { get { return _locked; } set {
            _locked = value;
            if(open) { movement = StartCoroutine(doorRotation()); }
        } }
    [SerializeField] private bool open;

    Coroutine movement;
    [SerializeField] Rigidbody rbody;

    // private Key myKey; // the key that unlocks this door

    public bool Interact(SpellCaster spellCaster)
    {
        if (_locked) {
            return false; // do not open for anyone
        }
        if (movement != null) { return false; } // do not allow interaction during motion.
        movement = StartCoroutine(doorRotation());
        return true;
    }

    public bool Interact() {
        if(_locked) {
            return false;
        }
        if(movement != null) { return false; }
        movement = StartCoroutine(doorRotation());
        return true;
    }

    public void Unlock()
    {
        locked = false;
    }

    IEnumerator doorRotation()
    {
        float prog = 0f;
        Quaternion start;
        Quaternion end;
        if (open) { start = Quaternion.Euler(openRotation); end = Quaternion.Euler(closedRotation); }
        else { start = Quaternion.Euler(closedRotation); end = Quaternion.Euler(openRotation); }
        open = !open;
        while (prog < 1f) {
            myHinge.localRotation = Quaternion.Lerp(start, end, prog);
            prog += Time.deltaTime * speed;
            yield return new WaitForEndOfFrame();
        }
        if(open) { myHinge.localRotation = Quaternion.Euler(openRotation); }
        else { myHinge.localRotation = Quaternion.Euler(closedRotation); }
        
        

        movement = null;
    }

    void OnTriggerEnter(Collider coll)
    {
        // check if thing touching is key
    }

    public override void Start()
    {
        if(open) { myHinge.localEulerAngles = openRotation; }
        else { myHinge.localEulerAngles = closedRotation; }
    }

    public override void TakeDamage(Transform attacker, int hpLost, Vector3 dir, float force)
    {
        if(_locked) { return; }
        base.TakeDamage(attacker, hpLost, dir, force);
        if(health <= 0) {
            transform.parent = null;
            rbody.useGravity = true;
            rbody.isKinematic = false;
            knockBack(dir, force);
        }
    }

    public override void Die() {
        dead = true;
        StartCoroutine(fadeAway());
    }

    IEnumerator fadeAway() {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }

    public override void InitiateTransmutation(float duration, GameObject replacement)
    {
        
    }

    public override void knockBack(Vector3 dir, float force)
    {
        if(!rbody.isKinematic) { rbody.AddForce(dir * force, ForceMode.Impulse); }
    }

    public override void Seduce(float duration, GameObject target, Transform owner)
    {
        
    }

    public override void vortexGrab(Transform center, float force)
    {
        
    }
}
