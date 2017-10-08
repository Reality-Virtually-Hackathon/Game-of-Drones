using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class WheelControls : MonoBehaviour, IReleaseable {

	[SerializeField] GameObject spinnerParent;
	[SerializeField] GameObject moving;
	[SerializeField] GameObject still;

	class ControllerTracker {
		public Transform controllerTransform;
		public Vector3 lastPosition;
	}

	Dictionary<int, ControllerTracker> holders = new Dictionary<int, ControllerTracker>();
	float effectiveRotation;

	const float DEG_CEIL = 15;

	public float GetIntensity() {
		return effectiveRotation / DEG_CEIL;
	}

	// Update is called once per frame
	void LateUpdate () {
		effectiveRotation = ComputeEffectiveRotation () / Time.deltaTime;
		DroneImpulseController.instance?.Yaw (Mathf.Clamp (effectiveRotation / DEG_CEIL, -1, 1));
		ShowRotation (effectiveRotation);

	}

	void ShowRotation(float rotation) {
		if (rotation != 0) {
			spinnerParent.transform.Rotate (new Vector3 (0, rotation, 0));
			spinnerParent.transform.localScale = new Vector3 (-Mathf.Sign (rotation), 1, 1);
			moving.SetActive (true);
			still.SetActive (false);
		} else {
			moving.SetActive (false);
			still.SetActive (true);
		}

	}

	float ComputeEffectiveRotation() {
//		float max = 0;
//		float netDeg = 0;
//		foreach (var tracking in holders.Values) {
//			var deg = CalculateAngle (tracking.lastPosition, tracking.controllerTransform.position);
//			netDeg += deg;
//			if (Mathf.Abs (deg) > Mathf.Abs (max)) {
//				max = deg;
//			}
//			tracking.lastPosition = tracking.controllerTransform.position;
//		}
//		if (Mathf.Abs (netDeg) > Mathf.Abs (max)) {
//			return netDeg;
//		} else {
//			return max;
//		}

		if (holders.Count == 0) {
			return 0;
		} else {
			var holder = holders.Values.First ();
			var ret = CalculateAngle (holder.lastPosition, holder.controllerTransform.position);
			foreach (var tracker in holders) { 
				tracker.Value.lastPosition = tracker.Value.controllerTransform.position;
			}
			return ret;
		}


	}

	public void AddHolder(Transform origin) {
		holders.Add (origin.GetHashCode (), new ControllerTracker () {
			controllerTransform = origin,
			lastPosition = origin.position
		});
	}

	public void Release(Transform origin) {
		if (holders.ContainsKey (origin.GetHashCode ())) {
			holders.Remove (origin.GetHashCode ());
		}
	}

	float CalculateAngle(Vector3 a0, Vector3 b0) {
		var a = transform.InverseTransformVector (a0);
		var b = transform.InverseTransformVector (b0);

		var cross = Vector3.Cross (new Vector3 (a.x, 0, a.z), new Vector3 (b.x, 0, b.z));
		return cross.y;
	}
}
