using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {

	//Logic for main menu

	bool increasingRot = false;

	// Use this for initialization
	GameObject gameplayHud;
	GameObject pinPreview;
	GameObject credits;
	GameObject mainMenuCanvas;
	GameObject rangeResultPanel;
	Text rangeResultScore;
	private bool creditsActive = false;
	GameObject player2Score;

	//Set Range elements to not be active
	public void BeginMenu () {
		mainMenuCanvas = GameObject.Find ("MainMenu");
		rangeResultPanel = GameObject.Find ("RangeResult");
		rangeResultScore = GameObject.Find ("RangeScore").GetComponent<Text>();
		gameplayHud = GameObject.Find ("GameplayHUD");
		pinPreview = GameObject.Find ("PinPreview");
		credits = GameObject.Find ("Credits");
		player2Score = GameObject.Find ("Player2");

		credits.SetActive (false);
		gameplayHud.SetActive (false);
		pinPreview.SetActive (false);
		GameObject.Find ("Ball").GetComponent<RangeBallRigidBody> ().enabled = false;
		rangeResultPanel.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		//Subtle camera rotation
		if (increasingRot) {
			transform.Rotate (new Vector3 (0, 0.06f, 0));
			if (transform.eulerAngles.y > 45.0f && transform.eulerAngles.y < 46.0f)
				increasingRot = false;
		} else {
			transform.Rotate (new Vector3 (0, -0.06f, 0));
			if (transform.eulerAngles.y < 330.0f && transform.eulerAngles.y > 329.0f)
				increasingRot = true;
		}
	
	}

	public void RangeStart(){
		//Load necessary objects (ball, cameras, hud...) for range play
		rangeResultPanel.SetActive (false);
		GameObject ball = GameObject.Find ("Ball");
		gameplayHud.SetActive (true);
		pinPreview.SetActive (true);
		GameObject.Find ("FollowCamera").GetComponent<Camera>().enabled = true;
		ball.GetComponent<RangeBallRigidBody> ().enabled = true;
		credits.SetActive (false);
		creditsActive = false;
		mainMenuCanvas.SetActive (false);
		gameObject.SetActive (false);
		GameObject.Find ("MenuTheme").GetComponent<AudioSource> ().Stop ();
	}

	public void RangeResults(){
		rangeResultPanel.SetActive (true);
		gameplayHud.SetActive (false);
		pinPreview.SetActive (false);
		GameObject.Find ("Ball").GetComponent<RangeBallRigidBody> ().enabled = false;
		GameObject.Find ("HighCamera").GetComponent<Camera>().enabled = true;
		GameObject.Find ("FollowCamera").GetComponent<Camera>().enabled = false;
		rangeResultScore.text = GameObject.Find("Ball").GetComponent<RangeBallRigidBody> ().rangePoints + " points";

	}

	public void RangeEnd(){
		//deactivate range objects, activate main menu
		GameObject.Find ("HighCamera").GetComponent<Camera>().enabled = false;
		rangeResultPanel.SetActive (false);
		gameObject.SetActive (true);
		mainMenuCanvas.SetActive (true);
		GameObject.Find ("MenuTheme").GetComponent<AudioSource> ().Play ();
	}

	//Main menu button functions
	public void CreditsShow(){
		creditsActive = !creditsActive;
		credits.SetActive (creditsActive);
	}

	public void QuitGame(){
		Application.Quit ();
	}

	public void LoadTutorial(){
		SceneManager.LoadScene ("tutorial");
	}

	public void LoadHole1(){
		GameObject.Find ("GameData").GetComponent<GameDataScript> ().currentMode = 0;
		GameObject.Find ("GameData").GetComponent<GameDataScript> ().clearScore ();
		player2Score.SetActive (false);
		SceneManager.LoadScene ("hole1");
	}

	public void LoadMultiHole1(){
		GameObject.Find ("GameData").GetComponent<GameDataScript> ().currentMode = 2;
		GameObject.Find ("GameData").GetComponent<GameDataScript> ().clearScore ();
		GameObject.Find ("GameDatap2").GetComponent<GameDataScript> ().clearScore ();
		player2Score.SetActive (true);
		SceneManager.LoadScene ("hole1multi");
	}

}
