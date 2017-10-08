using UnityEngine;
using System.Collections;

public class LRScrollingTexture : MonoBehaviour 
{

	public float initialFrequency = 10.0f;
	float scrollFrequency;

	public float scrollSpeed = 0.5F;
	[HideInInspector] 
	public float startingScrollSpeed;

	public Renderer rend;
	public bool X, Y;
	int XMul = 0, YMul = 0;

	float startTime = 0.0f;

//	Vector2 startingTilingValue, currentTiling, currentOffset;
	Vector2 startingOffsetValue;
	void Start() 
	{
		startingScrollSpeed = scrollSpeed;
		rend = GetComponent<Renderer>();
//		startingTilingValue = rend.materials[0].GetTextureScale("_MainTex");
		startingOffsetValue = rend.materials[0].GetTextureOffset("_MainTex");
		scrollFrequency = 1.0f / initialFrequency;
	}



	float offset;
	void Update() 
	{
		scrollFrequency -= (Time.deltaTime * initialFrequency);
		if(scrollFrequency < 0)
		{
			offset += scrollSpeed; //(Time.time - startTime) * scrollSpeed;
			AdjustMultipliers();
			rend.material.SetTextureOffset("_MainTex", new Vector2((offset * XMul) + startingOffsetValue.x, (offset * YMul) + startingOffsetValue.y));
//			currentOffset = rend.materials[0].GetTextureOffset("_MainTex");
			scrollFrequency = 1.0f / initialFrequency;
		}
	}


	public float loopDelay = 1.0f;
	public void ResetStartTime()
	{
		startTime = Time.time + loopDelay;
	}


	public void AdjustMultipliers()
	{
		if (X)
		{
			XMul = 1;
		}
		else
		{
			XMul = 0;
		}

		if (Y)
		{
			YMul = 1;
		}
		else
		{
			YMul = 0;
		}
	}



}