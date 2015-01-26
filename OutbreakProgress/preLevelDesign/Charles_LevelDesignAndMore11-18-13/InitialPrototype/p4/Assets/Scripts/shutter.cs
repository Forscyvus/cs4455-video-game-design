using UnityEngine;
using System.Collections;

public class shutter : MonoBehaviour {


	bool triggered = false;
	AudioSource source;
	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider c){
		if (!triggered){
			print("test");
			Transform t = gameObject.transform.parent.gameObject.transform;
			t.Translate (new Vector3(0, -10f, 0));
			triggered = true;
			//source.Play ();


			GameObject[] zeds = GameObject.FindGameObjectsWithTag ("zed");
			foreach(GameObject zed in zeds){
				Vector3 difference = zed.transform.position - gameObject.transform.position;
				float distance = difference.magnitude;
				if (distance < 30){
					//Destroy (zed);
					//zombie z = zed.GetComponent (zombie);
					//zombie.activate();
					zed.SendMessage ("activate");
				}
			}
		}
	}
}
