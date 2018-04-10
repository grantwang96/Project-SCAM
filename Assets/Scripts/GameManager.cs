using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour { // will handle game events such as item spawning

    public static GameManager Instance; // the singleton
    public List<GameObject> enemiesOnScreen = new List<GameObject> (); // list of enemies

    public SpellSpawn[] spellSpawns;
    public LayerMask spellBooksLayer;

    public bool gameIsRunning;
    public bool menuMode;

    [SerializeField] GameObject PauseCanvas;
    [SerializeField] GameObject PausePanel;
    public GameObject currentMenuPanel;

    #region Prefabs N Stuff
    public GameHint messagePrefab;
    #endregion

    // Use this for initialization
    void Start () {
        Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetButtonDown("Cancel") && !menuMode) {
            TogglePauseMenu();
        }
	}

    public void TogglePauseMenu() {
        if(menuMode) {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PauseCanvas.SetActive(false);
        }
        else {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            PauseCanvas.SetActive(true);
        }
        menuMode = !menuMode;
        SetCurrentMenu(PausePanel);
    }

    public void SetCurrentMenu(GameObject currMenu)
    {
        currentMenuPanel = currMenu;
    }
    
    void shuffle<T>(T[] array)
    {
        for(int i = 0; i < array.Length; i++)
        {
            T temp = array[i];
            int rand = Random.Range(0, array.Length);
            array[i] = array[rand];
            array[rand] = temp;
        }
    }

    void InitiateWinState()
    {
        Debug.Log("You Win!");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Win");
    }

    public void InitiateLoseState()
    {
        Debug.Log("You Lose!");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Lose");
    }

    public void SendHint(string message)
    {
        GameHint newMessage = Instantiate(messagePrefab, PlayerDamageable.Instance.playerCanvas);
        newMessage.message = message;
    }

    public void UnlockDoor(Door door)
    {
        door.locked = false;
    }

    public void LockDoor(Door door)
    {
        door.locked = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
