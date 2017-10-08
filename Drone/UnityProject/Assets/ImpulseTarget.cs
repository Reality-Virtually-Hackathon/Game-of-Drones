using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseTarget : MonoBehaviour {
	public static ImpulseTarget instance;
	// Use this for initialization
	void Start () {
		instance = this;
	}

	public void Impulse(Vector3 vector) {
		transform.position += vector * Time.deltaTime;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
