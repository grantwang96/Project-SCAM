using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour {

    [SerializeField] private bool triggered;
    [SerializeField] bool repeatable;

    [SerializeField] int maxTriggers;
    private int triggerCount = 0;

    [SerializeField] string targetTag;
    [SerializeField] float delayTime;

    [SerializeField] customEvent[] events;
    public UnityEvent myEvent;

    void OnEnable()
    {
        triggered = false;
    }

    void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == targetTag && triggerCount < maxTriggers && !triggered) {
            triggerCount++;
            if (repeatable) { DoThing(); }
            else if(triggerCount >= maxTriggers && maxTriggers != -1) {
                if (!repeatable) { DoThing(); }
                triggered = true;
            }
        }
    }

    private void DoThing()
    {
        foreach (customEvent thing in events) {
            StartCoroutine(thing.myFunction.doThing(thing.myTarget, thing.delay, thing.customMessage, thing.count));
        }
    }

    [System.Serializable]
    public class customEvent : System.Object
    {
        public EventFunction myFunction;
        public GameObject myTarget;
        public string customMessage;
        public int count;
        public float delay;
    }
}
