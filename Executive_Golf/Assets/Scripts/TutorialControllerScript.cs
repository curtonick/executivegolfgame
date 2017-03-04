using UnityEngine;
using System.Collections;

public class TutorialControllerScript : MonoBehaviour {

	//Cycles through the five pages of text for the tutorial hole

	GameObject page1;
	GameObject page2;
	GameObject page3;
	GameObject page4;
	GameObject page5;
	BallRigidBody ballrbS;
	bool page2Read = false;
	bool page3Read = false;
	bool page4Read = false;
	bool done = false;
	float page5time;

	// Use this for initialization
	void Start () {
		page1 = GameObject.Find ("TutPage1");
		page2 = GameObject.Find ("TutPage2");
		page3 = GameObject.Find ("TutPage3");
		page4 = GameObject.Find ("TutPage4");
		page5 = GameObject.Find ("TutPage5");
		ballrbS = GameObject.Find ("Ball").GetComponent<BallRigidBody> ();

		page2.SetActive (false);
		page3.SetActive (false);
		page4.SetActive (false);
		page5.SetActive (false);
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!done) {
			if (Input.GetKeyDown (KeyCode.W)) { //page 1 done when wind is checked with w
				page1.SetActive (false);
				page2.SetActive (true);
			}
			if (Input.GetKeyDown (KeyCode.Space)) { //page 2 done when swing started with space
				page1.SetActive (false);
				page2.SetActive (false);
				page3.SetActive (true);
				page2Read = true;
			}
			if (page2Read && ballrbS.launched) { //page 3 done when shot is taken
				page1.SetActive (false);
				page2.SetActive (false);
				page3.SetActive (false);
				page4.SetActive (true);
				page3Read = true;
			}
			if (page3Read && !ballrbS.launched) { //page 4 done when shot stops
				page1.SetActive (false);
				page2.SetActive (false);
				page3.SetActive (false);
				page4.SetActive (false);
				page5.SetActive (true);
				page4Read = true;
			}
			if (page4Read && Input.GetKeyDown(KeyCode.Space)) { //page 5 done next swing started
				page1.SetActive (false);
				page2.SetActive (false);
				page3.SetActive (false);
				page4.SetActive (false);
				page5.SetActive (false);
				done = true;
			}
		}
	
	}
}
