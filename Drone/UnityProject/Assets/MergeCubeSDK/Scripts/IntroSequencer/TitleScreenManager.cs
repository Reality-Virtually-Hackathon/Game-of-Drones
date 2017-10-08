using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MergeCube;

public class TitleScreenManager : MonoBehaviour 
{
	public static TitleScreenManager instance;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			DestroyImmediate(this.gameObject);

		titleScreen.SetActive(false);
	}

	public GameObject titleScreen;
	public Transform mergeModeButton;
	public Transform mobileModeButton;
	public Sprite mergeModeDefaultSprite;
	public Sprite mergeModeDisabledSprite;

	public Transform playTutorialToggle;
	public Sprite playTutorialTrueSprite;
	public Sprite playTutorialFalseSprite;

	public bool shouldPlayTutorial { get; private set; }
	public delegate void CallbackBooBoo ( bool boo1,bool boo2 );
	public CallbackBooBoo OnTitleSequenceComplete;

	//Entry Method
	public void ShowTitleScreen()
	{
		
		if (MergeCubeSDK.deviceIsTablet)
		{
			DisableMergeMode();
		}

		shouldPlayTutorial = (!PlayerPrefs.HasKey("HasPlayedBefore"));

		if (MergeTutorial.ins == null)
		{
			shouldPlayTutorial = false;
			playTutorialToggle.gameObject.SetActive(false);
		}

		playTutorialToggle.GetComponent<Image>().sprite = (shouldPlayTutorial) ? playTutorialTrueSprite : playTutorialFalseSprite;

		titleScreen.SetActive(true);
	}

	//State Management
	public void DisableMergeMode()
	{
		//Show disabled graphics for MergeMode Button
		mergeModeButton.GetComponent<Image>().sprite = mergeModeDisabledSprite;
		mergeModeButton.GetComponent<Button>().interactable = false;
	}

	public void ToggleShouldPlayTutorial()
	{
		shouldPlayTutorial = !shouldPlayTutorial;	
		playTutorialToggle.GetComponent<Image>().sprite = (shouldPlayTutorial) ? playTutorialTrueSprite : playTutorialFalseSprite;
	}


	//Exit Conditions called by GUI elements
	public void PlayInMergeMode()
	{
		bool shouldSwitch = false;
		titleScreen.SetActive(false);

//		Debug.Log(MergeCubeSDK.instance.viewMode);
		if (MergeCubeSDK.instance.viewMode != MergeCubeSDK.ViewMode.HEADSET)
		{
			shouldSwitch = true;
		}

		if (OnTitleSequenceComplete != null)
		{
			OnTitleSequenceComplete.Invoke( shouldPlayTutorial,shouldSwitch );
		}
		UpdateTutorialSetting ();
	}

	public void PlayInMobileMode()
	{
		bool shouldSwitch = false;
		titleScreen.SetActive(false);

//		Debug.Log(MergeCubeSDK.instance.viewMode);
		if (MergeCubeSDK.instance.viewMode != MergeCubeSDK.ViewMode.FULLSCREEN)
		{
			shouldSwitch = true;
		}

		if (OnTitleSequenceComplete != null)
		{
			OnTitleSequenceComplete.Invoke( shouldPlayTutorial,shouldSwitch );
		}
		UpdateTutorialSetting ();
	}
	void UpdateTutorialSetting(){
		if (!PlayerPrefs.HasKey("HasPlayedBefore"))
		{
			PlayerPrefs.SetString("HasPlayedBefore", "true");
		}
	}
	public void OpenMergeCubeUrl(){
		Application.OpenURL (@"https://mergecube.com/needamergecube");
	}
}
