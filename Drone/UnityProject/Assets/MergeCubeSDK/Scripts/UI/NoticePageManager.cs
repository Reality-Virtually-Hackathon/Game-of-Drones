using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeCube{
	public class NoticePageManager : MonoBehaviour {
		public Callback doneButton;
		public Callback actionButton;

		[Header("Dismissed when done button click.")]
		[Tooltip("Should page being dismissed when done button click.")]
		public bool doneDissmiss = true;

		[Header("Dismissed when action button click.")]
		[Tooltip("Should page being dismissed when action button click.")]
		public bool actionDissmiss = false;

		[Header("Done button only click once.")]
		[Tooltip("Done button can only be clicked once.")]
		public bool doneOnce = true;
		bool isDelay = false;
		// Use this for initialization
		void Start () {
			
		}
		void DelayReset(){
			isDelay = false;
		}
		public void Btn_DoneClick(){
			if (isDelay) {
				return;
			}
			isDelay = true;
			Invoke ("DelayReset", 1f);
			if (doneOnce) {
				UnityEngine.UI.Button [] allTp = GetComponentsInChildren<UnityEngine.UI.Button> (true);
				foreach (UnityEngine.UI.Button tp in allTp) {
					tp.interactable = false;
				}
			}
			if (doneButton != null) {
				doneButton.Invoke ();
			}
			if (doneDissmiss) {
				DismissPage ();
			}
		}
		public void Btn_ActionClick(){
			if (actionButton != null) {
				actionButton.Invoke ();
			}
			if (actionDissmiss) {
				DismissPage ();
			}
		}
		public void DismissPage(){
			gameObject.SetActive (false);
		}
		public void OpenURL(string url){
			Application.OpenURL (url);
		}
		public void Die(){
			Destroy (gameObject);
		}
	}
}
