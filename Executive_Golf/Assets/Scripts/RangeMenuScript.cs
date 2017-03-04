using UnityEngine;
using System.Collections;

public class RangeMenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void EndRangeButton(){
		GameObject.Find ("MainMenu").GetComponent<MainMenuScript> ().RangeResults ();

	}
}
