using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AR.Drone.Client;
using AR.Drone.Data;

// Reference: https://github.com/Ruslan-B/AR.Drone/blob/master/AR.Drone.WinApp/MainForm.cs
using System;


public class MouseBasedDemo : MonoBehaviour {
	DroneClient client;
	// Use this for initialization
	void Start () {
		client = new DroneClient ("192.168.1.1");
		client.NavigationPacketAcquired += HandleNavPacket;
	}

	void HandleNavPacket(NavigationPacket packet) {
		Debug.LogFormat ("[{0}] Received nav packet", packet.Timestamp);
	}

	// Update is called once per frame
	void Update () {
		Debug.LogFormat ("Active? {0}, Connected? {1}",client.IsActive, client.IsConnected);

		KeyDownDo (KeyCode.F1, client.Start);
		KeyDownDo (KeyCode.F2, client.Stop);
		KeyDownDo (KeyCode.F3, client.Takeoff);
		KeyDownDo (KeyCode.F4, client.Hover);
		KeyDownDo (KeyCode.F5, client.Land);


	}

	void OnDestroy() {
		client.Stop ();
		client.Dispose ();
	}

	void KeyDownDo(KeyCode key, Action callback) {
		if (Input.GetKeyDown (key)) {
			callback ();
		}
	}
}
