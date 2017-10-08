using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TugHandle : MonoBehaviour, IReleaseable {
	public Transform holder { get; protected set;}
	[SerializeField] TugControls controls;
	[SerializeField] float stiffness = 1;

	public void SetHolder(Transform holder) {
		this.holder = holder;
	}

	public void Release(Transform origin) {
		if (this.holder != null && this.holder == origin) {
			SetHolder (null);
		}
	}

	public Vector3 GetDelta(Vector3 pos) {
		return pos - controls.transform.position;
	}

	public float GetExtremity() {
		return holder == null ? 0 : GetDelta (holder.position).magnitude / TugControls.PULL_RANGE;
	}

	public Vector3 GetSpringForce() {
		return -GetDelta(transform.position) * stiffness;
	}

	void FixedUpdate() {
		if (holder != null) {
			Vector3 delta = GetDelta (holder.position);

			if (delta.magnitude > TugControls.PULL_RANGE) {
				delta = delta.normalized * TugControls.PULL_RANGE;
			}

			transform.position = controls.transform.position + delta;
		} else {
			// No conservation of velocity !?
			transform.position += GetSpringForce () * Time.deltaTime;
		}
	}
}
