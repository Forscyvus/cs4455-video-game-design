using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour {
	public Transform player;

	RaycastHit hit;
	Vector3 mouse;
	
	public bool shooting;
		
	float shootingTimer;
	float rifleDelay;
	public bool canShoot;
	float bowDelay;
	
	Vector3 target;
	Vector3 source;
	
	float range;
	
	int firingMode;
	public Rigidbody bullet;
	public Rigidbody arrow;
	public Transform bow;
	int arrowsLeft;
	
	GameObject targeter;
	GameObject targetLeft;
	GameObject targetRight;
	Vector3 startLeftP;
	Vector3 startRightP;
	Quaternion startLeftR;
	Quaternion startRightR;
	GameObject targetLeftEnd;
	GameObject targetRightEnd;

	private static float MAXRIFLEACC = .0001f;
	private static float MAXBOWACC = .075f;
	
	bool startFlag;
	bool hasGun;
	bool hasBow;
	
	AudioSource gunShot;
	
	GameObject deathText;

	public mainmove moveScript;

	void Start () {
		shooting = false;
		canShoot = true;
		hasGun = false;
		hasBow = false;
		firingMode = 0;
		
		arrowsLeft = 3;
		
		targeter = GameObject.Find("Targeting");
		targetLeft = GameObject.Find ("TargetLeft");
		targetRight = GameObject.Find ("TargetRight");
		targetLeftEnd = GameObject.Find ("TargetLeftEnd");
		targetRightEnd = GameObject.Find ("TargetRightEnd");
		
		targeter.SetActive (false);
		
		startFlag = true;
		
		AudioSource[] sound = gameObject.GetComponents<AudioSource>();
		gunShot = sound[0];
		
		deathText = GameObject.Find ("Death");
		deathText.SetActive(false);

		moveScript = gameObject.GetComponent<mainmove>();
	}
	
	void Update () {
		/*
		if (!startFlag){
			startFlag = Input.GetKey (KeyCode.Space);
		}
		*/
		
		if(startFlag) {
			if(!shooting) {
				if(Input.GetKeyDown(KeyCode.Alpha1) && hasGun) { 
					pickupGun ();
					/*
					GameObject[] zeds = GameObject.FindGameObjectsWithTag ("zed");
					foreach(GameObject zed in zeds){
						
						//Destroy (zed);
						//zombie z = zed.GetComponent (zombie);
						//zombie.activate();
						zed.SendMessage ("equipGun");
						
					}*/
				}
				if(Input.GetKeyDown(KeyCode.Alpha2) && hasBow) {
					firingMode = 1;
					pickupBow ();
					/*GameObject[] zeds = GameObject.FindGameObjectsWithTag ("zed");
					foreach(GameObject zed in zeds){

							//Destroy (zed);
							//zombie z = zed.GetComponent (zombie);
							//zombie.activate();
							zed.SendMessage ("equipBow");
						
					}*/
				}
			}
			if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)){
				return;
			}
			
			mouse = new Vector3(hit.point.x, player.transform.localPosition.y, hit.point.z);
			
			if(firingMode == 0 && hasGun) {
				if(canShoot && !moveScript.tripping) {
					if (Input.GetMouseButton (0) && !shooting) {
						shooting = true;
						//shootingTimer = .5f;
						//target = mouse;
						source = player.transform.localPosition;
						//range = Vector3.Magnitude (target - source);
						
						startLeftP = targetLeft.transform.localPosition;
						startLeftR = targetLeft.transform.localRotation;
						startRightP = targetRight.transform.localPosition;
						startRightR = targetRight.transform.localRotation;
						targeter.SetActive(true);
					}
					
					if (Input.GetMouseButton(0) && shooting) {
						if(targetLeft.transform.localRotation.y > MAXRIFLEACC) {
							targetLeft.transform.RotateAround (targeter.transform.position, Vector3.up, 35 * Time.deltaTime * (5f * targetLeft.transform.localRotation.y));
							targetRight.transform.RotateAround (targeter.transform.position, Vector3.up, -35 * Time.deltaTime * (5f * targetLeft.transform.localRotation.y));
						}
					}
					if (Input.GetMouseButtonUp (0) && shooting) {
						target = mouse;
						Vector3 fireVector = target-source;
						if(targetLeft.transform.localRotation.y > 0f) { // if the targeter has not reach max accuracy
							Vector3 leftPos = targetLeftEnd.transform.position;
							Vector3 rightPos = targetRightEnd.transform.position;
							float rand = Random.value;
							Vector3 randVec = leftPos-rightPos;
							float randVecLength = Vector3.Magnitude(randVec);
							Vector3 tempVec = randVec;
							tempVec.x = tempVec.x / randVecLength;
							tempVec.z = tempVec.z / randVecLength;
							tempVec.x = tempVec.x * rand * randVecLength;
							tempVec.z = tempVec.z * rand * randVecLength;
							tempVec.x += rightPos.x;
							tempVec.z += rightPos.z;
							tempVec.y = player.transform.position.y+1f;
							
							Vector3 randPoint = tempVec;
							fireVector = randPoint - source;
						}
						targetLeft.transform.localPosition = startLeftP;
						targetLeft.transform.localRotation = startLeftR;
						
						targetRight.transform.localPosition = startRightP;
						targetRight.transform.localRotation = startRightR;
						
						targeter.SetActive(false);

						Rigidbody bulletClone = (Rigidbody) Instantiate(bullet, bow.position, Quaternion.LookRotation(fireVector, Vector3.up));
						
						gunShot.Play();
						if (Physics.Raycast (source, fireVector, out hit)){
							if (hit.transform.gameObject.tag == "zed"){
								Destroy (hit.collider.gameObject);
							}
						}
						shooting = false;
						canShoot = false;
						rifleDelay = 1f;
					}
				}
				if(!canShoot) {
					rifleDelay -= Time.deltaTime;
					if(rifleDelay < 0) {
						canShoot = true;
					}
				}
			}
			else if (hasBow){
				if(canShoot && !moveScript.tripping) {
					if (Input.GetMouseButton (0) && arrowsLeft != 0 && !shooting) {
						shooting = true;
						source = player.transform.localPosition;
						
						startLeftP = targetLeft.transform.localPosition;
						startLeftR = targetLeft.transform.localRotation;
						startRightP = targetRight.transform.localPosition;
						startRightR = targetRight.transform.localRotation;
						targeter.SetActive(true);
					}
					
					if (Input.GetMouseButton(0) && shooting) {
						if(targetLeft.transform.localRotation.y > MAXBOWACC) {
							targetLeft.transform.RotateAround (targeter.transform.position, Vector3.up, 15 * Time.deltaTime * (4f * targetLeft.transform.localRotation.y));
							targetRight.transform.RotateAround (targeter.transform.position, Vector3.up, -15 * Time.deltaTime * (4f * targetLeft.transform.localRotation.y));
						}
					}
					if (Input.GetMouseButtonUp (0) && shooting) {
						target = mouse;
						Vector3 fireVector = target-source;
						if(targetLeft.transform.localRotation.y > 0f) { // if the targeter has not reach max accuracy
							Vector3 leftPos = targetLeftEnd.transform.position;
							Vector3 rightPos = targetRightEnd.transform.position;
							float rand = Random.value;
							Vector3 randVec = leftPos-rightPos;
							float randVecLength = Vector3.Magnitude(randVec);
							Vector3 tempVec = randVec;
							tempVec.x = tempVec.x / randVecLength;
							tempVec.z = tempVec.z / randVecLength;
							tempVec.x = tempVec.x * rand * randVecLength;
							tempVec.z = tempVec.z * rand * randVecLength;
							tempVec.x += rightPos.x;
							tempVec.z += rightPos.z;
							tempVec.y = player.transform.position.y;
							//tempVec.y = 1f;
							
							Vector3 randPoint = tempVec;
							fireVector = randPoint - source;
						}
						targetLeft.transform.localPosition = startLeftP;
						targetLeft.transform.localRotation = startLeftR;
						
						targetRight.transform.localPosition = startRightP;
						targetRight.transform.localRotation = startRightR;
						
						targeter.SetActive(false);

						//Rigidbody arrowClone = (Rigidbody) Instantiate(arrow, bow.position, bow.rotation);
						//Quaternion arrowDirection = bow.rotation;
						//arrowDirection.x = fireVector.x;
						//arrowDirection.y = fireVector.y;
						//arrowDirection.z = fireVector.z;
						//Rigidbody arrowClone = (Rigidbody) Instantiate(arrow, bow.position, arrowDirection);

						Rigidbody arrowClone = (Rigidbody) Instantiate(arrow, bow.position, Quaternion.LookRotation(fireVector, Vector3.up));
						arrowsLeft--;

						/*
						//gunShot.Play();
						if (Physics.Raycast (source, fireVector, out hit)){
							if (hit.transform.gameObject.tag == "zed"){
								Destroy (hit.collider.gameObject);
							}
						}
						*/
						shooting = false;
						canShoot = false;
						bowDelay = 1f;
					}
				}
				if(!canShoot) {
					bowDelay -= Time.deltaTime;
					if(bowDelay < 0) {
						canShoot = true;
					}
				}
			}

			/*
			if(Input.GetMouseButtonDown(0) && arrowsLeft != 0 && !shooting) {
				shooting = true;
				shootingTimer = 1f;
			}
			if(shooting) {
				shootingTimer -= Time.deltaTime;
				if(shootingTimer < 0) {
					shootingTimer = 0;
					shooting = false;
					Rigidbody arrowClone = (Rigidbody) Instantiate(arrow, bow.position, bow.rotation);
					arrowsLeft--;
				}
			}
		}
		*/
		}
	}

	void pickupBow(){
		hasBow = true;
		firingMode = 1;
		GameObject[] zeds = GameObject.FindGameObjectsWithTag ("zed");
		foreach(GameObject zed in zeds){
			
			//Destroy (zed);
			//zombie z = zed.GetComponent (zombie);
			//zombie.activate();
			zed.SendMessage ("equipBow");
			
		}
	}
	void pickupGun(){
		hasGun = true;
		firingMode = 0;
		GameObject[] zeds = GameObject.FindGameObjectsWithTag ("zed");
		foreach(GameObject zed in zeds){
			
			//Destroy (zed);
			//zombie z = zed.GetComponent (zombie);
			//zombie.activate();
			zed.SendMessage ("equipGun");
			
		}
	}
	
	
	void OnControllerColliderHit (ControllerColliderHit hit) {
		if(hit.collider.gameObject.name == "Arrow_Prefab(Clone)") {
			arrowsLeft++;
			if(arrowsLeft > 3) {
				arrowsLeft = 3;
			}
			Destroy (hit.collider.gameObject);
		}
		
		if(hit.collider.gameObject.name == "zombie") {
			deathText.SetActive(true);
			//Destroy (collider.gameObject);
			collider.gameObject.SetActive (false);
		}
	}
	
	
	/*
	void OnColllisionEnter (Collision collision) {
		if(collision.gameObject.name == "Arrow_Prefab(Clone)") {
			arrowsLeft++;
			if(arrowsLeft > 3) {
				arrowsLeft = 3;
			}
			Destroy (collision.gameObject);
		}
		
		if(collision.gameObject.name == "zombie") {
			deathText.SetActive(true);
			Destroy (collision.gameObject);
		}
	}
	*/
}
