using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]

public class move : MonoBehaviour {
	
	public CharacterController controller;
	public Transform camera;
	public Transform player;
	public Transform quathack;
	
	public AudioSource whistle;
	
	public AudioSource woo;
	
	public Animation anim;
	
	public Vector3 moveDirection;	public Vector3 horizontalDirection;
	public float horizontalSpeed;
	ImmediateStateMachine state;
	
	public static float GRAVITY = 20f;
	public static float MINJUMPHEIGHT = 12f;
	public static float TERMINALVELOCITY = -10f;
	public static float AIRACCEL = 10f;
	public static float MAXAIRCONTROL = 15f;
	public static float MAXGROUNDSPEED = 7f;
	public static float GROUNDTURNSPEED = 100f;
	public static float AIRTURNSPEED = 75f;
	public static float DRAG = 1.5f;
	
	public RaycastHit hit;
	
	public bool jumpheld;
	
	public float swingtimer;
	
	
	void enterJUMP(){
		print ("enter jump state");
		Vector3 temp = new Vector3(moveDirection.x, 0, moveDirection.z);
		//temp = Vector3.ClampMagnitude(temp, MAXAIRCONTROL);
		moveDirection.x = temp.x;
		moveDirection.z = temp.z;
		print (moveDirection);
		swingtimer = 0f;
		
	}
	void updateJUMP(){
		
		swingtimer += Time.deltaTime;
		
		horizontalDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
		horizontalDirection -= Time.deltaTime * DRAG * horizontalDirection;
		
		if (Input.GetKey (KeyCode.W)){
			if (horizontalDirection.magnitude < MAXAIRCONTROL){
				horizontalDirection += player.forward * Time.deltaTime * AIRACCEL;
			}
			//horizontalDirection = Vector3.ClampMagnitude(horizontalDirection, MAXAIRCONTROL);
		} else if (Input.GetKey (KeyCode.S)){
			if (horizontalDirection.magnitude < MAXAIRCONTROL){
				horizontalDirection += player.forward * Time.deltaTime * -AIRACCEL;
			}
			//horizontalDirection = Vector3.ClampMagnitude(horizontalDirection, MAXAIRCONTROL);
		}
		moveDirection.x = horizontalDirection.x;
		moveDirection.z = horizontalDirection.z;
		
		if(Input.GetKey (KeyCode.A)){
			player.Rotate(new Vector3 (0,1,0), -AIRTURNSPEED * Time.deltaTime );
		}
		if(Input.GetKey (KeyCode.D)){
			player.Rotate(new Vector3 (0,1,0), AIRTURNSPEED * Time.deltaTime );
		}
		
		//apply gravity and terminal velocity
		moveDirection.y -= GRAVITY * Time.deltaTime;
		moveDirection.y = Mathf.Max (TERMINALVELOCITY, moveDirection.y);
		
		//move the dude
		controller.Move (moveDirection * Time.deltaTime);

		//land
		if ((controller.collisionFlags & CollisionFlags.Below) != 0) {
			state.ChangeState (enterGROUND, updateGROUND, exitGROUND);
			return;
		}
	}
	void exitJUMP(){
	}
	
	void enterGROUND(){
		print ("enter ground state");
		moveDirection = Vector3.zero;
		jumpheld = true;
	}
	void updateGROUND(){
		if (Input.GetKey (KeyCode.W)){
			moveDirection = player.forward * MAXGROUNDSPEED;
			//moveDirection = Vector3.ClampMagnitude (moveDirection, MAXGROUNDSPEED);
			anim.Play ("run");
		} else if (Input.GetKey (KeyCode.S)){
			moveDirection = player.forward * -MAXGROUNDSPEED/2;
			//moveDirection = Vector3.ClampMagnitude (moveDirection, MAXGROUNDSPEED);
		} else {
			moveDirection = Vector3.zero;
			anim["run"].time = 0;
			anim.Stop ();
			anim.Play ("idle");
		}
		if(Input.GetKey (KeyCode.A)){
			player.Rotate(new Vector3 (0,1,0), -GROUNDTURNSPEED * Time.deltaTime * (Input.GetKey (KeyCode.W)?1:2));
		}
		if(Input.GetKey (KeyCode.D)){
			player.Rotate(new Vector3 (0,1,0), GROUNDTURNSPEED * Time.deltaTime * (Input.GetKey (KeyCode.W)?1:2));
		}
		if(Input.GetKey(KeyCode.Space) && !jumpheld) {
			moveDirection.y = MINJUMPHEIGHT;
			anim.Stop ();
			anim.Play ("jump");
			woo.Play();
			print (moveDirection);
			state.ChangeState (enterJUMP, updateJUMP, exitJUMP);
			return;
		}
		if (!Input.GetKey(KeyCode.Space)){
			jumpheld = false;
		}
		//HACK ALERT
		moveDirection.y = TERMINALVELOCITY;
		controller.Move (moveDirection * Time.deltaTime);
	}
	void exitGROUND(){
	}
	
	void enterSWING(){
		print ("enter swing state");
		anim.Stop ();
		anim.Play ("swing");
		jumpheld = true;
	}
	void updateSWING(){
		if(Input.GetKey(KeyCode.Space) && !jumpheld ) {
			anim.Stop ();
			anim.Play ("flip");
			moveDirection = player.forward * MAXGROUNDSPEED * 3f;
			moveDirection.y = MINJUMPHEIGHT;
			print (moveDirection);
			woo.Play ();
			whistle.Play ();
			state.ChangeState (enterJUMP, updateJUMP, exitJUMP);
			return;
		}
		if (!Input.GetKey(KeyCode.Space)){
			jumpheld = false;
		}
	}
	void exitSWING(){
	}
	
	void OnTriggerEnter(Collider coll) {
		if (state.IsInState(updateJUMP) && swingtimer > .1f) {

			Transform colltrans = coll.gameObject.transform;
			Vector3 branchpos = new Vector3(colltrans.localPosition.x, colltrans.localPosition.y, colltrans.localPosition.z);
			Vector3 newy = colltrans.TransformPoint(branchpos);
			Vector3 temp = player.transform.localPosition;
			temp.y = branchpos.y;
			//controller.Move (player.localPosition - branchpos);
			player.localPosition = branchpos;
			player.Translate (new Vector3(0,-1f,0));
			//player.Rotate (new Vector3(0,1,0), Vector3.Angle (player.forward, coll.transform.right) * (player.localRotation.y > 90 ? -1:1));
			//HACKTASTIC STUFF HERE
			player.rotation = Quaternion.RotateTowards (player.rotation, quathack.rotation, 360);
			
			state.ChangeState (enterSWING, updateSWING, exitSWING);
		}
	}
	
	// Use this for initialization
	void Start () {
		controller = gameObject.GetComponent<CharacterController>(); 
		moveDirection = Vector3.zero;
		horizontalDirection = Vector3.zero;
		
		state = new ImmediateStateMachine();
		state.ChangeState(enterGROUND, updateGROUND, exitGROUND);
	}
	
	// Update is called once per frame
	void Update () {
		state.Execute();
		//Vector3 forwardVec = player.transform.forward;
		//camera.localPosition = new Vector3(forwardVec.x*-10, player.transform.localPosition.y, forwardVec.z * -10);
	}
}
