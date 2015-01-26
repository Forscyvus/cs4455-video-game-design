using UnityEngine;
using System.Collections;

public class mainmove : MonoBehaviour {
	
	
	public Transform player;
	public Transform camera;
	
	public Vector3 movementVector;
		
	public RaycastHit hit;
	public Vector3 mouse;
	public Vector3 fwd;
	
	public float fwdToMouse;
	public float movementSpeed;
	private float speedLimit;
	
	public static float Y = 1.5f; // for raycasting
	public static float FWDACCEL = 10f;
	public static float BKACCEL = 5f;
	public static float SACCEL = 7.5f;
	public static float DFACCEL = 8.75f;
	public static float DBACCEL = 6.25f;
	public static float STOPTHRESH = 1f;
	public static float MAXSPEED = 5f;
	public static float DECELSPEED = .85f;
	public static float DECELTHRESH = .2f;
	
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
	}
	
	// Update is called once per frame
	void Update () {
		
		if (!startFlag){
			startFlag = Input.GetKey (KeyCode.Space);
		}
		
		if (startFlag) {
		
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
			
		if(shootingScript.shooting) {
			Vector3 temp = new Vector3(0.000000000001f* Time.deltaTime, 0f, 0f); //Used to detect collision with stationary Character Controllers
			controller.Move(temp);	
		}
		if (!shootingScript.shooting) {	

			if(!sprinting) {
				player.LookAt (mouse, new Vector3(0,Y,0));
				
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
			}
			else {
				if (keyW && !keyA && !keyS && !keyD) { // N
					movementSpeed += FWDACCEL * Time.deltaTime;
					movementVector = player.forward;
					speedLimit = MAXSPEED;
				}
			}
			movementSpeed = Mathf.Min(movementSpeed, speedLimit);
			// Sprinting stuff
			if(canSprint) {
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
				if(!(Input.GetKey(KeyCode.LeftShift))) {
					sprinting = false;
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
		
		
		}
		//controller.Move (new Vector3(1,-.1f,0));
		camera.transform.localPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y + 33, player.transform.localPosition.z);
	
	}
	
}
