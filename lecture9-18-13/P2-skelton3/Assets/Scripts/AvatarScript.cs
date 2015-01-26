using UnityEngine;
using System.Collections;

/// <summary>
/// PLEASE NOTE:  This is ONLY an example of how to use the CharController Update and 
/// OnControllerColliderHit methods for this example.
/// 
/// It is NOT meant to be an example of the kinds of avatar behavior you should aim for
/// in P1.  The motion of the avatar is INTENTIONALLY bad, serving only to show that you 
/// can really just use whatever inputs you want to control the motion of the transforms.
/// 
/// We recommend that instead of using simple checks on the values of the last moveDirection
/// that you create a simple state machine (storing the state in a private class variable)
/// and base your motion computations in each state with what you want that state to feel like.   
/// For the Mario example in the book, states might include STILL, MOVELEFT, MOVERIGHT,  
/// JUMP, STARTFALL, ENDFALL, LANDED.  A state might be started based on key input (press space),
/// time (first .1 seconds of fall after apex reached), or collision (hit ground, hit wall, hit
/// platform from below).
/// 
/// Think through your states, and how you can tell when they start, and how you compute their 
/// motion during the state.
/// 
/// Remember that Update is called frequently, and you should use Time.deltaTime to see how long 
/// its been since the last update.  By computing your desired motions based on time (e.g., I want
/// the character to move XXX far during the first 0.1 seconds) and then monitoring how long 
/// it's been in a state (e.g., adding Time.deltaTime to a counter that you initialize when you 
/// enter a state), you can create motion that behaves correctly no matter how fast it runs.
/// 
/// </summary>
/// 
public class AvatarScript : MonoBehaviour {
	// movement property references
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
	public float gravity = 10.0F;
		
	// could get these from the collider, but wanted to be able to change them
	public float cHeight = 4.5f; 
	public float cWidth = 2f; 
	// a small delta for making sure the avatar remains in contact with the object it collided with
	public float cDelta = 0.01f;
	
	// our movement vector
	private Vector3 moveDirection = Vector3.zero;

	// Animation references:  if you want to control the kids and do something with them
	int counter = 0;
	GameObject head;
	GameObject body;
	
	// a simple start to Character controller capabilities
	bool bumpTop;
	bool bumpLeft;
	bool bumpRight;
	bool bumpBottom;
	bool isGrounded = false;
	// "depth" of the collider into the object collided with.  Assume it's only in the 4 principle directions 
	float leftD, rightD, topD, bottomD;
	// need to move the clearing of variables out of fixedUpdate since fixedUpdate appears
	// to be called more often then collisions and collisions seem NOT to happen every
	// frame!??
	bool initController = false;
	float initY;
	float jumpTime;
	
	int jumps = 0;
	
	// state machine
	private ImmediateStateMachine stateMachine = new ImmediateStateMachine ();

	// public access to increment boost count
	void AddJump(){
		jumps += 1;
		GUIManager.SetJumps(jumps);
	}

	void Start (){
		stateMachine.ChangeState (enterGROUNDED, updateGROUNDED, exitGROUNDED);
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
		
     	// Assign body parts to variables;  
		// -> could also have these as properties you set in editor
		// -> could also have used Transform.Find to only search in the children of this object
	 	head = GameObject.Find ("Head");
     	body = GameObject.Find ("Body");
		
		gameObject.SetActive(false);	
	}
	
	void enterGROUNDED() {
	}
	void updateGROUNDED() {
	}
	void exitGROUNDED () {
	}
	
	
	void GameStart() {
		counter = 0;
		bumpTop = bumpLeft = bumpRight = bumpBottom = false;
		leftD = rightD = topD = bottomD = 0f;
		isGrounded = false;
		initController = false;
		initY = 0;
		jumpTime = 0;
		gameObject.SetActive(true);	
	}
	
	void GameOver() {
		// won't be called
		gameObject.SetActive(false);
	}
	
