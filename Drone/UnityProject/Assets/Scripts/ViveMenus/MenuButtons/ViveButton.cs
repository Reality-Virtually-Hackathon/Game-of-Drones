using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ViveButton : MonoBehaviour {
	public event Action onPress;

	public void Press() {
		onPress?.Invoke();
	}
}
