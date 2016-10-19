using UnityEngine;
using System.Collections;

public class MovingPlatformController : MonoBehaviour {

	private Transform _transform;
	private bool reverseMovement = false;
	private bool isTransportingPlayer = false;

	private Vector2 positionDelta;
	private Vector3 platformDisplacement;
	
	public float Speed = 1f;
	public Transform MovementStart;
	public Transform MovementEnd;


	// Use this for initialization
	void Start () {
		this._transform = GetComponent<Transform>();
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
		
		this.positionDelta = Vector2.MoveTowards(
			new Vector2(this._transform.position.x, this._transform.position.y),
			movementDestination,
			this.Speed * Time.deltaTime
		);

		// Store the platform displacement, so it can be
		// applied to whatever is on top of it
		this.platformDisplacement = new Vector3(
					this.positionDelta.x - this._transform.position.x,
					this.positionDelta.y - this._transform.position.y,
					0);

		// If the platform is moving down and transporting a player,
		// the player must be moved first (in OnCollisionStay2D),
		// otherwise it will stop colliding for a fraction of second
		// and chaos will ensue.
		if (!isTransportingPlayer) this._transform.position = this.positionDelta;

		
		if (this._transform.position.x == movementDestination.x && this._transform.position.y == movementDestination.y) {
			reverseMovement = !reverseMovement;
		}

	}
	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag("Player")) isTransportingPlayer = true;
	}

	private void OnCollisionStay2D(Collision2D other) {
		if (other.gameObject.CompareTag("Player")) {
			// Only act on player if it is over the platform
			if (other.transform.position.y > this._transform.position.y) {
				// Apply the same platform displacement to the player...
				other.gameObject.SendMessage("Displace", this.platformDisplacement);
				// ...then to the platform.
				// This must happen in THIS ORDER to prevent bugs
				// when the platform is moving down.
				this._transform.position = this.positionDelta;
			}
		}
	}

	private void OnCollisionExit2D(Collision2D other) {
		if (other.gameObject.CompareTag("Player")) isTransportingPlayer = false;
	}

}
