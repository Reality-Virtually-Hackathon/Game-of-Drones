using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveFrontCamera : MonoBehaviour {

	Material generatedMaterial;

	void Start()
	{
		SteamVR_TrackedCamera.Source(true).Acquire();
		var renderer = GetComponent<Renderer> ();
		generatedMaterial = new Material (renderer.sharedMaterial);
		renderer.sharedMaterial = generatedMaterial;
	}

	void OnDestroy()
	{
		SteamVR_TrackedCamera.Source(true).Release();
		if (generatedMaterial != null) {
			Destroy (generatedMaterial);
		}
	}

	void Update()
	{
		
		var source = SteamVR_TrackedCamera.Source(true);
		var texture = source.texture;
		if (texture != null)
		{
			texture.filterMode = FilterMode.Point;
			generatedMaterial.mainTexture = texture;
		}
	}
}
