﻿using UnityEngine;
using System.Collections;

public class CPResetController : MonoBehaviour {

	private Transform _transform;
	private Transform SpawnPoint;
	public Transform DestinationCheckpoint;

	// Use this for initialization
	void Start () {
		this._transform = GetComponent<Transform>();
		this.SpawnPoint = GameObject.FindWithTag("SpawnPoint").transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag("Player")) {
			this.SpawnPoint.position = this.DestinationCheckpoint.position;
		}
	}

}
