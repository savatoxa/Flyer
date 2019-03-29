using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDestroy : MonoBehaviour {

	public float Delay = 10.0f;

	private float _timeLeft;

	// Use this for initialization
	void Start () {
		_timeLeft = Delay;
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.Rotate (new Vector3 (180, 360, 720) * Time.deltaTime);

		_timeLeft -= Time.deltaTime;
		if (_timeLeft <= 0) {
			Destroy (gameObject);
		}
	}
}
