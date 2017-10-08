using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneImpulseController : MonoBehaviour {
	public static DroneImpulseController instance;

	public DroneMotionControls drone;

	// Use this for initialization
	void Awake () {
		instance = this;
		drone = new DroneMotionControls ();
	}

	IEnumerator Start() {
		drone.Start ();
		yield return new WaitForSeconds (1);
		drone.CalibrateFloor ();
	}

	public void Impulse(Vector3 vector) {
		intent.pitch = Mathf.Clamp (intent.pitch - vector.x, -1, 1);
		intent.roll = Mathf.Clamp (intent.roll + vector.z, -1, 1);
		intent.gaz = Mathf.Clamp (intent.gaz + vector.y, -1, 1);
	}

	public void Yaw(float yaw) {
		intent.yaw = Mathf.Clamp (intent.yaw + yaw, -1, 1);
	}

	public void Enqueue(Command cmd) {
		commands.Enqueue (cmd);
	}


	DroneMotionIntent intent = new DroneMotionIntent();

	public enum Command { Panic, Unpanic, Land, Takeoff }

	Queue<Command> commands = new Queue<Command>();

	void Update() {
		if(Input.GetKeyDown(KeyCode.F1)) {
			Enqueue (Command.Panic);
		}
		if (Input.GetKeyDown (KeyCode.F2)) {
			Enqueue (Command.Unpanic);
		}
		if (Input.GetKeyDown (KeyCode.L)) {
			Enqueue (Command.Land);
		}
	}

	void LateUpdate () {

		while (commands.Count > 0) {
			HandleCommand (commands.Dequeue ());
		}


		drone.SetTilt (intent.pitch, intent.roll);
		drone.SetVerticalSpeed (intent.gaz);
		drone.SetAngularSpeed (intent.yaw);

		if (drone.state.liftoff) {
			drone.ActOnMotionIntent ();
		} else {
			if (intent.gaz > 0) {
				drone.Takeoff ();
			}
		}

		intent = new DroneMotionIntent ();

	}


	void HandleCommand(Command cmd) {
		switch (cmd) {
		case Command.Land:
			drone.Land ();
			break;
		case Command.Panic:
			drone.Panic ();
			break;
		case Command.Unpanic:
			drone.Unpanic ();
			break;
		case Command.Takeoff:
			drone.Takeoff ();
			break;
		}
	}
}
