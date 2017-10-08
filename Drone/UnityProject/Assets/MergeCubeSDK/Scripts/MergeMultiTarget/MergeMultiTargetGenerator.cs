using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;
 
 
public class MergeMultiTargetGenerator : MonoBehaviour
{
	void Start(){
		VuforiaARController.Instance.RegisterVuforiaStartedCallback(LoadDataSet);
	}
	void LoadDataSet(){
		ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker> ();
		DataSet dataSet = objectTracker.CreateDataSet ();
		if (dataSet.Load (MergeCube.MergeMultiTargetCore.DataSet, VuforiaUnity.StorageType.STORAGE_ABSOLUTE)) {			
			objectTracker.Stop (); 
			if (!objectTracker.ActivateDataSet (dataSet)) {
				Debug.LogError ("Failed to Activate MergeCube DataSet");
			}
			if (!objectTracker.Start ()) {
				Debug.LogError ("Tracker Failed to Start.");
			}
			 
			int counter = 0;
			 
			IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager ().GetTrackableBehaviours ();
			foreach (TrackableBehaviour tb in tbs) {
				if (tb.name == "New Game Object") {
					tb.gameObject.name = ++counter + ":DynamicImageTarget-" + tb.TrackableName;
					tb.gameObject.AddComponent<MergeTrackableEventHandler> ();
					tb.gameObject.AddComponent<DND> ();
				}
			}
		} else {
			Debug.LogError ("Failed to Load MergeCube Dataset");
		}
	}
}