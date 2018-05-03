using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Victory : MonoBehaviour {

    public Text passingMessage;

    public float zoomSpeed;
    public float fadeSpeed;
    public float shakeDuration;
    public float shakeRadius;
    public float holdMessageTime;

    void OnTriggerEnter(Collider coll) {
        if(coll.gameObject == PlayerDamageable.Instance.gameObject) {
            InitiateVictory();
        }
    }

    private void InitiateVictory() {
        Time.timeScale = 0f;
        StartCoroutine(YouPass());
    }

    IEnumerator YouPass() {

        PlayerDamageable.Instance.dead = true;

        float time = 0f;
        while (time < 1f) {
            time += Time.unscaledDeltaTime * fadeSpeed;
            PlayerDamageable.Instance.fadeToBlackImage.color =
                Color.Lerp(Color.clear, Color.black, time);
            yield return null;
        }
        time = 0f;

        Text newPassMessage = Instantiate(passingMessage, PlayerDamageable.Instance.playerCanvas);
        newPassMessage.rectTransform.localScale = Vector3.one * 3f;
        newPassMessage.enabled = true;

        while (time < 1f) {
            time += Time.unscaledDeltaTime * zoomSpeed;
            newPassMessage.rectTransform.localScale = Vector3.Lerp(Vector3.one * 3f, Vector3.one, time);
            yield return null;
        }
        time = 0f;
        while(time < shakeDuration) {
            time += Time.unscaledDeltaTime;
            newPassMessage.rectTransform.anchoredPosition = Random.insideUnitCircle * shakeRadius;
            yield return null;
        }
        newPassMessage.rectTransform.anchoredPosition = Vector3.zero;

        yield return new WaitForSecondsRealtime(holdMessageTime);

        time = 0f;
        Color originColor = newPassMessage.color;
        while(time < 1f) {
            time += Time.unscaledDeltaTime;
            newPassMessage.color = Color.Lerp(originColor, Color.clear, time);
            yield return null;
        }

        AsyncOperation async = SceneManager.LoadSceneAsync(0);
        async.completed += Async_completed;
        while (!async.isDone) {
            yield return null;
        }

    }

    private void Async_completed(AsyncOperation obj)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
    }
}
