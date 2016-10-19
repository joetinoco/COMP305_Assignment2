using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private Transform _transform;
	private Rigidbody2D _rigidbody;
	private float _move;
	private float _jump;
	private bool _isFacingRight;
	private bool _isGrounded;
	private bool _playerWon;
	private Animator _animator;
	public GameController gameController;

	public float velocity = 20f;
	public float jumpForce = 400f;
	public Transform SpawnPoint;
	public Camera camera;

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

	// Use this for initialization
	void Start () {
		this._initialize();
	}

	void Update() {
			if (Input.GetKeyDown(KeyCode.Space) && !_playerWon) {
				this._jump = 1f;
				audioSource.PlayOneShot(this.jumpSound);
			}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (!_playerWon){

			if (this._isGrounded) {
				// Check if input for movement is present
				this._move = Input.GetAxis("Horizontal");
				if (this._move > 0f) {
					this._move = 1;
					this._animator.SetInteger("HeroState", 1);
					this._isFacingRight = true;
					this._flip();
				} 
				else if (this._move < 0f) {
					this._move = -1;
					this._animator.SetInteger("HeroState", 1);
					this._isFacingRight = false;
					this._flip();
				} 
				else {
					this._move = 0;
					this._animator.SetInteger("HeroState", 0);
				}

				if (Input.GetAxis ("Fire1") == 1) this._fire();
				timeTilNextFire -= Time.deltaTime;

				this._rigidbody.AddForce(new Vector2(
					this._move * this.velocity, 
					this._jump * this.jumpForce),
					ForceMode2D.Force);
			}
			else {
				// Not grounded
				this._move = 0f;
				this._jump = 0f;
			}

			// Follow player with the camera
			this.camera.transform.position = new Vector3(
				this._transform.position.x,
				this._transform.position.y,
				-10f);

		} else {
			// Player won, levitate it to infinity and end the game.
			this._rigidbody.AddForce(new Vector2(0f, 
					1f * (this.jumpForce / 8f)),
					ForceMode2D.Force);
			gameController.gameOver();
		}

	}

	private void _initialize(){
		this._transform = GetComponent<Transform>();
		this._rigidbody = GetComponent<Rigidbody2D>();
		this._animator = GetComponent<Animator>();
		this.audioSource = GetComponent<AudioSource>();
		this._move = 0f;
		this._isFacingRight = true;
		this._isGrounded = false;
		this._playerWon = false;
	}

	private void _flip() {
		if (this._isFacingRight) {
			this._transform.localScale = new Vector2(1f,1f);
		}
		else {
			this._transform.localScale = new Vector2(-1f,1f);
		}
	}

	// Collision detection methods
	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag("DeathPlane") || other.gameObject.CompareTag("Enemy")) {
			audioSource.PlayOneShot(this.deadSound);
			gameController.updateLivesCount(-1);
			if (gameController.playerLives >= 0) {
				this._transform.position = this.SpawnPoint.position;
			} else {
				gameController.gameOver();
				Destroy(this.gameObject);
			}
		}
		
		if (other.gameObject.CompareTag("Platform")) {
			audioSource.PlayOneShot(this.landSound);
			this._animator.SetInteger("HeroState", 0);
		}
	}

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
			this._playerWon = true;
		}

	}

	private void OnCollisionStay2D(Collision2D other) {
		if (other.gameObject.CompareTag("Platform")) {
			this._isGrounded = true;
		}
	}

	private void OnCollisionExit2D(Collision2D other) {
		this._isGrounded = false;
		this._animator.SetInteger("HeroState", 2);
	}

	// Fire bullets
	private void _fire(){
		if (timeTilNextFire <= 0) {
			if (gameController.playerAmmo == 0) {
				audioSource.PlayOneShot(this.emptySound);
			} else {
				Vector3 bulletPos = this._transform.position;

				// Determine the bullet angle
				float rotationAngle = this._transform.localEulerAngles.z - 90;

				// Calculate the position of the bullet in front of the player
				bulletPos.x = bulletPos.x + (bulletDistance * this.transform.localScale.x);
				bulletPos.y += 0.16f;

				// Create the bullet instance
				bullet.transform.localScale = this._transform.localScale;
				Instantiate (bullet, bulletPos, this.transform.rotation);
				this._animator.SetInteger("HeroState", 10);
				audioSource.PlayOneShot(this.fireSound);
				gameController.updateAmmoCount(-1);
			}
			timeTilNextFire = timeBetweenFires;
		}

	}

	// This public method is used by moving platforms
	// to displace the player together with them as they move
	public void Displace(Vector3 displacement) {
		this._transform.position += displacement;
	}
}
