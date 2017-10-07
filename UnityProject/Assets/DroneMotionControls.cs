using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AR.Drone.Client;
using AR.Drone.Data;


// Reference: https://github.com/Ruslan-B/AR.Drone/blob/master/AR.Drone.WinApp/MainForm.cs
using System;


public class DroneMotionControls {
	DroneClient client;
	public DroneMotionState state { get; protected set; }

	public DroneMotionControls() {
		state = new DroneMotionState();
		client = new DroneClient ("192.168.1.1");
		client.NavigationPacketAcquired += HandleNavPacket;
	}

	public void Start() {
		client.Start ();
	}

	public bool Stop() {
		if (state.liftoff) {
			Debug.LogError ("Land drone before disconnecting");
			return false;
		}
		client.Stop ();
		return true;
	}

	public void CalibrateFloor() {
		client.FlatTrim ();
		state.floorCalibrated = true;
	}

	public bool Takeoff() {
		if (!state.floorCalibrated) {
			Debug.LogError ("Calibrate floor before takeoff");
			return false;
		}
		client.Takeoff ();
		return true;
	}
	public bool Land() {
		client.Land ();
		state.liftoff = false;
		return true;
	}

	public void Panic() {
		client.Emergency ();
		state.liftoff = false;
	}

	public void Unpanic() {
		client.ResetEmergency ();
	}


	// Pitch and roll arguments are in the range [-1,1] and represent percentage of drone's maximum tilt.
	// Negative roll mages drone tilt left, positive right.
	// Negative pitch makes drone tilt forward, positive backward.
	public void SetTilt(float pitchPercentage, float rollPercentage) {
		state.SetIntent(pitch:pitchPercentage,roll:rollPercentage);
	}

	// Angular speed argument is in the range [-1,1].
	// Negative values are counterclockwise, positive clockwise.
	public void SetAngularSpeed(float value) {
		state.SetIntent (yaw: value);
	}

	// Angular speed argument is in the range [-1,1].
	// Negative values cause drone to fall, positive to rise.
	public void SetVerticalSpeed(float value) {
		state.SetIntent (gaz: value);
	}



	public void HandleNavPacket(NavigationPacket packet) {
		Debug.LogFormat ("[{0}] Received nav packet", packet.Timestamp);
	}

	public void ActOnMotionIntent() {
		if (!state.liftoff) {
			return;
		}

		if (!state.actedOnIntent) {
			var intent = state.lastReceivedIntent;
			state.ActOnIntent ();
			client.Progress (AR.Drone.Client.Command.FlightMode.Progressive,
				roll: intent.roll,
				pitch: intent.pitch,
				yaw: intent.yaw,
				gaz: intent.gaz
			);
		}
	}

}
