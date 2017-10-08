using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AR.Drone.Client;
using AR.Drone.Data;
using AR.Drone.Infrastructure;
using AR.Drone.Avionics;
using AR.Drone.Video;
using AR.Drone.Data.Navigation.Native;

// Reference: https://github.com/Ruslan-B/AR.Drone/blob/master/AR.Drone.WinApp/MainForm.cs
using System;
using AR.Drone.Client.Configuration;


public class DroneMotionControls {
	DroneClient client;
	public DroneMotionState state { get; protected set; }

	VideoPacketDecoder decoder;

	public event Action<VideoFrame> onVideo;
	public event Action<NavdataBag> onNav;

	public DroneMotionControls() {
		state = new DroneMotionState();
		client = new DroneClient ("192.168.1.1");
		Settings settings = new Settings ();
		client.NavigationPacketAcquired += HandleNavPacket;
		client.VideoPacketAcquired += HandleVideoPacket;
		decoder  = new VideoPacketDecoder (PixelFormat.BGR24);

	}



	public void Start() {
		client.Start ();
		Debug.Log ("Start");
	}

	public bool Stop() {
		if (state.liftoff) {
			Debug.LogError ("Land drone before disconnecting");
			return false;
		}
		Debug.Log ("Stop");
		client.Stop ();
		return true;
	}

	public void CalibrateFloor() {
		client.FlatTrim ();
		Debug.Log ("FlatTrim");
		state.floorCalibrated = true;
	}

	public bool Takeoff() {
		if (!state.floorCalibrated) {
			Debug.LogError ("Calibrate floor before takeoff");
			return false;
		}
		Debug.Log ("Takeoff");
		client.Takeoff ();
		state.liftoff = true;
		return true;
	}
	public bool Land() {
		client.Land ();
		Debug.Log ("Land");
		state.liftoff = false;
		return true;
	}

	public void Panic() {
		client.Emergency ();
		client.Land ();
		state.liftoff = false;
		Debug.Log ("Emergency");
	}

	public void Unpanic() {
		client.ResetEmergency ();
		Debug.Log ("ResetEmergency");
	}


	// Pitch and roll arguments are in the range [-1,1] and represent percentage of drone's maximum tilt.
	// Negative roll mages drone tilt left, positive right.
	// Negative pitch makes drone tilt forward, positive backward.
	public void SetTilt(float pitchPercentage, float rollPercentage) {
		var intent = state.lastReceivedIntent.Copy ();
		intent.pitch = pitchPercentage;
		intent.roll = rollPercentage;
		state.SetIntent(intent);
	}

	// Angular speed argument is in the range [-1,1].
	// Negative values are counterclockwise, positive clockwise.
	public void SetAngularSpeed(float value) {
		var intent = state.lastReceivedIntent.Copy ();
		intent.yaw = value;
		state.SetIntent (intent);
	}

	// Angular speed argument is in the range [-1,1].
	// Negative values cause drone to fall, positive to rise.
	public void SetVerticalSpeed(float value) {
		var intent = state.lastReceivedIntent.Copy ();
		intent.gaz = value;
		state.SetIntent (intent);
	}



	public void HandleNavPacket(NavigationPacket packet) {
//		Debug.LogFormat ("[{0}] Received nav packet", packet.Timestamp);
		if (onNav != null) {
			NavdataBag bag;
			if (NavdataBagParser.TryParse (ref packet, out bag)) {
				onNav (bag);
			}
		}
		
	}

	public void HandleVideoPacket(VideoPacket packet) {
		Debug.LogFormat ("[{0}] Received vid packet", packet.Timestamp);
		if (onVideo != null) {
			VideoFrame frame;
			Debug.Log ("Decode???");
			if (decoder.TryDecode (ref packet, out frame)) {
				Debug.Log ("Success");
				onVideo (frame);
			}
		}

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
			Debug.Log ("Progress");
		}
	}

}
