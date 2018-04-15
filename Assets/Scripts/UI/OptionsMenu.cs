using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour {

	public void SetMasterVolume(float val) {
        AudioListener.volume = val;
    }
}
