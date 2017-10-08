using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class IntroGazeCaster : MonoBehaviour 
{
	public static IntroGazeCaster instance;
	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			DestroyImmediate(this.gameObject);
	}


	RaycastHit hit;
	public LayerMask lMask;

	bool currentlyGazing = false;
	public bool GetCurrentGazeState(){return currentlyGazing;}

	GameObject gazedObject = null;
	IntroGazeResponder gazeResponder = null;
	IntroGazeResponder pressedObject = null;

	public delegate void GazeEvent();
	public event GazeEvent OnGaze_Start;
	public event GazeEvent OnGaze_End;
	public event GazeEvent OnGaze_InputDown;
	public event GazeEvent OnGaze_InputUp;

	/// <summary>
	/// Set This When Start Tutorial
	/// </summary>
	public bool isFullScreen = false;
	/// <summary>
	/// Set This when need switch between center ray all the time or tap to ray.
	/// </summary>
	public bool isFullScreenCenterRayAllTime = false;
	/// <summary>
	/// The only work in full screen mode. When active both point click and tap will work.
	/// </summary>
	public bool isBothClickMode = false;

	void Update ()
	{
		Ray ray = new Ray ();

		//Set up the ray to aim either at the screen position for tapping in Mono screen or for forward gaze direction for dual screen
		if (isFullScreen) 
		{
			if (isFullScreenCenterRayAllTime) {
				ray.origin = this.transform.position;
				ray.direction = this.transform.forward;
			} else {
				if (Input.GetMouseButtonDown (0)) {
					ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					if (isBothClickMode) {
						Debug.LogWarning ("In Click");
						RaycastHit hitB;
						if (Physics.Raycast (new Ray (transform.position, transform.forward), out hitB, 100000f, lMask)) {
							Debug.LogWarning ("Hit");
							if (hitB.transform.GetComponent<IntroGazeResponder> () != null) {
								Debug.LogWarning ("Have Receiver");
								hitB.transform.GetComponent<IntroGazeResponder> ().OnGazeEnter ();
								hitB.transform.GetComponent<IntroGazeResponder> ().OnGazeTrigger ();
							}
						}
					}
				} else {
					return;
				}
			}

		} 
		else 
		{
			ray.origin = this.transform.position;
			ray.direction = this.transform.forward;
		}
	
		if (Physics.Raycast (ray, out hit, 100000f, lMask)) 
		{                                  
			Debug.DrawRay(ray.origin, ray.direction);

			if (hit.transform.gameObject != gazedObject)
			{
				if (gazeResponder != null)
				{
					gazeResponder.OnGazeExit();

					if(IntroMergeReticle.instance != null) IntroMergeReticle.instance.OffHoverAction ();

					if ( OnGaze_End != null)
					{
						OnGaze_End.Invoke();
					}
				}

				currentlyGazing = false;
				gazedObject = hit.transform.gameObject;
				gazeResponder = hit.transform.GetComponent<IntroGazeResponder>();
			}
				
			if (!currentlyGazing && gazedObject != null && gazeResponder != null )
			{
//				Debug.Log("Gaze Start");
				gazeResponder.OnGazeEnter();

				if(IntroMergeReticle.instance != null) IntroMergeReticle.instance.OnHoverAction ();

				if( OnGaze_Start != null )
				{
					OnGaze_Start.Invoke();
				}

//				Debug.Log("Set CurGaze: " + gazedObject.name);
				currentlyGazing = true;
			}
				
		}
		else
		{
//			Debug.Log("Gaze End");
			//We aren't looking at anything at all. Clean up and stop looking at the previous guy
			//We were previously looking at something last tick, so lets have it do stuff
			if (currentlyGazing && gazeResponder != null && gazedObject != null)
			{
				gazeResponder.OnGazeExit();

				if(IntroMergeReticle.instance != null) IntroMergeReticle.instance.OffHoverAction ();

				if( OnGaze_End != null )
				{
					OnGaze_End.Invoke();
				}
			}
				
			currentlyGazing = false;
			gazedObject = null;
			gazeResponder = null;
		}

		if(Input.GetMouseButtonDown(0))
		{
//			Debug.Log("TAP");
			TriggerPressed();
		}

		if(Input.GetMouseButtonUp(0))
		{
//			Debug.Log("END TAP");
			TriggerReleased();

			if (isFullScreen)
			{
				currentlyGazing = false;
				gazedObject = null;
				gazeResponder = null;
			}
		}
	}

	public void TriggerPressed()
	{
		if (!enabled)
		{
			return;
		}

		if(IntroMergeReticle.instance != null) IntroMergeReticle.instance.OnClickAction ();

		//I pushed the button, is there somebody I'm looking at who would care about this?
		if (currentlyGazing && gazedObject != null && gazeResponder != null)
		{
			gazeResponder.OnGazeTrigger();

//			if(IntroMergeReticle.instance != null) IntroMergeReticle.instance.OnClickAction ();

			pressedObject = gazeResponder;
		}

		//Inform anybody else who would care about this
		if(OnGaze_InputDown != null )
		{
			OnGaze_InputDown.Invoke();
		}
	}

	public void TriggerReleased()
	{
		if (!enabled)
		{
			return;
		}

		if(IntroMergeReticle.instance != null) IntroMergeReticle.instance.OffClickAction ();

		//I stopped pushing the button, does the guy who would care about this still exist?
		if ( pressedObject != null )
		{
			pressedObject.OnGazeTriggerEnd();
			pressedObject = null;
		}

		//Inform anybody else who would care about this
		if (OnGaze_InputUp != null)
		{
			OnGaze_InputUp.Invoke();
		}
	}
}
