using UnityEngine;
using System.Collections;

public class zombie : MonoBehaviour {
	
	public Transform zed;

	public Transform player;
	
	public Vector3 movementVector;
	
	public static float SPEED = 3f;
	
	//private bool startFlag;
	
	int state;
	float alertDistShoot;
	float alertDistSight;
	int weapon;
	bool fired;
	float shootingTimer;
	CharacterController controller;
	
	// Use this for initialization
	void Start () {
		movementVector = Vector3.zero;
		//startFlag = true;
		state = 0;
		alertDistShoot = 40f;
		alertDistSight = 15f;
		weapon = 0;
		fired = false;
		controller = gameObject.GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(!(Input.GetMouseButton(0))) {
			if(Input.GetKeyDown(KeyCode.Alpha1)) { 
				weapon = 0;
			}
			if(Input.GetKeyDown(KeyCode.Alpha2)) {
				weapon = 1;
			}
		}

		Vector3 offset = player.position - zed.position;
		float sqLength = offset.sqrMagnitude;
		// Check after rifle shot
		if (weapon == 0) {
			if(Input.GetMouseButtonDown (0) && (sqLength < alertDistShoot * alertDistShoot)) {
				fired = true;
				//shootingTimer = .5f;
			}
			if(fired && (sqLength < alertDistShoot * alertDistShoot) && Input.GetMouseButtonUp(0)) {
				shootingTimer -= Time.deltaTime;
				if (shootingTimer < 0){
					shootingTimer = 0;
					activate ();
					fired = false;
				}
			}
		}
		// Check for visual proximity to zed 
		if(sqLength < alertDistSight * alertDistSight) {
			activate ();
		}
		
	}

	void activate(){
		if (state != 1){
			state = 1;
			AudioSource source = (AudioSource)gameObject.GetComponent("AudioSource");
			source.Play ();
		}
	}

	void FixedUpdate() {
		if(state == 1) {
			movementVector = player.transform.localPosition-zed.transform.localPosition;
			movementVector.Normalize();
			movementVector *= SPEED * Time.deltaTime;
			//zed.rigidbody.MovePosition(zed.transform.position + movementVector);
			controller.Move (movementVector);


		}
	}
}
