using UnityEngine;
using System;
using System.Collections;

public class MergeAndroidBridge : MonoBehaviour {
	public delegate void BridgeBoolCall(bool arg);	
	public static BridgeBoolCall recordingStartState;
	#if UNITY_ANDROID
	private static Action permitCallBack;
	private static Action notPermitCallBack;
	private static Action <string> deniedByUserCallBack;

	private static Action dialogueActionCallBack;

	const string PackageClassName = "com.merge.unityandroidpermission.PermissionManager";
	const string RecordingClassName = "com.merge.unityandroidpermission.ScreenCaptureWithAudio";
	static AndroidJavaClass permissionManager;
	static AndroidJavaClass recordingManager;

	static string gameObjectName;
	static string log = "MergeAndroidBridge : ";
    
	[Tooltip ("Set to true if you want to record device's native resolution")]
	public bool isRecordDeviceResolutionI = false;
	[Tooltip ("Please make sure resolution is one of these value - 480, 720, 1080, 1440, 1920")]
	[Range(480,1920)]
	public int resolutionI = 720;
	[Range(24,60)]
	public int frameRateI = 30;

	static bool isRecordDeviceResolution = false;
	static int resolution = 720;
	static int frameRate = 30;

	void Awake () {
		DontDestroyOnLoad (gameObject);
		gameObjectName = this.gameObject.name;
		permissionManager = new AndroidJavaClass (PackageClassName);
		recordingManager = new AndroidJavaClass (RecordingClassName);
		Debug.Log (log + "Awake : " + gameObjectName);

		// i had to make this as i need the control to be shown in inspector
		// at the same time i need the variable to be static.
		// i got a suggestion to make the class singletone instead of making variables static
		// will do it in future.
		resolution = resolutionI;
		frameRate = frameRateI;
		isRecordDeviceResolution = isRecordDeviceResolutionI;
	}

	public static bool HasPermission (AndroidPermission permission) {
		Debug.Log (log + "Awake : " + gameObjectName);
		return permissionManager.CallStatic<bool> ("hasPermission", GetPermittionStr (permission));
	}

	public static bool WasDeniedOnce (AndroidPermission permission) {
		Debug.Log (log + "WasDeniedOnce : " + gameObjectName);
		permissionManager = new AndroidJavaClass (PackageClassName);
		return permissionManager.CallStatic<bool> ("wasDeniedOnce", GetPermittionStr (permission));
	}

	public static void RequestPremission (AndroidPermission permission, Action onPermit = null, Action notPermit = null) {
		Debug.Log (log + "RequestPremission : " + gameObjectName);
		permissionManager.CallStatic ("requestPermission", GetPermittionStr (permission), gameObjectName);
		permitCallBack = onPermit;
		notPermitCallBack = notPermit;
	}

	public static void ReDirectToSettingsScreen () {
		Debug.Log (log + "ReDirectToSettingsScreen : " + gameObjectName);
		permissionManager.CallStatic ("goToSettings");
	}

	public static void CheckPermissionAndReDirectToSettingsScreen (AndroidPermission permission,
	                                                              Action <string> OnDeniedPermissionByUser = null) {
		Debug.Log (log + "CheckPermissionAndReDirectToSettingsScreen : " + gameObjectName);
		if (!HasPermission (permission)) {
			permissionManager.CallStatic ("ShowDialogAndReDirectToSettings", GetPermittionStr (permission), gameObjectName);
			deniedByUserCallBack = OnDeniedPermissionByUser;
		}
    
	}

	/*
 public static void BuildAndroidNativeDialogue( final String go, final String title, final String msg, final boolean isCancelable,
 final String nbTitle, final String pbTitle, final String nubTitle, final String responseFunc)
 */

