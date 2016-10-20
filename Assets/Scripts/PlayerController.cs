using UnityEngine;

/*

PLAYER CONTROLLER
=================

Control player object movement, animations, physics and collisions.

*/

public class PlayerController : MonoBehaviour {

	// ==========================================
	// Attributes
	// ==========================================

	// References to player components
	private Transform pTransform;
	private Rigidbody2D pRigidbody;
	
	// Vector components for horizontal/vertical movement
	private float move;
	private float jump;

	// Positioning/gameflow flags
	private bool isFacingRight;
	private bool isGrounded;
	private bool playerWon;

	// Animation components
	private Animator animator;

	// Reference to the game controller
	public GameController gameController;

	// Movement parameters
	public float velocity = 20f;
	public float jumpForce = 400f;

	// This spawn point is updated after reaching checkpoints
	public Transform SpawnPoint;
	public Camera gameCamera;

	// Bullets
	public Transform bullet;
	public float bulletDistance = .3f;
	public float timeBetweenFires = .3f; // "Cooloff" time
	private float timeTilNextFire = 0.0f;

	[Header("SoundClips")]
	public AudioClip jumpSound;
	public AudioClip landSound;
	public AudioClip fireSound;
	public AudioClip emptySound;
	public AudioClip cockSound;
	public AudioClip deadSound;
	public AudioClip winSound;
	private AudioSource audioSource;

	// ==========================================
	// Object initialization
	// ==========================================
	void Start () {
		this.initialize();
	}

	// ==========================================
	// Game Lifecycle Updates
	// ==========================================
	void Update() {
			if (Input.GetKeyDown(KeyCode.Space) && !playerWon) {
				this.jump = 1f;
				audioSource.PlayOneShot(this.jumpSound);
			}
	}
	
	void FixedUpdate () {

		if (!playerWon){

			// Process ground movement
			if (this.isGrounded) {
				this.move = Input.GetAxis("Horizontal");
				if (this.move > 0f) {
					this.move = 1;
					this.animator.SetInteger("HeroState", 1);
					this.isFacingRight = true;
					this.flipHorizontal();
				} else if (this.move < 0f) {
					this.move = -1;
					this.animator.SetInteger("HeroState", 1);
					this.isFacingRight = false;
					this.flipHorizontal();
				} else {
					this.move = 0;
					this.animator.SetInteger("HeroState", 0);
				}

				// Process weapon firing
				if (Input.GetAxis ("Fire1") == 1) this.fire();
				timeTilNextFire -= Time.deltaTime;

				// Add movement to player's body
				this.pRigidbody.AddForce(new Vector2(
					this.move * this.velocity, 
					this.jump * this.jumpForce),
					ForceMode2D.Force);
			}
			else {
				// Player is airbourne, apply zero extra force and let gravity act.
				this.move = 0f;
				this.jump = 0f;
			}

			// Follow player with the camera
			this.gameCamera.transform.position = new Vector3(
				this.pTransform.position.x,
				this.pTransform.position.y,
				-10f);

		} else {
			// Player won, levitate it to infinity and end the game.
			this.pRigidbody.AddForce(new Vector2(0f, 
					1f * (this.jumpForce / 8f)),
					ForceMode2D.Force);
			gameController.gameOver();
		}

	}

	// Auxiliary methods
	// ==========================================	

	// Player initialization
	private void initialize(){
		this.pTransform = GetComponent<Transform>();
		this.pRigidbody = GetComponent<Rigidbody2D>();
		this.animator = GetComponent<Animator>();
		this.audioSource = GetComponent<AudioSource>();
		this.move = 0f;
		this.isFacingRight = true;
		this.isGrounded = false;
		this.playerWon = false;
	}

	// Flip sprite when changing direction
	private void flipHorizontal() {
		if (this.isFacingRight) {
			this.pTransform.localScale = new Vector2(1f,1f);
		}
		else {
			this.pTransform.localScale = new Vector2(-1f,1f);
		}
	}

	// Fire bullets
	private void fire(){
		if (timeTilNextFire <= 0) {
			if (gameController.playerAmmo == 0) {
				audioSource.PlayOneShot(this.emptySound);
			} else {
				// Determine the position for the fired bullet instance
				Vector3 bulletPos = this.pTransform.position;

				// Calculate the position of the bullet in front of the player
				bulletPos.x = bulletPos.x + (bulletDistance * this.pTransform.localScale.x);
				bulletPos.y += 0.16f;

				// Create the bullet instance and trigger sounds/game updates
				bullet.transform.localScale = this.pTransform.localScale;
				Instantiate (bullet, bulletPos, this.pTransform.rotation);
				this.animator.SetInteger("HeroState", 10);
				audioSource.PlayOneShot(this.fireSound);
				gameController.updateAmmoCount(-1);
			}
			timeTilNextFire = timeBetweenFires;
		}
	}

	// Collision detection methods
	// =================================================

	// Process physical collisions (w/ platforms, enemies)
	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag("DeathPlane") || other.gameObject.CompareTag("Enemy")) {
			audioSource.PlayOneShot(this.deadSound);
			gameController.updateLivesCount(-1);

			if (gameController.playerLives >= 0) {
				this.pTransform.position = this.SpawnPoint.position;
			} else {
				gameController.gameOver();
				Destroy(this.gameObject);
			}
		
		}
		
		if (other.gameObject.CompareTag("Platform")) {
			audioSource.PlayOneShot(this.landSound);
			this.animator.SetInteger("HeroState", 0);
		}
	}

	private void OnCollisionStay2D(Collision2D other) {
		if (other.gameObject.CompareTag("Platform")) {
			this.isGrounded = true;
		}
	}

	private void OnCollisionExit2D(Collision2D other) {
		this.isGrounded = false;
		this.animator.SetInteger("HeroState", 2);
	}

	// Process triggers (for pickups)
	private void OnTriggerEnter2D(Collider2D other) {

		if (other.gameObject.CompareTag("AmmoPickup")) {
			other.gameObject.SendMessage("DestroyPickup");
			gameController.updateAmmoCount(+6);
			gameController.updateScore(+50);
			audioSource.PlayOneShot(this.cockSound);
		}

		if (other.gameObject.CompareTag("Gem")) {
			other.gameObject.SendMessage("DestroyPickup");
			audioSource.PlayOneShot(this.winSound);
			gameController.updateScore(+3000);
			this.playerWon = true;
		}

	}

	// Public methods
	// ==========================================	

	// This method is used by moving platforms
	// to displace the player together with them as they move
	public void Displace(Vector3 displacement) {
		this.pTransform.position += displacement;
	}
}
