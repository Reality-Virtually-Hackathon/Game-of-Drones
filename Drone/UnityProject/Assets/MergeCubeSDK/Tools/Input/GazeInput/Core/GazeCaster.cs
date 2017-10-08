using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using MergeCube;

public class GazeCaster : MonoBehaviour 
{
	public static GazeCaster instance;
	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			DestroyImmediate(this.gameObject);
	}

	[HideInInspector]
	public RaycastHit hit;
	public LayerMask lMask;

	bool currentlyGazing = false;
	public bool GetCurrentGazeState(){return currentlyGazing;}

	GameObject gazedObject = null;
	GazeResponder gazeResponder = null;
	GazeResponder pressedObject = null;

	public delegate void GazeEvent();
	public event GazeEvent OnGaze_Start;
	public event GazeEvent OnGaze_End;
	public event GazeEvent OnGaze_InputDown;
	public event GazeEvent OnGaze_InputUp;

	bool isVRMode = false;
	public void SwapScreenViewMode(bool isVRModeTp)
	{
//		isVRMode = isVRModeTp;
	}

	void Start(){
		MergeCubeSDK.instance.OnViewModeSwap += SwapScreenViewMode;
	}
	void OnDestroy(){
		MergeCubeSDK.instance.OnViewModeSwap -= SwapScreenViewMode;
	}
	void Update ()
	{
		Ray ray = new Ray ();

		//Set up the ray to aim either at the screen position for tapping in Mono screen or for forward gaze direction for dual screen
//		if (!isVRMode) 
//		{
//			ray = Camera.main.ScreenPointToRay (Input.mousePosition);
//		} 
//		else 
//		{
			ray.origin = this.transform.position;
			ray.direction = this.transform.forward;
//		}
			
		Debug.DrawRay(ray.origin, ray.direction, Color.red);

		if (Physics.Raycast (ray, out hit, 100000f, lMask)) 
		{                                  
			//Debug.DrawRay(ray.origin, ray.direction);

			//The thing we are looking at isnt the same guy!!
			if (hit.transform.gameObject != gazedObject)
			{
				//Stop looking at the old guy! Tell him to go away.
				if (gazeResponder != null)
				{
//					Debug.Log("Gaze End");
					gazeResponder.OnGazeExit();

					if(MergeReticle.instance != null) MergeReticle.instance.OffHoverAction ();

					if ( OnGaze_End != null)
					{
						OnGaze_End.Invoke();
					}
				}

//				Debug.Log("Cleaned CurGaze");
				//Clean up for the new guy
				currentlyGazing = false;
				gazedObject = hit.transform.gameObject;
				gazeResponder = hit.transform.GetComponent<GazeResponder>();
			}
				
			//We were  NOT previously looking at something last tick, so lets have it do stuff
			//I must be looking at something new, look at the new thing.
			if (!currentlyGazing && gazedObject != null && gazeResponder != null )
			{
//				Debug.Log("Gaze Start");
				gazeResponder.OnGazeEnter();

				if(MergeReticle.instance != null) MergeReticle.instance.OnHoverAction ();

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

				if(MergeReticle.instance != null) MergeReticle.instance.OffHoverAction ();

				if( OnGaze_End != null )
				{
					OnGaze_End.Invoke();
				}
			}
				
			currentlyGazing = false;
			gazedObject = null;
			gazeResponder = null;
		}

		if(Input.GetMouseButtonDown(0)&& MergeCubeSDK.instance.IsValidClick())
		{
//			Debug.Log("TAP");
			TriggerPressed();
		}

		if(Input.GetMouseButtonUp(0)&& MergeCubeSDK.instance.IsValidClick())
		{
//			Debug.Log("END TAP");
			TriggerReleased();

			if (isVRMode)
			{
//				Debug.Log("Is mono screen mode? " + isMonoScreenMode);
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

		if(MergeReticle.instance != null) MergeReticle.instance.OnClickAction ();

		//I pushed the button, is there somebody I'm looking at who would care about this?
		if (currentlyGazing && gazedObject != null && gazeResponder != null)
		{
			gazeResponder.OnGazeTrigger();
			pressedObject = gazeResponder;

//			if(MergeReticle.instance != null) MergeReticle.instance.OnClickAction ();

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
		if(MergeReticle.instance != null) MergeReticle.instance.OffClickAction ();
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
