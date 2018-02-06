using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcySurface : MonoBehaviour {

    public float friction;
    public float shrinkSpeed;
    public float growSpeed;

    public float lifeTime;
    public float startTime;

    public Vector3 targetScale;

    bool fullSized = false;
    bool dead = false;

    void Start()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(growToSize());
    }

    IEnumerator growToSize()
    {
        float prog = 0f;
        // Vector3 lossScale = new Vector3(targetScale.x / transform.parent.lossyScale.x, targetScale.y / transform.parent.lossyScale.y, targetScale.z / transform.parent.lossyScale.z);
        while (prog < 1f) {
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, prog);
            yield return new WaitForEndOfFrame();
            prog += Time.deltaTime * growSpeed;
        }
        fullSized = true;
        startTime = Time.time;
    }

    void Update()
    {
        if(fullSized && !dead && Time.time - startTime > lifeTime) {
            Die();
        }
    }
    
    void OnTriggerStay(Collider coll)
    {
        Movement move = coll.GetComponent<Movement>();
        if (move) {
            move.friction = friction;
        }
    }

    void OnTriggerExit(Collider coll)
    {
        Movement move = coll.GetComponent<Movement>();
        if (move) {
            move.friction = 1f;
        }
    }

    public void Die()
    {
        dead = true;
        StartCoroutine(processDie());
    }

    IEnumerator processDie()
    {
        Vector3 startScale = transform.localScale;
        friction = 1f;
        float prog = 0f;
        while(prog < 1f) {
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, prog);
            yield return new WaitForEndOfFrame();
            prog += Time.deltaTime * shrinkSpeed;
        }
        Destroy(gameObject);
    }
}
