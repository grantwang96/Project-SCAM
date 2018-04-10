﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour {

	public string[] labels;

	public AudioClip[] clips;

	AudioSource source;
	
	public void Start() {
		source = GetComponent<AudioSource>();

		if (labels.Length != clips.Length) {
			throw new UnityException("labels and clips lengths don't match!");
		}
	}

	public void PlayClip(string label) {
		//plays clip that shares index of label in array labels


		int index = -1;
		for (int i = 0; i < labels.Length; i ++) {
			if (labels[i] == label) {
				index = i;
			}
		}
		if (index > 0) {
			source.PlayOneShot(clips[index]);
		}
		else {
			Debug.Log(label + " not found!");
		}
	}
}
