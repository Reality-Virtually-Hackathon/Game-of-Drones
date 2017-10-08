using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MergeCube.MergeCubeSDK))]
public class MergeCubeSDKInspector : Editor
{
	//asdasda 
	private static readonly string[] _dontIncludeMe = new string[]{ "m_Script" };

	SerializedProperty viewMode;
	SerializedProperty useMergeUserAccount;
	SerializedProperty userAccountDebugMode;
	SerializedProperty cubeConfiguration;
	SerializedProperty isUsingMergeReticle;
	void OnEnable()
	{
		//viewMode = serializedObject.FindProperty("viewMode");	
		useMergeUserAccount = serializedObject.FindProperty("useMergeUserAccount");	
		userAccountDebugMode = serializedObject.FindProperty("userAccountDebugMode");
		isUsingMergeReticle = serializedObject.FindProperty("isUsingMergeReticle");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update ();
		EditorGUILayout.Space();
//		EditorGUILayout.PropertyField(viewMode);
		EditorGUILayout.PropertyField(useMergeUserAccount);
		EditorGUILayout.PropertyField(userAccountDebugMode);
		EditorGUILayout.PropertyField(isUsingMergeReticle);
		EditorGUILayout.Space();

		serializedObject.ApplyModifiedProperties ();
	}
}
