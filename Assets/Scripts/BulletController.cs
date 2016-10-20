using UnityEngine;
using System.Collections;

/*

BULLET CONTROLLER
=================

Moves bullets around, destroys them on contact.

*/

public class BulletController : MonoBehaviour {

	// ==========================================
	// Attributes
	// ==========================================
	public float speed;
	private float direction;

	// ==========================================
	// Object initialization
	// ==========================================
	void Start () {
		Destroy (gameObject, 1f); // Auto-destroy after a second
	}
	
	// ==========================================
	// Game Lifecycle Updates
	// ==========================================
	void Update () {
			// The "Vector3.right" means only a (1,0,0) vector, that might be flipped if
			// the bullet is traveling left. In this case, its x-rotation will be -1.
			this.transform.Translate (Vector3.right * Time.deltaTime * this.speed);
	}


	// Collision detection methods
	// =================================================

	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Platform")) {
			Destroy(gameObject);
		}
	}

	// Public methods
	// ==========================================	

	// Allow collided objects to destroy the bullet that hit them
	public void DestroyBullet(){
		Destroy (gameObject);
	}
}
