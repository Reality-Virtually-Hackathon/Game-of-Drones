using UnityEngine;
using System.Collections;

namespace Utilities
{
	public class LookAtARCamera : MonoBehaviour 
	{
		Transform ARCamera;

		void Start()
		{
			ARCamera = Camera.main.transform;
		}

		void LateUpdate () 
		{
			this.transform.LookAt (ARCamera, ARCamera.transform.up);
		}
	}
}