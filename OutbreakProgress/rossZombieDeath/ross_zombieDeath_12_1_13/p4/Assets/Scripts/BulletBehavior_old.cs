using UnityEngine;
using System.Collections;

public class BulletBehavior_old: MonoBehaviour {
	float speed;
	float gravity;

	void Start () {
		speed = 100f;
		gravity = -2f;
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.Translate(0f, gravity * Time.deltaTime, speed * Time.deltaTime, transform);
	}

	void OnCollisionEnter(Collision collision) {
		Destroy(this.gameObject);
	}
}
