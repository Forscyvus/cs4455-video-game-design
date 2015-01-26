using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]

public class AvatarScript : MonoBehaviour {
	// movement property references
    public float moveSpeed = 6.0F;
	public float rotateSpeed = 5.0f;
    public float jumpSpeed = 8.0F;
	public float gravity = 10.0F;
	public float windSlow = 0.2f;
	
	// two curves for attack/decay
	public AnimationCurve groundAttack;
	public AnimationCurve groundDecay;
	
	// our movement vector
	private Vector3 moveVector = Vector3.zero;
	//private Vector3 moveDirection;
	
	// Animation references:  if you want to control the kids and do something with them
	GameObject head;
	GameObject body;
		
	// initial position
	Vector3 initialPosition;
	Quaternion initialRotation;
	
	// number of jumps	
	int jumps = 0;
	
	// state machine:  not using it yet
	private ImmediateStateMachine stateMachine = new ImmediateStateMachine ();

	// variables for our state machine
    float inputRotate;
    float inputMove;
    Vector3 horizontalVelocity;
    float horizontalSpeed;  
    float verticalSpeed;
	float initY;
	float jumpTime;

	// for our input modulation
	bool fwdDown;
	float fwdPress;
	float fwdRelease;

	bool backDown;
	float backPress;
	float backRelease;
	
	// our character's controller
	CharacterController controller;
	CollisionFlags collisionFlags;
	bool isGrounded;
	
	// another way of doing some small debugging;  just put some GUI text boxes up and fill them in as needed
	GUIText debugText;
	string dbgTxt = "";
	
	// public access to increment boost count
	void AddJump(){
		jumps += 1;
		GUIManager.SetJumps(jumps);
	}

	void Start (){
		// must have both!
		controller = gameObject.GetComponent<CharacterController>(); 
		debugText = GameObject.Find("debugText").guiText;
		
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
		
		initialPosition = transform.position;
		initialRotation = transform.rotation;
			
     	// Assign body parts to variables;  
		// -> could also have these as properties you set in editor
		// -> could also have used Transform.Find to only search in the children of this object
	 	head = GameObject.Find ("Head");
     	body = GameObject.Find ("Body");
		
		// so we can go outside the bounds
		groundAttack.preWrapMode = WrapMode.ClampForever;
		groundAttack.postWrapMode = WrapMode.ClampForever;
		groundDecay.preWrapMode = WrapMode.ClampForever;
		groundDecay.postWrapMode = WrapMode.ClampForever;
		
		gameObject.SetActive(false);	
	}
	
	void GameStart() {
		initY = 0;
		jumpTime = 0;
		gameObject.SetActive(true);	
		
		// restart, reposition things
		switchToJumpFSM ();
		transform.position = initialPosition;
		transform.rotation = initialRotation;		
		
		collisionFlags = CollisionFlags.None;
		isGrounded = false;
		inputRotate = 0f;
        inputMove = 0f;
       	horizontalVelocity = Vector3.zero;
        horizontalSpeed = 0f;
        verticalSpeed = 0f;

		fwdPress = 0f;
		fwdRelease = 0f;
		backPress = 0f;
		backRelease = 0f;
		fwdDown = false;
		backDown = false;
		
		moveVector = Vector3.zero;
	}
	
	void GameOver() {
		// won't be called
		gameObject.SetActive(false);
	}
	
	//-----------------
	// GROUNDED state
	//-----------------
	void switchToGroundedFSM() {
		stateMachine.ChangeState (enterGROUNDED, updateGROUNDED, exitGROUNDED);
	}

	void enterGROUNDED() {
	}

