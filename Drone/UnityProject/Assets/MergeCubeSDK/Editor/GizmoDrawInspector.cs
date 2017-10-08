using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Utilities.GizmoDrawHelper))]
public class GizmoDrawInspector : Editor 
{
	private static readonly string[] _dontIncludeMe = new string[]{"m_Script", "OnRightInput", "OnLeftInput", "OnRightInputEnd", "OnLeftInputEnd"};

	SerializedProperty dMode;
	SerializedProperty sMode;
	SerializedProperty scaleFactor;

	SerializedProperty targetTransform;
	SerializedProperty measurementType;
	SerializedProperty arcRadius;

	SerializedProperty mesh;

	SerializedProperty colorChoice;

	SerializedProperty iconName;

	void OnEnable()
	{
		dMode = serializedObject.FindProperty("drawMode");
		sMode = serializedObject.FindProperty("scaleMode");
		scaleFactor = serializedObject.FindProperty("scale");

		targetTransform = serializedObject.FindProperty("target");
		measurementType = serializedObject.FindProperty("measurementType");
		arcRadius = serializedObject.FindProperty("radius");

		mesh = serializedObject.FindProperty("mesh");

		colorChoice = serializedObject.FindProperty("color");

		iconName = serializedObject.FindProperty("iconName");
	}

	public override void OnInspectorGUI ()
	{ 
		serializedObject.Update ();
		Utilities.GizmoDrawHelper targetComponent = Selection.activeGameObject.GetComponent<Utilities.GizmoDrawHelper>();

		EditorGUILayout.PropertyField( dMode );

		switch (dMode.enumValueIndex) 
		{
			case (int)Utilities.GizmoDrawHelper.DrawMode.Cube:
				ResetLabels();
				EditorGUILayout.PropertyField( sMode );

				if (sMode.enumValueIndex == (int)Utilities.GizmoDrawHelper.ScaleMode.Explicit) 
				{
					scaleFactor.vector3Value = EditorGUILayout.Vector3Field("Scale Factor:", targetComponent.scale);
				}

				colorChoice.colorValue = EditorGUILayout.ColorField("Color:", targetComponent.color);

				break;


			case (int)Utilities.GizmoDrawHelper.DrawMode.Line:

				EditorGUILayout.PropertyField( measurementType );

				EditorGUILayout.PropertyField(targetTransform);

				if (targetComponent.target != null)
				{
					switch (measurementType.intValue)
					{
						case (int)Utilities.GizmoDrawHelper.MeasurementFeedback.Distance:
							ResetWireArc();
							float distance = (targetComponent.target.position - Selection.activeGameObject.transform.position).magnitude;
							message = distance.ToString() + " m";
							position = (targetComponent.target.position - Selection.activeGameObject.transform.position) * 0.5f + Selection.activeGameObject.transform.position;
							drawLabels = true;
							break;

						case (int)Utilities.GizmoDrawHelper.MeasurementFeedback.Angle:

							EditorGUILayout.PropertyField(arcRadius);

							float angle = Vector3.Angle(Selection.activeGameObject.transform.forward, Selection.activeGameObject.transform.InverseTransformDirection(targetComponent.target.forward)) / 2f;
//							float angleDistance = (targetComponent.target.position - Selection.activeGameObject.transform.position).magnitude;
							message = angle.ToString() + " deg";
							position = (Selection.activeGameObject.transform.position);
							drawLabels = true;


							float direction = Vector3.Dot(Selection.activeGameObject.transform.forward, targetComponent.target.forward);

							if (direction < 0)
							{
								SetupWireArc(true, Selection.activeGameObject.transform.position, 
									Vector3.Cross( -targetComponent.target.transform.forward, Selection.activeGameObject.transform.forward), 
									-targetComponent.target.forward, angle, arcRadius.floatValue);
							}
							else
							{
								SetupWireArc(true, Selection.activeGameObject.transform.position, 
									Vector3.Cross(targetComponent.target.transform.forward, Selection.activeGameObject.transform.forward), 
									targetComponent.target.forward, angle, arcRadius.floatValue);
							}
							break;

						default:
							break;
					}
				}
				else
				{
					drawLabels = false;
					position = Vector3.zero;
					message = "null";
				}

				colorChoice.colorValue = EditorGUILayout.ColorField("Color:", targetComponent.color);

				break;


			case (int)Utilities.GizmoDrawHelper.DrawMode.Mesh:

				ResetLabels();

				EditorGUILayout.PropertyField( sMode );

				if (sMode.enumValueIndex == (int)Utilities.GizmoDrawHelper.ScaleMode.Explicit) 
				{
					scaleFactor.vector3Value = EditorGUILayout.Vector3Field("Scale Factor:", targetComponent.scale);
				}

				EditorGUILayout.PropertyField(mesh);

				drawLabels = false;

				colorChoice.colorValue = EditorGUILayout.ColorField("Color:", targetComponent.color);
				break;


			case (int)Utilities.GizmoDrawHelper.DrawMode.Icon:

				ResetLabels();

				EditorGUILayout.PropertyField(iconName);

				drawLabels = false;
				break;


			default:
				Debug.Log ("GizmosDrawInspector has found an unresolved case for draw mode");
				break;
		}


		//DrawPropertiesExcluding ( serializedObject, _dontIncludeMe );

		serializedObject.ApplyModifiedProperties ();
	}


	//Text Render:
	bool drawLabels = false;
	Vector3 position = Vector3.zero;
	string message = "null";

	bool drawWireArc = false;
	Vector3 center = Vector3.zero;
	Vector3 normal = Vector3.zero;
	Vector3 fromPos = Vector3.zero;
	float arcAngle = 0f;
	float radius = 1f;

	void ResetLabels()
	{
		drawLabels = false;
		position = Vector3.zero;
		message = "null";
	}

	void ResetWireArc()
	{
		SetupWireArc(false, Vector3.zero, Vector3.zero, Vector3.zero, 0f, 1f);
	}

	void SetupWireArc( bool shouldDraw, Vector3 centerOfArc, Vector3 normalDir, Vector3 startPos, float arcAmount, float aRadius )
	{
		drawWireArc = shouldDraw;
		center = centerOfArc;
		normal = normalDir;
		fromPos = startPos;
		arcAngle = arcAmount;
		radius = aRadius;
	}

	void OnSceneGUI()
	{
		if (drawLabels)
		{
			Handles.Label(position, message);

			if (drawWireArc)
			{
				Handles.color = colorChoice.colorValue;
				Handles.DrawWireArc(center, normal, fromPos, arcAngle, radius);
			}
		}
	}

}
