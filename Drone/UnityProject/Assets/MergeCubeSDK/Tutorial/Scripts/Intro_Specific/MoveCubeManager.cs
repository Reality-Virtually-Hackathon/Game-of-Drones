using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCubeManager : MonoBehaviour 
{


	public LayerMask layer;

	RaycastHit hit;

	public GameObject moveCubeCanvasPrefab;

	public LineRendererController LRController;


	//user enters move cube state
	//first move cube canvas spawns within range of user's screen

	public int areaCount = 4;
	public int markedCount = 0;
	void Start()
	{
		UpdateRotation();
		SpawnMoveCubeCanvas();
		LRController.isInMoveCubeTut = true;
	}


	void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(Camera.main.transform.position, (MergeMultiTarget.instance.transform.position - Camera.main.transform.position) * 100f);
	}


	// Update is called once per frame
	void Update()
	{
		if(markedCount < areaCount && MergeMultiTarget.instance.isTracking)
		{

			if( Physics.Raycast(Camera.main.transform.position, (MergeMultiTarget.instance.transform.position - Camera.main.transform.position) * 100f, out hit, 100f, layer))
			{
				if(hit.collider.GetComponent<MoveCubeCanvas>())
				{
					hit.collider.GetComponent<MoveCubeCanvas>().ConfirmAsMarked();
					hit.collider.enabled = false;
					markedCount++;

					PlayCheckmarkSFX();
					LRController.DisableLine();

					StartCoroutine(NextCanvas());
				}
			} 
		}

		if(LRController != null && LRController.TargetOnScreenFlag())
		{
			if (MergeMultiTarget.instance.isTracking)
			{
				LRController.EnableLine();
			}
			else
			{
				LRController.DisableLine();
			}
		}
	}


	public GameObject SpawnMoveCubeCanvas()
	{
		GameObject newTarget = (GameObject)Instantiate(moveCubeCanvasPrefab, this.transform.position, new Quaternion());
		newTarget.transform.parent = this.transform;
		ChooseRandomPosition(newTarget);  
		if (LRController != null)
		{
			LRController.SetCurrentTargetPos(newTarget);
			LRController.EnableLine();
		}
		return newTarget;
	}


	public float xMax = 0.105f;
	public float yMax = 0.059f;
	public float zMax = 0.404f;
	public float percent = 0.25f;
	Vector3 lastCanvasPos;
	public void ChooseRandomPosition(GameObject newTarget)
	{
		Random.InitState((int)System.DateTime.Now.Ticks);

		float x = 0f; 
		float y = 0f;
		float z = 0f;

		x = Random.Range(-xMax, xMax);
		y = Random.Range(-yMax, yMax);

		z = zMax;

		x = SpaceValue(lastCanvasPos.x, x, xMax);
		y = SpaceValue(lastCanvasPos.y, y, yMax);

		lastCanvasPos = new Vector3(x, 0, z);

		//newTarget.transform.localPosition += lastCanvasPos;
		newTarget.transform.localPosition = new Vector3(lastCanvasPos.x, y, lastCanvasPos.z);
		lastCanvasPos = newTarget.transform.localPosition;
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


	IEnumerator NextCanvas()
	{
		yield return new WaitForSeconds(1.0f);

		if (markedCount == areaCount)
		{
			MergeTutorial.ins.AllAreasHighlighted();
		}
		else
		{
			SpawnMoveCubeCanvas();
		}
	}


	public AudioSource myAudioSource;
	public void PlayCheckmarkSFX()
	{
		myAudioSource.Stop();
		myAudioSource.Play();
	}


	public void UpdateRotation()
	{
		//		this.transform.localRotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, 
		//			Camera.main.transform.eulerAngles.y, 
		//			transform.eulerAngles.z);

		this.transform.localRotation = Quaternion.Euler(0, 
			Camera.main.transform.eulerAngles.y, 
			transform.eulerAngles.z);
	}

}