	void updateGROUNDED() {
		// if we walked off the edge, start falling!
		if (!isGrounded)
		{
			switchToFallFSM();
			return;
		}

		// we only do input if on the ground. If you want to do left/right movement in the air, you 
		// need to deal with it differently because you can't just reset the vector (you need to 
		// add the input to the vector, as you do gravity)

		// rotate based on input
		transform.Rotate(0f, inputRotate * rotateSpeed, 0f);
		
		// create a forward/backward movement vector, then transform it into world coordinates based on our viewing direction 
		// (include a little downward motion to keep us on the ground)
        moveVector = new Vector3(0f, -0.001f, inputMove);
        moveVector = transform.TransformDirection(moveVector);
        moveVector *= moveSpeed;
		
		// move up off the ground by adding an upward impulse
		if (Input.GetButton("Jump")){
			AddJump();
			switchToJumpFSM();
			return;
		}
		
		// When we are walking, do something interesting that shows which is the direction of travel				
		if (horizontalSpeed > 0.1 ) { 	
			body.transform.localRotation = Quaternion.Euler (10,0,0);
		} else if (horizontalSpeed < -0.1) {
			body.transform.localRotation = Quaternion.Euler (-10,0,0);
		} else {
			body.transform.localRotation = Quaternion.Euler (0,0,0);
		}		
	}

	void exitGROUNDED () {
		// reset the avatar values we mucked with to known values
		body.transform.localRotation = Quaternion.Euler (0,0,0);
	}
	
	//-----------------
	// JUMP State
	//-----------------
	void switchToFallFSM() {
		stateMachine.ChangeState (enterFALL, updateJUMP, exitJUMP);
	}
	void switchToJumpFSM() {
		stateMachine.ChangeState (enterJUMP, updateJUMP, exitJUMP);
	}

	void enterFALL() {  
		initY = 0f;
		jumpTime = 0f;
	}

	void enterJUMP() {  
		moveVector.y = jumpSpeed;
		initY = transform.position.y;
		jumpTime = Time.time + 0.1f;
	}

	void updateJUMP() {
		// change states if need be
		if (isGrounded) {
			switchToGroundedFSM();
			return;
		}
			
		// no rotation while jumping.  transform.Rotate(0f, inputRotate * rotateSpeed / 2f, 0f);
        Vector3 vec = new Vector3(0f, 0f, inputMove * moveSpeed);
		vec = transform.TransformDirection(vec);
		moveVector +=  vec * Time.deltaTime;
		
		// slow the movement vector down a bit (wind resistance?) in the x/z plane
		vec = new Vector3(moveVector.x, 0f, moveVector.z);
		moveVector -= vec * windSlow * Time.deltaTime;
			
		// if we're on the ground, and are "inside" whatever we are on, move "almost" out.  If we are 
		// in the air, apply some gravity
		moveVector.y -= gravity * Time.deltaTime;		
		
		// if we are moving up (jumping) do silly things with the head object.  If we are moving down,
		// do something different, yet also silly.  When we are walking, do something different, yet

		if(verticalSpeed > 1) {
			// Jumping 
			head.transform.localPosition = new Vector3(0F,1f,0F);
			head.transform.localRotation = Quaternion.Euler (0f, 0f, 0f);
			if (jumpTime > Time.time) {
				body.transform.localPosition = new Vector3(0F, (initY - transform.position.y), 0f);
			} else {
				body.transform.localPosition = new Vector3(0F, 0f, 0f);
			}
			body.transform.localRotation = Quaternion.Euler (0,0,0);
		} else if(verticalSpeed < -0.35) {
			// Falling
			body.transform.localPosition = new Vector3(0F, 0f, 0f);
			if (horizontalSpeed > 0.01f) {
				head.transform.localPosition = new Vector3(0f,0f,1F);
				head.transform.localRotation = Quaternion.Euler (0f, 0f, 0f);
				body.transform.localRotation = Quaternion.Euler (-20,0,0f);
			} else if (horizontalSpeed < -0.01f) {
				head.transform.localPosition = new Vector3(-0.5f,0f,0F);
				head.transform.localRotation = Quaternion.Euler (0f, 110f, 0f);
				body.transform.localRotation = Quaternion.Euler (-5,0,0f);
			} else {
				head.transform.localPosition = new Vector3(0f,0f,0F);
				head.transform.localRotation = Quaternion.Euler (0f, 0f, 0f);
				body.transform.localRotation = Quaternion.Euler (0,0,0f);
			}
		} 
	}
	
	void exitJUMP () {		
		// reset the avatar to a known configuration
		head.transform.localPosition = new Vector3 (0F,0f,0F);
		head.transform.localRotation = Quaternion.Euler (0f, 0f, 0f);
		body.transform.localPosition = new Vector3 (0F,0f,0F);
		body.transform.localRotation = Quaternion.Euler (0,0,0);
	}
	
