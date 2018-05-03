using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBookCover : MonoBehaviour {

    public Material[] mats;
    public MeshRenderer rend;

    public int changeIdx;

	// Use this for initialization
	void Start () {
        rend.materials[changeIdx] = mats[Random.Range(0, mats.Length)];
	}
}
