using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

    public Slider MouseSensitivity;
    public Slider CTRSensitivity;
    public Slider MasterVolume;
    public Dropdown ScreenResOptions;
    public Toggle fullScreen;

    void OnEnable() {
        MasterVolume.value = AudioListener.volume;
        CTRSensitivity.value = CameraMovement.ctrSensitivity;
        MouseSensitivity.value = CameraMovement.mouseSensitivity;
    }

	public void SetMasterVolume(float val) {
        AudioListener.volume = val;
    }

    public void SetScreenResolution() {
        string res = ScreenResOptions.options[ScreenResOptions.value].text;
        string[] resBits = res.Split(' ');
        int width = -1;
        int height = -1;
        if(int.TryParse(resBits[0], out width) && int.TryParse(resBits[2], out height)) {
            Screen.SetResolution(width, height, fullScreen.isOn);
        }
    }

    public void SetFullScreen(bool yes)
    {
        Screen.fullScreen = yes;
    }

    public void SetMouseSensitivity(float val)
    {
        CameraMovement.mouseSensitivity = val;
    }

    public void SetCTRSensitivity(float val)
    {
        CameraMovement.ctrSensitivity = val;
    }
}
