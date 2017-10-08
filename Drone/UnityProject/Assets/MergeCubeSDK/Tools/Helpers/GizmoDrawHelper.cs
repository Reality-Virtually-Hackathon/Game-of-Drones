using UnityEngine;
using System.Collections;

namespace Utilities
{
	public class GizmoDrawHelper : MonoBehaviour 
	{

		//Always show
		public enum DrawMode { Cube, Mesh, Line, Icon };
		public enum ScaleMode { Transform, Explicit };
		public DrawMode drawMode = DrawMode.Cube;
		public ScaleMode scaleMode = ScaleMode.Transform;
		public string iconName;

		public Color color = Color.white;

		//show if Explicit:
		public Vector3 scale = Vector3.one;

		//show if Line:
		public enum MeasurementFeedback{ Distance, Angle }
		public MeasurementFeedback measurementType = MeasurementFeedback.Distance;
		public Transform target;
		public float radius = 1f;

		//show if Mesh:
		public Mesh mesh;

		void OnDrawGizmos()
		{
			Gizmos.color = color;
			Vector3 chosenScale = (scaleMode == ScaleMode.Transform) ? transform.localScale : scale;

			if (drawMode == DrawMode.Cube)
				Gizmos.DrawWireCube(transform.position, chosenScale);
			else if (drawMode == DrawMode.Mesh)
				Gizmos.DrawWireMesh(mesh, transform.position, transform.localRotation, chosenScale);
			else if (drawMode == DrawMode.Line && target != null)
			{
				if (measurementType == MeasurementFeedback.Distance)
				{
					Gizmos.DrawLine(this.transform.position, target.position);
				}
				else if (measurementType == MeasurementFeedback.Angle)
				{
					Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward );
					Gizmos.DrawLine(target.position, target.position + target.forward );
				}
			}
			else if (drawMode == DrawMode.Icon)
				Gizmos.DrawIcon(this.transform.position, iconName );
		}

	}
}