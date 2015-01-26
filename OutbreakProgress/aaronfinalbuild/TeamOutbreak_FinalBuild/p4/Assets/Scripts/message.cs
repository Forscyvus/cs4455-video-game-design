using UnityEngine;
using System.Collections;

public class message : MonoBehaviour {

	private bool active; 
	public Light ls;
	// Use this for initialization
	void Start () {
		active = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (active) {
			float rand = Random.value;
			if(rand < .05f) {
				ls.intensity = 7f;
			}
			else {
				ls.intensity = 8f;
			}
		} else {
			ls.intensity = 0f;
		}
	}

	void OnTriggerEnter(Collider c){
		if (!active){
			active = true;
		}
	}
}
