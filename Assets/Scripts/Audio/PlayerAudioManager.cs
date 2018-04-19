using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		AudioOccluder.listener = transform;
	}

}
