using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Secondary Spell Effect/Random Replacer Side Effect")]
public class RandomReplacerSideEffect : SpellSecondary {

    [SerializeField] private float speed;
    [SerializeField] Rigidbody[] possibleReplacedObjects;

    public override void MessUp(Transform user, Missile projectile) {
        // get the transform data and original velocity of projectile
        Vector3 originPosition = projectile.transform.position;
        Quaternion originRotation = projectile.transform.rotation;
        Destroy(projectile.gameObject);

        // replace with new random object
        Rigidbody prefab = possibleReplacedObjects[Random.Range(0, possibleReplacedObjects.Length)];
        Rigidbody newReplacement = Instantiate(prefab, originPosition, originRotation);
        newReplacement.velocity = newReplacement.transform.forward * speed;
    }
}
