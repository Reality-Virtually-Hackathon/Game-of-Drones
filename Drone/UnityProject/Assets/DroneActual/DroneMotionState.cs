using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMotionState {


	public bool connectionRequested = false;
	public bool floorCalibrated = false;
	public bool liftoff = false;


	public DroneMotionIntent lastReceivedIntent { get; protected set; }
	public bool actedOnIntent { get; protected set; }


	public void SetIntent(DroneMotionIntent intent) {
		if (lastReceivedIntent == null || intent != lastReceivedIntent) {
			lastReceivedIntent = intent;
			actedOnIntent = false;
		}
	}

	public void SetIntent(float roll=0, float pitch = 0, float yaw = 0, float gaz = 0) {
		SetIntent(new DroneMotionIntent(roll:roll,pitch:pitch,yaw:yaw,gaz:gaz));
	}

	public void ActOnIntent() {
		actedOnIntent = true;
	}
	public void WipeIntent() {
		actedOnIntent = false;
		lastReceivedIntent = null;
	}
}
