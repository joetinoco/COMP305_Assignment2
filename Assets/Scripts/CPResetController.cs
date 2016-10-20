using UnityEngine;

/*

CHECKPOINT RESET CONTROLLER
============================

Resets the spawn point to a definite location (set using the Unity GUI).
Useful when the enemy falls into an earlier section of the level.

*/

public class CPResetController : MonoBehaviour {

	// ==========================================
	// Attributes
	// ==========================================	

	private Transform spawnPoint;

	// The destination checkpoint the player should be set to
	// if it reaches this CP resetter.
	// Set this using Unity's GUI.
	public Transform destinationCheckpoint;

	// ==========================================
	// Object initialization
	// ==========================================
	void Start () {
		this.spawnPoint = GameObject.FindWithTag("SpawnPoint").transform;
	}

	// Collision detection methods
	// =================================================
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag("Player")) {
			this.spawnPoint.position = this.destinationCheckpoint.position;
		}
	}

}