	public static void CreateDialogueAndGetResultBack (string title, string msg, bool isCancelable,
	                                                  string pbTitle,
	                                                  string nbTitle,
	                                                  string nubTitle) {
		Debug.Log (log + "CreateDialogueAndGetResultBack : " + gameObjectName);
		permissionManager.CallStatic ("BuildAndroidNativeDialogue", gameObjectName, title, msg, isCancelable,
			pbTitle, nbTitle, nubTitle, "DoWork");
	}
                                                      
	public static void StartRecording() {
		Debug.Log ("Unity_StartRecording : Unity");

		if (recordingManager != null) {
			Debug.Log ("Unity_StartRecording : Just before StartRecordingFromUnity");
			recordingManager.CallStatic ("StartRecordingFromUnity", isRecordDeviceResolution, resolution, frameRate, MergeAndroidBridge.gameObjectName, "ScreenRecordingPermission");
			Debug.Log ("Unity_StartRecording : After StartRecordingFromUnity -> ");
		} else {
			Debug.Log ("Class not FOUND!");
		}			
	}

	public static void StopRecording() {
		Debug.Log ("Unity_StopRecording : Unity");

		if (recordingManager != null) {
			Debug.Log ("Unity_StartRecording : Just before StopRecordingFromUnity");
			recordingManager.CallStatic ("StopRecordingFromUnity");
			Debug.Log ("Unity_StartRecording : After StopRecordingFromUnity");
		} else {
			Debug.Log ("Class not FOUND!");
		}
	}
	public void ScreenRecordingPermission(string result) {
		Debug.Log ("MergeAndroidNative : " + result);
		int res = Convert.ToInt32(result);
		if (recordingStartState != null) {
			recordingStartState (res==1);
		}
	}

	//##########################################################################################################################################

	private static string GetPermittionStr (AndroidPermission permittion) {
		return "android.permission." + permittion.ToString ();
	}

	private void OnDdialogueButtonClicked (string str) {
		Debug.Log (log + "OnDdialogueButtonClicked : " + gameObjectName);
		Debug.Log (str);
		if (dialogueActionCallBack != null) {
			dialogueActionCallBack ();
		}
		ResetCallBacks ();
	}

	private void OnDeniedPermissionByUser (string str) {
		Debug.Log (log + "OnDeniedPermissionByUser : " + gameObjectName);
		Debug.Log (str);
		if (deniedByUserCallBack != null) {
			deniedByUserCallBack (str);
		}
		ResetCallBacks ();
	}

	private void OnPermit () {
		Debug.Log (log + "OnPermit : " + gameObjectName);
		if (permitCallBack != null) {
			permitCallBack ();
		}
		ResetCallBacks ();
	}

	private void NotPermit () {
		Debug.Log (log + "NotPermit : " + gameObjectName);
		if (notPermitCallBack != null) {
			notPermitCallBack ();
		}
		ResetCallBacks ();
	}

	private void ResetCallBacks () {
		Debug.Log (log + "ResetCallBacks : " + gameObjectName);
		notPermitCallBack = null;
		permitCallBack = null;
		deniedByUserCallBack = null;
		dialogueActionCallBack = null;
	}
	#endif
}

// Protection level: dangerous permissions
// http://developer.android.com/intl/ja/reference/android/Manifest.permission.html
public enum AndroidPermission {
	ACCESS_COARSE_LOCATION,
	ACCESS_FINE_LOCATION,
	ADD_VOICEMAIL,
	BODY_SENSORS,
	CALL_PHONE,
	CAMERA,
	GET_ACCOUNTS,
	PROCESS_OUTGOING_CALLS,
	READ_CALENDAR,
	READ_CALL_LOG,
	READ_CONTACTS,
	READ_EXTERNAL_STORAGE,
	READ_PHONE_STATE,
	READ_SMS,
	RECEIVE_MMS,
	RECEIVE_SMS,
	RECEIVE_WAP_PUSH,
	RECORD_AUDIO,
	SEND_SMS,
	USE_SIP,
	WRITE_CALENDAR,
	WRITE_CALL_LOG,
	WRITE_CONTACTS,
	WRITE_EXTERNAL_STORAGE
}
