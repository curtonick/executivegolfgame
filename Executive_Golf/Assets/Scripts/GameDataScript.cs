using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Contains score and helpful values accessible from any script, any time

public class GameDataScript : MonoBehaviour {

	public int overallScore;
	public int overPar;
	public int[] holePars = { 4, 3, 5 }; //size is 3 for now

	//CurrentMode
	//0 -> Single Stroke
	//1 -> Single Speed
	//2 -> Double Stroke
	//3 -> Double Speed
	public int currentMode;
	public int currentHole;

	public int numOfPlayers(){
		if (currentMode == 0 || currentMode == 1) 
			return 1;
		else
			return 2;
		
	}

	public void clearScore(){
		overallScore = 0;
		overPar = 0;
		currentHole = 1;
	}

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (gameObject);
		overPar = 0;
		overallScore = 0;
		currentMode = 0;
		currentHole = 1;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
