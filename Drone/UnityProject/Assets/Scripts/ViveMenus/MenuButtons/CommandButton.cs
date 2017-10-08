using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCommandButton : MonoBehaviour {
	public DroneImpulseController.Command command;
	// Use this for initialization
	void Start () {
		GetComponent<ViveButton> ().onPress += () => DroneImpulseController.instance?.Enqueue (command);
	}
}
