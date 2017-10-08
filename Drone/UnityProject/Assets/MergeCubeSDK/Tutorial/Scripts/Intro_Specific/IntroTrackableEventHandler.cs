using UnityEngine;
using Vuforia;

public class IntroTrackableEventHandler : MonoBehaviour
{

	public bool isTracking { get; private set; }

	void Start()
	{
		isTracking = false;

	}



	void HandleTrackingFound()
	{
		transform.SendMessage ("OnTrackingFound", SendMessageOptions.DontRequireReceiver);
	}

	void HandleTrackingLost()
	{
		transform.SendMessage ("OnTrackingLost", SendMessageOptions.DontRequireReceiver);
	}

	public void RemoveTrackingLogic()
	{
		Destroy (this);
	}

}


