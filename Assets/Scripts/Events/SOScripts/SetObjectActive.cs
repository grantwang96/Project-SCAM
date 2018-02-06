using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Delegate/SetObjectActive")]
public class SetObjectActive : EventFunction {

    [SerializeField] bool active;
    [SerializeField] GameObject fx;
    [SerializeField] float lifeSpan;
    [SerializeField] float switchDelay;

    public override IEnumerator doThing(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if(fx != null) {
            GameObject newFX = Instantiate(fx, target.transform.position, target.transform.rotation);
            Destroy(newFX, lifeSpan);
        }
        yield return new WaitForSeconds(switchDelay);
        target.active = active;
    }
}
