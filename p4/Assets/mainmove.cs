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
	
	public static float Y = 1.5f;
	public static float FWDACCEL = 10f;
	public static float STOPTHRESH = 1f;
	public static float MAXSPEED = 5f;
	
	private bool keyW, keyS, keyA, keyD;
	private bool shooting;
	
	private float shootingTimer;
	
	private Vector3 target;
	private Vector3 source;

	private float range;
	private bool startFlag;
	
	
	// Use this for initialization
	void Start () {
		movementVector = Vector3.zero;
		movementSpeed = 0;
		shooting = false;
		startFlag = false;
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
		
		if (Input.GetMouseButtonDown (0)) {
		
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
				if (Physics.Raycast (source, target-source, out hit, range)){
					print (target);
					print (hit.transform.gameObject.tag);
					if (hit.transform.gameObject.tag == "zed"){
						Destroy (hit.collider.gameObject);
					}
				}
			}
		}
		
		if (!shooting) {	
			
			player.LookAt (mouse, new Vector3(0,Y,0));
			
			
			
			if (keyW){
				
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
			
			movementVector = new Vector3(0,0,1) * movementSpeed * Time.deltaTime;
			
			//print (movementVector);
			player.transform.Translate ( movementVector);
		}
		}
		//controller.Move (new Vector3(1,-.1f,0));
		camera.transform.localPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y + 33, player.transform.localPosition.z);
		
	
	}
}
