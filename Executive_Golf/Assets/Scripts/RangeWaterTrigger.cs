using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RangeWaterTrigger : MonoBehaviour {

	//Similar to waterTrigger, but deducts points

	private float enterTime = 0;
	AudioSource holed;
	bool inWater = false;
	Rigidbody ballrb;
	Camera follow;
	bool soundplayed = false;
	Vector3 entryCameraPosition;
	Vector3 entryBallPosition;

	void Start(){
		ballrb = GameObject.Find ("Ball").GetComponent<Rigidbody> ();
		follow = GameObject.Find ("FollowCamera").GetComponent<Camera> ();
		entryCameraPosition = new Vector3 (0, 0, 0);
	}

	void Water_Effect (){
			if (!soundplayed) {
				GameObject.Find ("splash").GetComponent<AudioSource> ().Play();
				soundplayed = true;
			}
			if (Time.time < enterTime + 1.5f) {
				//Move ball below map to ensure it keeps moving
				ballrb.transform.position = new Vector3(entryBallPosition.x, entryBallPosition.y - 50.0f, entryBallPosition.z);
				GameObject.Find ("Ball").GetComponent<RangeBallRigidBody> ().MakeAnnouncement ("WATER HAZARD -50 points", enterTime + 2.5f, Color.blue);
				GameObject.Find ("Water").GetComponent<shallow_wave> ().generateSplash (entryBallPosition.x,entryBallPosition.z);
				follow.transform.position = entryCameraPosition;

			} else {
				GameObject.Find ("Water").GetComponent<shallow_wave> ().resetSplash ();
				GameObject.Find ("Ball").GetComponent<RangeBallRigidBody> ().Reset_From_Hazard ();
				GameObject.Find ("Ball").GetComponent<RangeBallRigidBody> ().addPoints (-50);
				GameObject.Find ("Announcement").GetComponent<Text> ().text = "";
				soundplayed = false;
				inWater = false;
			}
		


	}


	void OnTriggerEnter (Collider other){
		enterTime = Time.time;
		inWater = true;
		entryCameraPosition = follow.transform.position;
		entryBallPosition = ballrb.transform.position;

	}

	void OnTriggerStay (Collider other){

	}

	void OnTriggerExit (Collider other){


	}

	//Late update so follow camera uses this behavior instead of BallRigidBody behavior
	void LateUpdate(){
		if (inWater)
			Water_Effect ();

	}


}
