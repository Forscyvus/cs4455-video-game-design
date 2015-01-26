using UnityEngine;
using System.Collections;

public class BulletBehavior : MonoBehaviour {
	float speed;
	float gravity;
	float deleteTimer;

	void Start () {
		speed = 250f;
		gravity = -2f;
		deleteTimer = .2f;
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.Translate(0f, gravity * Time.deltaTime, speed * Time.deltaTime, transform);
		deleteTimer -= Time.deltaTime;
		if(deleteTimer < 0f) {
			Destroy (gameObject);
		}
	}
	
	void OnCollisionEnter(Collision collision) {
		Destroy(this.gameObject);
	}
}
