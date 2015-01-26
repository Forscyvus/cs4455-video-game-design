using UnityEngine;
using System.Collections;

public class FadeIn : MonoBehaviour {
	float fadeInTimer;
	float fadeIntensity;
	Color fontColor;
	
	float fadeInInsTimer;
	
	float fadeOutTimer;
	bool starting;
	
	AudioSource wind;
	float volume;
	void Start() {
		fadeInTimer = 4.5f;
		fadeIntensity = .005f;
		
		fadeInInsTimer = 2f;
		
		fontColor = gameObject.guiText.color;
		fontColor.a = 0f;
		gameObject.guiText.color = fontColor;
		
		starting = false;
		
		if(gameObject.name == "Title") {
			AudioSource[] sound = gameObject.GetComponents<AudioSource>();
			wind = sound[0];
		}
	}
	
	void Update() {
		if(gameObject.name == "Title") {
			if(fadeInTimer >= 0f) {
				fadeInTimer -= Time.deltaTime;
				fontColor = gameObject.guiText.color;
				fontColor.a += fadeIntensity;
				if(fontColor.a >= 1f) {
					fontColor.a = 1f;
				}
				gameObject.guiText.color = fontColor;
			}
		}
		
		else if(gameObject.name == "Instructions") {
			if(fadeInInsTimer >= 0f) {
				fadeInInsTimer -= Time.deltaTime;
			}
			else {
				if(fadeInTimer >= 0f) {
					fadeInTimer -= Time.deltaTime;
					fontColor = gameObject.guiText.color;
					fontColor.a += fadeIntensity;
					if(fontColor.a >= 1f) {
						fontColor.a = 1f;
					}
					gameObject.guiText.color = fontColor;
				}
		
			
			}
		}
		
		if(Input.GetKeyDown(KeyCode.Space)) {
			fadeOutTimer = 1.5f;
			starting = true;
    	}
		
		if(starting) {
			if(fadeOutTimer >= 0f) {
				fadeOutTimer -= Time.deltaTime;
				fontColor = gameObject.guiText.color;
				fontColor.a -= fadeIntensity*2f;
				gameObject.guiText.color = fontColor;
				
				if(gameObject.name == "Title") {	
					wind.volume -= fadeIntensity*2f;
				}
				
			}
			else {
        		Application.LoadLevel(1);
			}
		}
	}
}
