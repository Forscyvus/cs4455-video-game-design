using UnityEngine;
using System.Collections;

public class zombie : MonoBehaviour {
	
	public Transform zed;

	public Transform player;
	
	public Vector3 movementVector;
	
	public static float SPEED = 3.5f;
	
	//private bool startFlag;
	
	int state;
	float alertDistShoot;
	float alertDistSight;
	int weapon;
	bool fired;
	float shootingTimer;
	CharacterController controller;
	AudioSource source;

	float groantimer;
	bool groaned;
	
	// Use this for initialization
	void Start () {
		groaned = false;
		groantimer = -1;
		movementVector = Vector3.zero;
		//startFlag = true;
		state = 0;
		alertDistShoot = 40f;
		alertDistSight = 15f;
		weapon = 1;
		fired = false;
		controller = gameObject.GetComponent<CharacterController>();
		source = (AudioSource)gameObject.GetComponent("AudioSource");
	}

	void equipGun() {
		weapon = 0;
	}
	void equipBow(){
		weapon = 1;
	}
	// Update is called once per frame
	void Update () {
		
		if(!(Input.GetMouseButton(0))) {
			if(Input.GetKeyDown(KeyCode.Alpha1)) { 
				//weapon = 0;
			}
			if(Input.GetKeyDown(KeyCode.Alpha2)) {
				//weapon = 1;
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

		if (!groaned && state == 1) {
			if (groantimer > 0 ) {
				groantimer -= Time.deltaTime;
			} else {
				source.Play ();
				groaned = true;
			}
		}
		
	}

	void activate(){
		if (state != 1){
			state = 1;
			groantimer = Random.value * .5f; 
			GameObject[] zeds = GameObject.FindGameObjectsWithTag ("zed");
			foreach(GameObject otherzed in zeds){
				Vector3 difference = otherzed.transform.position - gameObject.transform.position;
				if (difference.magnitude > .5f){
					float distance = difference.magnitude;
					if (distance < 7f){
						//Destroy (zed);
						//zombie z = zed.GetComponent (zombie);
						//zombie.activate();
						otherzed.SendMessage ("activate");
					}
				}
			}
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
