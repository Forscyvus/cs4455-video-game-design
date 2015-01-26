using UnityEngine;
using System.Collections;

public class TitleLight : MonoBehaviour {

	float intensity;
	void Update () {
		if(gameObject.name == "Fire1") {
			intensity = 1f;
		}
		else if(gameObject.name == "Fire2") {
			intensity = 2f;
		}
		gameObject.light.intensity = Mathf.PerlinNoise(Time.time * intensity, 0.0F);
	}
}
