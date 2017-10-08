using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControls : MonoBehaviour, IReleaseable {

	class ControllerTracker {
		public Transform controllerTransform;
		public Vector3 lastPosition;
	}

	Dictionary<int, ControllerTracker> holders = new Dictionary<int, ControllerTracker>();

	// Update is called once per frame
	void Update () {
		foreach (var tracking in holders.Values) {

		}
	}


	public void AddHolder(Transform origin) {
		holders.Add (origin.GetHashCode (), new ControllerTracker () {
			controllerTransform = origin,
			lastPosition = origin.position
		});
	}

	public void Release(Transform origin) {
		if (holders.ContainsKey (origin.GetHashCode ())) {
			holders.Remove (origin.GetHashCode ());
		}
	}
}
