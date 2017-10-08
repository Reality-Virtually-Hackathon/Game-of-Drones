using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController: MonoBehaviour 
{

	public GameObject startPosGO;
	GameObject moveCubeStartPosGO;
//	[HideInInspector]
	public GameObject currentTarget;

	public LineRenderer lr;
	public LRScrollingTexture st;

	float startingLRLength, startingXTilingValue, startingScrollSpeed;
	void Start () 
	{
		moveCubeStartPosGO = MergeMultiTarget.instance.gameObject;
		startingLRLength = GetLineRendererDistance();
		startingXTilingValue = GetTextureXTiling();
		startingScrollSpeed = st.scrollSpeed;

		SetStartPos(startPosGO);
	}


	public float GetLineRendererDistance()
	{
		return Vector3.Distance(lr.GetPosition(0), lr.GetPosition(1));
	}

	public float GetTextureXTiling()
	{
		return lr.material.GetTextureScale("_MainTex").x;
	}

	public float GetTextureXOffset()
	{
		return lr.material.GetTextureOffset("_MainTex").x;
	}
		

	[HideInInspector]
	public bool isInMoveCubeTut = false;
	void Update () 
	{
		if(isInMoveCubeTut)
		{
			SetStartPos(moveCubeStartPosGO);
		}
		else
		{
			SetStartPos(startPosGO);
		}

		if (AdjustForNoTarget())
		{
			SetTargetPosGOLocation();
			//AdjustForLineLength();
			//LoopTexture();
		}

		//LoopTexture();

//		Debug.Log("Screen width: " + Screen.width);
//		Debug.Log("Screen height: " + Screen.height);

	}


	public void LoopTexture()
	{
		if(Mathf.Abs(GetTextureXTiling()) < Mathf.Abs(GetTextureXOffset()))
		{
			st.ResetStartTime();
		}
	}

	public void TimedStartPosUpdate()
	{
		float temp = lr.material.GetTextureOffset("_MainTex").x / st.scrollSpeed;
		Debug.Log(temp % 1.0f);
		if(temp % 1.0f == 0)
		{
			if(isInMoveCubeTut)
			{
				SetStartPos(moveCubeStartPosGO);
			}
			else
			{
				SetStartPos(startPosGO);
			}
		}
	}


	public void SetStartPos(GameObject newStartPos)
	{
		lr.SetPosition(0, newStartPos.transform.position);
	}


	public void SetCurrentTargetPos(GameObject newTarget)
	{
		currentTarget = newTarget;
		lr.SetPosition(1, newTarget.transform.position);
	}


	public bool AdjustForNoTarget()
	{
		return currentTarget != null;
	}


	public void EnableLine()
	{
		lr.enabled = st.enabled = true;
	}


	public void DisableLine()
	{
		lr.enabled = st.enabled = false;
	}


	public void EnableArrowIndicator()
	{
		arrowIndicator.SetActive(true);
	}


	public void DisableArrowIndicator()
	{
		arrowIndicator.SetActive(false);
	}


	public void AdjustForLineLength()
	{
		float lengthPercentage = GetLineRendererDistance() / startingLRLength;
		float newXTilingValue = startingXTilingValue * lengthPercentage;

		st.scrollSpeed = st.startingScrollSpeed * lengthPercentage;
		lr.materials[0].SetTextureScale("_MainTex", (new Vector2(newXTilingValue, 1)));
	}


	//**************************************************************************************************************************************************
	//Indicator Arrow Code
	//**************************************************************************************************************************************************

	public GameObject arrowIndicator;
	public float xMaxValue = 0.5f;
	public float yMaxValue = 0.35f;

//	public GameObject leftLens;
	public void SetTargetPosGOLocation()
	{
		//get target's screen position
		Vector3 screenPos = Camera.main.WorldToScreenPoint(currentTarget.transform.position);

		//target on screen
		if (TargetOnScreenFlag())
		{
			EnableLine();
			DisableArrowIndicator();
		}
		else //object is not on screen
		{
			//if target is behind camera, flip the axis to compensate
			if (screenPos.z < 0)
			{
				screenPos *= -1;
			}

			//find screen's center position
			Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0)/2;





			//NOTE: TEST CODE
			//Vector3 screenCenter = startPosGO.transform.localPosition;

			//Vector3 screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 2)/2);

			//screenCenter -= Camera.main.WorldToScreenPoint(leftLens.transform.localPosition);






			//make (0, 0) pivot at screen center
			screenPos -= screenCenter;

			//find angle from center of screen to target screen position
			float angle = Mathf.Atan2(screenPos.y, screenPos.x);
			angle -= 90 * Mathf.Deg2Rad;

			float cos = Mathf.Cos(angle);
			float sin = -Mathf.Sin(angle);

			screenPos = screenCenter + new Vector3(sin * 150, cos * 150, 0);

			float m = cos / sin;

			Vector3 screenBounds = screenCenter * 0.9f;

			float xMult, yMult;
			if (cos > 0)
			{
				screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
				yMult = 1;
				if (sin > 0)
				{
					xMult = 1;
				}
				else
				{
					xMult = 1;
				}
			}
			else
			{
				screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
				yMult = -1;
				if (sin > 0)
				{
					xMult = -1;
				}
				else
				{
					xMult = -1;
				}
			}

			if (screenPos.x > screenBounds.x)
			{
				screenPos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
			}
			else if(screenPos.x < -screenBounds.x)
			{
				screenPos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
			}

			float xPixelRatio = screenPos.x / Screen.width;
			float yPixelRatio = screenPos.y / Screen.height;

			Vector3 worldPos = new Vector3((xPixelRatio / 0.5f) * xMaxValue , (yPixelRatio / 0.5f) * yMaxValue, 0);

			arrowIndicator.transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);


			//NOTE: TESTING VARIBLE
			float tempMult = 1f;
			arrowIndicator.transform.GetChild(0).localPosition = new Vector3(xMult * worldPos.x * tempMult, yMult * worldPos.y * tempMult, 0);
			DisableLine();
			EnableArrowIndicator();
		}
	}


	public bool TargetOnScreenFlag()
	{
		Vector3 screenPoint = Camera.main.WorldToViewportPoint(currentTarget.transform.position);
		bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
		return onScreen;
	}


}
