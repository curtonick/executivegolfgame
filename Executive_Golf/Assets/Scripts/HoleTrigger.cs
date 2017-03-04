using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
//Determines if the ball enters the hole

public class HoleTrigger : MonoBehaviour {

	private float enterTime = 0;
	AudioSource holed;
	bool holedout = false;
	bool holedout2 = false;
	bool done = false;
	bool soundplayed = false;
	bool cardShown = false;
	Collider enteredObject;
	Collider oldEnteredObject;
	int playersHoled;
	int playersPlaying;

	void Start(){
		playersHoled = 0;
		playersPlaying = GameObject.Find ("GameData").GetComponent<GameDataScript> ().numOfPlayers();

	}


	//called in update since we want to do this even after ball is deactivated, not in trigger
	void Holed_Out(){
		
		//In the hole condition should go here
		if (!done) {
			Physics.IgnoreCollision (enteredObject, GameObject.Find ("Green").GetComponent<Collider> ()); //Make ball drop into hole
			if (!soundplayed) {
				if (GameObject.Find ("holed") != null) {
					GameObject.Find ("holed").GetComponent<AudioSource> ().Play ();
				} else if (holedout2 && GameObject.Find ("holedp2") != null) {
					GameObject.Find ("holedp2").GetComponent<AudioSource> ().Play ();
				}
				//play sound only once
				soundplayed = true;
			}
			if (Time.time > enterTime + 0.2f && Time.time < enterTime + 0.8f) {

				//TODO: Code to simplify on game mode additions.

				 //disable ball script
				if (enteredObject.name.Equals ("Ball")) {
					enteredObject.GetComponent<BallRigidBody> ().enabled = false;
					GameObject.Find ("FollowCamera").GetComponent<Camera> ().enabled = false;
					GameObject.Find ("HighCamera").GetComponent<Camera> ().enabled = true;
				} else if (enteredObject.name.Equals ("Ballp1")) {
					enteredObject.GetComponent<MultiBallRigidBodyP1> ().enabled = false;
					GameObject.Find ("FollowCamerap1").GetComponent<Camera> ().enabled = false;
					GameObject.Find ("HighCamerap1").GetComponent<Camera> ().enabled = true;
				} else if (enteredObject.name.Equals ("Ballp2")) {
					enteredObject.GetComponent<MultiBallRigidBodyP2> ().enabled = false;
					GameObject.Find("FollowCamerap2").GetComponent<Camera> ().enabled = false;
					GameObject.Find ("HighCamerap2").GetComponent<Camera> ().enabled = true;
				}

				enteredObject.attachedRigidbody.velocity = Vector3.zero;
				enteredObject.attachedRigidbody.angularVelocity = Vector3.zero;
				if (playersHoled >= playersPlaying) {
					if (!cardShown && !SceneManager.GetActiveScene ().name.Equals ("tutorial")) { //Display the ending score if not tutorial level
						cardShown = true;
						GameObject.Find ("Scorecard").GetComponent<ScorecardScript> ().showScoreCard ();
					}
				}
				oldEnteredObject.gameObject.transform.position = new Vector3 (-100, -100, -100);
				oldEnteredObject.attachedRigidbody.isKinematic = true;

			} 
			if (Time.time > enterTime + 1.2f) {
				if (SceneManager.GetActiveScene ().name.Equals ("tutorial")) {
					SceneManager.LoadScene ("mainmenu");
				}
				soundplayed = false;

			}
		}



	}

	// Use this for initialization
	void OnTriggerEnter (Collider other){
		enterTime = Time.time;

	}

	void OnTriggerStay (Collider other){
		if (other.attachedRigidbody.velocity.magnitude < 6.0f || other.attachedRigidbody.velocity.y < -1.0) {
			enteredObject = other;
			if (holedout && enteredObject != oldEnteredObject) {
				playersHoled = 2;
				holedout = true;
				holedout2 = true;
			}
			if (!holedout) {
				oldEnteredObject = other;
				playersHoled = 1;
				holedout = true;
			}
		}

	}

	void OnTriggerExit (Collider other){
		if (other.attachedRigidbody.velocity.magnitude > 6.0f) {
			other.transform.LookAt(GameObject.Find("CupHeight").transform);
			other.attachedRigidbody.AddRelativeForce (0,0, 5.0f, ForceMode.Impulse);
		}


	}

	void Update(){
		if (holedout)
			Holed_Out ();
		if (holedout2)
			Holed_Out ();
	}

}
