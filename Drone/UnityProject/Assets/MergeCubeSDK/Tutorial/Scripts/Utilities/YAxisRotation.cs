using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YAxisRotation : MonoBehaviour 
{

	public float rotationSpeed = 1.0f;

	Vector3 startRotation, endRotation;

	public Transform introChildRoot;


	[HideInInspector]
	public bool isDoneRotating = false, isGoodToRotate = false;

	public void Update()
	{
		if( isGoodToRotate == true && 
		    isDoneRotating == false)
		{
			Vector3 updatedAngles = Vector3.Lerp(startRotation, endRotation, (Time.time - startTime) * rotationSpeed);

			introChildRoot.transform.localEulerAngles = updatedAngles;

			if((Time.time - startTime) * rotationSpeed >= 1.0f)
			{
				StopTurn();
			}
		}
	}
		

	float startTime;
	public void StartTurn()
	{
		isGoodToRotate = true;
		startTime = Time.time;
		startRotation = introChildRoot.transform.localEulerAngles;
		endRotation = SolveForEndRotation();
	}

	public void StopTurn()
	{
		isDoneRotating = true;
		isGoodToRotate = false;
	}


	public Vector3 SolveForEndRotation()
	{
		Vector3 baseVector3 = new Vector3(0f, 0f, 0f);

		switch (FindRelevantAxis())
		{
			case Axes.X:
				baseVector3 += new Vector3(180f, 0f, 0f);
				break;
			case Axes.Y:
				baseVector3 += new Vector3(0f, 180f, 0f);
				break;
			case Axes.Z:
				baseVector3 += new Vector3(0f, 0f, 180f);
				break;
			default:
				Debug.Log("Invalid Axis");
				break;
		}

		return startRotation + baseVector3;
	}


	[HideInInspector]
	public enum Axes {X, Y, Z, Invalid};
	Axes[] axisArray = new Axes[4] {Axes.X, Axes.Y, Axes.Z, Axes.Invalid};
	public Axes FindRelevantAxis()
	{
		Vector3[] multiTargetDirections = { introChildRoot.right, introChildRoot.up, introChildRoot.forward};

		int i = 0;
		foreach(Vector3 direction in multiTargetDirections)
		{
			if (Vector3.Cross(transform.up, direction) == Vector3.zero)
			{
				return axisArray[i];
			}
			i++;
		}

		return Axes.Invalid;
	}
}
