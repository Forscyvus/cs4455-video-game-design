using UnityEngine;
using System.Collections;

public class ClickToMove : MonoBehaviour {
	public Vector3 targetPosition;   // for debugging
	public Vector3 truePosition;
	public Vector3 originPosition;
	
	public AudioSource chargeSource;
	public AudioSource shootSource;
	public AudioSource skidSource;
	public AudioSource sputterSource;
	public AudioSource hitSource;
	
	public Transform player;
	public Transform plane;
	public float speed;
	public float t;
	public float randx;
	public float randy;
	public float randramp;
	public float sputterOffsetX;
	public float sputterOffsetY;
	public float sputterx;
	public float sputtery;
	
	public static int CHARGE = 0;
	public static int SHOOT = 1;
	public static int INTERRUPT = 2;
	public static int SPUTTER = 3;
	public static int REST = 4;
	
	public static float CHARGETHRESH = 3.333f;
	public static double TARGETTOLERANCE = 0.001;
	public static float SPEEDREDUCER = 170.0f;
	public static float BACKUPRATE = .00005f;
	public static float RANDOMRANGE = 1f;
	public static float RANDOMRAMP = .00075f;
	public static float INTERRUPTDURATION = .200f;
	public static float INTERRUPTJITTER = 1f;
	public static float CHARGEMINIMUM = 0f;
	public static float MINSPEED = .0005f;
	public static float SPUTTERSPEED = .001f;
	public static float MINSPUTTERSPEED = .01f;
	public static float SPUTTERHEIGHT = 10f;
	
	public int state = REST;
	public float chargeCounter;
	public float interruptCounter;
	
	// Use this for initialization
	void Start () {
		targetPosition = player.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
 		RaycastHit hit;
		
		
		//state stuff
		if ((state == SHOOT || state == SPUTTER ) && Vector3.Distance(truePosition, targetPosition) < TARGETTOLERANCE ) {
			state = REST;
			sputterSource.Stop ();
			shootSource.Stop ();
			hitSource.Play ();
			Debug.Log ("REST");
		}
		if (state == CHARGE) {
			chargeCounter += Time.deltaTime;
		}
		if (state == CHARGE && !Input.GetMouseButton(0)){
			if (chargeCounter < CHARGEMINIMUM) {
				state = REST;
				chargeSource.Stop ();
				player.transform.localPosition = originPosition;
				truePosition = originPosition;
			} else {
				state = SHOOT;
				chargeSource.Stop ();
				shootSource.Play();
				speed = Mathf.Max( (chargeCounter / (float)CHARGETHRESH) / SPEEDREDUCER, MINSPEED);
				Debug.Log ("SHOOT");
			}
		}
		if (state == CHARGE && chargeCounter > CHARGETHRESH) {
			state = SPUTTER;
			chargeSource.Stop ();
			sputterSource.Play();
			chargeCounter = 0;
			interruptCounter = 0;
			t = 0;
			randramp = 0;
			originPosition = truePosition;
			sputterOffsetX = (targetPosition.y - originPosition.y);
			sputterOffsetY = (targetPosition.x - originPosition.x);
			Vector3 v = new Vector3(sputterOffsetX, sputterOffsetY, 0);
			v.Normalize();
			sputterOffsetX = v.x;
			sputterOffsetY = v.y;
			
			if (Random.Range (0,2) == 0){
				sputterOffsetX = -sputterOffsetX;
			} else {
				sputterOffsetY = -sputterOffsetY;
			}
			Debug.Log ("SPUTTER");
		}
		if (state == INTERRUPT) {
			interruptCounter += Time.deltaTime;
			if (interruptCounter > INTERRUPTDURATION){
				state = CHARGE;
				skidSource.Stop ();
				chargeSource.Play();
				originPosition = truePosition;
				interruptCounter = 0;
				chargeCounter = 0;
				t = 0;
				randramp = 0;
			}
		}
				
		// if clicked, do something
		if ( Input.GetMouseButtonDown(0)){
			
			if (state == REST) {
				state = CHARGE;
				chargeCounter = 0;
				t = 0;
				randramp = 0;
				originPosition = player.transform.localPosition;
				chargeSource.Play();
			}
			if (state == CHARGE) {
				chargeCounter += Time.deltaTime;
			}
			if (state == SHOOT || state == SPUTTER) {
				shootSource.Stop ();
				sputterSource.Stop ();
				skidSource.Play();
				state = INTERRUPT;
			}
			
			if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
    	        return;

			Debug.Log ("hit " + hit.collider.name);
			
			// if we clicked the plane, move the sphere there
			if (hit.collider.transform == plane) 
			{
				targetPosition = transform.InverseTransformPoint(hit.point);
			}			
		}
		
		//movement
		if (state == SHOOT) {
			// move using Lerp
			//NOT UNITY'S BUILT IN LERP NO SIRREE
			t += speed * 1000*Time.deltaTime;
			t = Mathf.Min (t, 1);
			player.transform.localPosition = originPosition + (targetPosition - originPosition) * t;
			truePosition = player.transform.localPosition;
			Debug.Log (speed * Time.deltaTime);
		}
		if (state == CHARGE) {
			t = t - BACKUPRATE * 1000*Time.deltaTime;
			player.transform.localPosition = originPosition + (targetPosition - originPosition) * t;
			truePosition = player.transform.localPosition;
			
			//jitter
			randramp += RANDOMRAMP * 1000*Time.deltaTime;
			randx = truePosition.x + Random.Range(-RANDOMRANGE * randramp, RANDOMRANGE * randramp);
			randy = truePosition.y + Random.Range(-RANDOMRANGE * randramp, RANDOMRANGE * randramp);
			player.transform.localPosition = new Vector3(randx, randy, player.transform.localPosition.z);
		}
		if (state == INTERRUPT) {
			//jitter
			randx = truePosition.x + Random.Range(-RANDOMRANGE * INTERRUPTJITTER, RANDOMRANGE * INTERRUPTJITTER);
			randy = truePosition.y + Random.Range(-RANDOMRANGE * INTERRUPTJITTER, RANDOMRANGE * INTERRUPTJITTER);
			player.transform.localPosition = new Vector3(randx, randy, player.transform.localPosition.z);
		}
		if (state == SPUTTER) {
			t += 1000*Time.deltaTime * SPUTTERSPEED * Mathf.Max(Mathf.Abs(Mathf.Sin(t*5*Mathf.PI)), MINSPUTTERSPEED);
			t = Mathf.Min (t, 1);
			player.transform.localPosition = originPosition + (targetPosition - originPosition) * t;
			truePosition = player.transform.localPosition;
			sputterx = truePosition.x + (sputterOffsetX * SPUTTERHEIGHT * Mathf.Abs(Mathf.Sin(t*5*Mathf.PI)));
			sputtery = truePosition.y + (sputterOffsetY * SPUTTERHEIGHT * Mathf.Abs(Mathf.Sin(t*5*Mathf.PI)));
			player.transform.localPosition = new Vector3(sputterx, sputtery, player.transform.localPosition.z);
			truePosition = player.transform.localPosition;
			
		}
	}
}
