using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CriticalPathNode : ScriptableObject {

    public delegate void OnEventTrigger();
    public OnEventTrigger scriptedEvent;
}
