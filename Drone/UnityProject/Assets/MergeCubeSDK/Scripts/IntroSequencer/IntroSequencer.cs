using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MergeCube;
public class IntroSequencer : MonoBehaviour
{
	public static IntroSequencer instance;
	public bool debug = false;
	void Awake()
	{
		if (debug) {
			PlayerPrefs.DeleteAll ();
		}
		if (instance == null)
			instance = this;
		else if (instance != this)
		{
			DestroyImmediate(this.gameObject);
			return;
		}
	}

	//This allows the intro sequence to play out of the box with no other managers handling calling it's start.
	public bool shouldAutoStart = true;
	public Callback OnIntroSequenceComplete;
	bool isIntroStart = false;
	void Start()
	{
		MergeCubeSDK.instance.OnInitializationComplete += SignalSDKReady;

		if (shouldAutoStart)
			StartCoroutine(WaitForSDKInit());
	}
		
	bool mergeCubeSDKReady = false;
	void SignalSDKReady()
	{
		mergeCubeSDKReady = true;
	}

	public void StartIntroSequencer()
	{
		StartCoroutine(WaitForSDKInit());
	}

	IEnumerator WaitForSDKInit()
	{
		if (isIntroStart) {
			yield break;
		}
		isIntroStart = true;
		yield return new WaitUntil( () => mergeCubeSDKReady );
		BeginSequencer();
	}

	//Entry
	void BeginSequencer()
	{
//		Screen.autorotateToLandscapeLeft = false;
//		Screen.autorotateToLandscapeRight = false;
//		Screen.autorotateToPortrait = true;
//		Screen.autorotateToPortraitUpsideDown = false;

		MergeCubeSDK.instance.RemoveMenuElement(MergeCubeSDK.instance.viewSwitchButton);

		SplashScreenManager.instance.OnSplashSequenceEnd += HandleSplashSequenceComplete;
		TitleScreenManager.instance.OnTitleSequenceComplete += HandleTitleSequenceComplete;

		SplashScreenManager.instance.StartSplashSequence();
	}

	void HandleSplashSequenceComplete()
	{
		TitleScreenManager.instance.ShowTitleScreen();
	}

	void HandleTitleSequenceComplete( bool shouldPlayTutorialTp, bool shouldSwitchModeTp )
	{
		shouldSwitchMode = shouldSwitchModeTp;
		shouldPlayTutorial = shouldPlayTutorialTp;
		PermissionProcessor.instance.permissionProcessDone += HandlePermissionProcessDone;
		PermissionProcessor.instance.StartProcess ();
	}
	bool shouldPlayTutorial = false;
	bool shouldSwitchMode = false;
	void HandlePermissionProcessDone(){
		Debug.LogWarning ("Process Should Done");
		if (shouldSwitchMode) {
			MergeCubeSDK.instance.SwitchView ();
		} else {
			MergeCubeScreenRotateManager.instance.SetOrientation (false);
		}
		if (shouldPlayTutorial)
		{
			MergeCubeSDK.instance.RemoveMenuElement(MergeCubeSDK.instance.viewSwitchButton);

			MergeTutorial.ins.OnTutorialComplete += HandleTutorialSequenceComplete;


			MergeTutorial.ins.StartTutorial(MergeCubeSDK.instance.viewMode == MergeCube.MergeCubeBase.ViewMode.FULLSCREEN);

		}
		else 
		{
			EndIntroSequence();
		}
	}
	void HandleTutorialSequenceComplete()
	{
		EndIntroSequence();
	}
		
	//Exit
	void EndIntroSequence()
	{
		MergeCubeSDK.instance.AddMenuElement(MergeCubeSDK.instance.viewSwitchButton, 3);

		if (TrackOnce.instance != null) {
			TrackOnce.instance.IntroDone ();
		}

		if(OnIntroSequenceComplete != null)
		{
			OnIntroSequenceComplete.Invoke();
		}
	}
}
