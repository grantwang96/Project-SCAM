using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHint : MonoBehaviour {

    public float lifeSpan;
    public float messageSpeed;
    public string message;
    public Text text;

    public Vector3 targetPosition;
    Vector3 startPosition;
    RectTransform rect;

    public static GameHint Instance; // Do not have more than one message on screen at a time!

	// Use this for initialization
	void Start () {

        // make sure you are the ONLY message out there
        if(Instance != null) {
            Instance.StopAllCoroutines();
            Destroy(Instance.gameObject);
        }
        Instance = this;

        rect = GetComponent<RectTransform>();
        text.text = message;
        startPosition = rect.anchoredPosition;
        StartCoroutine(messageAppear());
	}
	
    IEnumerator messageAppear() {
        float prog = 0f;
        float step = 0f;
        while(prog < 1f) {
            rect.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, prog);
            yield return new WaitForEndOfFrame();
            step += Time.deltaTime * messageSpeed;
            float newprog = Mathf.Sin(step);
            if(newprog < prog) { prog = 1f; }
            else { prog = newprog; }
        }
        yield return new WaitForSeconds(lifeSpan);
        StartCoroutine(messageDisappear());
    }

    public IEnumerator messageDisappear()
    {
        float prog = 0f;
        Image myBackground = GetComponent<Image>();
        float textStartAlpha = text.color.a;
        float textAlpha = textStartAlpha;
        float backgroundStartAlpha = myBackground.color.a;
        float backgroundAlpha = backgroundStartAlpha;
        while (text.color != Color.clear)
        {
            textAlpha = Mathf.Lerp(textAlpha, 0f, prog);
            backgroundAlpha = Mathf.Lerp(backgroundStartAlpha, 0f, prog);
            text.color = new Color(text.color.r, text.color.g, text.color.b, textAlpha);
            myBackground.color = new Color(myBackground.color.r, myBackground.color.g, myBackground.color.b, backgroundAlpha);
            yield return new WaitForEndOfFrame();
            prog += Time.deltaTime;
        }
        Destroy(gameObject);
    }
}
