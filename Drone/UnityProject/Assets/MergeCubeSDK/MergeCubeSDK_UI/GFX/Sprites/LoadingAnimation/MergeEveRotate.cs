using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeEveRotate : MonoBehaviour {
	public float rotateSpeedSecPer180 = .25f;
	public float waitInBetween = 2f;
	// Use this for initialization
	void OnEnable () {
		StartCoroutine (Rotate ());
	}
	float rotateCurr = 360f;
	IEnumerator Rotate(){
		while (rotateCurr > 180f) {
			rotateCurr -= Time.deltaTime * (180f / rotateSpeedSecPer180);
			transform.localEulerAngles = new Vector3 (0, 0, rotateCurr);
			yield return null;
		}
		rotateCurr = 180f;
		transform.localEulerAngles = new Vector3 (0, 0, rotateCurr);
		yield return new WaitForSeconds(waitInBetween);
		while (rotateCurr > 0f) {
			rotateCurr -= Time.deltaTime * (180f / rotateSpeedSecPer180);
			transform.localEulerAngles = new Vector3 (0, 0, rotateCurr);
			yield return null;
		}
		rotateCurr = 360f;
		transform.localEulerAngles = new Vector3 (0, 0, rotateCurr);
		yield return new WaitForSeconds(waitInBetween);

		StartCoroutine (Rotate ());
	}
}
