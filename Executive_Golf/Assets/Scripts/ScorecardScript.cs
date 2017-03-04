using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScorecardScript : MonoBehaviour {

	//Update the OverallScore text in toprightpanel if we are playing a hole
	void OnLevelWasLoaded(int level) {
		string n = SceneManager.GetActiveScene ().name;
		GameDataScript player1 = GameObject.Find ("GameData").GetComponent<GameDataScript> ();
		if (n.Equals ("hole1") || n.Equals ("hole2") || n.Equals ("hole3")) {
			Text score = GameObject.Find ("OverallScore").GetComponent<Text> ();
			if (player1.overPar == 0) {
				score.text = "Even";
			} else if (player1.overPar > 0) {
				score.text = player1.overPar + " Over";
			} else {
				score.text = Mathf.Abs (player1.overPar) + " Under";
			}
		}

	}

	//enable the canvas, play the music
	public void showScoreCard(){
		updateGameplayHUDAndScoreCard ();
		if (SceneManager.GetActiveScene ().name.Equals ("hole3")) { //special case when done with round
			GameObject.Find ("NextText").GetComponent<Text> ().text = "Done";
		}
		gameObject.GetComponent<Canvas> ().enabled = true;
		GameObject.Find ("ScoreMusic").GetComponent<AudioSource> ().PlayDelayed (0.3f);
	}

	//hide the canvas, stop the music
	public void hideScoreCard(){
		gameObject.GetComponent<Canvas> ().enabled = false;
		GameObject.Find ("ScoreMusic").GetComponent<AudioSource> ().Stop ();
	}

	//reset player score fields on score card
	public void clearScoreCard(){
		GameDataScript player1 = GameObject.Find ("GameData").GetComponent<GameDataScript> ();
		player1.overallScore = 0;
		player1.overPar = 0;
		GameObject.Find ("H1Score").GetComponent<Text> ().text = "";
		GameObject.Find ("H2Score").GetComponent<Text> ().text = "";
		GameObject.Find ("H3Score").GetComponent<Text> ().text = "";
		GameObject.Find ("AllHoleScore").GetComponent<Text> ().text = "";
	}

	//deals with loading the next hole? Located here because scorecard button does next hole
	public void nextHole(){
		hideScoreCard ();
		if(SceneManager.GetActiveScene().name.Equals("hole1")){
			SceneManager.LoadScene("hole2");
		}else if(SceneManager.GetActiveScene().name.Equals("hole2")){
			SceneManager.LoadScene("hole3");
		}else if(SceneManager.GetActiveScene().name.Equals("hole3")){
			clearScoreCard (); //reset scores, round complete
			SceneManager.LoadScene("mainmenu");
		}

		if(SceneManager.GetActiveScene().name.Equals("hole1multi")){
			SceneManager.LoadScene("hole2multi");
		}else if(SceneManager.GetActiveScene().name.Equals("hole2multi")){
			SceneManager.LoadScene("hole3multi");
		}else if(SceneManager.GetActiveScene().name.Equals("hole3multi")){
			clearScoreCard (); //reset scores, round complete
			SceneManager.LoadScene("mainmenu");
		}


	}

	private void updateGameplayHUDAndScoreCard(){
		GameDataScript player1 = GameObject.Find ("GameData").GetComponent<GameDataScript> ();
		if (player1.currentMode == 0 || player1.currentMode == 1) {
			int holeStrokesp1 = GameObject.Find ("Ball").GetComponent<BallRigidBody> ().strokeCount;

			//Actually update GameData and Scorecard text
			UpdateHoleScore (player1, player1.currentHole, holeStrokesp1);

			//Advance to next hole
			player1.currentHole++;
		} else if (player1.currentMode == 2 || player1.currentMode == 3) {
			GameDataScript player2 = GameObject.Find ("GameDatap2").GetComponent<GameDataScript> ();
			int holeStrokesp1 = GameObject.Find ("Ballp1").GetComponent<MultiBallRigidBodyP1> ().strokeCount;
			int holeStrokesp2 = GameObject.Find ("Ballp2").GetComponent<MultiBallRigidBodyP2> ().strokeCount;

			//Actually update GameData and Scorecard text
			UpdateHoleScore (player1, player1.currentHole, holeStrokesp1);
			UpdateHoleScore (player2, player2.currentHole, holeStrokesp2);

			//Advance to next hole
			player1.currentHole++;
			player2.currentHole++;
		}


	}


	void UpdateHoleScore(GameDataScript player, int holeNum, int holeStrokes) {
		Color scoreColor;
		string p2ifp2;

		if (player.name == "GameData") {
			p2ifp2 = "";
		} else {
			p2ifp2 = "p2";
		}

		int holePar = player.holePars [holeNum - 1];
		int amtOverPar = holeStrokes - holePar;
		player.overPar += amtOverPar;
		if (amtOverPar > 0)
			scoreColor = Color.blue;
		else if (amtOverPar == 0)
			scoreColor = Color.black;
		else
			scoreColor = Color.red;
		GameObject.Find ("H" + holeNum + "Score" + p2ifp2).GetComponent<Text> ().text = holeStrokes + "";
		GameObject.Find ("H" + holeNum + "Score" + p2ifp2).GetComponent<Text> ().color = scoreColor;

		int overPar = player.overPar;
		Text score = GameObject.Find("OverallScore").GetComponent<Text>();

		if (overPar == 0) {
			score.text = "Even";
		} else if (overPar > 0) {
			score.text = overPar + " Over";
		} else {
			score.text = Mathf.Abs (overPar) + " Under";
		}

		player.overallScore += holeStrokes;
		GameObject.Find ("AllHoleScore" + p2ifp2).GetComponent<Text> ().text = player.overallScore + "";

	}
		


	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Canvas> ().enabled = false;
		DontDestroyOnLoad (gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
