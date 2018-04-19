using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour {

	public string[] labels;

	public AudioClip[] clips;

	Dictionary<string, AudioClip> lib = new Dictionary<string, AudioClip>();

	AudioSource source;

	public bool isPlaying {
		get {
			return source.isPlaying;
		}
	}
	
	public void Start() {
		source = GetComponent<AudioSource>();

		if (labels.Length != clips.Length) {
			throw new UnityException("labels and clips lengths don't match!");
		}

		for (int i = 0; i < labels.Length; i ++) {
			lib.Add(labels[i], clips[i]);
		}

	}

	public void PlayClip(string label) {
		//plays clip that shares index of label in array labels

		AudioClip toPlay;

		lib.TryGetValue(label, out toPlay);

		if (toPlay != null) {
			source.PlayOneShot(toPlay);
		}
		else {
			Debug.Log(label + " clip not found!");
		}

	}

	public AudioClip GetClip(string label) {
		AudioClip toRet;

		lib.TryGetValue(label, out toRet);

		return toRet;
	}

}
