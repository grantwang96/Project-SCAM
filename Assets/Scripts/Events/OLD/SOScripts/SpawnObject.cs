using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Delegate/SpawnObject")]
public class SpawnObject : EventFunction {

    [SerializeField] GameObject prefab;
    [SerializeField] GameObject startingFX;
    [SerializeField] float startingFXLifeSpan;
    [SerializeField] GameObject spawnFX;
    [SerializeField] float spawnFXLifeSpan;

    public override IEnumerator doThing(GameObject target, float delay, string message, int count)
    {
        if(startingFX != null) {
            GameObject startFX = Instantiate(startingFX, target.transform.position, target.transform.rotation);
            Destroy(startFX, startingFXLifeSpan);
        }
        yield return new WaitForSeconds(delay);
        if(spawnFX != null) {
            GameObject spawn = Instantiate(spawnFX, target.transform.position, target.transform.rotation);
            Destroy(spawn, spawnFXLifeSpan);
        }
        Instantiate(prefab, target.transform.position, target.transform.rotation);
    }
}
