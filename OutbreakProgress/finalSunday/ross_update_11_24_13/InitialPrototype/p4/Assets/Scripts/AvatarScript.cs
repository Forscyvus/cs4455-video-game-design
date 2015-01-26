using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]

public class AvatarScript : MonoBehaviour {
	// Movement
    public float moveSpeed = 6.0f;
    public float jumpSpeed = 120.0f;
	public float gravity = 10.0f;
	public float zSpeed = 6.0f;
	public float drag = 0.5f;
	public AudioClip jumpSound;
	private Vector3 moveVector = Vector3.zero;
	
	// Animation stuff
	GameObject avatar;
		
	// Initial conditions
	Vector3 initialPosition;
	Quaternion initialRotation;
	
	// State machine
	private ImmediateStateMachine stateMachine = new ImmediateStateMachine ();
    float inputStrafe;
	
	// Controller stuff
	CharacterController controller;
	CollisionFlags collisionFlags;
	bool isGrounded;
	
	// Various state flags	
	bool turningLeft = false;
	bool turningRight = false;
	bool speedInc = false;

	void Start (){
		controller = gameObject.GetComponent<CharacterController>();
		
//		GameEventManager.GameStart += GameStart;
//		GameEventManager.GameOver += GameOver;
		
		initialPosition = transform.position;
		initialRotation = transform.rotation;
		
	 	avatar = GameObject.Find ("Sphere");
		
		gameObject.SetActive(false);	
	}
	
	void GameStart() {
		gameObject.SetActive(true);	
		
		// Restart, reposition
		switchToJumpFSM ();
		transform.position = initialPosition;
		transform.rotation = initialRotation;		
		
		collisionFlags = CollisionFlags.None;
		isGrounded = false;
		inputStrafe = 0f;
		
		moveVector = Vector3.zero;
	}
	
	void GameOver() {
		gameObject.SetActive(false);
	}
	
	//-----------------
	// Grounded State
	//-----------------
	void switchToGroundedFSM() {
		stateMachine.ChangeState (enterGROUNDED, updateGROUNDED, exitGROUNDED);
	}

	void enterGROUNDED() {
	}

	void updateGROUNDED() {
		if (!isGrounded) {
			switchToFallFSM();
			return;
		}
		
		// Handles 90-degree turns while grounded
		if (turningLeft) {
			transform.Rotate(0f, 270, 0f);
			turningLeft = false;
		}
		if (turningRight) {
			transform.Rotate(0f, 90, 0f);
			turningRight = false;
		}
		
		// Deal with movement, which in this case is limited to one axis
        moveVector = new Vector3(inputStrafe, -0.001f, zSpeed);
        moveVector = transform.TransformDirection(moveVector);
        moveVector *= moveSpeed;
		
		// User inputs a jump via the spacebar
		if (Input.GetButton("Jump")){
			switchToJumpFSM();
			return;
		}
	}

	void exitGROUNDED () {
	}
	
	//-----------------
	// Jump State
	//-----------------
	void switchToFallFSM() {
		stateMachine.ChangeState (enterFALL, updateJUMP, exitJUMP);
	}
	void switchToJumpFSM() {
		stateMachine.ChangeState (enterJUMP, updateJUMP, exitJUMP);
	}

	void enterFALL() {
	}

	void enterJUMP() {
		moveVector.y = jumpSpeed;
		audio.PlayOneShot(jumpSound);
	}

	void updateJUMP() {
		// Change states (if necessary)
		if (isGrounded) {
			switchToGroundedFSM();
			return;
		}
		
		// Account for drag and gravity in the avatar's movement
		Vector3 v = new Vector3(moveVector.x, 0f, moveVector.z);
		moveVector -= v * drag * Time.deltaTime;
		moveVector.y -= gravity * Time.deltaTime;		
	}
	
	void exitJUMP () {		
	}
	
	// ----------------
	// Main update loop
	// ----------------
	void Update() {
		
		// The avatar grows is size as it progresses through the level,
		// here temporarily handled by a simple check for position.
		if (avatar.transform.position.z > 100) {
			avatar.transform.localPosition = new Vector3(0f, 2f, 0f);
			avatar.transform.localScale = new Vector3(2f, 2f, 2f);
		}
		if (avatar.transform.position.z > 250) {
			avatar.transform.localPosition = new Vector3(0f, 3f, 0f);
			avatar.transform.localScale = new Vector3(3f, 3f, 3f);
		}
		if (avatar.transform.position.z > 350) {
			avatar.transform.localPosition = new Vector3(0f, 4f, 0f);
			avatar.transform.localScale = new Vector3(4f, 4f, 4f);
		}
		if (avatar.transform.position.z > 400) {
			avatar.transform.localPosition = new Vector3(0f, 9f, 0f);
			avatar.transform.localScale = new Vector3(9f, 9f, 9f);
			if (!speedInc) {
				moveSpeed *= 1.25f;
				speedInc = true;
			}
		}
		
		// The avatar will appear to be rolling while grounded
		if (isGrounded) avatar.transform.Rotate(30f, 0f, 0f);
		
		// Check to see if the user has decided to make a 90-degree turn
		if (Input.GetKeyDown (KeyCode.LeftArrow) && isGrounded) turningLeft = true;
		if (Input.GetKeyDown (KeyCode.RightArrow) && isGrounded) turningRight = true;
		
		// State machine stuff
        inputStrafe = Input.GetAxis("Horizontal");

		// Check to see if reset key has been pressed
        if (Input.GetKeyDown(KeyCode.R)) {
			avatar.transform.localScale = new Vector3(1f, 1f, 1f);
			speedInc = false;
			moveSpeed = 5.5f;
//			GameEventManager.TriggerGameOver();
//			GameEventManager.TriggerGameStart();
			return;
		}

		stateMachine.Execute();
		collisionFlags = controller.Move(moveVector * Time.deltaTime);	

		// Determine if the avatar is now grounded
		isGrounded = ((collisionFlags & CollisionFlags.CollidedBelow) != 0);
		
		if ((collisionFlags & CollisionFlags.CollidedAbove) != 0) {
			moveVector.y = -gravity * Time.deltaTime * 2f;
			moveVector.x /= 1.15f;
			moveVector.z /= 1.15f;
		}
    }
	
    void OnControllerColliderHit(ControllerColliderHit hit) {}
}