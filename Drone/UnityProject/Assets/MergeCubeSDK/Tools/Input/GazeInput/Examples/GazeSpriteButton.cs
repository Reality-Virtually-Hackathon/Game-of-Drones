using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GazeSpriteButton : MonoBehaviour, GazeResponder 
{
	private Image image;
	private BoxCollider collider;
	
	public Sprite defaultState;
	public Color defaultColor = Color.white;

	public Sprite hoverState;
	public Color hoverColor = Color.white;

	public Sprite downState;
	public Color downColor = Color.white;

	public Sprite disabledState;
	public Color disabledColor = Color.white;

	public bool isDisabled = false;

	public UnityEvent OnGazeStart;
	public UnityEvent OnGazeEnd;
	public UnityEvent OnGazeInput;
	public UnityEvent OnGazeInputEnd;

	void Start()
	{
		if(image == null)
			image = gameObject.GetComponent<Image>();
		
		if(collider == null)
			collider = gameObject.GetComponent<BoxCollider>();
	}

	void OnValidate()
	{
		if(image == null)
			image = gameObject.GetComponent<Image>();
		
		if(collider == null)
			collider = gameObject.GetComponent<BoxCollider>();

		if (isDisabled)
		{
			DisableButton();
		}
		else
		{
			EnableButton();
		}
	}

	bool isGazing = false;

	public void OnGazeEnter()
	{
		if (isDisabled)
			return; 
		
		isGazing = true;

		OnGazeStart.Invoke();

		if(hoverState != null)
			image.sprite = hoverState;

		image.color = hoverColor;
	}

	public void OnGazeExit()
	{
		if (isDisabled)
			return; 
		
		isGazing = false; 

		OnGazeEnd.Invoke();

		if( defaultState != null)
			image.sprite = defaultState;

		image.color = defaultColor;
	}

	public void OnGazeTrigger()
	{
		if (isDisabled)
			return; 
		
		OnGazeInput.Invoke();

		if(downState != null)
			image.sprite = downState;

		image.color = downColor;
	}

	public void OnGazeTriggerEnd()
	{
		if (isDisabled)
			return; 
		
		OnGazeInputEnd.Invoke();

		if (isGazing && defaultState != null)
		{
			image.sprite = hoverState;
		}
		else if( hoverState != null)
		{
			image.sprite = defaultState;
		}

		if(isGazing && defaultState != null)
		{
			image.color = hoverColor;
		}
		else if( hoverState != null)
		{
			image.color = defaultColor;
		}
	}


	public void DisableButton()
	{
		isDisabled = true;
		collider.enabled = false;

		if (disabledState != null)
			image.sprite = disabledState;

		image.color = disabledColor;
	}

	public void EnableButton()
	{
		isDisabled = false;
		collider.enabled = true;

		if (defaultState != null)
			image.sprite = defaultState;

		image.color = defaultColor;
	}
}
