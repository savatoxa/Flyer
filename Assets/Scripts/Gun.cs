using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

	private static int HitCollisionMask;

	public Transform Dummy_nose;
	public float ShootDistance = 300.0f;
	public Camera flyerCamera;

	public GameObject Bullet;
	public float BulletSpeed = 1.0f;
	public float fireRate = 1.0f;
	private float lastShot = 0.0f;
	private float fireDelay;
	private float angDisp = 1.0f;
	public Vector3 viewPos;
	public Vector3 worldPos;

	// Use this for initialization
	void Start () {
		HitCollisionMask = 1 << LayerMask.NameToLayer ("Ground");
		fireDelay = 1.0f / fireRate;
	}
	
	// Update is called once per frame
	void Update () {

		var origin = Dummy_nose.transform.position;
		var direction = transform.forward;
		RaycastHit hitInfo;

		if (Physics.Raycast (origin, direction, out hitInfo, ShootDistance, HitCollisionMask)) {
				worldPos = hitInfo.point;
		} 
		else {
				worldPos = origin + ShootDistance * direction;
			}
			

		if (Input.GetKey (KeyCode.Space)) {
			Fire ();
		}
	}

	private void Fire()
	{
		if (Time.time > fireDelay + lastShot) {
			
			//Debug.DrawLine (Dummy_nose.transform.position, 10.0f * Dummy_nose.transform.forward + Dummy_nose.transform.position);

			var DummyNoseOrientInit = Dummy_nose.transform.localEulerAngles;
			Dummy_nose.transform.localEulerAngles = new Vector3 (Random.Range(-angDisp, angDisp), Random.Range(-angDisp, angDisp), Random.Range(-angDisp, angDisp));

			var bullet = Instantiate (Bullet);
			bullet.transform.position = Dummy_nose.transform.position;
			bullet.GetComponent<Rigidbody> ().velocity = GetComponent<Rigidbody> ().velocity + BulletSpeed * Dummy_nose.transform.forward;

			Dummy_nose.transform.localEulerAngles = DummyNoseOrientInit;
			lastShot = Time.time;
		}
	}
}
