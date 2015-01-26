using UnityEngine;
using System.Collections;

public class zombie : MonoBehaviour {
	
	public Transform zed;
	public Transform player;
	
	public Vector3 movementVector;
	
	public static float SPEED = 3f;
	
	private bool startFlag;
	
	// Use this for initialization
	void Start () {
		movementVector = Vector3.zero;
		startFlag = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (!startFlag){
			startFlag = Input.GetKey (KeyCode.Space);
		}
		
		if (startFlag) {
		
		movementVector = player.transform.localPosition-zed.transform.localPosition;
		movementVector.Normalize();
		movementVector *= SPEED * Time.deltaTime;
		
		zed.transform.Translate (movementVector);
		}
	}
}
