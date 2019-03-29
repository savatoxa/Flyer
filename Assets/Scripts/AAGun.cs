using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AAGun : MonoBehaviour {

	public Transform Dummy_gun;
	public GameObject Bullet;
	public GameObject Flyer;
	public float bulletVel;
	public float distShoot = 10.0f;
	public float fireRate = 1.0f;
	private float fireDelay; 
	private float lastShot = 0.0f;

	// Use this for initialization
	void Start () {
		fireDelay = 1.0f / fireRate;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (Vector3.Magnitude (Dummy_gun.transform.position - Flyer.transform.position) <= distShoot) 
		{
			Fire ();
		}
		//Debug.DrawLine (origin, destination, Color.red);
	}

	private void Fire() 
	{
		if (Time.time > fireDelay + lastShot)
		{
			var bullet = Instantiate (Bullet);
			bullet.transform.position = Dummy_gun.transform.position;
			var dirShoot =  Vector3.Normalize(Flyer.transform.position - Dummy_gun.transform.position);
			bullet.GetComponent<Rigidbody> ().velocity =  dirShoot * bulletVel;

			//Debug.Log (Vector3.Magnitude(dirShoot));
			//Debug.Log (Vector3.Magnitude(bullet.GetComponent<Rigidbody> ().velocity));

			lastShot = Time.time;
		}
	}

	void OnTriggerEnter (Collider other)
	{
		Destroy (gameObject);
	}

}
