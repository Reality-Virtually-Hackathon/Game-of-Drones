using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour 
{

	public Text[] replaceableTextObjects;

	public string[] replacingStrings;



	void Awake()
	{
		if(MergeCube.MergeCubeSDK.instance.viewMode == MergeCube.MergeCubeSDK.ViewMode.FULLSCREEN)
		{
			ReplaceTextStrings ();
		}
	}


	void ReplaceTextStrings()
	{
		if (replaceableTextObjects.Length == replacingStrings.Length) 
		{
			int i = 0;
			foreach (Text text in replaceableTextObjects) 
			{
				text.text = replacingStrings [i];
				i++;
			}
		}
		else 
		{
			Debug.LogError ("Unable to update intro text boxes. Make sure the text box object array is the same size and the replacing string array");
		}

	}

}
