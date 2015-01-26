using UnityEngine;
using System.Collections;

public class zombie : MonoBehaviour {
	
	public Transform zed;

	public Transform player;
	
	public static Vector3 movementVector;
	
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

	bool dying;
	Vector3 adjust;
	
	// Use this for initialization
	void Start () {
		groaned = false;
		dying = false;
		adjust = Vector3.zero;
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

	void stop () {
		SPEED = 0f;
	}

	void equipGun() {
		weapon = 0;
	}
	void equipBow(){
		weapon = 1;
	}
	// Update is called once per frame
	void Update () {

		if (dying){
			//die ();
		}
		
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

	void die() {
		float death = 1f;
		//if (dying) {
			Vector3 q = zed.transform.localEulerAngles;
			death -= Time.deltaTime;
			float r = 2 * Time.deltaTime * 45;
			
			if (Vector3.Dot(adjust, Vector3.right) < 1.5 && Vector3.Dot(adjust, Vector3.right) > 0.5) {
				// look up, hit left
				if (q.y > 45 && q.y < 135)
					zed.transform.Rotate((-1)*r,0,0);
				// look right, hit left
				else if ((q.y > 0 && q.y < 45) || (q.y > 315 && q.y < 360))
					zed.transform.Rotate(0,0,r);
				// look down, hit left
				else if (q.y > 225 && q.y < 315)
					zed.transform.Rotate(r,0,0);
				// look left, hit left
				else if (q.y > 135 && q.y < 225)
					zed.transform.Rotate(0,0,(-1)*r);
			}
			else if (Vector3.Dot(adjust, Vector3.forward) < 1.5 && Vector3.Dot(adjust, Vector3.forward) > 0.5) {
				// look right, hit down
				if (q.y > 45 && q.y < 135)
					zed.transform.Rotate(0,0,(-1)*r);
				// look up, hit down
				else if ((q.y > 0 && q.y < 45) || (q.y > 315 && q.y < 360))
					zed.transform.Rotate((-1)*r,0,0);
				// look left, hit down
				else if (q.y > 225 && q.y < 315)
					zed.transform.Rotate(0,0,r);
				// look down, hit down
				else if (q.y > 135 && q.y < 225)
					zed.transform.Rotate(r,0,0);
			}
			else if (Vector3.Dot(adjust, Vector3.left) < 1.5 && Vector3.Dot(adjust, Vector3.left) > 0.5) {
				// look right, hit right
				if (q.y > 45 && q.y < 135)
					zed.transform.Rotate(r,0,0);
				// look up, hit right
				else if ((q.y > 0 && q.y < 45) || (q.y > 315 && q.y < 360))
					zed.transform.Rotate(0,0,(-1)*r);
				// look left, hit right
				else if (q.y > 225 && q.y < 315)
					zed.transform.Rotate((-1)*r,0,0);
				// look down, hit right
				else if (q.y > 135 && q.y < 225)
					zed.transform.Rotate(0,0,r);
			}
			else if (Vector3.Dot(adjust, Vector3.back) < 1.5 && Vector3.Dot(adjust, Vector3.back) > 0.5) {
				// look right, hit up
				if (q.y > 45 && q.y < 135)
					zed.transform.Rotate(0,0,r);
				// look up, hit up
				else if ((q.y > 0 && q.y < 45) || (q.y > 315 && q.y < 360))
					zed.transform.Rotate(r,0,0);
				// look left, hit up
				else if (q.y > 225 && q.y < 315)
					zed.transform.Rotate(0,0,(-1)*r);
				// look down, hit up
				else if (q.y > 135 && q.y < 225)
					zed.transform.Rotate((-1)*r,0,0);
			}

			if (death < 0) dying = false;
		//}
	}

	void OnControllerColliderHit (ControllerColliderHit hit) {
		if (hit.gameObject.name == "bulletClone" || hit.gameObject.name == "arrowClone") {
			adjust = hit.normal;
			//dying = true;
		}
	}


	void getKilled(){
		//dying = true;
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
