using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFunction : ScriptableObject {
    
    public virtual IEnumerator doThing(GameObject target, float delay, string message, int count)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log(target.name);
    }
}
