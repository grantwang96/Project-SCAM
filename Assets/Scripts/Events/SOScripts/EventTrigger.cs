using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour {

    [SerializeField] private bool triggered;
    [SerializeField] bool repeatable;

    [SerializeField] string targetTag;
    [SerializeField] float delayTime;

    [SerializeField] customEvent[] events;
    
    void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == targetTag && !triggered) {
            if (!repeatable) { triggered = true; }
            foreach (customEvent thing in events) {
                StartCoroutine(thing.myFunction.doThing(thing.myTarget, thing.delay));
            }
        }
    }

    [System.Serializable]
    public class customEvent : System.Object
    {
        public EventFunction myFunction;
        public GameObject myTarget;
        public float delay;
    }
}
