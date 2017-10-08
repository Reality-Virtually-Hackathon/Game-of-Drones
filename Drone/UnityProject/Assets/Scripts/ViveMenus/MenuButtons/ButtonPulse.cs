using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPulse : MonoBehaviour {
	[SerializeField] Transform box;

	const float DURATION = 0.5f;
	const float IN = 0.8f;

	// Use this for initialization
	void Start () {
		GetComponent<ViveButton> ().onPress += StartPulse;
	}

	Coroutine current;

	void StartPulse() {
		if (current != null) {
			StopCoroutine (current);
		}
		current = StartCoroutine (DoPulse ());
	}

	IEnumerator DoPulse() {
		float t = 0;
		while (t < DURATION) {
			box.localScale = Vector3.one * Mathf.Lerp (IN, 1, t / DURATION);
			yield return new WaitForEndOfFrame ();
			t += Time.deltaTime;
		}
		box.localScale = Vector3.one;
		current = null;
	}
}
