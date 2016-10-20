using UnityEngine;
using System.Collections;

/*

ENEMY CONTROLLER
================

Manage enemy movement and death.

*/

public class EnemyController : MonoBehaviour {

	// ==========================================
	// Attributes
	// ==========================================

	private Transform eTransform;
	private GameObject gameController;
	
	// Enemy attributes
	public float speed = 2f;
	public int enemyPoints = 200;
	
	// Movement follows the same rationale of moving platforms,
	// each enemy instance has those two points set in Unity GUI for easy reuse. 
	public Transform movementStart;
	public Transform movementEnd;
	private bool reverseMovement = false;
	

	// ==========================================
	// Object initialization
	// ==========================================
	void Start () {
		this.eTransform = GetComponent<Transform>();
		gameController = GameObject.Find("GameController");
	}
	
	// ==========================================
	// Game Lifecycle Updates
	// ==========================================
	void FixedUpdate () {
		
		// Prevent movement (and runtime errors)
		// if the enemy has no waypoints set
		if (!this.movementStart || !this.movementEnd) return;
		
		Debug.DrawLine(this.movementStart.position, 
				this.movementEnd.position);
		
		// Select a starting (or ending) point to 
		// calculate movement towards.
		Vector2 movementDestination;
		if (reverseMovement == false) {
			movementDestination = new Vector2(
				this.movementEnd.position.x,
				this.movementEnd.position.y);
		} else {
			movementDestination = new Vector2(
				this.movementStart.position.x,
				this.movementStart.position.y);
		}
		
		// Move the enemy towards it
		this.eTransform.position = Vector2.MoveTowards(
			new Vector2(this.eTransform.position.x, this.eTransform.position.y),
			movementDestination,
			this.speed * Time.deltaTime
		);

		// Flip the start/end waypoints if the enemy reaches either
		if (this.eTransform.position.x == movementDestination.x && this.eTransform.position.y == movementDestination.y) {
			reverseMovement = !reverseMovement;
		}

	}

	// Collision detection methods
	// =================================================
	
	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag("Bullet")) {
			other.gameObject.SendMessage("DestroyBullet");
			gameController.SendMessage("enemyDied");
			gameController.SendMessage("updateScore", this.enemyPoints);
			Destroy(this.gameObject);
		}
	}

}
