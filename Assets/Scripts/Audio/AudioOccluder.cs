using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioOccluder : MonoBehaviour {

	//inspired by https://answers.unity.com/questions/222750/sound-through-wall-audio-zones.html

	AudioSource source;

	float maxDistance;

	public static Transform listener;
//	public float OccludedDistance = 5f;
	public float FadeSpeed = 10f;
	public LayerMask mask;

	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource>();
		maxDistance = source.maxDistance;
	}
	
	// Update is called once per frame
	void Update () {
		float target;
		RaycastHit rch;
		if (Physics.Linecast(transform.position, listener.position, out rch, mask.value)) {
			target = rch.distance;
		}
		else {
			target = maxDistance;
		}
		source.maxDistance = Mathf.MoveTowards(source.maxDistance, target, Time.deltaTime * FadeSpeed);
	}
}
