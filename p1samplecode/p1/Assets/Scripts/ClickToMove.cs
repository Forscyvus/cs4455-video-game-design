using UnityEngine;
using System.Collections;

public class ClickToMove : MonoBehaviour {
	public Vector3 targetPosition;   // for debugging
	
	public Transform player;
	public Transform plane;
	public float speed;
	
	// Use this for initialization
	void Start () {
		targetPosition = player.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
 		RaycastHit hit;
				
		// move using Lerp
		player.transform.localPosition = Vector3.Lerp (player.transform.localPosition, targetPosition, speed * Time.deltaTime);
		Debug.Log (speed * Time.deltaTime);
		
		// if clicked, do something
		if ( Input.GetMouseButtonDown(0)){
			if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
    	        return;

			Debug.Log ("hit " + hit.collider.name);
			
			// if we clicked the plane, move the sphere there
			if (hit.collider.transform == plane) 
			{
				targetPosition = transform.InverseTransformPoint(hit.point);
			}			
		}	
	}
}
