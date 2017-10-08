using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TugControls : MonoBehaviour {

	[SerializeField] Transform tugHandle;
	[SerializeField] Transform droneModel;


	public const float PULL_RANGE = 0.25f;
	
	const float SHOWN_ELEVATION_MULTIPLIER = 0.5f;
	const float SHOWN_ANGLE_MULTIPLIER = 22.5f;
	// Use this for initialization

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
