using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAnimationManager : MonoBehaviour 
{

	public Animator headsetModelAnimator, phoneModelAnimator;
	Animator activeModelAnimator;

	string animTag = "Tutorial_Anim";
	public List<GameObject> floatingTargets = new List<GameObject>();

	void Awake()
	{
		activeModelAnimator = headsetModelAnimator;

		headsetModelAnimator.gameObject.SetActive (MergeCube.MergeCubeSDK.instance.viewMode == MergeCube.MergeCubeSDK.ViewMode.HEADSET);
		phoneModelAnimator.gameObject.SetActive (MergeCube.MergeCubeSDK.instance.viewMode != MergeCube.MergeCubeSDK.ViewMode.HEADSET);

		if(MergeCube.MergeCubeSDK.instance.viewMode == MergeCube.MergeCubeSDK.ViewMode.FULLSCREEN)
		{
			activeModelAnimator = phoneModelAnimator;
		}
	}

	bool isInit = false;
	void OnEnable()
	{
		if (!isInit) {
			isInit = true;
			transform.parent.FaceToCamera (this.transform);
		}
	}
		

	AnimatorStateInfo currentState;
	public void ResetCurrentState()
	{
		currentState = activeModelAnimator.GetCurrentAnimatorStateInfo(0);
		if(currentState.IsTag(animTag))
		{
			activeModelAnimator.Play(currentState.fullPathHash, -1, 0f);
		}
	}


	public void setState(int stateIndex)
	{
		switch (stateIndex) {
		case 0:
			activeModelAnimator.SetTrigger ("0");
			break;
		case 1:
			activeModelAnimator.SetTrigger ("1");
			break;
		case 2:
			activeModelAnimator.SetTrigger ("2");
			break;
		case 3:
			activeModelAnimator.SetTrigger ("3");
			break;
		case 4: 
			DisableFloatingTargets();
			activeModelAnimator.SetTrigger ("4");
			break;
		case 5: 
			activeModelAnimator.SetTrigger ("5");
			break;
		case 6: 
			activeModelAnimator.SetTrigger ("6");
			break;
		default:
			Debug.Log ("Animation change failure.");
			break;
		}
	}


	public void DisableFloatingTargets()
	{
		foreach (GameObject target in floatingTargets)
		{
			target.SetActive(false);
		}
	}
}
