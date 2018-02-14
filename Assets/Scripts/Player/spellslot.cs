using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class spellslot : MonoBehaviour {

    public Text spellTitle;
    // public Text spellDescription;
    public Image ammoBar;
    public Image ammoBarInner;

    public int fullSizeWidth;
    public int fullSizeHeight;
    public int minimizedWidth;
    public int minimizedHeight;

    public static float speed = 10f;

    Coroutine resizingProcess;
    RectTransform myRect;

    void Awake()
    {
        myRect = GetComponent<RectTransform>();
    }
    
    public void Select(string title, string description, int currAmmo, int maxAmmo, Color ammoColor)
    {
        modifyDetails(title, description, currAmmo, maxAmmo, ammoColor);
        if (myRect.rect.width == fullSizeWidth && myRect.rect.height == fullSizeHeight) { return; } // if already selected
        if(resizingProcess != null) {
            StopCoroutine(resizingProcess);
        }
        // setTitle(title);
        resizingProcess = StartCoroutine(processSelection(description, currAmmo, maxAmmo, ammoColor));
    }

    public void Deselect()
    {
        // spellDescription.gameObject.SetActive(false);
        ammoBar.gameObject.SetActive(false);
        if (myRect.rect.width == minimizedWidth && myRect.rect.height == minimizedHeight) { return; }
        if (resizingProcess != null) {
            StopCoroutine(resizingProcess);
        }
        resizingProcess = StartCoroutine(processDeselection());
    }

    public void setTitle(string newTitle) {
        spellTitle.text = newTitle;
    }

    public void modifyDetails(string title, string description, int currAmmo, int maxAmmo, Color ammoColor)
    {
        spellTitle.text = title;
        // spellDescription.text = description;
        ammoBarInner.color = ammoColor;
        ammoBarInner.fillAmount = (float)currAmmo / maxAmmo;
    }

    public IEnumerator processSelection(string description, int currAmmo, int maxAmmo, Color ammoColor)
    {
        float prog = 0f;
        float startWidth = myRect.rect.width;
        float startHeight = myRect.rect.height;
        while(prog < 1f) {
            float newWidth = Mathf.Lerp(startWidth, fullSizeWidth, prog);
            float newHeight = Mathf.Lerp(startHeight, fullSizeHeight, prog);
            myRect.sizeDelta = new Vector2(newWidth, newHeight);
            prog += Time.deltaTime * speed;
            yield return new WaitForEndOfFrame();
        }
        myRect.sizeDelta = new Vector2(fullSizeWidth, fullSizeHeight);
        // spellDescription.gameObject.SetActive(true);
        ammoBar.gameObject.SetActive(true);
        resizingProcess = null;
    }

    public IEnumerator processDeselection() {
        float prog = 0f;
        float startWidth = myRect.rect.width;
        float startHeight = myRect.rect.height;
        while (prog < 1f)
        {
            float newWidth = Mathf.Lerp(startWidth, minimizedWidth, prog);
            float newHeight = Mathf.Lerp(startHeight, minimizedHeight, prog);
            myRect.sizeDelta = new Vector2(newWidth, newHeight);
            prog += Time.deltaTime * speed;
            yield return new WaitForEndOfFrame();
        }
        myRect.sizeDelta = new Vector2(minimizedWidth, minimizedHeight);
        resizingProcess = null;
    }
}
