using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCubeCanvas : MonoBehaviour 
{

	public GameObject highlightedArea, particleEffect; //, postImage;

	public Behaviour halo;

	ParticleSystem particleSystem;

	public Animator canvasAnimator;

	// Use this for initialization
	void Start () 
	{
		SetToDefault();
		particleSystem = particleEffect.transform.GetChild(0).GetComponent<ParticleSystem>();
	}

	public void SetToDefault()
	{
		highlightedArea.SetActive(true);
	}

	public void ConfirmAsMarked()
	{
		highlightedArea.SetActive(false);
		halo.enabled = false;
		StartCoroutine(DelayedAnim());
	}

	public IEnumerator DelayedAnim()
	{
		canvasAnimator.SetBool("isDespawning", true);
		particleEffect.SetActive(true);
		particleSystem.Play();
		yield return new WaitForSeconds(5.0f);
		Destroy(particleEffect);
		Destroy(this.transform.parent.gameObject);
	}

}
