using UnityEngine;
using System.Collections;

public class mainmove : MonoBehaviour {
	
	
	public Transform player;
	public Transform camera;


	public Vector3 movementVector;
	public float movementAngle;
		
	public RaycastHit hit;
	public Vector3 mouse;
	public Vector3 fwd;
	
	public float fwdToMouse;
	public float movementSpeed;
	private float speedLimit;
	private float movementPercentageUpperBound;
	private float movementPercentage;
	
	public  float Y = 1.5f; // for raycasting
	public  float FWDACCEL = 10f;
	public  float BKACCEL = 5f;
	public  float SACCEL = 7.5f;
	public  float DFACCEL = 8.75f;
	public  float DBACCEL = 6.25f;
	public  float STOPTHRESH = 1f;
	public  float MAXSPEED = 5f;
	public  float DECELSPEED = .85f;
	public  float DECELTHRESH = .2f;
	
	private bool keyW, keyS, keyA, keyD;
	//private bool shooting;
	public Shooting shootingScript;
	
	private float shootingTimer;
	
	private Vector3 target;
	private Vector3 source;

	private float range;
	private bool startFlag;
	
	// Controller stuff
	CharacterController controller;
	CollisionFlags collisionFlags;

	// Sprinting variables
	public float stamina;
	public bool canSprint;
	public bool sprinting;
	private float staminaRegenTimer;
	private bool stamRecharging;
	public bool lookingForward;

	//Tripping variables
	public bool tripping;
	float trippingTimer;
	Vector3 adjust;
	Vector3 q;
	
	// Use this for initialization
	void Start () {
		controller = gameObject.GetComponent<CharacterController>();
		movementVector = Vector3.zero;
		movementSpeed = 0;
		speedLimit = 0;
		//shooting = false;
		startFlag = true;

		stamina = 2f;
		canSprint = true;
		sprinting = false;

		shootingScript = gameObject.GetComponent<Shooting>();
	}
	
	// Update is called once per frame
	void Update () {

		if (!startFlag){
			startFlag = Input.GetKey (KeyCode.Space);
		}
		
		if (startFlag) {

			if (Input.GetKeyDown (KeyCode.Backslash)){
				if (MAXSPEED == 5f) {
					MAXSPEED = 20f;
				} else {
					MAXSPEED = 5f;
				}
			}

			keyW = Input.GetKey (KeyCode.W);
			keyS = Input.GetKey (KeyCode.S);
			keyA = Input.GetKey (KeyCode.A);
			keyD = Input.GetKey (KeyCode.D);
			
			if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)){
				return;
			}
			
			mouse = new Vector3(hit.point.x, player.transform.localPosition.y, hit.point.z);
			//fwd = new Vector3(player.forward.x, Y, player.forward.z);
			
