using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MenuPanel : MonoBehaviour {

    [SerializeField] RectTransform rect;

    public Vector3 startPosition;
    public float startRotation;
    public Vector3 targetPosition;
    public float targetRotation;
    public float speed;
    public float timeStep;

    private static bool moving = false;

    [SerializeField] Button[] menuButtons;

    public UnityEvent OnExitMenu;

    void OnEnable() {
        StartCoroutine(MoveToPosition());
    }

    public void ExitMenu() {
        StartCoroutine(MoveOffScreen());
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ProcessInputs();
	}

    void ProcessInputs() {
        if(Input.GetButtonDown("Cancel") && GameManager.Instance.menuMode
           && GameManager.Instance.currentMenuPanel == gameObject) {
            ExitMenu();
        }
    }

    IEnumerator MoveToPosition() {
        moving = true;
        rect.anchoredPosition = startPosition;
        rect.localEulerAngles = new Vector3(0, 0, startRotation);
        float time = 0f;
        while(time < 1f) {
            rect.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, time);
            rect.localEulerAngles = Vector3.Lerp(new Vector3(0, 0, startRotation), new Vector3(0, 0, targetRotation), time);
            yield return new WaitForEndOfFrame();
            time += speed * timeStep;
        }
        rect.anchoredPosition = targetPosition;
        rect.localEulerAngles = new Vector3(0, 0, targetRotation);
        moving = false;
    }

    IEnumerator MoveOffScreen() {
        moving = true;
        rect.anchoredPosition = targetPosition;
        rect.localEulerAngles = new Vector3(0, 0, targetRotation);
        float time = 0f;
        while (time < 1f) {
            rect.anchoredPosition = Vector3.Lerp(targetPosition, startPosition, time);
            rect.localEulerAngles = Vector3.Lerp(new Vector3(0, 0, targetRotation), new Vector3(0, 0, startRotation), time);
            yield return new WaitForEndOfFrame();
            time += speed * timeStep;
        }
        rect.anchoredPosition = startPosition;
        rect.localEulerAngles = new Vector3(0, 0, startRotation);
        OnExitMenu.Invoke();
        moving = false;
    }

    public void SetButtonsInteractable(bool interactable) {
        for(int i = 0; i < menuButtons.Length; i++) {
            menuButtons[i].interactable = interactable;
        }
    }
}
