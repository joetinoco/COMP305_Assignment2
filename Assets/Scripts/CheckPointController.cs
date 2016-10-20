using UnityEngine;

/*

CHECKPOINT CONTROLLER
=====================

A hidden collider that moves the spawn point forward when the player reaches it.

*/

public class CheckPointController : MonoBehaviour {

	// ==========================================
	// Attributes
	// ==========================================	

	private Transform cpTransform;

	// Reference to the single spawn point in the game
	public Transform spawnPoint;

	// ==========================================
	// Object initialization
	// ==========================================
	void Start () {
		this.cpTransform = GetComponent<Transform>();
		this.spawnPoint = GameObject.FindWithTag("SpawnPoint").transform;
	}

	// Collision detection methods
	// =================================================

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag("Player")) {
			this.spawnPoint.position = this.cpTransform.position;
		}
	}

}
