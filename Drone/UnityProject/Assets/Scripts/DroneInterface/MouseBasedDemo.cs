using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AR.Drone.Client;
using AR.Drone.Data;

// Reference: https://github.com/Ruslan-B/AR.Drone/blob/master/AR.Drone.WinApp/MainForm.cs
using System;


public class MouseBasedDemo : MonoBehaviour {

	const float PITCH_SCALE = 1;// 0.05f;
	const float YAW_SCALE = 1f;
	const float ROLL_SCALE = 1f;
	const float GAZ_SCALE = 1f;

	class InputState {
		public float roll = 0;
		public float pitch = 0;
		public float yaw = 0;
		public float gaz = 0;

		public override bool Equals (object obj)
		{
			if (obj == null || GetType () != obj.GetType ()) {
				return false;
			}
			var other = (InputState)obj;
			return this.roll == other.roll && this.pitch == other.pitch && this.yaw == other.yaw &&  this.gaz == other.gaz;
		}

		public bool IsZero() {
			return roll == 0 && pitch == 0 && yaw == 0 && gaz == 0;
		}
		public override string ToString ()
		{

			return string.Format ("{0}\t{1}\t{2}\t{3}", roll, pitch, yaw, gaz);
		}
	}

	DroneClient client;
	// Use this for initialization
	void Start () {
		client = new DroneClient ("192.168.1.1");
		client.NavigationPacketAcquired += HandleNavPacket;
	}

	void HandleNavPacket(NavigationPacket packet) {
		Debug.LogFormat ("[{0}] Received nav packet", packet.Timestamp);
	}

	InputState prevState = new InputState();
	// Update is called once per frame
	void Update () {
//		Debug.LogFormat ("Active? {0}, Connected? {1}",client.IsActive, client.IsConnected);

		bool acted = KeyDownDo (KeyCode.F1, client.Start) ||
			         KeyDownDo (KeyCode.F2, client.FlatTrim) ||
			         KeyDownDo (KeyCode.F3, client.Stop) ||
		             KeyDownDo (KeyCode.F5, client.Takeoff) ||
		             KeyDownDo (KeyCode.F6, client.Hover) ||
		             KeyDownDo (KeyCode.F7, client.Land);

		if (!acted) {
			InputState newState = new InputState ();
			newState.roll = ROLL_SCALE * Input.GetAxis ("LeftRight");
			newState.pitch = PITCH_SCALE * Input.GetAxis ("ForwardBack");
			newState.yaw = YAW_SCALE * Input.GetAxis("ClockwiseCounterclockwise");
			newState.gaz = GAZ_SCALE * Input.GetAxis("UpDown");

			if (!(newState.Equals(prevState) && newState.IsZero())) {
				prevState = newState;

				Debug.Log (newState);
				client.Progress (AR.Drone.Client.Command.FlightMode.Progressive, roll: newState.roll, pitch: newState.pitch, yaw: newState.yaw, gaz: newState.gaz);
				acted = true;
			}
		}
	}

	void OnDestroy() {
		client.Stop ();
		client.Dispose ();
	}

	bool KeyPressDo(KeyCode key, Action callback) {
		if (Input.GetKey (key)) {
			callback ();
			return true;
		}
		return false;
	}

	bool KeyDownDo(KeyCode key, Action callback) {
		if (Input.GetKeyDown (key)) {
			callback ();
			return true;
		}
		return false;
	}
}
