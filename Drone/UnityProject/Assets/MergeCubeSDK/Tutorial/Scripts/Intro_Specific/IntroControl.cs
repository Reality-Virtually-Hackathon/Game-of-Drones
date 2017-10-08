using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vuforia;

public class IntroControl : MonoBehaviour 
{
	static IntroControl s_ins;

	void Awake()
	{
		SetupIntro();

		if (startImmediately)
		{
			StartIntro();
		}
	}


	public static IntroControl ins
	{
		get{ return s_ins; }
	}


	public Callback introDone;
//	public bool initAwake = true;
	public bool skipIntro = false;
	public bool startImmediately = false;
	public GameObject customReticle;

	[Space(40)]

	public GameObject cubeObjRoot;
	public GameObject subRoot;
	private GameObject multiTarget;


	[HideInInspector]
	public IntroTrackableEventHandler myTrackingSystem;


	public void SetupIntro()
	{
		s_ins = this;

		if (skipIntro)
		{
			IntroDone();
			return;
		}
			
		AddIntroEventHandler();

		MergeMultiTarget.instance.OnTrackingFound += OnTrackingFound;
		MergeMultiTarget.instance.OnTrackingLost += OnTrackingLost;

		audioPlayer = GetComponent<AudioSource> ();

		if (customReticle != null)
		{
			customReticle.SetActive(false);
		}

		tutorialModel = tutorialModelRoot.GetComponent<ModelAnimationManager>();
		if (LRController != null)
		{
			lrg = LRController.GetComponent<LineRendererController>();	
		}
	}


	void AddIntroEventHandler()
	{
		multiTarget.AddComponent<IntroTrackableEventHandler>();
	}


	void RemoveIntroEventHandler()
	{
		Destroy(multiTarget.GetComponent<IntroTrackableEventHandler>());
	}


	public void StartIntro()
	{
		StartCoroutine (IntroSeq ());
	}

	public GameObject step1_scanToBegin;
	public GameObject step1_scanner;
	public GameObject step2_clickBtnToBeginText;
	public GameObject step3_cursor;
	public GameObject step3_cursorHighlight; 
	public GameObject step3_cursorHighlightText;
	public GameObject step3_targetCube;
	public GameObject step3_frontTarget;
	public GameObject step4_backTarget;
	public GameObject step4_lookBackTargetText;
	public GameObject step5_lookAroundText;
	public GameObject step5_lookAroundTargets;
	public GameObject step5_returnToCubeText;
	public GameObject step6_moveCubeCanvasesRoot;
	public GameObject step6_moveCubeText;
	public GameObject step7_clickFrontTargetText;
	public GameObject step8_nextArrowUI;
	public GameObject LRController;
	LineRendererController lrg;

	public BoxCollider hologramBoxCollider;
	public GameObject tutorialModelRoot;
	ModelAnimationManager tutorialModel;

	public AudioClip [] voiceOver;
	public AudioClip stepCompleteSFX;

	public GameObject gazeCaster;//, dimmer;


	void OnEnable()
	{
		//reset current model animation when tracking is found
		if(tutorialModelRoot.activeInHierarchy)
		{
			tutorialModel.ResetCurrentState();
		}
	}


	public bool HeadTrackingFlag()
	{
		bool headTrackingEnabled;

		if (VuforiaARController.Instance.WorldCenterModeSetting == VuforiaARController.WorldCenterMode.DEVICE_TRACKING)
		{
			headTrackingEnabled = true;
		}
		else
		{
			headTrackingEnabled = false;
		}

		return headTrackingEnabled;
	}



