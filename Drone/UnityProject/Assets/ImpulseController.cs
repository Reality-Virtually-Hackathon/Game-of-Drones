using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseController : MonoBehaviour {
	SteamVR_TrackedObject to;
	const float multiplier = 3;
	// Use this for initialization
	void Start () {
		to = GetComponent<SteamVR_TrackedObject> ();
	}

	Vector3 lastPosition;
	// Update is called once per frame
	void Update () {

		var device = SteamVR_Controller.Input ((int)to.index);

		var pressure = device.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;

		if (pressure > 0.01f) {

			Vector3 delta = (transform.position - lastPosition) * pressure / Time.deltaTime;

			ImpulseTarget.instance?.Impulse (delta * multiplier);
		}

		lastPosition = transform.position;
	}
}
