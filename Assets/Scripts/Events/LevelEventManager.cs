using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEventManager : MonoBehaviour {

    public static LevelEventManager Instance; // this should be attached to the player
    public CriticalPathNode currentNode;
    [SerializeField] List<CriticalPathNode> nodePath = new List<CriticalPathNode>();

	// Use this for initialization
	void Start () {
        Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		if(currentNode != null) {
            
        }
	}
}
