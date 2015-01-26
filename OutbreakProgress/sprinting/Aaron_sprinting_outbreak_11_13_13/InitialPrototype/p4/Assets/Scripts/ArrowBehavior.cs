using UnityEngine;
using System.Collections;

public class ArrowBehavior : MonoBehaviour {
	GameObject floor;
	
	float speed;
	float gravity;
	Vector3 velocity;
	
	bool inFlight;
	
	void Start () {
		floor = GameObject.Find("Floor");
		
		speed = 20f;
		gravity = -1f;
		
		inFlight = true;
	}
	
	void Update () {
		if(inFlight) {
			gameObject.transform.Translate(0f, gravity * Time.deltaTime, speed * Time.deltaTime, transform);
		}
	}
	
	void OnCollisionEnter(Collision collision) {
		if(collision.collider.gameObject.name == "Floor") {
			gameObject.transform.Translate(0f, floor.transform.position.y + .05f, 0f);
		}
		
		if(collision.collider.gameObject.name == "zombie" && inFlight) {
			Destroy (collision.collider.gameObject);
			gameObject.transform.Translate(0f, floor.transform.position.y + .05f, 0f);
		}
		inFlight = false;
	}
}