				/*
			if (Input.GetMouseButtonDown (0) && !shooting) {
			
				shooting = true;
				shootingTimer = .5f;
				target = mouse;
				source = player.transform.localPosition;
				range = Vector3.Magnitude (target - source);
			}
			
			if (shooting) {
				shootingTimer -= Time.deltaTime;
				if (shootingTimer < 0 ){
					print ("bang");
					shootingTimer = 0;
					shooting = false;
					print (player.transform.localPosition);
				}
				Vector3 temp = new Vector3(0.000000000001f* Time.deltaTime, 0f, 0f); //Used to detect collision with stationary Character Controllers
				controller.Move(temp);
			}
			*/	
			if(!sprinting && !tripping) {
				player.LookAt (mouse, new Vector3(0,Y,0));
			}
			if(shootingScript.shooting || tripping) {
				Vector3 temp = new Vector3(0.000000000001f* Time.deltaTime, 0f, 0f); //Used to detect collision with stationary Character Controllers
				controller.Move(temp);	
			}
			if (!tripping) {	

				if(!sprinting) {
					//player.LookAt (mouse, new Vector3(0,Y,0));
					/*
					if (keyW && !keyA && !keyS && !keyD) { // N
						movementSpeed += FWDACCEL * Time.deltaTime;
						movementVector = player.forward;
						speedLimit = MAXSPEED;
					}
					else if (keyW && keyA && !keyS && !keyD) { // NW
						movementSpeed += DFACCEL * Time.deltaTime;
						movementVector = Quaternion.Euler(0, -45, 0) * player.forward;
						speedLimit = MAXSPEED * (7f/8f);
					}
					else if (!keyW && keyA && !keyS && !keyD) { // W
						movementSpeed += SACCEL * Time.deltaTime;
						movementVector = Quaternion.Euler(0, -90, 0) * player.forward;
						speedLimit = MAXSPEED * (6f/8f);
					}
					else if (!keyW && keyA && keyS && !keyD) { // SW
						movementSpeed += DBACCEL * Time.deltaTime;
						movementVector = Quaternion.Euler(0, -135, 0) * player.forward;
						speedLimit = MAXSPEED * (5f/8f);					
					}
					else if (!keyW && !keyA && keyS && !keyD) { // S
						movementSpeed += BKACCEL * Time.deltaTime;
						movementVector = Quaternion.Euler(0, 180, 0) * player.forward;
						speedLimit = MAXSPEED * (4f/8f);					
					}
					else if (!keyW && !keyA && keyS && keyD) { // SE
						movementSpeed += DBACCEL * Time.deltaTime;
						movementVector = Quaternion.Euler(0, 135, 0) * player.forward;
						speedLimit = MAXSPEED * (5f/8f);					
					}
					else if (!keyW && !keyA && !keyS && keyD) { // E
						movementSpeed += SACCEL * Time.deltaTime;
						movementVector = Quaternion.Euler(0, 90, 0) * player.forward;
						speedLimit = MAXSPEED * (6f/8f);					
					}
					else if (keyW && !keyA && !keyS && keyD) { // NE
						movementSpeed += DFACCEL * Time.deltaTime;
						movementVector = Quaternion.Euler(0, 45, 0) * player.forward;
						speedLimit = MAXSPEED * (7f/8f);					
					}
					else {
						movementSpeed *= DECELSPEED;
						if (movementSpeed < DECELTHRESH){
							movementSpeed = 0;	
						}	
					}
					*/

					if (keyW && !keyA && !keyS && !keyD) { // N
						movementSpeed += FWDACCEL * Time.deltaTime;
						movementVector = Vector3.forward;
						movementAngle = Vector3.Angle(player.forward, movementVector);
						if(movementAngle == 0f) {
							movementAngle += .01f;
						}
						movementPercentageUpperBound = Mathf.Min (1f, 1.25f-(movementAngle / 180f));
						movementPercentage = Mathf.Max (.5f, movementPercentageUpperBound);
						speedLimit = MAXSPEED * movementPercentage;
					}
					else if (keyW && keyA && !keyS && !keyD) { // NW
						movementSpeed += FWDACCEL * Time.deltaTime;
						movementVector = Quaternion.Euler(0, -45, 0) * Vector3.forward;
						movementAngle = Vector3.Angle(player.forward, movementVector);
						if(movementAngle == 0f) {
							movementAngle += .01f;
						}
						movementPercentageUpperBound = Mathf.Min (1f, 1.25f-(movementAngle / 180f));
						movementPercentage = Mathf.Max (.5f, movementPercentageUpperBound);
						speedLimit = MAXSPEED * movementPercentage;
					}
					else if (!keyW && keyA && !keyS && !keyD) { // W
						movementSpeed += FWDACCEL * Time.deltaTime;
						movementVector = Quaternion.Euler(0, -90, 0) * Vector3.forward;
						movementAngle = Vector3.Angle(player.forward, movementVector);
						if(movementAngle == 0f) {
							movementAngle += .01f;
						}
						movementPercentageUpperBound = Mathf.Min (1f, 1.25f-(movementAngle / 180f));
						movementPercentage = Mathf.Max (.5f, movementPercentageUpperBound);
						speedLimit = MAXSPEED * movementPercentage;
					}
					else if (!keyW && keyA && keyS && !keyD) { // SW
						movementSpeed += FWDACCEL * Time.deltaTime;
						movementVector = Quaternion.Euler(0, -135, 0) * Vector3.forward;
						movementAngle = Vector3.Angle(player.forward, movementVector);
						if(movementAngle == 0f) {
							movementAngle += .01f;
						}
						movementPercentageUpperBound = Mathf.Min (1f, 1.25f-(movementAngle / 180f));
						movementPercentage = Mathf.Max (.5f, movementPercentageUpperBound);
						speedLimit = MAXSPEED * movementPercentage;
					}
					else if (!keyW && !keyA && keyS && !keyD) { // S
						movementSpeed += FWDACCEL * Time.deltaTime;
						movementVector = Vector3.back;
						movementAngle = Vector3.Angle(player.forward, movementVector);
						if(movementAngle == 0f) {
							movementAngle += .01f;
						}
						movementPercentageUpperBound = Mathf.Min (1f, 1.25f-(movementAngle / 180f));
						movementPercentage = Mathf.Max (.5f, movementPercentageUpperBound);
						speedLimit = MAXSPEED * movementPercentage;
					}
					else if (!keyW && !keyA && keyS && keyD) { // SE
						movementSpeed += FWDACCEL * Time.deltaTime;
						movementVector = Quaternion.Euler(0, 135, 0) * Vector3.forward;
						movementAngle = Vector3.Angle(player.forward, movementVector);
						if(movementAngle == 0f) {
							movementAngle += .01f;
						}
						movementPercentageUpperBound = Mathf.Min (1f, 1.25f-(movementAngle / 180f));
						movementPercentage = Mathf.Max (.5f, movementPercentageUpperBound);
						speedLimit = MAXSPEED * movementPercentage;
					}
					else if (!keyW && !keyA && !keyS && keyD) { // E
						movementSpeed += FWDACCEL * Time.deltaTime;
						movementVector = Quaternion.Euler(0, 90, 0) * Vector3.forward;
						movementAngle = Vector3.Angle(player.forward, movementVector);
						if(movementAngle == 0f) {
							movementAngle += .01f;
						}
						movementPercentageUpperBound = Mathf.Min (1f, 1.25f-(movementAngle / 180f));
						movementPercentage = Mathf.Max (.5f, movementPercentageUpperBound);
						speedLimit = MAXSPEED * movementPercentage;
					}
					else if (keyW && !keyA && !keyS && keyD) { // NE
						movementSpeed += FWDACCEL * Time.deltaTime;
						movementVector = Quaternion.Euler(0, 45, 0) * Vector3.forward;
						movementAngle = Vector3.Angle(player.forward, movementVector);
						if(movementAngle == 0f) {
							movementAngle += .01f;
						}
						movementPercentageUpperBound = Mathf.Min (1f, 1.25f-(movementAngle / 180f));
						movementPercentage = Mathf.Max (.5f, movementPercentageUpperBound);
						speedLimit = MAXSPEED * movementPercentage;
					}
					else {
						movementSpeed *= DECELSPEED;
						if (movementSpeed < DECELTHRESH){
							movementSpeed = 0;	
						}	
					}

					if(movementAngle >= 0f && movementAngle <= 20f) {
						lookingForward = true;
					}
					else {
						lookingForward = false;
					}
				}
				if(shootingScript.shooting) {
					speedLimit = MAXSPEED * .25f;
				}
				movementSpeed = Mathf.Min(movementSpeed, speedLimit);
				// Sprinting stuff
				if(canSprint && lookingForward) {
					if(Input.GetKey(KeyCode.LeftShift)) {
						stamina -= Time.deltaTime;
						movementSpeed *= 2f;
						sprinting = true;
						if(stamina <= 0f) {
							stamina = 0f;
							canSprint = false;
							sprinting = false;
						}
					}
					if((Input.GetKeyUp(KeyCode.LeftShift))) {
						sprinting = false;
						staminaRegenTimer = 1f;
						stamRecharging = true;
					}
				}
				else if(!canSprint) {
					stamina += Time.deltaTime;
					movementSpeed *= .93f;
					if(stamina >= 2f) {
						stamina = 2f;
						canSprint = true;
					}
				}
				if(!sprinting && stamRecharging) {
					staminaRegenTimer -= Time.deltaTime;
					if(staminaRegenTimer < 0f) {
						stamina += Time.deltaTime;
						if(stamina >= 2f) {
							stamina = 2f;
							stamRecharging = false;
						}
					}
				}
				movementVector = new Vector3(movementVector.x, 0, movementVector.z);
				movementVector.Normalize();
				movementVector *= movementSpeed * Time.deltaTime;
				movementVector = new Vector3(movementVector.x, -1f, movementVector.z);
				movementVector.x -= 0.000000000001f* Time.deltaTime; //Used to detect collision with stationary Character Controllers
				controller.Move(movementVector);
				
					
				/*if (keyW){
					
					movementSpeed += FWDACCEL * Time.deltaTime;
					
				} else if (keyS) {
					
					movementSpeed -= FWDACCEL * Time.deltaTime;
				
				} else {
					movementSpeed *= .75f;
					if (movementSpeed < .2f && movementSpeed > -.2f) {
						movementSpeed = 0;
					}
				}
				
				movementSpeed = Mathf.Min(movementSpeed, MAXSPEED);
				if (movementSpeed < 0) {
					movementSpeed = Mathf.Max(movementSpeed, -.5f * MAXSPEED);
				}
				
				//print(player.forward);
				
				//print(movementSpeed);
				
				movementVector = player.forward * movementSpeed * Time.deltaTime;
				Vector3 v = new Vector3 (0f, 0f, Input.GetAxis("Horizontal"));
				v.Normalize();
				movementVector += v * movementSpeed * Time.deltaTime;
					
				//print (movementVector);
				//player.transform.Translate ( movementVector);*/

			}

