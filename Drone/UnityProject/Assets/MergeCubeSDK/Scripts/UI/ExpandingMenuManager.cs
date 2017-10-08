using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MergeCube;

public class ExpandingMenuManager : MonoBehaviour 
{
	Vector2 originalTransformValues;
	Vector2 targetTransformValues;
	public GameObject expansionMenu;
	public RectTransform contentSpace;
	public ToggleSprite menuButton;
	public bool isOpen = false;

	[SerializeField]
	public List<HeadsetListButton> headsetList;
	public GameObject mergeHeadsetDataPrefab;
	public GameObject mergeHeadsetDataInstance;
	public GameObject headsetButtonPrefab;
	public Transform headsetLayoutHandler;
	public GameObject dividerPrefab;
	public Transform dividerLayoutHandler;

	private bool headsetListIsInitialized = false;

	public static ExpandingMenuManager instance;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			DestroyImmediate(this.gameObject);
	}

	void Start()
	{
		originalTransformValues = this.GetComponent<RectTransform>().sizeDelta;
		targetTransformValues = headsetLayoutHandler.GetComponent<RectTransform>().sizeDelta;

		//Make sure we don't go past the max size we defined in editor.
		if (targetTransformValues.y > originalTransformValues.y)
		{
			targetTransformValues = new Vector2(targetTransformValues.x, originalTransformValues.y);
		}

		this.GetComponent<RectTransform>().sizeDelta = new Vector2(originalTransformValues.x, 0f);
		expansionMenu.SetActive(false);

		//add merge headset to headset list.
		GameObject tmp = GameObject.Instantiate(mergeHeadsetDataPrefab,headsetLayoutHandler);
		mergeHeadsetDataInstance = tmp;
		HeadsetListButton mergeHeadsetData = tmp.GetComponentInChildren<HeadsetListButton>();

		GameObject divider = GameObject.Instantiate(dividerPrefab, dividerLayoutHandler);
		divider.GetComponent<Image>().enabled = false;
		headsetList.Add(mergeHeadsetData);

		SortHeadsetList();

		mergeHeadsetData.SelectHeadset();
	}

	public void ToggleMenu()
	{
		if (!isOpen)
			ExpandMenu();
		else
			CollapseMenu();
		
		isOpen = !isOpen;
	}


	public void ExpandMenu()
	{
		StartCoroutine(BeginExpansion());
	}

	IEnumerator BeginExpansion()
	{
		expansionMenu.SetActive(true);


		float duration = .2f;
		float elapsed = 0f;

		while (elapsed < duration)
		{
			if (elapsed > duration)
				elapsed = duration;
			
			this.GetComponent<RectTransform>().sizeDelta = new Vector2(originalTransformValues.x, Mathf.Lerp(0f, targetTransformValues.y, elapsed / duration));

			elapsed += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
			
		this.GetComponent<RectTransform>().sizeDelta = new Vector2( originalTransformValues.x, originalTransformValues.y );

		if (!headsetListIsInitialized)
		{
			Debug.LogError("Need to update List!");
			yield return new WaitUntil(() => HeadsetCompatibilityCore.instance.dataIsReady);
			PopulateList();
		}
	}


	void PopulateList()
	{
		Debug.LogError("Populating pulled data!");
		//Populate other items from list
		List<HeadsetsButtonData> headsetRawDataList = HeadsetCompatibilityCore.instance.GetHeadsets();

		for (int index = 0; index < headsetRawDataList.Count; index++)
		{
			Debug.LogError("Reading: " + headsetRawDataList[index].name);

			GameObject tmp = GameObject.Instantiate(headsetButtonPrefab, headsetLayoutHandler);
			GameObject divider = GameObject.Instantiate(dividerPrefab, dividerLayoutHandler);

			HeadsetListButton buttonComp = tmp.GetComponentInChildren<HeadsetListButton>();
			buttonComp.Initialize(headsetRawDataList[index]);

			headsetList.Add(buttonComp);
		}
			
		//I shouldn't be needing to multiply this by count, but for some reason it is only grabbing the size of one of them.
		contentSpace.sizeDelta = new Vector2(contentSpace.sizeDelta.x, headsetLayoutHandler.GetComponent<RectTransform>().sizeDelta.y * headsetList.Count);

		SortHeadsetList();

		headsetListIsInitialized = true;
	}

	HeadsetListButton currentSelectedHeadset;
	public GameObject highlight;
	public void SetCurrentHeadset( HeadsetListButton headset )
	{
		if (currentSelectedHeadset != null)
		{
			currentSelectedHeadset.priority = 0;
			currentSelectedHeadset.backPanel.enabled = true;
		}

		currentSelectedHeadset = headset;
		currentSelectedHeadset.priority = -2;
		currentSelectedHeadset.backPanel.enabled = false;


		SortHeadsetList();
		menuButton.ToggleSpriteVisuals();
		isOpen = false;
		CollapseMenu();
	}

	public void SortHeadsetList()
	{
		headsetLayoutHandler.transform.SetAsFirstSibling();
		highlight.transform.SetAsFirstSibling();

		//Make sure the merge headset is initialized to its default value
		headsetList.Find( x => x == mergeHeadsetDataInstance.GetComponentInChildren<HeadsetListButton>()).priority = -1;

		//sort list
		headsetList.Sort( (x,y) => x.priority.CompareTo(y.priority) );

		//visually arrange list
		for (int index = 0; index < headsetList.Count; index++)
		{
			headsetList[index].transform.parent.SetSiblingIndex(index);
			headsetList[index].backPanel.enabled = true;
		}

		headsetList[0].backPanel.enabled = false;
	}

	public void CollapseMenu()
	{
		StartCoroutine(BeginCollapse());
	}

	IEnumerator BeginCollapse()
	{
		float duration = .2f;
		float elapsed = 0f;

		while (elapsed < duration)
		{
			if (elapsed > duration)
				elapsed = duration;

			this.GetComponent<RectTransform>().sizeDelta = new Vector2( originalTransformValues.x, Mathf.Lerp( originalTransformValues.y, 0f , elapsed/duration ));

			elapsed += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		this.GetComponent<RectTransform>().sizeDelta = new Vector2( originalTransformValues.x, 0f );
		expansionMenu.SetActive(false);
	}
}
