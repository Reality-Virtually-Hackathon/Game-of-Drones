using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MergeCube;

public class LensGenerator : MonoBehaviour 
{
	private int xDivisions = 20;
	private int yDivisions = 20;
	private Vector2 dimensions = Vector2.one;
	private Vector3[] vertices;

	private float k1Co;
	private float k2Co;

	private Vector2 resolution;
	private Vector2 position;
	private Vector2 distortionCoefficient;

	private GameObject[] activeEyes = null;

	public Material renderTextureMaterial;

	public static LensGenerator instance;
	public Transform spawnLocation;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			DestroyImmediate(this.gameObject);
	}
		

	public void AdjustLens ( HeadsetsConfigurationData confData ) 
	{

		if (activeEyes != null)
		{
			for (int index = 0; index < activeEyes.Length; index++)
			{
				Destroy(activeEyes[index]);
			}

			activeEyes = null;
		}

		dimensions = confData.size;
		position = confData.positions;
		distortionCoefficient = confData.distortionCoefficients;

		k1Co = distortionCoefficient.x;
		k2Co = distortionCoefficient.y;

		GameObject[] eyes = GenerateEyes ();
		activeEyes = eyes;

//		Debug.Log ( "Left eye: " + eyes[0] + ", Right eye: " + eyes[1] );
	}

	private GameObject[] GenerateEyes () 
	{
		GameObject[] compiledEyes = new GameObject[2];

		for (int index = 0; index < 2; index++)
		{
			GameObject eye = (index == 0) ? new GameObject("L") : new GameObject("R");

			Mesh generatedMesh = GenerateMesh();
			MeshFilter eyeMesh = eye.AddComponent<MeshFilter>();
			MeshRenderer eyeRend = eye.AddComponent<MeshRenderer>();

			eyeMesh.mesh = generatedMesh;

			eye.transform.parent = spawnLocation.transform;
			eye.transform.localPosition = Vector3.zero;
			eye.transform.localScale = Vector3.one;
			eye.transform.localRotation = Quaternion.identity;

			#region LENS ADJUSTMENT CODE
			float adjustment = 0f;
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				adjustment = MergeUtility.GetAdjustment (MergeUtility.BASE_IPHONE_DIMENSION.x, position.x);
				//Debug.Log ("Base Width = " + MergeCubeUtility.BASE_IPHONE_DIMENSION.x + " Base Position = " + position);
			} else if (Application.platform == RuntimePlatform.Android) {
				adjustment = MergeUtility.GetAdjustment (MergeUtility.BASE_ANDROID_DIMENSION.x, position.x);
				//Debug.Log ("Base Width = " + MergeCubeUtility.BASE_ANDROID_DIMENSION.x + " Base Position = " + position);
			} else {
				adjustment = 0f;
			}
			//Debug.Log ("Actual Screen Size = " + MergeCubeUtility.GetActualLadscapeScreenSize () + " adjustment = " + adjustment);


			float posX = 0;
			if (index == 0) {
				posX = -position.x / 2f + adjustment / 2f;
			} else {
				posX = position.x / 2f - adjustment / 2f;
			}

//			Debug.Log (eye.name + " positionX = " + posX + " positionY = " + position.y);
			#endregion LENS ADJUSTMENT CODE

			eyeMesh.transform.localPosition = (index == 0) ? new Vector3 (posX, position.y) : new Vector3 (posX, position.y);
			eyeRend.material = renderTextureMaterial;
			eye.layer = 29;

			compiledEyes[index] = eye;
		}

		return compiledEyes;
	}

	Mesh GenerateMesh()
	{
		Mesh mesh = new Mesh();

		vertices = new Vector3[(xDivisions + 1) * (yDivisions + 1)];
		Vector2[] uv = new Vector2[vertices.Length];

		for (int i = 0, yIndex = 0; yIndex <= yDivisions; yIndex++)
		{
			for (int xIndex = 0; xIndex <= xDivisions; xIndex++, i++)
			{
				//position
				vertices[i] = new Vector3((((((float)xIndex / (float)xDivisions) * dimensions.x) - (dimensions.x / 2))), (((((float)yIndex / (float)yDivisions) * dimensions.y) - (dimensions.y / 2))));

				//uvs
				//applies uvs setup to range of 0 to 1. no distortion happening.
				uv[i] = ApplyDistortion(new Vector2((float)(xIndex) / xDivisions, (float)(yIndex) / yDivisions));
			}
		}

		mesh.vertices = vertices;
		mesh.uv = uv;

		//a quad is 3 verts and 3 verts for 6.
		//hence xDivisions * yDivisions * 6
		//triangles isn't a count of triangles but rather the collection of verts to make a triangle.
		int[] triangles = new int[xDivisions * yDivisions * 6];

		//for each y division
		for (int triangleIndex = 0, vertexIndex = 0, y = 0; y < yDivisions; y++, vertexIndex++)
		{
			//for each x division, progress to the next set of triangles. (6 triangle indicies) go to the next vertex index
			for (int x = 0; x < xDivisions; x++, triangleIndex += 6, vertexIndex++)
			{
				//Assuming xDivisions is 1 and triangleIndex and vertexIndex are 0:
				//tri 1: 0,2,1
				//tri 2: 1,2,3

				//triangle 1 solo vertex:
				triangles[triangleIndex] = vertexIndex;

				//shared verts between both triangles
				triangles[triangleIndex + 3] = triangles[triangleIndex + 2] = vertexIndex + 1;
				triangles[triangleIndex + 4] = triangles[triangleIndex + 1] = vertexIndex + xDivisions + 1;

				//triangle 2 solo vertex:
				triangles[triangleIndex + 5] = vertexIndex + xDivisions + 2;
			}
		}

		mesh.triangles = triangles;
		mesh.RecalculateNormals();

		return mesh;
	}

	Vector2 ApplyDistortion (Vector2 originalPosition) {
		float halfWidth = (1f / 2f);
		float halfHeight = (1f / 2f);

		Vector2 distortedPosition = Vector2.zero;
		distortedPosition.x = originalPosition.x - halfWidth;
		distortedPosition.y = originalPosition.y - halfHeight;

		float distance = Mathf.Sqrt ((distortedPosition.x * distortedPosition.x) + (distortedPosition.y * distortedPosition.y));

		float distSquared = distance * distance;
		float distortion = (1 + (k1Co * distSquared) + (k2Co * (distSquared * distSquared)));
		float xDistorted = distortion * (distortedPosition.x);
		float yDistorted = distortion * (distortedPosition.y);

		Vector2 finalCalc = new Vector2 (halfWidth + xDistorted, halfHeight + yDistorted);
		return finalCalc;
	}

}
