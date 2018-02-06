using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrans : MonoBehaviour {

    public string sceneName;

	public void replay()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // GameObject.Find("Scorekeeper").GetComponent<ScoreKeeper>().resetScore();
        SceneManager.LoadScene(sceneName);
    }
}
