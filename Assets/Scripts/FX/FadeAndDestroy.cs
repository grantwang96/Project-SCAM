using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FadeAndDestroy : MonoBehaviour {

    public float lifeTime = 5f;
    public float fadeSpeed = 1f;

    MeshRenderer rend;

	// Use this for initialization
	void Start () {
        rend = GetComponent<MeshRenderer>();
        StartCoroutine(fadeNDie());
	}

    IEnumerator fadeNDie() {

        yield return new WaitForSeconds(lifeTime);

        float time = 0f;
        Color originColor = rend.material.color;
        while(time < 1f) {
            time += Time.deltaTime * fadeSpeed;
            rend.material.color = new Color(originColor.r, originColor.g, originColor.b, 1f - time);
            Debug.Log(rend.material.color.a);
            yield return new WaitForEndOfFrame();
        }
        Destroy(this.gameObject);
    }
}
