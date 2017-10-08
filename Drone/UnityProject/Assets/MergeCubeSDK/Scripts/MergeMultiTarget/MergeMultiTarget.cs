using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class MergeMultiTarget : MonoBehaviour{
	static MergeMultiTarget s_ins;
	void Awake(){
		if (MergeMultiTarget.instance != null) {
			Destroy (gameObject);
			return;
		}
		s_ins = this;
		KillReferenceCube ();
		if (MergeMultiTargetScalerRoot == null) {
			MergeMultiTargetScalerRoot = GameObject.Find ("MergeMultiTargetScalerRoot").transform;
		}
		DontDestroyOnLoad (gameObject);
	}
	public static MergeMultiTarget instance{ get { return s_ins; } }
	public enum HandleType{
		DoNothing,HideChild,HideMeshOnly,DisableChild,DisableSelected
	}
	[Tooltip("HideChild: Hide all children without disable gameobjects;\nHideMeshOnly: Hide all children without hide collider;\nDisableChild: will disable all immediate children;\nDisableSelected: will disable all referenced children; ")]
	public HandleType trackingHandleType = HandleType.DisableChild;

	public delegate void TrackingEvent();
	public event TrackingEvent OnTrackingFound;
	public event TrackingEvent OnTrackingLost;
	public Transform MergeMultiTargetScalerRoot;
	public GameObject[] selectToDisable;

	public bool isTracking { get; private set; }

	List<MergeTrackableEventHandler> mergeTrackables = new List<MergeTrackableEventHandler>();
	List<MergeTrackableEventHandler> trackers = new List<MergeTrackableEventHandler>();

	public void AddMergeTrackable(MergeTrackableEventHandler trackable){
		mergeTrackables.Add (trackable);
	}
	public void LockToTrackable(MergeTrackableEventHandler trackable){
		foreach (MergeTrackableEventHandler tp in mergeTrackables) { 
			if (tp != trackable) {
				tp.Die ();
			}
		}
	}
	public void OnMergeTrackingFound(MergeTrackableEventHandler tracker){
		int countTp = trackers.Count;
		if (!trackers.Contains (tracker)) {
			trackers.Add (tracker);
		}
		if (transform.parent != tracker.transform) {
			transform.parent = tracker.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}
		if (trackers.Count > 0 && countTp == 0) {
			MergeTrackingFoundHandler ();
		}
	}
	public void OnMergeTrackingLost(MergeTrackableEventHandler tracker){
		if (trackers.Contains (tracker)) {
			trackers.Remove (tracker);
		}
		if (trackers.Count == 0) {
			MergeTrackingLostHandler ();
		}
	}
	void MergeTrackingFoundHandler(){
		isTracking = true;
		if (OnTrackingFound != null)
		{
			OnTrackingFound();
		}
		VuforiaTrackingEventHandle (true);
	}
	void MergeTrackingLostHandler(){
		isTracking = false;
		if (OnTrackingLost != null)
		{
			OnTrackingLost();
		}
		VuforiaTrackingEventHandle (false);
	}

	void VuforiaTrackingEventHandle(bool isTracking){
		if (trackingHandleType == HandleType.HideChild || trackingHandleType == HandleType.HideMeshOnly) {
			Renderer[] rendererComponents = GetComponentsInChildren<Renderer> (true);
			Collider[] colliderComponents = GetComponentsInChildren<Collider> (true);

			// Enable/Disable rendering:
			foreach (Renderer component in rendererComponents) {
				component.enabled = isTracking;
			}

			if (trackingHandleType != HandleType.HideMeshOnly) {
				// Enable/Disable colliders:
				foreach (Collider component in colliderComponents) {
					component.enabled = isTracking;
				}
			}
		}
		else if (trackingHandleType == HandleType.DisableSelected) {
			for (int i = 0; i < selectToDisable.Length; i++) {
				selectToDisable[i].SetActive (isTracking);
			}
		}
		else if (trackingHandleType == HandleType.DisableChild) {
			for (int i = 0; i < transform.childCount; i++) {
				transform.GetChild (i).gameObject.SetActive (isTracking);
			}
		}
	}
	void KillReferenceCube(){
		foreach (Transform child in GetComponentsInChildren<Transform>()) {
			if (child.name == @"ReferenceCube(SelfDestroyOnRun)") {
				Destroy (child.gameObject);
			}
		}
	}

	#if UNITY_ANDROID
	void Start()
	{
		SetCameraFocus ();
	}
	void SetCameraFocus()
	{
		if (!isTracking)
		{
			CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);		
		}
		Invoke("SetCameraFocus", 2f);
	}
	#endif
}
