using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*

GAME CONTROLLER
===============

Manages score, player lives and GUI updates.

*/


public class GameController : MonoBehaviour {

	public int initialPlayerLives = 5;
	public int initialPlayerAmmo = 0;
	public int maxPlayerAmmo = 6;

	public int playerLives;
	public int playerAmmo;
	public int playerScore = 0;

	public Canvas canvas;
	public Text scoreText;
	public Text gameOverText;

	// These sounds are handled here because the gameObject that produces them
	// is destroyed when they're supposed to play.
	[Header("SoundClips")]
	public AudioClip enemyDeadSound;
	public AudioClip gameOverSound;

	private AudioSource audioSource;

	// These arrays store references to the corresponding GUI objects,
	// to make updates easier. 	
	private GameObject[] ammo;
	private GameObject[] lives;

	void Start () {

		gameOverText.gameObject.SetActive(false);

		audioSource = GetComponent<AudioSource>();

		// Collect canvas elements to update later
		ammo = new GameObject[maxPlayerAmmo];
		for (int i = 0; i < maxPlayerAmmo; i++){
			ammo[i] = GameObject.Find("Ammo" + (i+1));
		}

		lives = new GameObject[initialPlayerLives];
		for (int i = 0; i < initialPlayerLives; i++){
			lives[i] = GameObject.Find("Life" + (i+1));
		}
	
		playerLives = initialPlayerLives;
		this.updateAmmoCount(initialPlayerAmmo);
	}
	
	// Update is called once per frame
	void Update () {
		updateUI();
	}

	private void updateUI(){
		scoreText.text = "Score: " + playerScore;
	}

	// Allow player to update the game lives count
	public int updateLivesCount(int delta) {
		playerLives += delta;
		for (int i = 0; i < initialPlayerLives; i++){
			lives[i].SetActive(i < playerLives); 
		}
		return this.playerLives;
	}

	// Allow player to update the ammo count
	public int updateAmmoCount(int delta) {
		playerAmmo += delta;
		if (playerAmmo > maxPlayerAmmo) playerAmmo = maxPlayerAmmo; 
		for (int i = 0; i < maxPlayerAmmo; i++){
			ammo[i].SetActive(i < playerAmmo); 
		}
		return this.playerAmmo;
	}

	// Allow player to update the score
	public void updateScore(int delta) {
		playerScore += delta;
	}

	// Update score and play sounds when enemies die
	public void enemyDied() {
		audioSource.PlayOneShot(this.enemyDeadSound);
	}

	// Update GUI when the game ends
	public void gameOver() {
		if (playerLives < 0) {
			gameOverText.text = "GAME OVER\n";
			audioSource.PlayOneShot(this.gameOverSound);
		} else {
			gameOverText.text = "YOU WON!\n";
		}
		gameOverText.text += "SCORE: " + playerScore;
		gameOverText.gameObject.SetActive(true);
	}
}
