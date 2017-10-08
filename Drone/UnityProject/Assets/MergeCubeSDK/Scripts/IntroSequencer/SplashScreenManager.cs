using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MergeCube;

[RequireComponent(typeof(AudioSource))]
public class SplashScreenManager : MonoBehaviour 
{
	public static SplashScreenManager instance;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			DestroyImmediate(this.gameObject);
	}

	public bool skipSplashScreen = false;
	public Callback OnSplashSequenceEnd;
	public Callback OnTitleMusicStartPoint;

	public CanvasGroup gameSplash;
	public float splashFadeoutInSec = .3f;
	[Range(0f,10f)]
	public float logoDuration = 3f;
	public Animator darkFader;

	public AudioClip logoSound;
	public GameObject loadingAnimaRef;
	bool isBlocked = true;
	bool isCubePageShow = false;
	public void StartSplashSequence()
	{
		if (gameSplash != null)
		{
			gameSplash.alpha = 0f;
		}
		darkFader.Play("FadeStayUp");

		if (skipSplashScreen)
		{
			EndSplashSequence();
		}
		else
		{
			StartCoroutine(BeginSplashSequencer());
		}
	}

	IEnumerator BeginSplashSequencer()
	{
		darkFader.Play("FadeIn");

		if (gameSplash != null)
		{
			gameSplash.alpha = 1;

			//Fade from black to user defined logo
			darkFader.Play("FadeOut");
			yield return new WaitForSeconds(0.5f);

			//Get end user's audio selection if not null
			if (logoSound != null)
			{
				GetComponent<AudioSource>().PlayOneShot(logoSound);
			}
			yield return new WaitUntil (() => !isBlocked);

			if (OnTitleMusicStartPoint != null) {
				OnTitleMusicStartPoint.Invoke ();
			}

			yield return new WaitForSeconds(logoDuration);
//			darkFader.Play("FadeIn");
//			yield return new WaitForSeconds(1.5f);

			EndSplashSequence();
			while (gameSplash.alpha > 0) {
				gameSplash.alpha -= Time.deltaTime*(1f/splashFadeoutInSec);
				yield return null;
			}
			if(gameSplash != null)
				gameSplash.gameObject.SetActive(false);
		}
	}

	void EndSplashSequence()
	{
		if (loadingAnimaRef != null) {
			Destroy (loadingAnimaRef);
		}
		if (OnSplashSequenceEnd != null)
		{
			OnSplashSequenceEnd.Invoke();
		}
//		darkFader.Play("FadeOut");
	}

	public NoticePageManager Page_MergeCubeRequired;
	void Start(){
		Invoke ("Init", .1f);
	}
	void Init(){
//		Debug.LogWarning ("Init Splash");
		if (PlayerPrefs.GetString("CubePagePop") == "") {
			isCubePageShow = true;
//			Debug.LogWarning ("Should Open Cube");
			Page_MergeCubeRequired.gameObject.SetActive (true);
			Page_MergeCubeRequired.doneButton += CubePopDone;
			PlayerPrefs.SetString ("CubePagePop", "done");
		}
		else {
			isBlocked = false;
//			Debug.LogWarning ("ShouldOpenCodePage = "+false);
			Destroy (Page_MergeCubeRequired.gameObject);
		}
	}
		
	void CubePopDone(){
		isCubePageShow = false;
		if (isBlocked) {
//			Debug.LogWarning ("CubePopDone");
			isBlocked = false;
			Page_MergeCubeRequired.doneButton -= CubePopDone;
			Destroy (Page_MergeCubeRequired.gameObject);
		}
	}
	public void OpenMergeCubeUrl(){
		Application.OpenURL (@"https://mergecube.com/needamergecube");
	}
	void Update(){
		if (Input.GetKeyDown (KeyCode.R)) {
			CubePopDone ();
		}
	}
}
