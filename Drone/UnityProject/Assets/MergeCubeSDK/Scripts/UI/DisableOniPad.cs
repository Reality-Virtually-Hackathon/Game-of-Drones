using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MergeCube;

public class DisableOniPad : MonoBehaviour {
	public bool disableGameobject = true;
	public bool disableRender = true;
	public bool disableCollider = true;

	void Start () 
	{
		if(MergeCubeSDK.deviceIsTablet){
			SetEnableDisable (false);
		} else {
			SetEnableDisable (true);
		}
	}

	void SetEnableDisable(bool isEnable){
		if (disableGameobject) {
			gameObject.SetActive (isEnable);
		}

		if (disableRender) {
			if (GetComponent<Renderer> () != null) {
				GetComponent<Renderer> ().enabled = isEnable;
			}
		}

		if (disableCollider) {
			if (GetComponent<Collider> () != null) {
				GetComponent<Collider> ().enabled = isEnable;
			}
		}
	}
}
