﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Custom Delegate/ToggleDoorLock")]
public class ToggleDoorLock : EventFunction {

    [SerializeField] bool locked;

    public override IEnumerator doThing(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        Door door = target.GetComponent<Door>();
        if(door == null) { yield break; }
        door.locked = locked;
    }
}
