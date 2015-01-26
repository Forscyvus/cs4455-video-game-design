using UnityEngine;
using System.Collections;

public class DeathText_old: MonoBehaviour {
	float fadeInTimer;
	float fadeIntensity;
	Color fontColor;
	
	
	void Start() {
		fadeInTimer = 2f;
		fadeIntensity = .005f;
		
		fontColor = gameObject.guiText.color;
		fontColor.a = 0f;
		gameObject.guiText.color = fontColor;
	}
	
	void Update() {
		if(fadeInTimer >= 0f) {
			fadeInTimer -= Time.deltaTime;
			fontColor = gameObject.guiText.color;
			fontColor.a += fadeIntensity;
			if(fontColor.a >= 1f) {
				fontColor.a = 1f;
			}
			gameObject.guiText.color = fontColor;
		}
		
		if(Input.GetKeyDown(KeyCode.R)) {
			Application.LoadLevel(1);
    	}
	}
}