using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickStartManager : MonoBehaviour {
	public GameObject MyGameRoot;
	// Use this for initialization
	void Start () {
		MyGameRoot.SetActive (false);
		MyGameRoot.transform.parent = transform;

		IntroSequencer.instance.OnIntroSequenceComplete += OnIntroDone;
		IntroSequencer.instance.StartIntroSequencer ();
	}
	
	void OnIntroDone(){
		MyGameRoot.SetActive (true);
		MyGameRoot.transform.parent = MergeMultiTarget.instance.MergeMultiTargetScalerRoot;
		MyGameRoot.transform.localPosition = Vector3.zero;
		MyGameRoot.transform.localRotation = Quaternion.identity;
	}
}
