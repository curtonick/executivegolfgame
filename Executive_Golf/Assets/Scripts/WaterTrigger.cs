using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaterTrigger : MonoBehaviour {

	private float enterTime = 0;
	AudioSource holed;
	bool inWater = false;
	Rigidbody ballrb;
	Camera follow;
	bool soundplayed = false;
	Vector3 entryCameraPosition;
	Vector3 entryBallPosition;

	void Start(){
		//ballrb = GameObject.Find ("Ball").GetComponent<Rigidbody> ();
		//follow = GameObject.Find ("FollowCamera").GetComponent<Camera> ();
		entryCameraPosition = new Vector3 (0, 0, 0);
	}

	void Water_Effect (){
			if (!soundplayed) {
				GameObject.Find ("splash").GetComponent<AudioSource> ().Play();
				soundplayed = true;
				//Move ball below map to ensure it keeps moving
				ballrb.transform.position = new Vector3(entryBallPosition.x, entryBallPosition.y - 200.0f, entryBallPosition.z);
			}
			if (Time.time < enterTime + 1.5f) {
				//GameObject.Find ("Ball").GetComponent<BallRigidBody> ().MakeAnnouncement ("WATER HAZARD", enterTime + 1.5f, Color.blue);
				AnnouncementDeal("WATER HAZARD");

				GameObject.Find ("Water").GetComponent<shallow_wave> ().generateSplash (entryBallPosition.x,entryBallPosition.z); //shallow wave start
				follow.transform.position = entryCameraPosition;

			} else {
				GameObject.Find ("Water").GetComponent<shallow_wave> ().resetSplash (); //shallow wave end
				
				//GameObject.Find ("Ball").GetComponent<BallRigidBody> ().Reset_From_Hazard (); //reset ball
				ResetDeal();

				AnnouncementDeal("");
				soundplayed = false;
				inWater = false;
			}
		


	}

	void AnnouncementDeal(string announce){
		if (ballrb.name.Equals ("Ball")) {
			ballrb.GetComponent<BallRigidBody>().MakeAnnouncement(announce, enterTime + 1.5f, Color.blue);
		} else if (ballrb.name.Equals ("Ballp1")) {
			ballrb.GetComponent<MultiBallRigidBodyP1>().MakeAnnouncement(announce, enterTime + 1.5f, Color.blue);
		} else if (ballrb.name.Equals ("Ballp2")) {
			ballrb.GetComponent<MultiBallRigidBodyP2>().MakeAnnouncement(announce, enterTime + 1.5f, Color.blue);
		}
	}


	void ResetDeal(){
		if (ballrb.name.Equals ("Ball")) {
			ballrb.GetComponent<BallRigidBody> ().Reset_From_Hazard ();
		} else if (ballrb.name.Equals ("Ballp1")) {
			ballrb.GetComponent<MultiBallRigidBodyP1> ().Reset_From_Hazard ();
		} else if (ballrb.name.Equals ("Ballp2")) {
			ballrb.GetComponent<MultiBallRigidBodyP2> ().Reset_From_Hazard ();
		}
	}


	void OnTriggerEnter (Collider other){
		enterTime = Time.time;
		inWater = true;

		if (other.name.Equals ("Ball")) {
			follow = GameObject.Find ("FollowCamera").GetComponent<Camera> ();
		} else if (other.name.Equals ("Ballp1")) {
			follow = GameObject.Find ("FollowCamerap1").GetComponent<Camera> ();
		} else if (other.name.Equals ("Ballp2")) {
			follow = GameObject.Find ("FollowCamerap2").GetComponent<Camera> ();
		}

		entryCameraPosition = follow.transform.position; //keep camera at same location when ball entered water

		ballrb = other.attachedRigidbody;
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
