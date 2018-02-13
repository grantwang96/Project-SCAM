using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Delegate/GravityToggle")]
public class GravityOn : EventFunction {

    public override IEnumerator doThing(GameObject target, float delay, string message, int count)
    {
        yield return new WaitForSeconds(delay);
        Rigidbody rbody = target.GetComponent<Rigidbody>();
        if(count > 0) {
            rbody.useGravity = true;
            rbody.isKinematic = false;
            rbody.AddExplosionForce(1f, rbody.position + Random.insideUnitSphere, 1f);
        }
        else { rbody.useGravity = false; rbody.isKinematic = true; }
    }
}
