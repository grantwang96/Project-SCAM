using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Secondary Spell Effect/Split Side Effect")]
public class SplitSideEffect : SpellSecondary {

    [SerializeField] private int splitLowerBound;
    [SerializeField] private int splitUpperBound;
    [SerializeField] private float lowerHeightOffset;
    [SerializeField] private float upperHeightOffset;

    [SerializeField] private float radius;

    public override void OnHit(Transform user, Missile projectile, Collision coll)
    {
        int shrapnel = Random.Range(splitLowerBound, splitUpperBound);
        float angInterval = 360f / shrapnel;
        for(int i = 0; i < shrapnel; i++) {
            float ang = angInterval * i;
            Vector3 offset;
            offset.x = projectile.transform.position.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            offset.y = projectile.transform.position.y + Random.Range(lowerHeightOffset, upperHeightOffset);
            offset.z = projectile.transform.position.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            subBlast(projectile, offset, projectile.transform.position);
        }
    }

    private void subBlast(Missile projectile, Vector3 startpos, Vector3 origin)
    {
        Missile newproj = Instantiate(projectile, startpos, Quaternion.identity);
        newproj.transform.forward = startpos - origin;
        newproj.derped = false;

        Rigidbody rbody = newproj.GetComponent<Rigidbody>();
        rbody.AddForce(newproj.transform.forward * projectile.GetComponent<Rigidbody>().velocity.magnitude * 2, ForceMode.Impulse);
    }
}
