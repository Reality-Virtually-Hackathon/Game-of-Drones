using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongAnima : MonoBehaviour {
	SpriteRenderer myRender;
	Color myColor;
	// Use this for initialization
	void Start () {
		myRender = GetComponent<SpriteRenderer> ();
		myColor = myRender.color;
	}
	
	// Update is called once per frame
	void Update () {
		myColor.a = Mathf.PingPong (Time.time, 1f);;
		myRender.color = myColor;
	}
}
