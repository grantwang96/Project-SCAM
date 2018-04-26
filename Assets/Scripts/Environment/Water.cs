using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Water : MonoBehaviour {

	public float rho = 1f;
	public float dmgTickTime = 3f;

	class inWaterType {
		public GameObject obj;
		public float timeInWater;

		public inWaterType(GameObject obj) {
			this.obj = obj;
			this.timeInWater = 0f;
		}
	}

	List<inWaterType> inWater = new List<inWaterType>();
	public GameObject FindInWater(GameObject obj) {
		for (int i = 0; i < inWater.Count; i ++) {
			if (inWater[i].obj.Equals(obj)) {
				return inWater[i].obj;
			}
		}

		return null;
	}

	inWaterType FindStruct(GameObject obj) {
		for (int i = 0; i < inWater.Count; i ++) {
			if (inWater[i].obj.Equals(obj)) {
				return inWater[i];
			}
		}

		return new inWaterType(null);
	}

	void RemoveFromList(GameObject obj) {
		inWaterType victim = FindStruct(obj);
		if (victim.obj != null) {
			inWater.Remove(victim);
		}
	}


	Collider col;

	// Use this for initialization
	void Start () {
		col = GetComponent<Collider>();
	}
	
	void OnTriggerEnter(Collider other) {
		inWater.Add(new inWaterType(other.gameObject));
		Debug.Log("In Water: " + other.name);
	}

	void Update() {
		foreach (inWaterType obj in inWater) {
			//damage tick
			obj.timeInWater += Time.deltaTime;
			if (obj.timeInWater >= dmgTickTime) {
				obj.timeInWater = 0;
				Drownable drown = obj.obj.GetComponent<Drownable>();
				if (drown != null) {
					drown.DealDrownDamage();
				}
			}

			//physics

		}
	}

	void OnTriggerExit(Collider other) {
		inWater.Remove(FindStruct(other.gameObject));
	}

}