	AudioSource audioPlayer;
	bool nextStep = false;
	int currentAnimIndex;
	bool isInScanMode = false;
	IEnumerator IntroSeq()
	{
		isInScanMode = true;

		//Scan Cube
		//====================================================
		//pop up scan
		step1_scanToBegin.SetActive(true);
		step1_scanner.SetActive(true);

		yield return new WaitForSeconds (1f);
		PlayVoiceOver (0);

		//Wait for user scan
		StartCoroutine(WaitForScan());
		yield return new WaitUntil (() => nextStep);
		nextStep = false;

		//enable dimmer
		//		dimmer.SetActive(true);

		isInScanMode = false;

		//Click headset button
		//=======================================================================
		//pop up button click
		step1_scanToBegin.SetActive(false);
		step1_scanner.SetActive (false);

		//enable the tutorial model root
		tutorialModelRoot.SetActive(true);

		yield return new WaitForSeconds (.5f);
		step2_clickBtnToBeginText.SetActive(true);

		//Voice line
		PlayVoiceOver(1);

		//play animation
		tutorialModel.setState(0);
		currentAnimIndex = 0;


		//wait for user click
		yield return new WaitUntil (() => Input.GetMouseButtonDown(0));
		step2_clickBtnToBeginText.SetActive(false);

		//play step complete sfx
		PlayStepCompleteSFX();
		yield return new WaitForSeconds(1.0f);


		//Use cursor to look at front target
		//=======================================================================
		//pop up cursor
		Debug.Log("Cursor enabled");
		step3_cursor.SetActive(true);


		//pop up cursor Hightlight, show cursor message
		step3_cursorHighlight.SetActive(true);
		step3_cursorHighlightText.SetActive(true);

		//Voice line
		PlayVoiceOver(2);

		//play animation
		tutorialModel.setState(1);
		currentAnimIndex = 1;

		yield return new WaitForSeconds (.5f);
		step3_targetCube.SetActive(true);
		yield return new WaitForSeconds (3.5f);

		step3_frontTarget.GetComponent<BoxCollider> ().enabled = true;

		//look and point cursor at front target on cube
		StartCoroutine(WaitForPointAtCubeTarget());
		yield return new WaitUntil (() => nextStep);
		nextStep = false;		

		//play step complete sfx
		PlayStepCompleteSFX();
		yield return new WaitForSeconds(1.0f);


		//Rotate cube and look at back target
		//=======================================================================
		//deactive cursor highlight
		step3_cursorHighlight.SetActive(false);
		step3_cursorHighlightText.SetActive(false);

		//deactivate front target
		step3_frontTarget.GetComponent<BoxCollider> ().enabled = false;
		step3_frontTarget.GetComponent<TargetCubeControl>().StopHove();
		step3_frontTarget.SetActive(false);

		//play animation
		tutorialModel.setState(2);
		currentAnimIndex = 2;

		//reset pointCounter for back target
		pointCounter = 0f;

		//enable back target and text box
		step4_backTarget.SetActive(true);
		step4_lookBackTargetText.SetActive(true);

		//Voice line
		PlayVoiceOver(3);

		//enable back target collider after voice line is completed
		step4_backTarget.GetComponent<BoxCollider> ().enabled = true;

		//look and point cursor at back target on cube
		StartCoroutine(WaitForPointAtCubeTarget());
		yield return new WaitUntil (() => nextStep);
		nextStep = false;	

		//play step complete sfx
		PlayStepCompleteSFX();
		yield return new WaitForSeconds(1.0f);

		//disable the lookBackTextBox for next text box
		step4_lookBackTargetText.SetActive(false);

		//Fix cube rotation for user
		//begin cube 180 degree y axis lerp
		tutorialModelRoot.GetComponent<YAxisRotation>().StartTurn();
		yield return new WaitUntil(() => tutorialModelRoot.GetComponent<YAxisRotation>().isDoneRotating == true);


		//Device Tracking Check
		if(HeadTrackingFlag())
		{
			if(LRController != null)
			{
				LRController.SetActive(true);	
			}

			//Look around at floating targets
			//=======================================================================
			//enable front target and disable back target for later check

			step4_backTarget.GetComponent<BoxCollider> ().enabled = false;
			step4_backTarget.SetActive(false);

			//turn off main box collider
			hologramBoxCollider.enabled = false;

			//disable whole target cube until used later
			step3_targetCube.SetActive(false);

			//Voice line
			PlayVoiceOver(4);

			//play animation
			tutorialModel.setState(3);
			currentAnimIndex = 3;

			//After user points at the targets, tell them about looking around
			step5_lookAroundText.SetActive(true);
			step5_lookAroundTargets.SetActive(true);

			//wait for all targets to be looked at
			StartCoroutine(WaitForTargetsLookedAt());
			yield return new WaitUntil (() => nextStep);
			nextStep = false;
			//disable line renderer until next step
			//lrg.lr.enabled = false;
			if(LRController != null)
			{
				LRController.SetActive(false);
			}

			yield return new WaitForSeconds(1.0f);

			step5_lookAroundText.SetActive(false);
			step5_lookAroundTargets.SetActive(false);

			//activate overlay ui to tell user to look at cube again
			if (myTrackingSystem.isTracking == false)
			{
				step5_returnToCubeText.SetActive(true);

				//Voice line
				PlayVoiceOver(5);
			}

			yield return new WaitUntil(() => myTrackingSystem.isTracking);

			//Move cube around to highlighted areas
			//=======================================================================
			//reorientate cube to face user
			cubeObjRoot.transform.FaceToCamera(tutorialModelRoot.transform);
			//Merge.CubeOrientation.OrientateToCamera(multiTarget.transform, cubeObjRoot.transform);

			//cancel "look at cube again" voice line
			audioPlayer.Stop ();

			//deactivate the return canvas
			step5_returnToCubeText.SetActive(false);

			yield return new WaitForSeconds(0.5f);

			//play animation
			tutorialModel.setState(4);
			currentAnimIndex = 4;

			//Voice line
			PlayVoiceOver(6);
			//disable line renderer until next step
			//lrg.lr.enabled = false;

			if(LRController != null)
			{
				LRController.SetActive(true);
			}

			//turn on the move cube canvases root
			step6_moveCubeCanvasesRoot.SetActive(true);

			//turn on move cube text box
			step6_moveCubeText.SetActive(true);

			//call coroutine to check all areas are highlighted
			StartCoroutine(WaitForAreasHighlighted());
			yield return new WaitUntil(() => nextStep);
			nextStep = false;
			if (LRController != null)
			{
				LRController.SetActive(false);	
			}
			yield return new WaitForSeconds(1.0f);
		}


		//Click front cube target
		//=======================================================================
		//reactivate the target on the cube
		//enable main cube collider
//		hologramBoxCollider.enabled = true;
//
//		step6_moveCubeCanvasesRoot.SetActive(false);
//		step6_moveCubeText.SetActive(false);
//
//		//click target to continue
//		step3_targetCube.SetActive(true);
//		step3_frontTarget.SetActive(true);
//		step4_backTarget.SetActive(false);
//
//		step3_frontTarget.GetComponent<BoxCollider> ().enabled = true;
//
//		step7_clickFrontTargetText.SetActive (true);
//
//		//Voice line
//		PlayVoiceOver(7);
//
//		//play animation
//		tutorialModel.setState(5);
//		currentAnimIndex = 5;
//
//		//Point on front target and click
//		frontTargetClicked = false;
//		StartCoroutine(WaitForPointAndClickFrontTarget());
//		yield return new WaitUntil (() => nextStep);
//		nextStep = false;
//
//		//play step complete sfx
//		PlayStepCompleteSFX();
//		yield return new WaitForSeconds(1.0f);


		//Click UI Arrow
		//=======================================================================
		step7_clickFrontTargetText.SetActive (false);
		step3_targetCube.SetActive(false);

		step4_backTarget.SetActive (false);

		//pop UI, tell user click ui to start
		yield return new WaitForSeconds (.5f);
		step8_nextArrowUI.SetActive(true);
		StartCoroutine(WaitForPointAndClickArrow());

		//Voice line
		PlayVoiceOver(8);

		//play animation
		tutorialModel.setState(6);
		currentAnimIndex = 6;

		yield return new WaitUntil (() => nextStep);
		nextStep = false;

		//play step complete sfx
		PlayStepCompleteSFX();
		yield return new WaitForSeconds(1.0f);


		//Start Preview app
		//=======================================================================
		//user point and click
		step8_nextArrowUI.SetActive(false);
		tutorialModelRoot.SetActive(false);

		//done
		IntroDone();
		yield return null;
	}


