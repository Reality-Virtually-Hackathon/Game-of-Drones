using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
namespace MergeCube{
	public class PermissionProcessor:MonoBehaviour{
		public static PermissionProcessor instance;

		void Awake()
		{
//			PlayerPrefs.DeleteAll ();
			if (instance == null)
				instance = this;
		}
		public Callback permissionProcessDone;
		public NoticePageManager Page_CameraAccess;
		public NoticePageManager Page_CameraDisabled;
		public NoticePageManager Page_PhotoAccess;
		public NoticePageManager Page_UserAccount;
		public GameObject userAccountSkipBtn;
		bool cameraAccessPop = true;
		bool cameraDisabledPop = true;
		bool photoAccessPop = true;
		bool isPopPause = false;
		bool skip = false;
		public void StartProcess(){
			cameraAccessPop = (PlayerPrefs.GetString ("MCSDK_cameraAccessPop")!="done");
			photoAccessPop = (PlayerPrefs.GetString ("MCSDK_photoAccessPop")!="done");
			StartCoroutine (Processing ());
		}
		bool proceed = false;
		bool Temp_CheckCamera = false;
		void Update(){
			if (Temp_CheckCamera) {
				#if UNITY_IOS && !UNITY_EDITOR
				proceed = MergeIOSBridge.CheckCamera ();
				#endif
				#if UNITY_ANDROID && !UNITY_EDITOR
				proceed = MergeAndroidBridge.HasPermission(AndroidPermission.CAMERA);
				#endif
			}
		}	
		void OpenPhoneSetting(){
			#if UNITY_IOS && !UNITY_EDITOR
			MergeIOSBridge.OpenSet ();
			#endif
			#if UNITY_ANDROID && !UNITY_EDITOR
			MergeAndroidBridge.ReDirectToSettingsScreen();
			#endif
			#if UNITY_EDITOR
			proceed = true;
			#endif
		}
		IEnumerator Processing(){	
			proceed = false;
			if (cameraAccessPop) {
				bool isCameraPermit = false;
				#if UNITY_IOS && !UNITY_EDITOR
				isCameraPermit = MergeIOSBridge.CheckCamera ();
				#endif
				#if UNITY_ANDROID && !UNITY_EDITOR
				isCameraPermit = MergeAndroidBridge.HasPermission(AndroidPermission.CAMERA);
				#endif
				if(!isCameraPermit){
//					Debug.LogWarning ("Should Do First Cam");
					Page_CameraAccess.gameObject.SetActive (true);
					Page_CameraAccess.doneButton += Btn_DoneAction;
					yield return new WaitUntil (() => proceed);
					Page_CameraAccess.doneButton -= Btn_DoneAction;
					proceed = false;
					#if UNITY_IOS && !UNITY_EDITOR
					MergeIOSBridge.RequestCamera ();
					yield return new WaitUntil(()=>isPopPause);
					#endif
					#if UNITY_ANDROID && !UNITY_EDITOR
					MergeAndroidBridge.RequestPremission(AndroidPermission.CAMERA);
					yield return new WaitForSeconds(.1f);	
					#endif
					#if !UNITY_EDITOR
					yield return new WaitUntil(()=>!isPopPause);
					yield return new WaitForSeconds(.2f);					
					#else
						yield return new WaitForSeconds(.5f);
					#endif

					PlayerPrefs.SetString ("MCSDK_cameraAccessPop","done");
					Page_CameraAccess.gameObject.SetActive (false);
				}
			} 
			#if UNITY_IOS && !UNITY_EDITOR
			cameraDisabledPop = !MergeIOSBridge.CheckCamera ();
			#endif
			#if UNITY_ANDROID && !UNITY_EDITOR
			cameraDisabledPop = !MergeAndroidBridge.HasPermission(AndroidPermission.CAMERA);
			#endif
			#if  !UNITY_EDITOR
			if (cameraDisabledPop) {
//				Debug.LogWarning ("Should Do Cam Disabled");
				Page_CameraDisabled.gameObject.SetActive (true);
				Page_CameraDisabled.doneButton += OpenPhoneSetting;
				Temp_CheckCamera = true;
				yield return new WaitUntil (() => proceed);
				Temp_CheckCamera = false;
				Page_CameraDisabled.doneButton -= OpenPhoneSetting;
				proceed = false;
				yield return null;
				Page_CameraDisabled.gameObject.SetActive (false);
			} 
			#endif

			#if UNITY_IOS && !UNITY_EDITOR
			int photoState = MergeIOSBridge.CheckPhoto();
			if(photoState == 2){
				photoAccessPop = true;
			}
			else{
				photoAccessPop = false;
			}
			#endif
			#if UNITY_ANDROID && !UNITY_EDITOR
				if(MergeAndroidBridge.WasDeniedOnce(AndroidPermission.READ_EXTERNAL_STORAGE)){
					photoAccessPop = false;
				}else{
					photoAccessPop = !MergeAndroidBridge.HasPermission(AndroidPermission.READ_EXTERNAL_STORAGE);
				}
			#endif
			if (photoAccessPop) {
				skip = false;
				proceed = false;
//				Debug.LogWarning ("Should Do Photo");
				//pop Cam
				Page_PhotoAccess.gameObject.SetActive(true);
				Page_PhotoAccess.doneButton += Btn_DoneAction;
				Page_PhotoAccess.actionButton += SkipAction;
				yield return new WaitUntil (() => (proceed || skip));
				proceed = false;
				if (!skip) {
					#if UNITY_IOS && !UNITY_EDITOR
					MergeIOSBridge.RequestPhoto();
					yield return new WaitUntil(()=>isPopPause);
					#endif
					#if UNITY_ANDROID && !UNITY_EDITOR
					MergeAndroidBridge.RequestPremission(AndroidPermission.READ_EXTERNAL_STORAGE);
					yield return new WaitForSeconds(.1f);
					#endif
				
					#if !UNITY_EDITOR
					yield return new WaitUntil(()=>!isPopPause);
					yield return new WaitForSeconds(.2f);
					#else
					yield return new WaitForSeconds (.5f);
					#endif
				}
				if(!skip){
					#if UNITY_IOS && !UNITY_EDITOR
					//currently Do nothing since not recording Mic.
					#endif
					#if UNITY_ANDROID && !UNITY_EDITOR
					MergeAndroidBridge.RequestPremission(AndroidPermission.RECORD_AUDIO);
					yield return new WaitForSeconds(.1f);
					#endif

					#if !UNITY_EDITOR
					yield return new WaitUntil(()=>!isPopPause);
					yield return new WaitForSeconds(.2f);
					#endif
				}
				Page_PhotoAccess.doneButton -= Btn_DoneAction;
				Page_PhotoAccess.actionButton -= SkipAction;
				PlayerPrefs.SetString ("MCSDK_photoAccessPop","done");
				Page_PhotoAccess.gameObject.SetActive(false);
			} 
//			Debug.LogWarning ("All If Done");

			DoneProcess ();
			PlayerPrefs.Save();
			MergeCubeSDK.instance.InitVuforia();
			yield return null;
		}
		void EmptyCall(){
		}
		void Btn_DoneAction(){
//			Debug.LogWarning ("Btn_DoneAction ---- Proceed");
			proceed = true;
		}
		void SkipAction(){
			skip = true;
		}
		void DoneProcess(){
			if (permissionProcessDone != null) {
				permissionProcessDone.Invoke ();
			}
			Destroy (gameObject);
		}

		void OnApplicationPause(bool pauseStatus)
		{
			isPopPause = pauseStatus;
		}

	}
}