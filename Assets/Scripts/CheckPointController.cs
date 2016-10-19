using UnityEngine;
using System.Collections;

public class CheckPointController : MonoBehaviour {

	private Transform _transform;
	public Transform SpawnPoint;

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
			this.SpawnPoint.position = this._transform.position;
		}
	}

}
