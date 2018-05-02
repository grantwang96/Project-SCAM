using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchLightCorrector : MonoBehaviour {

    public ParticleSystem ps;
    public Light l;

	// Use this for initialization
	void Start () {
        ps = GetComponent<ParticleSystem>();
        l = GetComponent<Light>();
        ParticleSystem.MainModule main = ps.main;
        main.startColor = l.color;
	}
}
