using UnityEngine;
using System.Text.RegularExpressions;

/*

MOVING PLATFORM CONTROLLER
==========================

Makes a platform move back and forth between two points.

(Prof said this would be hard to implement. Took me one hour. ¯\_(ツ)_/¯ )

*/

public class MovingPlatformController : MonoBehaviour {

	// ==========================================
	// Attributes
	// ==========================================

	// References to platform components
	private Transform mpTransform;
	
	// Movement flags and attributes
	private bool reverseMovement = false;
	private bool isTransportingPlayer = false;
	public float speed = 1f;

	// Platform displacement per frame
	private Vector2 positionDelta;
	private Vector3 platformDisplacement;
	
	// Those two transforms mark the line where the platform will travel.
	// They are set on Unity's GUI for each prefab instance, 
	// to allow easy reuse across the level.
	public Transform movementStart;
	public Transform movementEnd;


	// ==========================================
	// Object initialization
	// ==========================================
	void Start () {
		this.mpTransform = GetComponent<Transform>();

		string instanceNumber = this.getInstanceNumber(gameObject.ToString());

		// Attempt to infer the waypoints using object names
		GameObject found;
		if (!this.movementStart) {
			found = GameObject.Find("MP" + instanceNumber + "Start");
			if (found) this.movementStart = found.transform;
		}

		if (!this.movementEnd) {
			found = GameObject.Find("MP" + instanceNumber + "End");
			if (found) this.movementEnd = found.transform;
		}
	}
	
	// ==========================================
	// Game Lifecycle Updates
	// ==========================================
	void FixedUpdate () {
		
		// Prevent movement (and runtime errors)
		// if the platform has no waypoints set
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
		
		// Move the platform towards it
		this.positionDelta = Vector2.MoveTowards(
			new Vector2(this.mpTransform.position.x, this.mpTransform.position.y),
			movementDestination,
			this.speed * Time.deltaTime
		);

		// Store the platform displacement, so it can be
		// applied to whatever is on top of it
		this.platformDisplacement = new Vector3(
					this.positionDelta.x - this.mpTransform.position.x,
					this.positionDelta.y - this.mpTransform.position.y,
					0);

		// If the platform is moving down and transporting a player,
		// the player must be moved first (in OnCollisionStay2D),
		// otherwise it will stop colliding for a fraction of second
		// and chaos will ensue.
		if (!isTransportingPlayer) this.mpTransform.position = this.positionDelta;

		// Flip the start/end waypoints if the platform reaches either
		if (this.mpTransform.position.x == movementDestination.x && this.mpTransform.position.y == movementDestination.y) {
			reverseMovement = !reverseMovement;
		}

	}

	// Collision detection methods
	// =================================================

	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag("Player")) isTransportingPlayer = true;
	}

	private void OnCollisionExit2D(Collision2D other) {
		if (other.gameObject.CompareTag("Player")) isTransportingPlayer = false;
	}

	private void OnCollisionStay2D(Collision2D other) {
		if (other.gameObject.CompareTag("Player")) {
			// Only act on player if it is over the platform
			if (other.transform.position.y > this.mpTransform.position.y) {
				// Apply the same platform displacement to the player...
				other.gameObject.SendMessage("Displace", this.platformDisplacement);
				// ...then to the platform.
				// This must happen in THIS ORDER to prevent bugs
				// when the platform is moving down.
				this.mpTransform.position = this.positionDelta;
			}
		}
	}

	// Auxiliary methods
	// ==========================================

	// Extract the instance number from the prefab obj name
	// (e.g.: returns 29 for "Platform (29)")
	private string getInstanceNumber(string objName) {
		Regex rx = new Regex(@"([0-9]+)");
		MatchCollection matches = rx.Matches(objName);
		return matches[0].ToString();
	}	

}