	float waitCount = 0f;
	IEnumerator WaitForScan()
	{
		while (waitCount < 2f) 
		{
			if (isTracking) 
			{
				waitCount += Time.deltaTime;
			}
			yield return new WaitForFixedUpdate ();
		}
		nextStep = true;
		yield return null;
	}



	float pointCounter = 0f;
	float pointerDuration = .5f;
	public void TargetPointed()
	{
		pointCounter += Time.deltaTime;
	}


	IEnumerator WaitForPointAtCubeTarget()
	{
		yield return new WaitUntil (() => pointCounter>=.5f);
		nextStep = true;
		yield return null;
	}


	bool allTargetsHovered = false;
	public void AllTargetsLookedAt()
	{
		allTargetsHovered = true;
	}


	IEnumerator WaitForTargetsLookedAt()
	{
		yield return new WaitUntil(() => allTargetsHovered);
		nextStep = true;
		yield return null;
	}


	bool allAreasHighlighted = false;
	public void AllAreasHighlighted()
	{
		allAreasHighlighted = true;
	}


	IEnumerator WaitForAreasHighlighted()
	{
		yield return new WaitUntil(() => allAreasHighlighted);
		nextStep = true;
		yield return null;
	}


	bool frontTargetClicked = false;
	public void FrontTargetClicked(){
		frontTargetClicked = true;
	}


