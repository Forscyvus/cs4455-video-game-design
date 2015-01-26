using UnityEngine;
using System.Collections;

public class Lighting : MonoBehaviour {

	void Update () {
		if(this.name == "Spotlight") {
			float rand = Random.value;
			if(rand < .05f) {
				this.light.intensity = 1.2f;
			}
			else {
				this.light.intensity = 1.5f;
			}
		}
		
		if(this.name == "Point light") {
			float rand = Random.value;
			if(rand < .5f) {
				if(!(this.transform.localEulerAngles.x < 12f)) {
					this.transform.Rotate(-Vector3.right * Time.deltaTime * 10f);
				}

				/*
				if(!(this.transform.localEulerAngles.y - 360f < -3f) || !(this.transform.localEulerAngles.y < 3f)) {
					this.transform.Rotate(-Vector3.up * Time.deltaTime * 10f);
				}
				*/
			}
			else {
				if(!(this.transform.localEulerAngles.x > 19f)) {
					this.transform.Rotate(Vector3.right * Time.deltaTime * 10f);
				}
	
				/*
				if(!(this.transform.localEulerAngles.y > 3f)) {
					this.transform.Rotate(Vector3.up * Time.deltaTime * 10f);
				}
				*/
			}
		}
	}
}
