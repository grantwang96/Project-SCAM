using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarHandler : MonoBehaviour {

    public static StatusBarHandler instance;

    #region Status Icons
    public Sprite transmutationIcon;
    public Sprite seductionIcon;
    #endregion

    List<IconHandler> statuses = new List<IconHandler>();
    public Image statusIconPrefab; // prefab to instantiate

    // Use this for initialization
    void Awake () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		foreach(IconHandler handler in statuses)
        {
            if(handler.currDuration > handler.startDuration) {
                Destroy(handler.icon.gameObject);
                statuses.Remove(handler);
            }
            handler.currDuration += Time.deltaTime;
        }
	}

    public void applyStatus(string type, float duration) {
        
    }

    class IconHandler
    {
        public Image icon;
        public float startDuration;
        public float currDuration;
    }
}
