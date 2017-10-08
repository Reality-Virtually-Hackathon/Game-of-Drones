using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCubeControl : MonoBehaviour 
{
	bool isHovering = false;

	public Color defaultColor;

	public void StartHove()
	{
		isHovering = true;
		transform.parent.localScale = Vector3.one * 1.2f;
		GetComponentInChildren<Renderer> ().material.color = Color.green;
	}

	public void StopHove()
	{
		isHovering = false;
		transform.parent.localScale = Vector3.one * 1f;
		GetComponentInChildren<Renderer>().material.color = defaultColor;

	}
		
	// Update is called once per frame
	void Update () 
	{
		if (isHovering) 
		{
			MergeTutorial.ins.TargetPointed ();
		}
	}


		
}
