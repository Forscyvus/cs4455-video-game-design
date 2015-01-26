﻿using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour {
	public Transform player;

	RaycastHit hit;
	Vector3 mouse;
	
	public bool shooting;
		
	float shootingTimer;
	float rifleDelay;
	bool canShoot;
	
	Vector3 target;
	Vector3 source;
	
	float range;
	
	int firingMode;
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
	
	bool startFlag;
	
	AudioSource gunShot;
	
	GameObject deathText;

	void Start () {
		shooting = false;
		canShoot = true;
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
	}
	
	void Update () {
		/*
		if (!startFlag){
			startFlag = Input.GetKey (KeyCode.Space);
		}
		*/
		
		if(startFlag) {
			if(Input.GetKeyDown(KeyCode.Alpha1)) { 
				firingMode = 0;
			}
			if(Input.GetKeyDown(KeyCode.Alpha2)) {
				firingMode = 1;
			}
			
			if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)){
				return;
			}
			
			mouse = new Vector3(hit.point.x, player.transform.localPosition.y, hit.point.z);
			
			if(firingMode == 0) {
				if(canShoot) {
					if (Input.GetMouseButtonDown (0)) {
						shooting = true;
						//shootingTimer = .5f;
						target = mouse;
						source = player.transform.localPosition;
						//range = Vector3.Magnitude (target - source);
						
						startLeftP = targetLeft.transform.localPosition;
						startLeftR = targetLeft.transform.localRotation;
						startRightP = targetRight.transform.localPosition;
						startRightR = targetRight.transform.localRotation;
						targeter.SetActive(true);
					}
					
					if (Input.GetMouseButton(0) && shooting) {
						if(targetLeft.transform.localRotation.y > 0f) {
							targetLeft.transform.RotateAround (targeter.transform.position, Vector3.up, 30 * Time.deltaTime);
							targetRight.transform.RotateAround (targeter.transform.position, Vector3.up, -30 * Time.deltaTime);
						}
					}
					if (Input.GetMouseButtonUp (0) && shooting) {
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
			else {
				if(Input.GetMouseButtonDown(0) && arrowsLeft != 0 && !shooting) {
					shooting = true;
					shootingTimer = .5f;
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
			Destroy (collider.gameObject);
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
