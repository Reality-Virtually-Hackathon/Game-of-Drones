using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAroundTargetManager : MonoBehaviour 
{
	public GameObject floatingTargetPrefab;

	bool isHovering = false;

	int currentHoveredOn = 0;

	public int targetsToSpawn = 3;

	public LineRendererController LRController;


	void Start () 
	{
		UpdateRotation();
		currentHoveredOn = 0;
		SpawnTarget();
	}
		

	GameObject currentTarget;
	public GameObject SpawnTarget()
	{
		currentTarget = (GameObject)Instantiate(floatingTargetPrefab, this.transform.position, new Quaternion());
		currentTarget.transform.parent = this.transform;
		ChooseRandomPosition(currentTarget);
		if (LRController != null)
		{
			LRController.SetCurrentTargetPos(currentTarget);
			LRController.EnableLine();
		}
		return currentTarget;
	}


	public float xMax = 0.105f;
	public float yMax = 0.059f;
	public float zMax = 0.404f;
	public float percent = 0.25f;
	Vector3 lastTargetPos;
	public void ChooseRandomPosition(GameObject newTarget)
	{
		Random.InitState((int)System.DateTime.Now.Ticks);

		float x = 0f; 
		float y = 0f;
		float z = 0f;

		x = Random.Range(-xMax, xMax);
		y = Random.Range(-yMax, yMax);

		z = zMax;

		x = SpaceValue(lastTargetPos.x, x, xMax);
		y = SpaceValue(lastTargetPos.y, y, yMax);

		lastTargetPos = new Vector3(x, 0, z);

		newTarget.transform.localPosition = new Vector3(lastTargetPos.x, y, lastTargetPos.z);
		lastTargetPos = newTarget.transform.localPosition;
	}


	public float SpaceValue(float oldValue, float newValue, float maxValue)
	{
		if(Mathf.Abs(oldValue - newValue) < (maxValue * percent))
		{
			if(newValue < oldValue)
			{
				newValue -= (maxValue * percent);
			}
			else
			{
				newValue += (maxValue * percent);
			}
		}

		return Mathf.Clamp(newValue, -maxValue, maxValue);

	}


	public bool IncCheckHoverFlags()
	{
		currentHoveredOn++;
		PlayCheckmarkSFX();
		if (LRController != null)
		{
			LRController.DisableLine();
		}

		if (currentHoveredOn == targetsToSpawn)
		{
			MergeTutorial.ins.AllTargetsLookedAt();
			return true;
		}

		SpawnTarget();

		return false;
	}


	public void UpdateRotation()
	{
		this.transform.localRotation = Quaternion.Euler(0, 
			Camera.main.transform.eulerAngles.y, 
			transform.eulerAngles.z);
	}


	public AudioSource myAudioSource;
	public void PlayCheckmarkSFX()
	{
		myAudioSource.Stop();
		myAudioSource.Play();
	}
}
