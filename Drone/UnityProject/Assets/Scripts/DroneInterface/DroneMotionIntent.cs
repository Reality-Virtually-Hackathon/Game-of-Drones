using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMotionIntent {

	// All values are in the range [-1,1], where 1 is the maximum rate.
	public float roll = 0;
	public float pitch = 0;
	public float yaw = 0;
	public float gaz = 0; // Vertical speed


	public DroneMotionIntent() { }
	public DroneMotionIntent(float roll = 0, float pitch = 0, float yaw = 0, float gaz = 0) {
		this.roll = roll;
		this.pitch = pitch;
		this.yaw = yaw;
		this.gaz = gaz;
	}

	public DroneMotionIntent Copy() {
		return new DroneMotionIntent (roll, pitch, yaw, gaz);
	}

	public bool IsNeutral() {
		return roll == 0 && pitch == 0 && yaw == 0 && gaz == 0;
	}

	public override bool Equals (object obj)
	{
		if (obj == null || GetType () != obj.GetType ()) {
			return false;
		}
		var other = (DroneMotionIntent)obj;
		return this.roll == other.roll && this.pitch == other.pitch && this.yaw == other.yaw &&  this.gaz == other.gaz;
	}

	public override string ToString ()
	{
		return string.Format ("{0}\t{1}\t{2}\t{3}", roll, pitch, yaw, gaz);
	}

}
