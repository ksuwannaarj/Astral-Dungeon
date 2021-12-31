using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour {
	private Canvas CanvasObject;

	void Start() {
		CanvasObject = GameObject.Find("Canvas2").GetComponent<Canvas>();
		CanvasObject.enabled = !CanvasObject.enabled;
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			CanvasObject.enabled = !CanvasObject.enabled;
			if (Time.timeScale == 1) {
				Time.timeScale = 0;
			}
			else if (Time.timeScale == 0) {
				Time.timeScale = 1;
			}
		}
	}
}
