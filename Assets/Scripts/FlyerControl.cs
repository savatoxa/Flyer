using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyerControl : MonoBehaviour {


	float rotX_speed;
	float rotY_speed;
	float rotZ_speed;
	float Acc;
	float Dec;
	float max_speed;
	float min_speed;
	float max_roll;
	float max_pitch;
	float epsilonZAng;
	Rigidbody m_rigidbody;

	// Use this for initialization
	void Start () {
		Acc = 10.0f;
		Dec = 10.0f;
		max_speed = 20.0f;
		min_speed = 0.1f;
		rotX_speed = 100.0f;
		rotY_speed = 100.0f;
		rotZ_speed = 100.0f;
		max_roll = 45.0f;
		max_pitch = 45.0f;
		epsilonZAng = 1;
		m_rigidbody = GetComponent<Rigidbody> ();
	}
	

	void Update () {
		

		if (Input.GetKey ("w")) {
			if (Vector3.Magnitude (m_rigidbody.velocity) < max_speed) {
				m_rigidbody.velocity += transform.forward * Acc * Time.deltaTime;
			}
		}

		if (Input.GetKey ("s")) {
			if (Vector3.Magnitude (m_rigidbody.velocity) > min_speed) {				
				m_rigidbody.velocity -= transform.forward * Dec * Time.deltaTime;
			}
			else {
				m_rigidbody.velocity = transform.forward * 0.0f;
			}
		}

		float angleX = Time.deltaTime * rotX_speed;
		float angleY = Time.deltaTime * rotY_speed;
		float angleZ = Time.deltaTime * rotZ_speed;

		if (Input.GetKey (KeyCode.UpArrow)) {
			transform.Rotate(Vector3.right, angleX, Space.Self);

			if (transform.eulerAngles.x >= max_pitch && transform.eulerAngles.x <= 360 - max_pitch ) {
				transform.eulerAngles = new Vector3 (max_pitch, transform.eulerAngles.y, transform.eulerAngles.z);
			}

			m_rigidbody.velocity = transform.forward * Vector3.Magnitude (m_rigidbody.velocity);
		}

		if (Input.GetKey (KeyCode.DownArrow)) {
			transform.Rotate(Vector3.right, -angleX, Space.Self);

			if (transform.eulerAngles.x <= 360 - max_pitch && transform.eulerAngles.x >= max_pitch) {
				transform.eulerAngles = new Vector3 (360 - max_pitch, transform.eulerAngles.y, transform.eulerAngles.z);
			}

			m_rigidbody.velocity = transform.forward * Vector3.Magnitude (m_rigidbody.velocity);
		}



		if (Input.GetKey (KeyCode.LeftArrow)) {
			transform.Rotate (Vector3.forward, angleZ, Space.Self);

			transform.Rotate (Vector3.up, -angleY, Space.World);

			if (transform.eulerAngles.z >= max_roll && transform.eulerAngles.z <= 360 - max_roll) {
				transform.eulerAngles = new Vector3 (transform.eulerAngles.x, transform.eulerAngles.y, max_roll);
			} 

			m_rigidbody.velocity = transform.forward * Vector3.Magnitude (m_rigidbody.velocity);
		} 


		if (Input.GetKey (KeyCode.RightArrow)) {
			transform.Rotate(Vector3.forward, -angleZ, Space.Self);

			transform.Rotate(Vector3.up, angleY, Space.World);

			if (transform.eulerAngles.z <= 360 - max_roll && transform.eulerAngles.z >= max_roll ) {
				transform.eulerAngles = new Vector3 (transform.eulerAngles.x, transform.eulerAngles.y, 360 - max_roll);
			} 

			m_rigidbody.velocity = transform.forward * Vector3.Magnitude (m_rigidbody.velocity);
		}

		if (Input.GetKey (KeyCode.LeftArrow) == false && Input.GetKey (KeyCode.RightArrow) == false) {

			if (transform.eulerAngles.z > epsilonZAng && transform.eulerAngles.z > 180) {
				transform.Rotate (Vector3.forward, angleZ, Space.Self);
			}

				if (transform.eulerAngles.z > epsilonZAng && transform.eulerAngles.z <= 180) {
					transform.Rotate (Vector3.forward, -angleZ, Space.Self);
				}	
		}
			

	}

//	private bool RollPossible()
//	{
//		return
//			(0 <= transform.eulerAngles.z && transform.eulerAngles.z <= max_roll) ||
//			(0 <= transform.eulerAngles.z && transform.eulerAngles.z <= max_roll);
//	}

}