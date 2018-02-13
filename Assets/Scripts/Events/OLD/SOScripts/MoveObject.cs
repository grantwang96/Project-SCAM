using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Delegate/MoveObject")]
public class MoveObject : EventFunction {

    [SerializeField] GameObject teleportFX;
    [SerializeField] float teleFXLifeSpan;

    public override IEnumerator doThing(GameObject target, float delay, string message, int count)
    {
        yield return new WaitForSeconds(delay);
        GameObject toBeMoved = GameObject.Find(message);
        if(teleportFX != null) {
            GameObject newtelefx = Instantiate(teleportFX, toBeMoved.transform.position, toBeMoved.transform.rotation);
            Destroy(newtelefx, teleFXLifeSpan);
        }
        Vector3 newpos = target.transform.position;
        newpos += Vector3.up * count; // always place on top of
        toBeMoved.transform.position = newpos;
        if (teleportFX != null) {
            GameObject newtelefx = Instantiate(teleportFX, toBeMoved.transform.position, toBeMoved.transform.rotation);
            Destroy(newtelefx, teleFXLifeSpan);
        }
    }
}
