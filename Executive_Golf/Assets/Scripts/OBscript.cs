using UnityEngine;
using System.Collections;

public class OBscript : MonoBehaviour {

	Rigidbody ballrb;
	float enterTime;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void AnnouncementDeal(string announce){
		if (ballrb.name.Equals ("Ball")) {
			ballrb.GetComponent<BallRigidBody>().MakeAnnouncement(announce, enterTime + 1.5f, Color.white);
		} else if (ballrb.name.Equals ("Ballp1")) {
			ballrb.GetComponent<MultiBallRigidBodyP1>().MakeAnnouncement(announce, enterTime + 1.5f, Color.white);
		} else if (ballrb.name.Equals ("Ballp2")) {
			ballrb.GetComponent<MultiBallRigidBodyP2>().MakeAnnouncement(announce, enterTime + 1.5f, Color.white);
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
		ballrb = other.attachedRigidbody;
		enterTime = Time.time;

		AnnouncementDeal ("OUT OF BOUNDS");
		ResetDeal ();


	}


}