	// ----------------
	// Main update loop!
	void Update() {
		dbgTxt = "";
		dbgTxt += "velocity: " + controller.velocity + "\n";
		dbgTxt += "flags: " + controller.collisionFlags + "\n";

		// set some globals used by the state machine!
        inputRotate = Input.GetAxis("Horizontal");
		
		float rawInput = Input.GetAxis("Vertical");
		inputMove = 0f;
		if (fwdDown && rawInput <= 0f) {
			fwdDown = false;
			fwdPress = 0f;
			fwdRelease = Time.time;
		}
		else if (!fwdDown && rawInput > 0f) {
			fwdDown = true;
			fwdPress = Time.time;
			fwdRelease = 0f;
		}
		if (backDown && rawInput >= 0f) {
			backDown = false;
			backPress = 0f;
			backRelease = Time.time;
		}
		else if (!backDown && rawInput < 0f) {
			backDown = true;
			backPress = Time.time;
			backRelease = 0f;
		}
		if (fwdDown) 
		{
			float val = groundAttack.Evaluate(Time.time - fwdPress);
			if (val < 0.001) val = 0f; 
			inputMove += val;
			dbgTxt += "fwdDown: " + val + " ";
		}
		else
		{
			float val = groundDecay.Evaluate(Time.time - fwdRelease);
			if (val < 0.001) val = 0f; 
			inputMove += val;
			dbgTxt += "fwdUp: " + val + " ";
		}
		if (backDown)
		{
			float val = groundAttack.Evaluate(Time.time - backPress);
			if (val < 0.001) val = 0f; 
			inputMove -= val;
			dbgTxt += "backDown: " + val + " ";
		}
		else
		{
			float val = groundDecay.Evaluate(Time.time - backRelease);
			if (val < 0.001) val = 0f; 
			inputMove -= val;
			dbgTxt += "backUp: " + val + " ";
		}
		if (inputMove > 1f) inputMove = 1f;			
		if (inputMove < -1f) inputMove = -1f;
		dbgTxt += "inputMove: " + inputMove + "\n";
			
       	horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
		dbgTxt += "hv: " + horizontalVelocity + "\n";
		horizontalVelocity = transform.InverseTransformDirection(horizontalVelocity);
		dbgTxt += "xformed hv: " + horizontalVelocity + "\n";
        horizontalSpeed = horizontalVelocity.z;  // want the plus or minus on speed
        verticalSpeed = controller.velocity.y;
        //float overallSpeed = controller.velocity.magnitude;
		
		// check for restart
        if (Input.GetKeyDown(KeyCode.R)) 
		{
			GameEventManager.TriggerGameOver();
			GameEventManager.TriggerGameStart();
			return;
		}

		stateMachine.Execute();
				
		dbgTxt += "move: " + moveVector + "\n";
		dbgTxt += "xformed mv: " + transform.InverseTransformDirection(moveVector) + "\n";
		
		// move, and adjust speeds based on collisions.  Need to do this to avoid the horrible sliding motions
		// that the Controller does otherwise
		collisionFlags = controller.Move(moveVector * Time.deltaTime);	

		// did our last move result in "grounding"
		isGrounded = ((collisionFlags & CollisionFlags.CollidedBelow) != 0);
			
		if ((collisionFlags & CollisionFlags.CollidedSides) != 0)
		{
			// keep it moving the same direction but at a VERY small rate (so the collision stays consistently on if the player
			// is pushing in that direction)
			moveVector.x /= 100.0f;
			moveVector.z /= 100.0f;
			moveVector.y /= 2.0f;  // slow down the vertical movement 
		}
		
		if ((collisionFlags & CollisionFlags.CollidedAbove) != 0)
		{
			// start moving down immediately by a little.  Ouch, my head!
			moveVector.y = -gravity * Time.deltaTime * 2f;
			moveVector.x /= 1.15f;  // slow down sideways movement
			moveVector.z /= 1.15f;  // slow down sideways movement
		}
		
		// update our collected debugText
		debugText.text = dbgTxt;
    }
	
    void OnControllerColliderHit(ControllerColliderHit hit) {
		// here you would put things related to specific collisions
    }
}