using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterfall : MonoBehaviour {

    public MeshRenderer rend;
    float time = 0f;

    public float speed;

	// Use this for initialization
	void Start () {
        float x = rend.transform.localScale.x;
        float y = rend.transform.localScale.y;

        Vector2 newMatScale = new Vector2();
        if(x > y) {
            newMatScale.x = x / y;
            newMatScale.y = 1f;
        }
        else {
            newMatScale.x = 1f;
            newMatScale.y = y / x;
        }
        rend.material.mainTextureScale = newMatScale;
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime * speed;
        if(time > 1f) { time = 0f; }
        rend.material.mainTextureOffset = new Vector2(0f, time);
	}
}
