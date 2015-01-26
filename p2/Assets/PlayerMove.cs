using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMove : MonoBehaviour {

	
	public static float LIGHTGRAVITYACCEL = 30f;
	public static float LIGHTRUNSPEED = 10f;
	public static float LANDINGRESETTHRESH = -.2f;
	public static float MAXLIGHTFALLSPEED = 20f;
	public static float PLAYERBOTTOMOFFSET = 1.25f;
	public static float PLAYERTOPOFFSET = .75f;
	public static float PLAYERRADIUS = .25f;
	public static float MINLIGHTJUMP = 10f;
	public static float JUMPLIMIT = .2f;
	public static float WALLJUMPTIMER = 0;
	public static float LIGHTAIRACCEL = .4f;
	public static float MAXAIRCONTROL = .15f;
	public static float UNLATCHTIMER = .1f;
	
	public static float LANDSPEEDTHRESH = 19.5f;
	
	public static float HEAVYGRAVITYACCEL = 75f;
	public static float MAXHEAVYFALLSPEED = 75f;
	public static float HEAVYAIRACCEL = .2f;
	public static float MAXHEAVYAIRCONTROL = .07f;
	public static float HEAVYRUNACCEL = .1f;
	public static float MAXHEAVYRUNSPEED = .1f;
	public static float HEAVYFRICTION = .1f;
	public static float MINJUMPSTRENGTH = 20f;
	public static float JUMPSTRENGTHCHARGESPEED = 20f;
	public static float MAXJUMPSTRENGTH = 50f;
	
	
	public Transform player;
	public Transform camera;
	public Transform truecamera;
	
	public AudioSource heavyCharge;
	public AudioSource heavyJump;
	public AudioSource heavyLand;
	public AudioSource heavyRun;
	public AudioSource heavyTurbo;
	public AudioSource lightJump;
	public AudioSource lightLand;
	public AudioSource lightRunStart;
	public AudioSource lightSteps;
	public AudioSource lightWallJump;
	public AudioSource ninja;
	public AudioSource robot;
	
	public float xVel;
	public float yVel;
	public string wall;
	public float walljumpcounter;
	public float walljumptimeout;
	public float jumpstrength;
	public float jumphold;
	public float walljumptimer;
	public bool spaceheld;
	public float unlatchtimer;
	public bool turbo;
	public float landcount;
	
	public RaycastHit hit;
 
	// Defines the required parameters and return value type for a StateMachine state.
	public delegate void ImmediateStateDelegate ();
	 
	// To use, create an instance of StateMachine inside of a MonoBehaviour, load it up with
	// references to state methods with ChangeState(), then call its Execute() method during the
	// MonoBehaviour's Update cycle. An example MonoBehaviour is included at the bottom of this file.
	
	// LIGHT GROUND STATE
	void enterLightGround (){
		Debug.Log ("enterLightGround");
		spaceheld = true;
	}
	void updateLightGround (){
		if (Input.GetKey (KeyCode.Alpha2)){
			state.ChangeState (enterHeavyGround, updateHeavyGround, exitHeavyGround);
			robot.Play();
			return;
		}
		
		if(xVel != 0 && !lightSteps.isPlaying) {
			lightSteps.Play();
		}
		if(xVel == 0 && lightSteps.isPlaying) {
			lightSteps.Stop();
		}
		
		
		if (Input.GetKey (KeyCode.RightArrow) ){
			if(xVel <= 0){
				lightRunStart.Play();
			}
			xVel = LIGHTRUNSPEED * Time.deltaTime;		
		}
		else if (Input.GetKey (KeyCode.LeftArrow) ){
			if(xVel >= 0){
				lightRunStart.Play();
			}
			xVel = -LIGHTRUNSPEED * Time.deltaTime;		
		}
		else {
			xVel = 0;
		}
		
		if (Input.GetKey (KeyCode.Space) && !spaceheld){
			yVel = MINLIGHTJUMP;
			state.ChangeState (enterLightJump, updateLightJump, exitLightJump);
			lightJump.Play();
			return;
			
		} 
		if (!Input.GetKey(KeyCode.Space)){
			spaceheld = false;
		}
			
		
		if (!Physics.Raycast (player.position, new Vector3(0,-1,0),  Mathf.Abs (1) + PLAYERBOTTOMOFFSET )){
			state.ChangeState (enterLightJump, updateLightJump, exitLightJump);
			return;
		}
		
		if (Physics.Raycast (player.position, new Vector3(xVel, 0, 0), out hit, Mathf.Abs (xVel * Time.deltaTime) + PLAYERRADIUS)){
			if (xVel < 0) {
				player.position = hit.point + new Vector3(PLAYERRADIUS, 0, 0);
			}
			else {
				player.position = hit.point - new Vector3(PLAYERRADIUS, 0, 0);
			}
			xVel = 0;
		}
		else {
			player.Translate (new Vector3(xVel, 0, 0));
		}
		
		
		
	}
	void exitLightGround (){
		Debug.Log("exitLightGround");
		lightSteps.Stop ();
	}
	
	//LIGHT JUMP STATE
	void enterLightJump (){
		print ("enterLightJump");
	}
	
	void updateLightJump (){
		
		if (Input.GetKey (KeyCode.Alpha2)){
			state.ChangeState (enterHeavyJump, updateHeavyJump, exitHeavyJump);
			robot.Play ();
			return;
		}
		
		if (jumphold >= JUMPLIMIT || !Input.GetKey (KeyCode.Space)){
			yVel -= LIGHTGRAVITYACCEL * Time.deltaTime;
			yVel = Mathf.Max(yVel, -MAXLIGHTFALLSPEED);
			jumphold = JUMPLIMIT;
		}
		
		if (jumphold < JUMPLIMIT) {
			jumphold += Time.deltaTime;
		}
		
		
 		if (Input.GetKey (KeyCode.RightArrow)){
			xVel += LIGHTAIRACCEL * Time.deltaTime;		
		}
		if (Input.GetKey (KeyCode.LeftArrow)){
			xVel += -LIGHTAIRACCEL * Time.deltaTime;		
		}
		xVel = Mathf.Max (xVel, -MAXAIRCONTROL);
		xVel = Mathf.Min (xVel, MAXAIRCONTROL);
		
		if (Physics.Raycast (player.position, new Vector3(0,yVel,0), out hit,  Mathf.Abs (yVel * Time.deltaTime) + ((yVel>0)?PLAYERTOPOFFSET:PLAYERBOTTOMOFFSET) )){
			if (yVel < 0) {
				player.position = hit.point + new Vector3(0,PLAYERBOTTOMOFFSET,0);
			}
			else {
				player.position = hit.point - new Vector3(0,PLAYERTOPOFFSET,0);
				jumphold = JUMPLIMIT;
				yVel = 0;
			}
			if (Mathf.Abs (yVel) > (LANDSPEEDTHRESH)) {
				state.ChangeState (enterLightLand, updateLightLand, exitLightLand);
				lightLand.Play();
			} else {
				state.ChangeState (enterLightGround, updateLightGround, exitLightGround);
		    }	
		}
		else {
			player.Translate (new Vector3(0, yVel * Time.deltaTime , 0));
		}
		
		if (Physics.Raycast (player.position, new Vector3(xVel, 0, 0), out hit, Mathf.Abs (xVel * Time.deltaTime) + PLAYERRADIUS)){
			if (xVel < 0) {
				player.position = hit.point + new Vector3(PLAYERRADIUS, 0, 0);
				wall = "left";
			}
			else {
				player.position = hit.point - new Vector3(PLAYERRADIUS, 0, 0);
				wall = "right";
			}
			state.ChangeState (enterLightWall, updateLightWall, exitLightWall);
		}
		else {
			player.Translate (new Vector3(xVel, 0, 0));
		}
	}
	
	void exitLightJump(){
		yVel = 0; 
		jumphold = 0;
		walljumptimer = 0;
		print ("exitLightJump");
	}
	
	//LIGHT WALL STATE
	void enterLightWall(){
		print ("enterLightWall");
		spaceheld = true;
		unlatchtimer = 0;
		
	}
	void updateLightWall(){
		if (Input.GetKey (KeyCode.Alpha2)){
			state.ChangeState (enterHeavyJump, updateHeavyJump, exitHeavyJump);
			robot.Play();
			return;
		}
		
		if (Input.GetKey (KeyCode.Space) && !spaceheld){
			xVel = MAXAIRCONTROL * ( (wall=="left")?1:-1);
			state.ChangeState (enterLightJump, updateLightJump, exitLightJump);
			lightWallJump.Play();
			yVel = MINLIGHTJUMP;
			return;
		} 
		if (!Input.GetKey (KeyCode.Space)){
			spaceheld = false;
		}
		if (Input.GetKey (KeyCode.LeftArrow) && wall == "right"){
			
			if (unlatchtimer >= UNLATCHTIMER) {
				xVel = -MAXAIRCONTROL/2 * Time.deltaTime;
				jumphold = JUMPLIMIT;
				state.ChangeState (enterLightJump, updateLightJump, exitLightJump);
			}
			unlatchtimer += Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.RightArrow) && wall == "left"){
			if (unlatchtimer >= UNLATCHTIMER) {
				xVel = MAXAIRCONTROL/2 * Time.deltaTime;
				jumphold = JUMPLIMIT;
				state.ChangeState (enterLightJump, updateLightJump, exitLightJump);
			}
			unlatchtimer += Time.deltaTime;
		}
	}
	void exitLightWall(){
		print ("exitLightWall");
		unlatchtimer = 0;
	}
		
	
	public class ImmediateStateMachine
	{ 	
		// These states will be cached when calling ChangeState().
		// The Execute() method will execute onUpdate() of the current state on each update,
		// running whatever method is stored there.
		ImmediateStateDelegate enter;
		ImmediateStateDelegate update;
		ImmediateStateDelegate exit;
	 
		// Change state from the previous state, if any, and begin working in the new state.
		// The previous state's onExit() and the new state's onEnter() will both be called.
		public void ChangeState (ImmediateStateDelegate enter, ImmediateStateDelegate update, ImmediateStateDelegate exit)
		{ 
			// If a state is currently running, it should be allowed to gracefully exit
			// before the next state takes overS
			if (this.exit != null)
				this.exit();
	 
			// Cache the given state values
			this.enter = enter;
			this.update = update;
			this.exit = exit;
	 
			// If a state isn't currently running, we can immediately switch to our entering
			// state using the state delegates we cached a few lines above
			if (this.enter != null)
				this.enter();
		}
	 
		// Call this during
		public void Execute ()
		{
			if (this.update != null)
				this.update(); 
		}
	
		public bool IsInState(ImmediateStateDelegate update)
		{
			return (this.update == update);
		}
		
	}
	
	void enterHeavyGround() {
		print ("enterHeavyGround");
		spaceheld = true;
		turbo = false;
	}
	void updateHeavyGround(){
		turbo = false;
		if (Input.GetKey (KeyCode.Alpha1)){
			state.ChangeState (enterLightGround, updateLightGround, exitLightGround);
			ninja.Play();
			return;
		}
		
		if (Input.GetKey (KeyCode.Space) && !spaceheld){
			jumpstrength = MINJUMPSTRENGTH;
			state.ChangeState (enterHeavyCharge, updateHeavyCharge, exitHeavyCharge);
			heavyCharge.Play();
			return;
		} 
		if (!Input.GetKey(KeyCode.Space)){
			spaceheld = false;
		}
		
		if(Input.GetKey (KeyCode.LeftShift)){
			turbo = true;
		}
		
		if (!turbo && heavyTurbo.isPlaying){
			heavyTurbo.Stop ();
		}
		if (!(Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.RightArrow)) && heavyRun.isPlaying){
			heavyRun.Stop ();
		}
		
		if (Input.GetKey (KeyCode.RightArrow)){
			xVel += HEAVYRUNACCEL * ((turbo)?3f:1) * Time.deltaTime;
			if (!heavyRun.isPlaying){
				heavyRun.Play ();
			}
			if (turbo && !heavyTurbo.isPlaying){
				heavyTurbo.Play ();
			}
		}else if(xVel > 0){
			xVel -= HEAVYFRICTION * Time.deltaTime;
			xVel = Mathf.Max (xVel,0);
		}
		if (Input.GetKey (KeyCode.LeftArrow)){
			xVel += -HEAVYRUNACCEL * ((turbo)?3f:1) * Time.deltaTime;	
			if (!heavyRun.isPlaying){
				heavyRun.Play ();
			}
			if (turbo && !heavyTurbo.isPlaying){
				heavyTurbo.Play ();
			}
		}else if(xVel < 0){
			xVel += HEAVYFRICTION * Time.deltaTime;
			xVel = Mathf.Min (xVel,0);
		}
		
		xVel = Mathf.Max (xVel, (-MAXHEAVYRUNSPEED * ((turbo)?2f:1)));
		xVel = Mathf.Min (xVel, (MAXHEAVYRUNSPEED * ((turbo)?2f:1)));
		
		if (Physics.Raycast (player.position, new Vector3(xVel, 0, 0), out hit, Mathf.Abs (xVel * Time.deltaTime) + PLAYERRADIUS)){
			if (xVel < 0) {
				player.position = hit.point + new Vector3(PLAYERRADIUS, 0, 0);
				wall = "left";
			}
			else {
				player.position = hit.point - new Vector3(PLAYERRADIUS, 0, 0);
				wall = "right";
			}
		}
		else {
			player.Translate (new Vector3(xVel, 0, 0));
		}
		if (!Physics.Raycast (player.position, new Vector3(0,-1,0),  Mathf.Abs (1) + PLAYERBOTTOMOFFSET )){
			state.ChangeState (enterHeavyJump, updateHeavyJump, exitHeavyJump);
			return;
		}
		
		
		
		
		
		
	}
	void exitHeavyGround() {
		print ("exitHeavyGround");
		heavyTurbo.Stop ();
		heavyRun.Stop ();
	}
	
	void enterHeavyJump() {
		print ("enterHeavyJump");
		yVel = jumpstrength;
	}
	void updateHeavyJump(){
		if (Input.GetKey (KeyCode.Alpha1)){
			state.ChangeState (enterLightJump, updateLightJump, exitLightJump);
			ninja.Play ();
			return;
		}
		
		yVel -= HEAVYGRAVITYACCEL * Time.deltaTime;
		yVel = Mathf.Max (yVel, -MAXHEAVYFALLSPEED);
		
		if (Input.GetKey (KeyCode.RightArrow)){
			xVel += HEAVYAIRACCEL * Time.deltaTime;		
		}
		if (Input.GetKey (KeyCode.LeftArrow)){
			xVel += -HEAVYAIRACCEL * Time.deltaTime;		
		}
		xVel = Mathf.Max (xVel, -MAXHEAVYAIRCONTROL);
		xVel = Mathf.Min (xVel, MAXHEAVYAIRCONTROL);
		
		if (Physics.Raycast (player.position, new Vector3(0,yVel,0), out hit,  Mathf.Abs (yVel * Time.deltaTime) + ((yVel>0)?PLAYERTOPOFFSET:PLAYERBOTTOMOFFSET) )){
			if (yVel < 0) {
				player.position = hit.point + new Vector3(0,PLAYERBOTTOMOFFSET,0);
			}
			else {
				player.position = hit.point - new Vector3(0,PLAYERTOPOFFSET,0);
			}
			if (Mathf.Abs (yVel) > LANDSPEEDTHRESH) {
				state.ChangeState (enterHeavyLand, updateHeavyLand, exitHeavyLand);
				heavyLand.Play();
			} else {
				state.ChangeState (enterHeavyGround, updateHeavyGround, exitHeavyGround);
		    }
		}
		else {
			player.Translate (new Vector3(0, yVel * Time.deltaTime , 0));
		}
		
		if (Physics.Raycast (player.position, new Vector3(xVel, 0, 0), out hit, Mathf.Abs (xVel * Time.deltaTime) + PLAYERRADIUS)){
			if (xVel < 0) {
				player.position = hit.point + new Vector3(PLAYERRADIUS, 0, 0);
				wall = "left";
			}
			else {
				player.position = hit.point - new Vector3(PLAYERRADIUS, 0, 0);
				wall = "right";
			}
		}
		else {
			player.Translate (new Vector3(xVel, 0, 0));
		}
	}
	void exitHeavyJump() {
		print ("exitHeavyJump");
		yVel = 0; 
		jumphold = 0;
		jumpstrength = 0;
		walljumptimer = 0;
	}
	
	void enterHeavyCharge() {
		print ("enterHeavyCharge");
		xVel=0;
	}
	void updateHeavyCharge(){
		if (Input.GetKey (KeyCode.Alpha1)){
			state.ChangeState (enterLightGround, updateLightGround, exitLightGround);
			ninja.Play();
			return;
		}
		jumpstrength += JUMPSTRENGTHCHARGESPEED * Time.deltaTime;
		jumpstrength = Mathf.Min (jumpstrength, MAXJUMPSTRENGTH);
		
		if (!Input.GetKey (KeyCode.Space) || jumpstrength == MAXJUMPSTRENGTH){
			state.ChangeState(enterHeavyJump, updateHeavyJump, exitHeavyJump);
			if (heavyCharge.isPlaying) {
				heavyCharge.Stop ();
			}
			heavyJump.Play();
		}
	}
	void exitHeavyCharge() {
		print ("exitHeavyCharge");
	}
	
	
	void enterLightLand(){
		landcount = 0;
		xVel = 0;
		yVel = 0;
		print ("enterLightL");
	}
	void updateLightLand(){
		if (landcount > .3f) {
			state.ChangeState (enterLightGround, updateLightGround, exitLightGround);
		}
		landcount += Time.deltaTime;
			
	}
	void exitLightLand(){
	}
	void enterHeavyLand(){
		landcount = 0;
		xVel = 0;
		yVel = 0;
	}
	void updateHeavyLand(){
		
		if(landcount > .5f) {
			state.ChangeState(enterHeavyGround, updateHeavyGround, exitHeavyGround);
		}
		landcount += Time.deltaTime;
		camera.Translate (new Vector3(Random.Range (-.5f,.5f), Random.Range (-.5f,0.5f), Random.Range(-.5f,.5f)));
		
	}
	void exitHeavyLand(){
	}
	
	
	ImmediateStateMachine state;
	// Use this for initialization
	void Start () {
		wall = "right";
		hit = new RaycastHit();
		state = new ImmediateStateMachine();
		state.ChangeState (enterStart, updateStart, exitStart);
		xVel = 0;
		yVel = 0;
		truecamera = camera;
	}
	
	// Update is called once per frame
	void Update () {
		state.Execute();
		if (!state.IsInState(updateHeavyLand)){
			camera.transform.localPosition = player.transform.localPosition;
			camera.Translate (new Vector3(0,0,-50));
		}
	}
	
	void enterStart(){}
	void updateStart(){
		if (Input.GetKey (KeyCode.Return)){
			state.ChangeState(enterHeavyJump, updateHeavyJump, exitHeavyJump);
		}
	}
	void exitStart(){}
}
