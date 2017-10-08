using UnityEngine;
using System.Collections;

namespace BaseCES
{
	public class IntroSkySphere : MonoBehaviour
	{

		private MeshRenderer myMeshRenderer;

		void Awake()
		{
			myMeshRenderer = GetComponent<MeshRenderer>();
		}

//		public Coroutine FadeAlpha(float endAlpha)
//		{
//			float startAlpha = myMeshRenderer.material.color.a;
//			return FadeAlpha(startAlpha, endAlpha);
//		}

		public Coroutine FadeAlpha(float desiredAlpha, float desiredTime)
		{
			return StartCoroutine(FadeAlphaRoutine(desiredAlpha, desiredTime));
		}

		IEnumerator FadeAlphaRoutine(float desiredAlpha, float desiredTime)
		{
			float lerpHelper = 0f;
			Color startColor = myMeshRenderer.material.color;
			Color endColor = startColor;
			endColor.a = desiredAlpha;

			while (lerpHelper < 1f)
			{
//				if (Input.GetMouseButtonDown(0))
//				{
//					lerpHelper = 1f;
//				}

				lerpHelper += Time.deltaTime / desiredTime;

				myMeshRenderer.material.color = Color.Lerp(startColor, endColor, lerpHelper);

				yield return null;
			}
		}

	} // End class
} // End namespace
