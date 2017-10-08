using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionWatcher : MonoBehaviour {

	public List<Collider> intersected = new List<Collider>();


	void OnTriggerEnter(Collider other) {
		intersected.Add (other);
	}

	void OnTriggerExit(Collider other) {
		intersected.Remove(Collider);
	}
}