	// do you application logic, managing states and so on, in here.  These examples have no explicit
	// states, but you should consider adding some to keep the code better organized
	void Update() {
		// we only do input if on the ground. If you want to do left/right movement in the air, you 
		// need to deal with it differently because you can't just reset the vector (you need to 
		// add the input to the vector, as you do gravity)
        if (isGrounded) {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
			
			// move up off the ground by adding an upward impulse
			if (Input.GetButton("Jump")){
				AddJump();
                moveDirection.y = jumpSpeed;
				body.transform.Rotate(new Vector3(0,0,0));    
				initY = transform.position.y;
				jumpTime = Time.time + 0.1f;
			}
        } else {
			Vector3 vec = new Vector3(Input.GetAxis("Horizontal") + moveDirection.x * speed * -0.001f, 0f, 0f);
			moveDirection += transform.TransformDirection(vec) * speed * 0.01f;
		}
		
		// some rediculous random variables
		float delta = (Mathf.Sin(counter*0.2F)/2)+1.37118F;
		float delta2 = (Mathf.Sin(counter*0.2F));

		// if we've bumped the top, and are moving upwards, stop the upward movement
		// also, move down "almost" out of whatever we colided with
		if (bumpTop && moveDirection.y > 0)
		{
			moveDirection.y = 0f;
			transform.Translate(new Vector3(0f, -topD + cDelta, 0f));				
		}
		
		// if we've bumped the left or right, and are moving in that direction, stop the movement
		// also, move "almost" out of whatever we colided with
		if (bumpLeft && moveDirection.x < 0)
		{
			moveDirection.x = 0f;
			transform.Translate(new Vector3(leftD - cDelta, 0f, 0f));							
		} else if (bumpRight && moveDirection.x > 0)
		{
			moveDirection.x = 0f;
			transform.Translate(new Vector3(-rightD + cDelta, 0f, 0f));							
		}

		// if we are moving up (jumping) do silly things with the head object.  If we are moving down,
		// do something different, yet also silly.  When we are walking, do something different, yet
		// just as silly
		if (isGrounded) 
		{ 
			Debug.Log ("GROUNDED");
			head.transform.localPosition = new Vector3 (0F,0f,0F);
			if (moveDirection.x > 0 ) { 	
				body.transform.localRotation = Quaternion.Euler (0,0,-10);
			} else if (moveDirection.x < 0) {
				body.transform.localRotation = Quaternion.Euler (0,0,10);
			} else {
				body.transform.localRotation = Quaternion.Euler (0,0,0);
			}
		} else {
			Debug.Log ("NOT GROUNDED");
			if(moveDirection.y > 1) {
				// Jumping 
				head.transform.localPosition = new Vector3(0F,1f,0F);
				if (jumpTime > Time.time) {
					body.transform.localPosition = new Vector3(0F, (initY - transform.position.y), 0f);
				} else {
					body.transform.localPosition = new Vector3(0F, 0f, 0f);
				}
				body.transform.localRotation = Quaternion.Euler (0,0,0);
			} else if(moveDirection.y < -0.35) {
				// Falling
				body.transform.localPosition = new Vector3(0F, 0f, 0f);
				if (moveDirection.x > 0) {
					head.transform.localPosition = new Vector3(1f,0f,0F);
					body.transform.localRotation = Quaternion.Euler (0,0,20f);
				} else {
					head.transform.localPosition = new Vector3(-1f,0f,0F);
					body.transform.localRotation = Quaternion.Euler (0,0,-20f);
				}
		    } 
		}

		// if we're on the ground, and are "inside" whatever we are on, move "almost" out.  If we are 
		// in the air, apply some gravity
		if (isGrounded) 
		{
			// if below ground
			if (bottomD > 0)
			{
				transform.Translate(new Vector3(0f, bottomD - cDelta, 0f));				
			}
		} else {
			if (moveDirection.y < 0 && moveDirection.y > -1.0)
				moveDirection.y -= gravity * Time.deltaTime * 10f;
			else
				moveDirection.y -= gravity * Time.deltaTime;
		} 

		// after all the movement is computed ... move!
		transform.Translate(moveDirection * Time.deltaTime);
		counter++;
    }
	
