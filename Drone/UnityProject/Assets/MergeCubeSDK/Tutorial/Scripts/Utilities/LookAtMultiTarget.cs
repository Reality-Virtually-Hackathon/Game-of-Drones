using UnityEngine;
using System.Collections;

namespace Utilities
{
	public class LookAtMultiTarget: MonoBehaviour 
	{
		Transform target;

		void Start()
		{
			target = MergeMultiTarget.instance.transform;
		}

		void LateUpdate () 
		{
			this.transform.LookAt(target, target.up);
		}
	}
}