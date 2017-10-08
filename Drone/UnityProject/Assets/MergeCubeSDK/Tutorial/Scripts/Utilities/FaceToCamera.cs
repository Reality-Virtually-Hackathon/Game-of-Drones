using UnityEngine;
using System.Collections;

public static class ExtensionMethods
{
	//Even though they are used like normal methods, extension
	//methods must be declared static. Notice that the first
	//parameter has the 'this' keyword followed by a Transform
	//variable. This variable denotes which class the extension
	//method becomes a part of.
	public static void FaceToCamera(this Transform trans, Transform toMatch)
	{
		int comboX = -1;
		int comboY1 = -1;
		int comboY2 = 0;
		float dis = 1000f;
		float dis2 = 1000f;
		Vector3 cameraPosition = Camera.main.transform.position;
		Vector3[] toCompair = new Vector3[checkPoints.Length];
		Vector3 frontPos =  new Vector3 (cameraPosition.x,trans.position.y,cameraPosition.z);
		Vector3 topPos =  new Vector3 (trans.position.x,trans.position.y+10f,trans.position.z);

		for(int i=0;i<checkPoints.Length;i++){
			toCompair [i] = trans.TransformPoint( checkPoints [i] );
			float disTp = Vector3.Distance (frontPos, toCompair[i]);
			if (dis > disTp) {
				dis = disTp;
				comboX = i;
			}
			disTp = Vector3.Distance (topPos, toCompair[i]);
			if (dis2 > disTp) {
				dis2 = disTp;
				comboY2 = comboY1;
				comboY1 = i;
			}
		}

		if (!SetRotation (comboX*10 + comboY1,toMatch)) {
			SetRotation (comboX*10 + comboY2,toMatch);
		}
	}
	static Vector3 [] checkPoints = new [] {  new Vector3(0f,0f,-0.1f), new Vector3(-0.1f,0f,0f), new Vector3(0f,0f,0.1f),
		new Vector3(0.1f,0f,0f), new Vector3(0f,0.1f,0f), new Vector3(0f,-0.1f,0f) };
	

	static bool SetRotation(int combo, Transform toSet){
		Vector3 setValue = Vector3.zero;
		switch (combo) {
		case 04:
			break;
		case 01:
			setValue.z = 90f;
			break;
		case 05:
			setValue.z = 180f;
			break;
		case 03:
			setValue.z = 270f;
			break;

		case 14:
			setValue.y = 90f;
			break;
		case 12:
			setValue.y = 90f;
			setValue.z = 90f;
			break;
		case 15:
			setValue.y = 90f;
			setValue.z = 180f;
			break;
		case 10:
			setValue.y = 90f;
			setValue.z = 270f;
			break;


		case 24:
			setValue.y = 180f;
			break;
		case 23:
			setValue.y = 180f;
			setValue.z = 90f;
			break;
		case 25:
			setValue.y = 180f;
			setValue.z = 180f;
			break;
		case 21:
			setValue.y = 180f;
			setValue.z = 270f;
			break;

		case 34:
			setValue.y = 270f;
			break;
		case 30:
			setValue.y = 270f;
			setValue.z = 90f;
			break;
		case 35:
			setValue.y = 270f;
			setValue.z = 180f;
			break;
		case 32:
			setValue.y = 270f;
			setValue.z = 270f;
			break;

		case 42:
			setValue.x = 90f;
			break;
		case 41:
			setValue.x = 90f;
			setValue.z = 90f;
			break;
		case 40:
			setValue.x = 90f;
			setValue.z = 180f;
			break;
		case 43:
			setValue.x = 90f;
			setValue.z = 270f;
			break;

		case 50:
			setValue.x = 270f;
			break;
		case 51:
			setValue.x = 270f;
			setValue.z = 90f;
			break;
		case 52:
			setValue.x = 270f;
			setValue.z = 180f;
			break;
		case 53:
			setValue.x = 270f;
			setValue.z = 270f;
			break;

		default:
			return false;
		}
		toSet.localEulerAngles = setValue;
		return true;
	}
}