using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControls : MonoBehaviour {

	class ControllerTracker {
		public Transform controllerTransform;
		public Vector3 lastPosition;
	}

	List<Transform> connectedControllers;

	// Update is called once per frame
	void Update () {
		var delta = transform.InverseTransformVector (tugHandle.position - transform.position) / PULL_RANGE;

		if (delta.magnitude > 1) {
			delta = delta.normalized;
		}

		var pitch = delta.z;
		var roll = -delta.x;
		var gaz = delta.y;

		droneModel.transform.localRotation = Quaternion.Euler (SHOWN_ANGLE_MULTIPLIER*pitch, 0, SHOWN_ANGLE_MULTIPLIER*roll);
		droneModel.transform.localPosition = Vector3.up * gaz * SHOWN_ELEVATION_MULTIPLIER * PULL_RANGE;
		DroneImpulseController.instance?.Impulse (delta);
	}
}
