
//Uncomment the #define line to use the recording extensions for iVidCapPro for iOS.
//Android recording support is in progress and will be released in a later patch
//The plugin can be acquired here, but is built into the project:
//http://eccentric-orbits.com/eoe/site/ividcappro-unity-plugin/

//For iOS:

#define SHOULD_USE_RECORDING_PLUGIN

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MergeCube;
public class RecordingManager : MonoBehaviour 
{

	public GameObject recordStartExample;
	public GameObject recordSavingExample;
	public GameObject fullscreenRecordingCamera;
	private RenderTexture recordingTexture;

	#if SHOULD_USE_RECORDING_PLUGIN
	public readonly static bool isUsingRecordingPlugin = true;
	#else
	public readonly static bool isUsingRecordingPlugin = false;
	#endif

	public static RecordingManager instance;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			DestroyImmediate(this.gameObject);
	}

	#if SHOULD_USE_RECORDING_PLUGIN


	#if UNITY_IOS
	private iVidCapPro vidCapManager;
	private int recFrames = 0;
	#endif

	bool isRecording = false;
	bool isInitialized = false;

	Callback cb;

	void Start()
	{
		Initialize();

	#if UNITY_IOS
	vidCapManager = this.GetComponent<iVidCapPro>();
	vidCapManager.RegisterSessionCompleteDelegate(HandleSessionCompleteDelegate);
	#endif

	}

	void Initialize()
	{
		if (Camera.main.gameObject.GetComponent<AudioListener>() == null)
		{
			Camera.main.gameObject.AddComponent<AudioListener>();

			if (Camera.main.transform.parent.GetComponent<AudioListener>() != null)
			{
				DestroyImmediate(Camera.main.transform.parent.GetComponent<AudioListener>());
			}
		}

		if (Camera.main.transform.GetComponent<iVidCapProAudio>() == null)
		{
			Camera.main.gameObject.AddComponent<iVidCapProAudio>();
		}

		#if UNITY_IOS
		vidCapManager = this.GetComponent<iVidCapPro>();
		vidCapManager.saveAudio = Camera.main.GetComponent<iVidCapProAudio>();

		#endif

		MergeAndroidBridge.recordingStartState += StartSuccessHandle;
		isInitialized = true;
	}

	public void StartRec( string outputName, Callback recCompleteCb )
	{
	#if UNITY_IOS && !UNITY_EDITOR
		if (MergeIOSBridge.CheckPhoto () != 1) {
			return;
		}
	#endif
	#if UNITY_ANDROID && !UNITY_EDITOR
		if(!MergeAndroidBridge.HasPermission(AndroidPermission.WRITE_EXTERNAL_STORAGE)){
			return;
		}
		if(!MergeAndroidBridge.HasPermission(AndroidPermission.RECORD_AUDIO)){
			return;
		}
	#endif
		if (!isInitialized)
			return;
		
		if (!isRecording)
		{			
			StartCoroutine (StartRecAction (outputName,recCompleteCb));
		}
	}
	bool userGrantDone = false;
	bool userGrantResult = false;
	IEnumerator StartRecAction(string outputName, Callback recCompleteCb){
//		Debug.LogWarning ("Start Recording");
	#if UNITY_ANDROID && !UNITY_EDITOR
		userGrantDone = false;
		userGrantResult = false;
		MergeAndroidBridge.StartRecording();
		yield return new WaitUntil (() => userGrantDone);
		if (!userGrantResult) {			
			yield break;
		}
	#endif

//		Debug.LogWarning ("Remove Buttons");
		MergeCubeSDK.instance.RemoveMenuElement(MergeCubeSDK.instance.viewSwitchButton);
		MergeCubeSDK.instance.RemoveMenuElement(MergeCubeSDK.instance.headsetCompatabilityButton);
//		Debug.LogWarning ("Remove Buttons Done");

		MergeCubeScreenRotateManager.instance.LockToCurrentOrientation();
		SetRecordingTexture();
//		Debug.LogWarning ("Set Texture Done");


	#if UNITY_IOS && !UNITY_EDITOR
//	Debug.LogWarning ("Start VidCap");
	vidCapManager.BeginRecordingSession(outputName, recordingTexture.width, recordingTexture.height, 30f, iVidCapPro.CaptureAudio.Audio, iVidCapPro.CaptureFramerateLock.Unlocked);
	#endif
		
		if(recCompleteCb != null)
		{
			cb = recCompleteCb;
		}

//		Debug.Log("Now recording");

		isRecording = true;

		HandleRecStartSetup();
//		Debug.LogWarning ("All Recording Start Action Done");
		yield return null;
	}
	public void StartSuccessHandle(bool isSuccess){
//		if (!isSuccess) {
//			StopRec (true);
//		}
		userGrantResult = isSuccess;
		userGrantDone = true;
	}
	private void SetRecordingTexture() {
		if (Screen.orientation == ScreenOrientation.Landscape || Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight) {
			#if UNITY_IOS || UNITY_EDITOR
			if (MergeCube.MergeCubeSDK.instance.viewMode == MergeCubeBase.ViewMode.HEADSET) {
				recordingTexture = MergeCube.MergeCubeSDK.instance.GetHeadsetTexture ();
			} else {
				recordingTexture = new RenderTexture (1334, 750, 24);				
			}
			recordingTexture.Create ();
//			#elif UNITY_ANDROID
//			if(MergeCube.MergeCubeSDK.instance.viewMode == MergeCubeBase.ViewMode.HEADSET)
//			{
//				recordingTexture = MergeCube.MergeCubeSDK.instance.GetHeadsetTexture();
//			}
			#endif
		} else {
			#if UNITY_IOS || UNITY_EDITOR
			recordingTexture = new RenderTexture (750, 1334, 24);
			recordingTexture.Create ();
//			#elif UNITY_ANDROID
//			recordingTexture = new RenderTexture(432, 768, 24);
			#endif
		}

		#if UNITY_IOS || UNITY_EDITOR
		Camera.main.targetTexture = recordingTexture;
//		#elif UNITY_ANDROID
//		if (MergeCube.MergeCubeSDK.instance.viewMode == MergeCubeBase.ViewMode.HEADSET) {
//			Camera.main.targetTexture = recordingTexture;
//		}
		#endif
	#if UNITY_IOS
		MergeCubeSDK.instance.videoTexture.SetTexture("_Texture", recordingTexture);
	#endif
		#if UNITY_IOS || UNITY_EDITOR
		if (MergeCubeSDK.instance.viewMode == MergeCube.MergeCubeBase.ViewMode.FULLSCREEN)
		{
			fullscreenRecordingCamera.SetActive(true);
		}
		#endif 
		
		#if UNITY_IOS
//		Debug.Log("Should have set a recording texture to: " + vidCapManager);
		vidCapManager.SetCustomRenderTexture(Camera.main.targetTexture);
		#endif
	}
	public ToggleSprite RecordingButton;
	public void StopRec()
	{
		if (!isInitialized)
			return;

//		Debug.Log("Stop Rec try");

		if (isRecording)
		{
//			Debug.Log("Stop Rec -- Start Stop");
			#if UNITY_ANDROID  && !UNITY_EDITOR
			MergeAndroidBridge.StopRecording();
			#endif
//			Debug.Log("Stop Rec -- Native Done");
			#if UNITY_IOS && !UNITY_EDITOR
			vidCapManager.EndRecordingSession(iVidCapPro.VideoDisposition.Save_Video_To_Album, out recFrames);
			#endif
			
			if (MergeCubeSDK.instance.viewMode == MergeCube.MergeCubeBase.ViewMode.FULLSCREEN)
			{
				Camera.main.targetTexture = null;
				fullscreenRecordingCamera.SetActive(false);
			}
			else
			{
	#if UNITY_IOS
				Camera.main.targetTexture = MergeCubeSDK.instance.GetHeadsetTexture();
	#endif
			}
//			Debug.Log("Stop Rec -- ViewSetDone");
	#if UNITY_IOS
			MergeCubeSDK.instance.videoTexture.SetTexture("_Texture", MergeCubeSDK.instance.GetHeadsetTexture());
	#endif
			recordingTexture = null;

			MergeCubeScreenRotateManager.instance.UnlockCurrentOrientation();
			Resources.UnloadUnusedAssets ();
//			Debug.Log("Stop Rec -- Set Texture Done");

			MergeCubeSDK.instance.AddMenuElement(MergeCubeSDK.instance.viewSwitchButton, 3);

			if (MergeCubeSDK.instance.viewMode == MergeCubeBase.ViewMode.HEADSET)
			{
				MergeCubeSDK.instance.AddMenuElement(MergeCubeSDK.instance.headsetCompatabilityButton, 4);
			}
				
//			Debug.Log("Stop Rec -- Done Set Recording False");

			isRecording = false;

			HandleRecCompleteSetup();

			if( cb != null )
			{
				cb.Invoke();
				cb = null;
			}
		}
	}

	private void HandleSessionCompleteDelegate()
	{
		if (cb != null)
		{
			cb.Invoke();
			cb = null;
		}
	}
		
	void HandleRecStartSetup()
	{
//		Debug.Log("HandleRecStartSetup");
//		recordStartExample.SetActive(true);
		RecordingButton.SetState (true);
	}

	void HandleRecCompleteSetup()
	{
//		Debug.Log("HandleRecCompleteSetup");
//		recordStartExample.SetActive(false);
		RecordingButton.SetState (false);
	}
		
	void HandleRecSaveComplete()
	{
//		Debug.Log("HandleRecSaveComplete");
	}

	#endif

	public void ToggleRecording()
	{
		#if SHOULD_USE_RECORDING_PLUGIN

		if (!isRecording)
		{
			#if UNITY_IOS && !UNITY_EDITOR
			if(MergeIOSBridge.CheckPhoto() == 2){
				MergeIOSBridge.RequestPhoto();
				return;
			}
			else if(MergeIOSBridge.CheckPhoto() != 1){
				MergeIOSBridge.OpenPhotoSettings();
				return;
			}
			#endif
			#if UNITY_ANDROID && !UNITY_EDITOR
			if(!MergeAndroidBridge.HasPermission(AndroidPermission.WRITE_EXTERNAL_STORAGE )){
				MergeAndroidBridge.CheckPermissionAndReDirectToSettingsScreen(AndroidPermission.READ_EXTERNAL_STORAGE);
				return;
			}
			if(!MergeAndroidBridge.HasPermission(AndroidPermission.RECORD_AUDIO)){
				MergeAndroidBridge.CheckPermissionAndReDirectToSettingsScreen(AndroidPermission.RECORD_AUDIO);
				return;
			}
			#endif
			
			StartRec( System.DateTime.Now.Day.ToString()+"_"+System.DateTime.Now.Month.ToString()+"_"+
				System.DateTime.Now.Year.ToString()+"_"+System.DateTime.Now.Hour.ToString()+"_"+System.DateTime.Now.Minute.ToString(), HandleRecSaveComplete );
		}
		else
		{
//			Debug.LogWarning("Should Stop Recording.");

			StopRec();

//			Debug.LogWarning("Should Set Recording to False.");
		}

		#endif
	}

}
