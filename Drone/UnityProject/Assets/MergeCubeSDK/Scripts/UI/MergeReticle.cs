using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MergeCube;
public class MergeReticle : MonoBehaviour {
	public static MergeReticle instance;
	public Transform reticle;
	public Sprite fullScreenSprite;
	public Sprite vrScreenSprite;
	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () {
		defaultScale = reticle.localScale;
		MergeCubeSDK.instance.OnViewModeSwap += SetMode;
	}
	void OnDestroy(){
		MergeCubeSDK.instance.OnViewModeSwap -= SetMode;
	}
	public void SetMode(bool isVRMode){
		reticle.GetComponent<SpriteRenderer> ().sprite = isVRMode ? vrScreenSprite : fullScreenSprite;
	}
	public void ActiveIt(bool isActive){
		reticle.gameObject.SetActive (isActive);
	}
		

	void Update()
	{
	}

	//grow
	public void OnHoverAction()
	{
//		Debug.Log ("Reticle Hover On");
		StartScaleLerp(maxScaleMult, scaleUpDuration);
	}

	//shrink
	public void OffHoverAction()
	{
//		Debug.Log ("Reticle Hover Off");
		StartScaleLerp(minScaleMult, scaleDownDuration);
	}

	//pulse
	public void OnClickAction()
	{
//		Debug.Log ("Reticle Click On");
		reticle.transform.localScale = defaultScale * .5f;
	}

	//pulse
	public void OffClickAction()
	{
//		Debug.Log ("Reticle Click Off");
		reticle.transform.localScale = defaultScale;
	}
		

	Vector3 defaultScale;
	public float maxScaleMult = 1.5f;
	public float minScaleMult = .8f;

	[Space(20)]
	public float scaleUpDuration = 1f;
	public float scaleDownDuration = 1f;


	IEnumerator ScaleLerp(float targetScaleMult, float timerDuration)
	{
		Vector3 startingScale = reticle.localScale;
		Vector3 targetScale = defaultScale * targetScaleMult;
		float time = 0f;

		while((time/timerDuration) < 1f)
		{
			reticle.localScale = Vector3.Lerp(startingScale, targetScale, time/timerDuration);
			time += Time.deltaTime;
			yield return null;
		}
		reticle.localScale = targetScale;
	}


	Coroutine scaleLerpCo;
	void StartScaleLerp(float targetScaleMult, float timerDuration)
	{
		StopScaleLerp ();
		scaleLerpCo = StartCoroutine (ScaleLerp (targetScaleMult, timerDuration));
	}


	void StopScaleLerp()
	{
		if(scaleLerpCo != null)
		{
			StopCoroutine (scaleLerpCo);
		}
	}
		
}
