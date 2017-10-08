using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeCube{
	public class MergeCubeScreenRotateManager : MonoBehaviour {
		public static MergeCubeScreenRotateManager instance;
		void Awake()
		{
			if (instance == null)
				instance = this;
			else if (instance != this)
				DestroyImmediate(this);
			lastOrientation = Screen.orientation;
		}

		public Callback<ScreenOrientation> OnOrientationEvent;
		ScreenOrientation lastOrientation;
		bool isVRMode = false;
		bool isLockedMode = false;
		void Update()
		{
			if (!isLockedMode) {
				CheckOrientation ();
			}
		}
		void CheckOrientation(){
			if (lastOrientation != Screen.orientation)
			{
				lastOrientation = Screen.orientation;
				if (OnOrientationEvent != null)
				{
					OnOrientationEvent.Invoke(Screen.orientation);
				}
			}
		}
		public void SetOrientation(bool isVR){
			if (!isLockedMode) {
				isVRMode = isVR;
				Screen.orientation = ScreenOrientation.AutoRotation;
				Screen.autorotateToLandscapeLeft = true;
				Screen.autorotateToLandscapeRight = true;
				if (isVR) {				
					Screen.autorotateToPortrait = false;
					Screen.autorotateToPortraitUpsideDown = false;
				} else {
					Screen.autorotateToPortrait = true;
					Screen.autorotateToPortraitUpsideDown = false;
				}
			}
		}
		public void LockToCurrentOrientation(){
			isLockedMode = true;
			Screen.orientation = Screen.orientation;
		}
		public void UnlockCurrentOrientation(){			
			isLockedMode = false;
			SetOrientation (isVRMode);
		}
	}
}