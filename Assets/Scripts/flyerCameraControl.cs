using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flyerCameraControl : MonoBehaviour {

	float camRotYSpeed;
	float epsilonCamAng;
	int rotYDir;

	public GameObject flyer_prefab;
	public GameObject flyerCameraHelper; 

	private Vector3 offset;

		void Start () 
		{
		camRotYSpeed = 150;
		epsilonCamAng = 3;
		rotYDir = 0;
		}

	void LateUpdate () {
		float camAngleY = Time.deltaTime * camRotYSpeed;

		flyerCameraHelper.transform.position = flyer_prefab.transform.position;

		float deltaYAng = flyer_prefab.transform.eulerAngles.y - flyerCameraHelper.transform.eulerAngles.y;


//		if (deltaYAng > 0 ) {
//			//flyer turns to right
//			rotYDir = 1;
//		}
//
//		if (deltaYAng < 0) {
//			//flyer turns to left
//			rotYDir = -1;
//		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			rotYDir = 1;
		}
			
		if (Input.GetKey (KeyCode.LeftArrow)) {
			rotYDir = -1;
		}
			
			
//		if (Mathf.Abs(deltaYAng) > epsilonCamAng) {
//			flyerCameraHelper.transform.Rotate (Vector3.up, rotYDir * camAngleY, Space.World);
//		}

		flyerCameraHelper.transform.eulerAngles = new Vector3 (flyerCameraHelper.transform.eulerAngles.x, flyer_prefab.transform.eulerAngles.y, flyerCameraHelper.transform.eulerAngles.z);


	}

}