	IEnumerator WaitForPointAndClickFrontTarget()
	{
		yield return new WaitUntil (() => frontTargetClicked);
		nextStep = true;
		yield return null;
	}


	bool arrowClicked = false;
	public void ArrowClicked()
	{
		arrowClicked = true;
	}


	IEnumerator WaitForPointAndClickArrow()
	{
		yield return new WaitUntil (() => arrowClicked);
		nextStep = true;
		yield return null;
	}


	bool isTracking = false;
	public void OnTrackingFound()
	{
		isTracking = true;
		cubeObjRoot.SetActive (true);
		subRoot.SetActive (true);
		tutorialModel.setState(currentAnimIndex);
	}


	public void OnTrackingLost()
	{
		isTracking = false;
		cubeObjRoot.SetActive (false);
		subRoot.SetActive (false);
	}


	void PlayVoiceOver(int index)
	{
		audioPlayer.Stop ();
		audioPlayer.PlayOneShot (voiceOver [index]);
	}


	void PlayStepCompleteSFX()
	{
		audioPlayer.PlayOneShot(stepCompleteSFX);
	}


	public void IntroDone()
	{
		if (introDone != null)
		{
			introDone.Invoke();
		}

		if (skipIntro == false)
		{
			myTrackingSystem.RemoveTrackingLogic();
			RemoveIntroEventHandler();
		}

		if (customReticle != null)
		{
			customReticle.SetActive(true);
		}

		//		dimmer.SetActive(false);
		step3_cursor.SetActive (true);
		step3_cursorHighlight.SetActive (false);
		Destroy(LRController);
		Destroy(step1_scanToBegin);
		Destroy(cubeObjRoot);
		Destroy(step5_returnToCubeText);
		Destroy(step3_cursor);
		Destroy(gazeCaster);
		Destroy (gameObject);
	}

}
