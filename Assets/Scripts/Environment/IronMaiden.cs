using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronMaiden : MonoBehaviour {

    public Animator anim;
    public Transform mouth;
    public float suckSpeed = 2f;
    public Damageable currentDamageable;

    Coroutine trapRoutine;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider coll) {
        if(coll.tag == "Enemy" && trapRoutine == null) {
            Damageable dam = coll.GetComponent<Damageable>();
            if(dam) {
                currentDamageable = dam;
                trapRoutine = StartCoroutine(TrapRoutine(dam));
            }
        } else if(coll.tag == "Player" && trapRoutine == null) {
            // float dist = Vector3.Distance(coll.transform.position, mouth.position);
            Damageable dam = coll.GetComponent<Damageable>();
            if (dam) {
                currentDamageable = dam;
                trapRoutine = StartCoroutine(TrapRoutine(dam));
            }
        }
    }

    IEnumerator TrapRoutine(Damageable dam) {
        float time = 0f;
        if(dam.tag == "Enemy") {
            Movement move = dam.myMovement;
            if (move != null)
            {
                move.anim.enabled = false;
                move.agent.isStopped = true;
                move.agent.updatePosition = false;
                move.agent.updateRotation = false;
                if (move.rbody) { move.rbody.isKinematic = true; }
                move.enabled = false;
            }
        }
        Vector3 startPos = dam.transform.position;
        while(time < 1f) {
            time += Time.deltaTime * suckSpeed;
            dam.transform.position = Vector3.Lerp(startPos, mouth.position, time);
            yield return new WaitForEndOfFrame();
        }
        /*
        RuntimeAnimatorController runanim = anim.runtimeAnimatorController;
        for (int i = 0; i < runanim.animationClips.Length; i++) {
            if (runanim.animationClips[i].name == "IronMaidenClamp") {
                AnimationEvent evt = runanim.animationClips[i].events[0];
                Debug.Log("setting up anim event");
                evt.functionName = "DoDamage";
                evt.objectReferenceParameter = dam;
                break;
            }
        }*/
        anim.Play("IronMaidenClamp");
        trapRoutine = null;
    }

    public void DoDamage()
    {
        if(currentDamageable.tag == "Enemy") { currentDamageable.TakeDamage(null, 999, Vector3.zero, 0f); }
        else if(currentDamageable.tag == "Player") { currentDamageable.TakeDamage(null, 10, transform.forward, 20f); }
        // play sfx?
    }
}
