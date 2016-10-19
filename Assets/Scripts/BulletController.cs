using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

	// How fast will it move
	public float speed;
	private float direction;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, 1f); // Auto-destroy after some time
	}
	
	// Update is called once per frame
	void Update () {
			this.transform.Translate (Vector3.right * Time.deltaTime * this.speed);
	}

	// Allow collided objects to destroy the bullet that hit them
	public void DestroyBullet(){
		Destroy (gameObject);
	}

	// Auto-destroy bullet if it hits a wall or platform
	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag("Wall") || 
				other.gameObject.CompareTag("Platform")) {
			Destroy(gameObject);
		}
	}
}