    void FixedUpdate() {
		initController = true;
	}
	
	void InitController() {
		// reinitialize for checking for collisions.  FixedUpdate is called BEFORE any collisions.
		bumpTop = bumpLeft = bumpRight = bumpBottom = false;
		leftD = rightD = topD = bottomD = 0f;
		isGrounded = false;
		initController = false;
		Debug.Log ("Init Controller");
	}

	// a simple function that sets the left/right/top/bottom based on a single collision contact point.
	// the function also returns a boolean, indicating if we are "grounded", so that we can call the 
	// function from collisionStay as well as collisionEnter
	bool checkContactPoint (ContactPoint c)
	{			
		float dotUp = Vector3.Dot (c.normal, Vector3.up);
		float dotLeft = Vector3.Dot (c.normal, Vector3.left);
		Vector3 pt = transform.InverseTransformPoint(c.point);
		float ydiff = cHeight - Mathf.Abs (pt.y); 
		float xdiff = cWidth - Mathf.Abs (pt.x);
				
		//Debug.Log ("dots: " + dotUp + " " + dotLeft);
		if (dotUp < -0.5) {
			if (ydiff > topD) 
				topD = ydiff;
			bumpTop = true;
			Debug.Log ("Bumped with Top");
		}
		else if (dotUp > 0.5)
		{
			if (ydiff > bottomD)
				bottomD = ydiff;
			bumpBottom = true;
			Debug.Log ("Bumped with Bottom");
		}
		
		if (dotLeft > 0.5) 
		{
			if (xdiff > rightD)
				rightD = xdiff;
			bumpRight = true;
			Debug.Log ("Bumped with Right");
		}
		else if (dotLeft < -0.5)
		{
			if (xdiff > leftD)
				leftD = xdiff;
			bumpLeft = true;
			Debug.Log ("Bumped with Left");
		}
		
		// return if it's hit the bottom so we can check for grounded below
		return (dotUp > 0.5);
	}
	
	// Collision handling.  Update global variables for use in state machines.
	// DO NOT do any of the application logic associated with states here.  Just compute the 
	// various results of collisions, so that they can be used in Update once all the collisions 
	// are processed
	void OnCollisionEnter (Collision collision) {
        //Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        //Vector3 pos = contact.point;

		// see if this is the first time this is called for this loop through the 
		// collision routines
		if (initController) InitController();

		foreach (ContactPoint c in collision.contacts) {
            Debug.Log(c.thisCollider.name + " COLLIDES WITH " + c.otherCollider.name);
            Debug.Log("Collision: " + transform.InverseTransformPoint(c.point) + ", Normal: " + c.normal);
    		if (checkContactPoint(c))
			{
				isGrounded = true;			
				Debug.Log ("Collision Enter GROUNDED");
			}
		}
	}

	void OnCollisionStay (Collision collision) {
		// see if this is the first time this is called for this loop through the 
		// collision routines
		if (initController) InitController();

		foreach (ContactPoint c in collision.contacts) {
            Debug.Log(c.thisCollider.name + " STAY ON " + c.otherCollider.name);
            Debug.Log("Collision: " + transform.InverseTransformPoint(c.point) + ", Normal: " + c.normal);
    		if (checkContactPoint(c))
			{
				isGrounded = true;			
				Debug.Log ("Collision Stay GROUNDED");
			}
        }
	}
	
	void OnCollisionExit () {
		// see if this is the first time this is called for this loop through the 
		// collision routines
		if (initController) InitController();

		Debug.Log ("Collision Exit");
		
		// technically, there could be multiple simultaneous collisions (e.g., in a corner), so we should 
		// keep track of which ones are ending here
		isGrounded = false;
	}
}