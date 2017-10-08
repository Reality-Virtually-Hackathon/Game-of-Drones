using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AR.Drone.Video;

public class DroneVideoDisplay : MonoBehaviour {

	private Texture2D tex;
	private Color32[] colors;

	bool hasNewFrame;
	VideoFrame frame;
	object frameLock = new object();

	// Use this for initialization
	void Start () {
		DroneImpulseController.instance.drone.onVideo += HandleVideoFrame;
	}

	void Update() {
		lock (frameLock) {
			if (hasNewFrame) {
				if (tex == null) {
					tex = new Texture2D (frame.Width, frame.Height,TextureFormat.RGB24,false);
					colors = new Color32[frame.Width * frame.Height];
					GetComponent<Renderer> ().material.mainTexture = tex;
				}

				if (frame.PixelFormat == PixelFormat.BGR24) {
					for (int i = 0; i < frame.Data.Length / 3; i++) {
						int offset = i * 3;
						colors [i] = new Color32 (frame.Data [i + 2], frame.Data [i + 1], frame.Data [i],1);
					}
				}

				tex.SetPixels32 (colors);
				tex.Apply ();
			}
		}
	}

	void HandleVideoFrame(VideoFrame frame){
		Debug.Log ("Handling video frame");
		lock (frameLock) {
			hasNewFrame = true;
			this.frame = frame;
		}
	}
}
