using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveImpulseMotionControls : MonoBehaviour {
	

	// Use this for initialization
	void Start () {
		to = GetComponent<SteamVR_TrackedObject> ();
	}


	SteamVR_TrackedObject to;
	const float multiplier = 10;
	// Use this for initialization


	Vector3 lastPosition;
	Vector2 lastTouch;
	// Update is called once per frame
	void Update () {

		var device = SteamVR_Controller.Input ((int)to.index);


		if (device.GetPressDown (Valve.VR.EVRButtonId.k_EButton_ApplicationMenu)) {
			DroneImpulseController.instance?.Enqueue (DroneImpulseController.Command.Land);
		}
		if (device.GetPressDown (Valve.VR.EVRButtonId.k_EButton_Grip)) {
			DroneImpulseController.instance?.Enqueue (DroneImpulseController.Command.Panic);
		}

		var pressure = device.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;

		if (pressure > 0.01f) {

			Vector3 deltaRaw = (transform.position - lastPosition) * pressure / Time.deltaTime;

			Vector3 deltaRawXZ = Vector3.ProjectOnPlane (deltaRaw, Vector3.up);
			Vector3 delta = new Vector3 (Vector3.Dot (deltaRawXZ, transform.right), deltaRaw.y, Vector3.Dot (deltaRawXZ, transform.forward));

			Debug.Log (delta);
			DroneImpulseController.instance?.Impulse (delta * multiplier);
		}

		if (device.GetTouchDown (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad)) {
			lastTouch = device.GetAxis ();
		} else if(device.GetTouch(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad)) {
			var currentTouch = device.GetAxis ();

			var touchDelta = currentTouch - lastTouch;

			var angle = Vector2.Angle (lastTouch,currentTouch);
			Debug.Log (angle/90);

			DroneImpulseController.instance?.Yaw (angle/90);

			lastTouch = currentTouch;
		}


		lastPosition = transform.position;
	}
}
