using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class MergeIOSBridge
{
//	#if UNITY_IOS
	[DllImport ("__Internal")] 
	private static extern bool HasCameraPermissions();

	[DllImport ("__Internal")] 
	private static extern void RequestCameraPermission();

	[DllImport ("__Internal")] 
	private static extern void OpenSettings();

	[DllImport ("__Internal")] 
	private static extern int HasPhotoPermission();

	[DllImport ("__Internal")] 
	private static extern void RequestPhotoPermission();

	[DllImport ("__Internal")] 
	private static extern void NativeOpenPhotoSettings();

	public static void RequestCamera(){
		RequestCameraPermission();
	}

	public static bool CheckCamera(){
		return HasCameraPermissions();
	}

	public static void OpenSet(){
		OpenSettings();
	}

	public static int CheckPhoto(){
		return HasPhotoPermission();
	}

	public static void RequestPhoto(){
		RequestPhotoPermission();
	}

	public static void OpenPhotoSettings(){
		NativeOpenPhotoSettings ();
	}
//	#endif
}