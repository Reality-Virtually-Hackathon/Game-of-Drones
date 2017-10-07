using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMotionControls : MonoBehaviour {
	DroneMotionControls drone = new DroneMotionControls();

	// Use this for initialization
	void Start () {
		drone.Start ();
		drone.CalibrateFloor ();
	}

	void OnDestroy() {
		if (!drone.Stop ()) {
			drone.Panic ();
			drone.Stop ();
		}
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.F1)) {
			drone.Panic ();
		}
		if (Input.GetKeyDown (KeyCode.F2)) {
			drone.Unpanic ();
		}

		if (Input.GetKeyDown (KeyCode.L)) {
			drone.Land ();
		}

		if (drone.state.liftoff) {
			drone.SetTilt (Input.GetAxis ("LeftRight"), -Input.GetAxis ("ForwardBack"));
			drone.SetAngularSpeed (Input.GetAxis ("ClockwiseCounterclockwise"));
			drone.SetVerticalSpeed (Input.GetAxis ("UpDown"));

			drone.ActOnMotionIntent ();
		} else {
			if (Input.GetAxis ("UpDown") > 0) {
				drone.Takeoff ();
			}
		}
	}
}