			if(tripping) {
				q = player.transform.localEulerAngles;
				trippingTimer -= Time.deltaTime;
				float r = 2 * Time.deltaTime * 45;

				if (Vector3.Dot(adjust, Vector3.right) < 1.5 && Vector3.Dot(adjust, Vector3.right) > 0.5) {
					// look up, hit left
					if (q.y > 45 && q.y < 135)
						player.transform.Rotate((-1)*r,0,0);
					// look right, hit left
					else if ((q.y > 0 && q.y < 45) || (q.y > 315 && q.y < 360))
						player.transform.Rotate(0,0,r);
					// look down, hit left
					else if (q.y > 225 && q.y < 315)
						player.transform.Rotate(r,0,0);
					// look left, hit left
					else if (q.y > 135 && q.y < 225)
						player.transform.Rotate(0,0,(-1)*r);
				}
				else if (Vector3.Dot(adjust, Vector3.forward) < 1.5 && Vector3.Dot(adjust, Vector3.forward) > 0.5) {
					// look right, hit down
					if (q.y > 45 && q.y < 135)
						player.transform.Rotate(0,0,(-1)*r);
					// look up, hit down
					else if ((q.y > 0 && q.y < 45) || (q.y > 315 && q.y < 360))
						player.transform.Rotate((-1)*r,0,0);
					// look left, hit down
					else if (q.y > 225 && q.y < 315)
						player.transform.Rotate(0,0,r);
					// look down, hit down
					else if (q.y > 135 && q.y < 225)
						player.transform.Rotate(r,0,0);
				}
				else if (Vector3.Dot(adjust, Vector3.left) < 1.5 && Vector3.Dot(adjust, Vector3.left) > 0.5) {
					// look right, hit right
					if (q.y > 45 && q.y < 135)
						player.transform.Rotate(r,0,0);
					// look up, hit right
					else if ((q.y > 0 && q.y < 45) || (q.y > 315 && q.y < 360))
						player.transform.Rotate(0,0,(-1)*r);
					// look left, hit right
					else if (q.y > 225 && q.y < 315)
						player.transform.Rotate((-1)*r,0,0);
					// look down, hit right
					else if (q.y > 135 && q.y < 225)
						player.transform.Rotate(0,0,r);
				}
				else if (Vector3.Dot(adjust, Vector3.back) < 1.5 && Vector3.Dot(adjust, Vector3.back) > 0.5) {
					// look right, hit up
					if (q.y > 45 && q.y < 135)
						player.transform.Rotate(0,0,r);
					// look up, hit up
					else if ((q.y > 0 && q.y < 45) || (q.y > 315 && q.y < 360))
						player.transform.Rotate(r,0,0);
					// look left, hit up
					else if (q.y > 225 && q.y < 315)
						player.transform.Rotate(0,0,(-1)*r);
					// look down, hit up
					else if (q.y > 135 && q.y < 225)
						player.transform.Rotate((-1)*r,0,0);
				}

				if(trippingTimer < 0f) {
					player.transform.Rotate(q);
					player.transform.Translate(adjust.x,1f,adjust.z,Space.World);
					tripping = false;
				}
			}
			
		
		}
		//controller.Move (new Vector3(1,-.1f,0));
		camera.transform.localPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y + 33, player.transform.localPosition.z);
	}

	void OnControllerColliderHit (ControllerColliderHit hit) {
		if (hit.gameObject.name == "Trip" && !tripping) {
			adjust = hit.normal;
			tripping = true;
			trippingTimer = 1f;
		}
	}
}
