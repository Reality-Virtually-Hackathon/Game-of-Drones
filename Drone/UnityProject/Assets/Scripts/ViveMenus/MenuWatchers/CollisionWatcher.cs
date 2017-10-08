using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionWatcher<T> : MonoBehaviour {

	public List<T> intersected = new List<T>();
	public bool empty { get { return intersected.Count == 0; } }
	public T latest { get { return intersected [intersected.Count - 1]; } }

	void OnTriggerEnter(Collider other) {
		var c = other.GetComponent<T> ();
		if (c != null) {
			intersected.Add (c);
		}
	}

	void OnTriggerExit(Collider other) {
		var c = other.GetComponent<T> ();
		if (c != null) {
			intersected.Remove (c);
		}
	}
}
