using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	private Transform _transform;
	
	public float Speed = 2f;
	public int enemyPoints = 200;
	public Transform MovementStart;
	public Transform MovementEnd;
	private bool reverseMovement = false;
	private GameObject gameController;

	// Use this for initialization
	void Start () {
		this._transform = GetComponent<Transform>();
		gameController = GameObject.Find("GameController");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (!this.MovementStart || !this.MovementEnd) return;
		
		Debug.DrawLine(this.MovementStart.position, 
				this.MovementEnd.position);
		
		Vector2 movementDestination;
		if (reverseMovement == false) {
			movementDestination = new Vector2(
				this.MovementEnd.position.x,
				this.MovementEnd.position.y);
		} else {
			movementDestination = new Vector2(
				this.MovementStart.position.x,
				this.MovementStart.position.y);
		}

		this._transform.position = Vector2.MoveTowards(
			new Vector2(this._transform.position.x, this._transform.position.y),
			movementDestination,
			this.Speed * Time.deltaTime
		);

		if (this._transform.position.x == movementDestination.x && this._transform.position.y == movementDestination.y) {
			reverseMovement = !reverseMovement;
		}

	}

	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag("Bullet")) {
			other.gameObject.SendMessage("DestroyBullet");
			gameController.SendMessage("enemyDied");
			gameController.SendMessage("updateScore", this.enemyPoints);
			Destroy(this.gameObject);
		}
	}

}
