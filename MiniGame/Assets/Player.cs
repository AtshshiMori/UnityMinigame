﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Player : MonoBehaviour {

	public float jumpPower;
	public float boostPower;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Jump")) {
			GetComponent<Rigidbody> ().velocity = new Vector3 (0, jumpPower, 0);
		}
		if (Input.GetButtonDown ("Boost")) {
			GetComponent<Rigidbody>().AddForce( new Vector3(boostPower, 0, 0), ForceMode.VelocityChange);
		}
		if (transform.position.x < -10) {
			SceneManager.LoadScene ("Main");
		}
	}
}
