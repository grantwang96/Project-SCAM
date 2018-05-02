using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    public Image fadeOut;
    public float moveSpeed;
    public bool moving = false;

    public GameObject loadScreenSkele;
    public Text loadScreenMessage;
    public Image loadScreenBar;

    public GameObject QuitMenu;
    
    void Awake() {
        Instance = this;
    }

    public void NewGame() {
        StartCoroutine(LoadScene(1));
    }

    public void GoHere(Transform point)
    {
        if (moving) { return; }
        StartCoroutine(GoingHere(point));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator GoingHere(Transform point) {

        moving = false;

        Vector3 startPos = Camera.main.transform.position;
        Quaternion startRot = Camera.main.transform.rotation;

        float time = 0f;
        
        while(time < 1f) {
            time += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(startPos, point.position, Mathf.SmoothStep(0f, 1f, time));
            Camera.main.transform.rotation = Quaternion.Lerp(startRot, point.rotation, Mathf.SmoothStep(0f, 1f, time));
            yield return new WaitForEndOfFrame();
        }

        Camera.main.transform.position = point.position;
        Camera.main.transform.rotation = point.rotation;

        moving = false;
    }

    IEnumerator LoadScene(int sceneNum) {
        fadeOut.enabled = true;
        float time = 0f;
        while(time < 1f) {
            time += Time.deltaTime;
            fadeOut.color = Color.Lerp(Color.clear, Color.black, time);
            yield return new WaitForEndOfFrame();
        }
        loadScreenSkele.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNum);
        loadScreenMessage.gameObject.SetActive(true);
        while(!asyncLoad.isDone) {
            if(asyncLoad.progress >= .75f) {
                loadScreenMessage.text = "Murdering Tim...";
            } else if(asyncLoad.progress >= .5f) {
                loadScreenMessage.text = "Ordering spellbooks from eBay...";
            } else if(asyncLoad.progress >= .25f) {
                loadScreenMessage.text = "Removing the bodies...";
            } else {
                loadScreenMessage.text = "Dusting castle...";
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
