using UnityEngine;
using System.Collections;

public class CubeScan : MonoBehaviour
{
	public float speed = 1f;
	private Material myMaterial;
	private Vector2 offset;
	private const string PROPERTY = "_MainTex";

	void Awake()
	{
		myMaterial = GetComponent<MeshRenderer>().material;
		offset = myMaterial.GetTextureOffset(PROPERTY);
	}

	void Update()
	{
		offset += Vector2.right * Time.deltaTime * speed;
		myMaterial.SetTextureOffset(PROPERTY, offset);
	}

}
