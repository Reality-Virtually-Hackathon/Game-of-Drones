using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabController : MonoBehaviour {
	
	SteamVR_TrackedObject to;
	[SerializeField] ButtonWatcher buttons;
	[SerializeField] TugWatcher tugs;
	[SerializeField] WheelWatcher wheels;

	IReleaseable held = null;

	// Use this for initialization
	void Start () {
		to = GetComponentInParent<SteamVR_TrackedObject> ();
		StartCoroutine (DoHaptics ());
	}

	// Update is called once per frame
	void Update () {
		var device = SteamVR_Controller.Input ((int)to.index);

		var pressure = device.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;

		if (held != null) {
			if (device.GetPressUp (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
				held.Release (transform);
				held = null;
			}
		} else {
			if (device.GetPressDown (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
				if (!tugs.empty) {
					var tug = tugs.latest;
					tug.SetHolder (transform);
					held = tug;
				} else if (!wheels.empty) {
					var wheel = wheels.latest;
					wheel.AddHolder (transform);
					held = wheel;
				} else if (!buttons.empty) {
					buttons.latest.Press ();
				}
			}
		}

//		if (device.GetPressDown (Valve.VR.EVRButtonId.k_EButton_ApplicationMenu)) {
//			DroneImpulseController.instance.Enqueue (DroneImpulseController.Command.Land);
//		}
	}

	IEnumerator DoHaptics() {
		while (true) {
			if (held != null) {
				var f = held.GetIntensity ();
				var device = SteamVR_Controller.Input ((int)to.index);
				ushort magnitude = (ushort)Mathf.Lerp (0, 2000, f * 0.8f);
				device.TriggerHapticPulse (magnitude);
				yield return new WaitForSeconds (0.02f);
			} else {
				yield return new WaitForEndOfFrame ();
			}


		}
	}
}
