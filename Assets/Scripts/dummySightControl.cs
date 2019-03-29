using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dummySightControl : MonoBehaviour {

	public Gun gun;

	// Use this for initialization
	void Start () {
		transform.position = gun.worldPos;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = gun.worldPos;
	}
}
