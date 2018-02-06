using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour, Interactable {

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

    // private Key myKey; // the key that unlocks this door

    public void Interact(SpellCaster spellCaster)
    {
        if (_locked) {
            return;
        }
        if (movement != null) { return; }
        movement = StartCoroutine(doorRotation());
    }

    public void Interact() {
        if(_locked) {
            return;
        }
        if(movement != null) { return; }
        movement = StartCoroutine(doorRotation());
    }

    public void Unlock(string keyCode)
    {

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

    void Start()
    {
        if(open) { myHinge.localEulerAngles = openRotation; }
        else { myHinge.localEulerAngles = closedRotation; }
    }
}
