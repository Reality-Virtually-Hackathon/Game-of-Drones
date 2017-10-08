using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAroundTargetControl : MonoBehaviour 
{
	//[HideInInspector]
	public bool isHovering = false;

	public Animator targetAnimator;

	LookAroundTargetManager parent;
	BoxCollider collider;

	void Start()
	{
		parent = transform.parent.parent.GetComponent<LookAroundTargetManager>();
		collider = GetComponent<BoxCollider>();
	}

	public void StartHove()
	{
		if(isHovering == false)
		{
			collider.enabled = false;
			isHovering = true;
			GetComponentInChildren<Renderer> ().material.color = Color.green;

			parent.IncCheckHoverFlags();

			StartCoroutine(DelayedAnim());
		}
	}


	public IEnumerator DelayedAnim()
	{
		targetAnimator.SetBool("isDespawning", true);
		yield return new WaitForSeconds(1.0f);
		Destroy(this.transform.parent.gameObject);
		//Destroy(this.gameObject);
	}

}
