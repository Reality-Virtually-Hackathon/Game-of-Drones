using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMotionState {


	public bool connectionRequested = false;
	public bool floorCalibrated = false;
	public bool liftoff = false;


	public DroneMotionIntent lastReceivedIntent { get; protected set; } = new DroneMotionIntent();
	public bool actedOnIntent { get; protected set; }
	

	public void SetIntent(DroneMotionIntent intent) {
		if (lastReceivedIntent == null || intent != lastReceivedIntent) {
			lastReceivedIntent = intent;
			actedOnIntent = false;
		}
	}


	public void ActOnIntent() {
		actedOnIntent = true;
	}
	public void WipeIntent() {
		actedOnIntent = false;
		lastReceivedIntent = null;
	}
}
